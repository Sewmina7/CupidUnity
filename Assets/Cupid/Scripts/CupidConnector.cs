using System.Collections;
using System.Collections.Generic;
using Mirror;
using UnityEngine;

public class CupidConnector : MonoBehaviour
{
    public kcp2k.KcpTransport transport;
    void Start()
    {
        #if UNITY_SERVER
        //Server code
        string[] args = System.Environment.GetCommandLineArgs();
        for (int i = 0; i < args.Length; i++)
        {
            if (args[i].Contains("-port"))
            {
                Cupid.RoomPort = int.Parse(args[i+1]);
                Logger.SetFileName(Cupid.RoomPort.ToString());
            } 
        }
        if(Cupid.RoomPort < 0){
            Logger.Log("Invalid port, Did you pass the -port arguement?");
            return;
        }
        transport.Port = (ushort)Cupid.RoomPort;
        Logger.Log($"Starting server at port {Cupid.RoomPort}");
        NetworkManager.singleton.StartServer();
        #else
        //Client code
        if(Cupid.RoomPort <0){
            Logger.Log("Invalid Room port");
            return;
        }
        transport.Port = (ushort)Cupid.RoomPort;
        NetworkManager.singleton.networkAddress = Cupid.ServerAddress;
        Logger.Log($"Starting client at ${Cupid.ServerAddress}:{Cupid.RoomPort}");
        NetworkManager.singleton.StartClient();
        #endif
    }
    #if UNITY_SERVER
    float t=0;
    int _t = 0;
    void Update(){
        if(t < 60){
            t += Time.deltaTime;
        }else{
            if(NetworkServer.connections.Count <= 0){
                Logger.Log("Closing port " + Cupid.RoomPort + " due to no players");
                Application.Quit();
            }
        }
        if((int)t !=_t){
            _t = (int)t;
            Logger.Log(NetworkServer.connections.Count.ToString());
        }
    }
    #else
    void Update(){
        // Debug.Log(NetworkServer.connections.Count);
    }
    #endif

    void OnValidate(){        
        if(GetComponent<NetworkManager>()!=null){
            GetComponent<NetworkManager>().autoStartServerBuild=false;
        }
        if(transport == null){
            transport = GetComponent<kcp2k.KcpTransport>();
        }
    }
}
