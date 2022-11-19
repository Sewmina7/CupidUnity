using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class CupidLobby : MonoBehaviour
{
    [SerializeField]private GameObject MatchmakingUI;
    [SerializeField]private string GameScene;
    [Header("Server info")]
    [SerializeField]private string serverAddress = "xx.xx.xxx.xx";
    [SerializeField]private int cupidPort = 1601;
    [SerializeField]private string password = "xyz@123";

    public static string Username
    {
        get
        {
            if (PlayerPrefs.HasKey("username"))
            {
                return PlayerPrefs.GetString("username");
            }else{
                string username = Cupid.RandomUsername;
                PlayerPrefs.SetString("username",username);
                PlayerPrefs.Save();

                return username;
            }
        }
    }

    void Start()
    {
        Cupid.Init(serverAddress, cupidPort, password);
    }
    bool matchmaking = false;
    public void Matchmake()
    {
        Logger.Log("Starting matchmake as " + Username);
        StartCoroutine(matchmake());
    }

    IEnumerator matchmake()
    {
        matchmaking = true;
        while (matchmaking)
        {
            RefreshMatchmakingPanel();

            WWW req = new WWW(Cupid.CupidURI + "/?password=" + password + "&&username=" + Username);
            yield return req;
            // Debug.Log(req.text);
            CupidRoom? room = Cupid.ParseRoom(req.text);
            if(room == null){
                Logger.Log("No room : " + req.text);
            }else{
                CupidRoom _room = (CupidRoom)room;
                Logger.Log("Got into a room");
                Logger.Log(req.text);
                Logger.Log("Setting cupid to load game scene");
                Cupid.RoomPort = _room.Port;
                matchmaking=false;
                Logger.Log("Loading game scene");
                SceneManager.LoadScene(GameScene);
            }


            // string[] data = req.text.Split(',');
            // if(data.Length ==2){
            //     Logger.Log(req.text);
            //     if(data[0] == "1"){
            //         //Game started
            //         Logger.Log("Setting cupid room to " + data[1]);

            //         Cupid.RoomPort = int.Parse(data[1]);
            //         Logger.Log("Loading scene " + GameScene);

            //         SceneManager.LoadScene(GameScene);
            //         matchmaking=false;
            //         break;
            //     }else{
            //         //Game not started gotta continue
            //     }
            //     int gamePort = -1;
            //     try{
            //         gamePort = int.Parse(data[1]);
            //     }catch(Exception e){
            //         Logger.Log("Couldn't parse game port: " + req.text);
            //     }
            // }

            yield return new WaitForSeconds(1);
        }
    }

    IEnumerator CancelMatchmake(){
        WWW www = new WWW(Cupid.CupidURI + "/cancel?password=" + password + "&&username=" + Username);
        yield return www;
        if(www.text == "1"){
            Logger.Log("Cancelled matchmaking success");
        }else{
            Logger.Log("Matchmaking cancellation said " + www.text);
        }
    }

    public void Cancel()
    {
        matchmaking = false;
        StartCoroutine(CancelMatchmake());
        RefreshMatchmakingPanel();
    }

    void RefreshMatchmakingPanel()
    {
        MatchmakingUI.SetActive(matchmaking);
    }
}
