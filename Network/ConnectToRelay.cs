using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using TMPro;
using Unity.Netcode;
using Unity.Netcode.Transports.UTP;
using Unity.Services.Authentication;
using Unity.Services.Core;
using Unity.Services.Relay;
using Unity.Services.Relay.Models;
using UnityEngine;
using UnityEngine.SceneManagement;


public class ConnectToRelay : MonoBehaviour
{
    private void Start()
    {
        if (CrossSceneNetworkData.LobbyID == null)
        {
            switch (CrossSceneNetworkData.PlayerRole)
            {
                case "Host":
                    createLobby();
                    break;
                case "Client":
                    joinLobby();
                    break;
                default:
                    SceneManager.LoadScene("Base");
                    break;
            }
            SetInitialData();
        }
    }
    private async void joinLobby()
    {
        if(CrossSceneNetworkData.LobbyID.Length == 0)
            await LobbyManager.JoinLobby(CrossSceneNetworkData.LobbyJoincode,true);
        else
            await LobbyManager.JoinLobby(CrossSceneNetworkData.LobbyID);

        NetworkManager.Singleton.StartClient();
    }

    private async void createLobby()
    {
        LobbyConfig lobbyConfig = new LobbyConfig()
        {
            lobbyName = CrossSceneNetworkData.LobbyName,
            maxPlayerAmount = CrossSceneNetworkData.maxPlayerCount,
            isPrivate = CrossSceneNetworkData.privatLobby
        };

        await LobbyManager.CreateLobby(lobbyConfig);

        NetworkManager.Singleton.StartHost();
    }

    private void SetInitialData()
    {
        CrossSceneNetworkData.lastLevel = -1;
    }


}
