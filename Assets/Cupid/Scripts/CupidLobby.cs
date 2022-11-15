using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class CupidLobby : MonoBehaviour
{
    public GameObject MatchmakingUI;
    public string GameScene;
    [Header("Server info")]
    public string serverAddress = "xx.xx.xxx.xx";
    public int cupidPort = 1601;
    public string password = "xyz@123";

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
            
            string[] data = req.text.Split(',');
            if(data.Length ==2){
                if(data[0] == "1"){
                    //Game started
                    Cupid.RoomPort = int.Parse(data[1]);
                    SceneManager.LoadScene(GameScene);
                    matchmaking=false;
                    break;
                }else{
                    //Game not started gotta continue
                }
                int gamePort = -1;
                try{
                    gamePort = int.Parse(data[1]);
                }catch(Exception e){
                    Logger.Log("Couldn't parse game port: " + req.text);
                }
            }

            yield return new WaitForSeconds(1);
        }
    }

    public void Cancel()
    {
        matchmaking = false;
        RefreshMatchmakingPanel();
    }

    void RefreshMatchmakingPanel()
    {
        MatchmakingUI.SetActive(matchmaking);
    }
}
