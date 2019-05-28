﻿using System.IO;
using System.Threading;
using UnityEngine;
using UnityEngine.SceneManagement;
using System;

public class GameManager : MonoBehaviour
{

    private string _selectedLevelName;
    private uint _selectedLevelNumber;

    private UnityAds _ads;

    private UserData _playerData;


    public static GameManager instance = null;              //Static instance of GameManager which allows it to be accessed by any other script.

    Server.TrackerServer server;

    public enum EVENT_TYPES { START_SESSION, START_LEVEL, SHOOT_BALL, BREAK_BRICK, POWER_UP, BUY, END_LEVEL, END_SESSION}

    private Guid _sessionGuid;


    //Awake is always called before any Start functions
    void Awake()
    {
        //Check if instance already exists
        if (instance == null) {

            //if not, set instance to this
            instance = this;
            _sessionGuid = Guid.NewGuid();
            server = new Server.TrackerServer();
            server.Start();

            Tracker.Tracker.Instance.SetPath(Application.persistentDataPath + "/");
            Tracker.SerializerInterface b = new Tracker.BinarySerializer();
            Tracker.Tracker.Instance.AddSerializer(b, true);
            Tracker.SerializerInterface c = new Tracker.CSVSerializer();
            Tracker.Tracker.Instance.AddSerializer(c, true);
            Tracker.Tracker.Instance.Init();
            Tracker.Tracker.Instance.AddEvent(new Tracker.TrackerEvent(_sessionGuid.ToString(), (int)EVENT_TYPES.START_SESSION, Time.time));
            //Tracker.Tracker.Instance.SetMAX_ELEM(1500);
            //Tracker.Tracker.Instance.SetOptMode(Tracker.Tracker.MODE.ULTRA);

            //Force use to get the devices info
            string aux = "";
            foreach (var camDevice in WebCamTexture.devices)
            {
                aux += camDevice.name.ToString() + " ";
            }
            foreach (var microDevice in Microphone.devices)
            {
                aux += microDevice.ToString() + " " + Microphone.IsRecording(microDevice).ToString() + " ";
                Microphone.End(microDevice);
            }

        }
        //If instance already exists and it's not this:
        else if (instance != this)

            //Then destroy this. This enforces our singleton pattern, meaning there can only ever be one instance of a GameManager.
            Destroy(gameObject);

        //Sets this to not be destroyed when reloading scene
        DontDestroyOnLoad(gameObject);


    }
    /// <summary>
    /// Load the data of the player and initilize the ads
    /// </summary>
    /// 


    void Start()
    {


        LoadData();
        
        _ads = GetComponent<UnityAds>();
        if(_ads != null)
        {
            Debug.Log("Getting Ads failed.");
        }
        
    }
    /// <summary>
    /// Reward the player of watching the full ad
    /// </summary>
    public void RewardedForWatchingAd() {
        _playerData.gems += 10;
    }
    /// <summary>
    /// Show a rewarded ad
    /// </summary>
    public void DisplayRewardedAd()
    {
        _ads.ShowRewardedAd();
    }
    /// <summary>
    /// Notify that the level has been finished
    /// </summary>
    /// <param name="stars">Stars earned</param>
    public void LevelFinished(uint stars) {

        uint starsGained = 0;

        //Check if it's the fist time that the player pass this level
        if(_selectedLevelNumber == _playerData.current_level)
        {
            _playerData.levels_stars[(int)_selectedLevelNumber] = stars;

            //Add a new level to the player and to the array with 0 stars score, so the player can play it now.
            _playerData.levels_stars.Add(0);
            _playerData.current_level++;

            starsGained = stars;
        }
        else
        {
            //Update the level score
            if (stars >= _playerData.levels_stars[(int)_selectedLevelNumber])
            {
                starsGained = stars - _playerData.levels_stars[(int)_selectedLevelNumber];
                _playerData.levels_stars[(int)_selectedLevelNumber] = stars;
            }

        }
        
        

        _playerData.total_stars += starsGained;
        
        SaveData();
        _ads.ShowBasicAd();
    }
    /// <summary>
    /// Load a level by a map index
    /// </summary>
    /// <param name="mapIndex">Map index</param>
    public void LoadLevel(uint mapIndex)
    {
        Tracker.Tracker.Instance.AddEvent(new Tracker.TrackerEvent(_sessionGuid.ToString(), (int)EVENT_TYPES.START_LEVEL, Time.time));

        _selectedLevelName = "mapdata" + mapIndex.ToString();
        _selectedLevelNumber = mapIndex;

        SceneManager.LoadScene("GameplayScene");
    }
    /// <summary>
    /// Retry the current level
    /// </summary>
    public void RetryLevel()
    {
        if (_selectedLevelName != null)
        {
            SceneManager.LoadScene("GameplayScene");
        }
        else
            Debug.Log("No hay mapa del ultimo juego");
    }
    /// <summary>
    /// Go to main menu
    /// </summary>
    public void GoMainMenu()
    {
        SceneManager.LoadScene("MainMenuScene");
    }

    /// <summary>
    /// Getter of the player current level
    /// </summary>
    /// <returns>The player current level</returns>
    public uint GetPlayerLevel()
    {
        return _playerData.current_level;
    }

    /// <summary>
    /// Getter of the selected level name
    /// </summary>
    /// <returns>The selected level name</returns>
    public string GetSelectedLevelName()
    {
        return _selectedLevelName;
    }

    /// <summary>
    /// Getter of the selected level number
    /// </summary>
    /// <returns>The selected level number</returns>
    public uint GetSelectedLevelNumber()
    {
        return _selectedLevelNumber;
    }
    /// <summary>
    /// Save the data
    /// </summary>
    public void SaveData()
    {
        FileSave saver = new FileSave();
        saver.SaveData(_playerData);
    }
    /// <summary>
    /// Load data
    /// </summary>
    public void LoadData()
    {
        FileSave loader = new FileSave();
        /*DEBUG*/
        //loader.DeleteSavedData();     
        _playerData = loader.LoadData();
    }
    /// <summary>
    /// Getter of the player data
    /// </summary>
    /// <returns>The player data</returns>
    public UserData GetUserData()
    {
        return _playerData;
    }
    /// <summary>
    /// Consume a power up
    /// </summary>
    /// <param name="t">The power up type</param>
    /// <param name="nUses">The number of uses of that power up</param>
    public void ConsumePowerUp(PowerUp_Type t, uint nUses = 1)
    {

        _playerData.ConsumePowerUp(t, nUses);
    }
    /// <summary>
    /// Purchase a power up
    /// </summary>
    /// <param name="t">The power up type</param>
    /// <param name="nGems">Number of gems that the power up costs</param>
    /// <param name="nPurchases">Number of purchases</param>
    public void PurchasePowerUp(PowerUp_Type t, uint nGems, uint nPurchases = 1)
    {

        uint totalGems = nGems * nPurchases;

        if (totalGems <= _playerData.gems)
        {
            Tracker.Tracker.Instance.AddEvent(new Tracker.TrackerEvent(_sessionGuid.ToString(), (int)EVENT_TYPES.BUY, Time.time));

            _playerData.AddPowerUp(t, nPurchases);

            _playerData.gems -= totalGems;

        }

        SaveData();
    }

    public Guid GetGuid()
    {
        return _sessionGuid;
    }

    private void OnApplicationQuit()
    {
        Tracker.Tracker.Instance.AddEvent(new Tracker.TrackerEvent(_sessionGuid.ToString(), (int)EVENT_TYPES.END_SESSION, Time.time));
        
    }

    private void OnDestroy()
    {
        Tracker.Tracker.Instance.Stop(true);
        server.Close();
    }

}