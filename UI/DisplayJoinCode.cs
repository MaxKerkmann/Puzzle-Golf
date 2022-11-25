using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System;

public class DisplayJoinCode : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI joinCodeDisplay;
    void Start()
    {
        if (!String.IsNullOrEmpty(LobbyManager.currentLobby.LobbyCode))
            joinCodeDisplay.text = "JoinCode: " + LobbyManager.currentLobby.LobbyCode;
    }

}
