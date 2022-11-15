using System;
using System.IO;
using UnityEngine;

public class Logger
{
    public static bool Enabled = true;
    private static Logger m_instance = null;
    private static string ApplicationDirectory
    {
        get
        {
            string path = Application.dataPath;
            if (Application.platform == RuntimePlatform.OSXPlayer)
            {
                path += "/../../";
            }
            else if (Application.platform == RuntimePlatform.WindowsPlayer)
            {
                path += "/../";
            }
            return path;
        }
    }
    public string LogFilePath {get; private set;}
    public static Logger instance
    {
        get
        {
            if (m_instance == null)
            {
                m_instance = new Logger();
            }

            return m_instance;
        }
    }

    public Logger()
    {
        if(!Enabled){return;}
        Debug.Log("Starting logger @ " + ApplicationDirectory);
        if(LogFilePath == null){
            LogFilePath = ApplicationDirectory + "Log.txt";
        }
        File.WriteAllText(LogFilePath, "Logger initiated at " + DateTime.Now + "\n\n");
    }

    public void log(string message){
        if(!Enabled){return;}

        File.AppendAllText(LogFilePath,$"[{DateTime.Now}] {message}\n");
        Debug.Log(message);
    }

    public static void Log(string message){
        instance.log(message);
    }

    public static void SetFileName(string fileName){
        instance.LogFilePath = ApplicationDirectory+fileName + ".txt";
    }

    public static void SetFilePath(string path){
        instance.LogFilePath= path;
    }
}