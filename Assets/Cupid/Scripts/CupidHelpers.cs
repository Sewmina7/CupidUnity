using System;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Networking;

public static class Cupid
{
    private static string serverAddress;
    public static string ServerAddress => serverAddress;
    public static string CupidURI => "http://" + serverAddress + ":" + port;

    private static int port;
    public static int Port => port;

    private static string password;
    private static CupidSettings settings = null;
    public static CupidSettings Settings => settings;

    public static bool isInitialized => settings!=null;


    public static int RoomPort =-1;


    public static async Task Init(string _serverAddress, int _port, string _password){
        serverAddress= _serverAddress;
        port = _port;
        password = _password;

        using (UnityWebRequest www = UnityWebRequest.Get(CupidURI + "/settings?password="+password))
        {
            var operation = www.SendWebRequest();
            while (!operation.isDone)
            {
                await Task.Yield();
            }

            try{
                settings = JsonUtility.FromJson<CupidSettings>(www.downloadHandler.text);
                if(settings==null){throw new NullReferenceException();}
                Logger.Log("Cupid init success");
            }catch(Exception e){
                Logger.Log("Error retreiving settings from server " + e.Message);
                Logger.Log(www.downloadHandler.text);
            }
        }
    }

    
    public static string RandomUsername{
        get{
            string pool="ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
            string output ="";
            for(int i=0; i < 5; i++){
                output += pool.ToCharArray()[UnityEngine.Random.Range(0,pool.Length)];
            }

            return output;
        }
    }

    public static CupidRoom? ParseRoom(string data){
        CupidRoom? room = null;
        try{
            room = JsonUtility.FromJson<CupidRoom>(data);
        }catch{}

        return room;
    }
}

[System.Serializable]
public class CupidSettings
{
    public int minimum_players;
    public int maximum_players;
    public int waiting_time;
}

[System.Serializable]
public struct CupidRoom{
    public CupidQueueEntry[] Players;
    public int Port;
    public uint InitTime;
}

[System.Serializable]
public struct CupidQueueEntry{
    string Name;
    uint LastSeen;
}