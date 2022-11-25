using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Unity.Netcode;
using UnityEngine.SceneManagement;
using Oculus.Platform;
using Oculus.Platform.Models;
using Unity.Services.Authentication;
using Unity.Services.Core;
using Unity.Services.Relay.Models;
using Unity.Services.Relay;
using System;
using TMPro;
using System.Threading.Tasks;
using Unity.Services.Lobbies.Models;

public class NetworkStartUI : MonoBehaviour
{

    [SerializeField] private TMP_InputField playerNameInput;


    [Header("Create Lobby")]

    [SerializeField] private TMP_InputField lobbyNameInput;
    [SerializeField] private TMP_Dropdown playerCountSelector;
    [SerializeField] private int maxPlayers;
    [SerializeField] private Toggle privatToggle;

    [SerializeField] private Button createLobbyButton;


    [Header("Join Lobby")]

    [SerializeField] private TMP_InputField lobbyFilter;
    [SerializeField] private TMP_Dropdown lobbySelector;
    [SerializeField] private TMP_InputField joinCodeInput;

    [SerializeField] private Button connectToLobbyButton;


    List<Lobby> foundLobbies = null;

    private string selectedLobbyId = "";
    

    private async void Awake()
    {
        
        await Authentication.Login();
        await UnityServices.InitializeAsync();


        createLobbyButton.onClick.AddListener(() =>
        {
            CrossSceneNetworkData.PlayerRole = "Host";

            CrossSceneNetworkData.LobbyName = lobbyNameInput.text;
            CrossSceneNetworkData.maxPlayerCount = playerCountSelector.value;
            CrossSceneNetworkData.privatLobby = privatToggle.isOn;

            SceneManager.LoadScene("GameLobby");

        });

        connectToLobbyButton.onClick.AddListener(() =>
        {
            CrossSceneNetworkData.PlayerRole = "Client";
            if (joinCodeInput.text.Length == 0)
            {
                CrossSceneNetworkData.LobbyID = selectedLobbyId;
            }
            else
            {
                CrossSceneNetworkData.LobbyID = "";
                CrossSceneNetworkData.LobbyJoincode = joinCodeInput.text;
            }

            SceneManager.LoadScene("GameLobby");
        });

    }

    private void Start()
    {
        FillPlayerCountList();
    }

    public void ChangeSelectedLobby()
    {
        selectedLobbyId = foundLobbies[lobbySelector.value].Id;
    }

    public async void FillLobbyList()
    {
        foundLobbies = await LobbyManager.SearchLobbies();
        List<string> lobbyNames = new List<string>();
        Debug.Log("LobbyCount: " + foundLobbies.Count);
        if (foundLobbies.Count > 0)
        {
            for (int i = 0; i < foundLobbies.Count; i++)
            {
                lobbyNames.Add(foundLobbies[i].Name);
                Debug.Log("FoundLobbyName: " + foundLobbies[i].Name);
            }


            lobbySelector.AddOptions(lobbyNames);
            selectedLobbyId = foundLobbies[lobbySelector.value].Id;
        }
    }

    private void FillPlayerCountList()
    {
        List<TMP_Dropdown.OptionData> options = new List<TMP_Dropdown.OptionData>();
        for(int i = 0; i < maxPlayers; i++)
        {
            TMP_Dropdown.OptionData option = new TMP_Dropdown.OptionData();
            option.text = (i+1).ToString();
            options.Add(option);
        }
        playerCountSelector.AddOptions(options);
    }

    public void SetPlayerName()
    {
        CrossSceneNetworkData.PlayerName = playerNameInput.text;
    }

}
