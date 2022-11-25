using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class VRLogger : MonoBehaviour
{
    Dictionary<string, string> debugsLogs = new Dictionary<string, string>();

    public TMP_Text display;

    private void OnEnable()
    {
        Application.logMessageReceived += HandleLog;
    }

    private void OnDisable()
    {
        Application.logMessageReceived += HandleLog;
    }

    public void HandleLog(string logString, string stacktrace, LogType type)
    {
       
            string[] splitString = logString.Split(char.Parse(":"));
            string debugKey = splitString[0];
            string debugValue = splitString.Length > 1 ? splitString[1] : "";

            if (debugsLogs.ContainsKey(debugKey))
            {
                debugsLogs[debugKey] = debugValue + " (" +System.DateTime.Now.ToString("mm-ss") + ")";
            }
            else
                debugsLogs.Add(debugKey, debugValue);
        

        string displayText = "";

        foreach (KeyValuePair<string, string> kvp in debugsLogs)
        {
            if(kvp.Value == "")
                displayText += kvp.Key + "\n";
            else
                displayText += kvp.Key + ": " + kvp.Value + "\n";
        }
        display.text = displayText;
    }



}
