using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
public class ConsoleLogger : MonoBehaviour
{
    private TMP_Text selfText;
    // Start is called before the first frame update
    void Start()
    {
        selfText = GetComponent<TMP_Text>();
        Application.logMessageReceived += LogCallback;
    }

    void LogCallback(string logString, string stackTrace, LogType type)
    {
        //selfText.text = logString;
        //Or Append the log to the old one
        if(type != LogType.Warning)
            selfText.text = logString + "\r\n";
    }
}
