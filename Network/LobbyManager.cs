using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Unity.Netcode.Transports.UTP;
using Unity.Services.Lobbies;
using Unity.Services.Lobbies.Models;
using Unity.Services.Relay;
using UnityEngine;
using Object = UnityEngine.Object;

public class LobbyManager 
{
    private const int lobbyPingTimer = 15;
    private const int lobbyRefreshRate = 2;

    private static UnityTransport transport;

    public static Lobby currentLobby { get; private set; }

    private static CancellationTokenSource lobbyPingSource, updateLobbySource;


    public static event Action<Lobby> CurrentLobbyRefreshed;

    private static UnityTransport Transport
    {
        get => transport != null ? transport : transport = Object.FindObjectOfType<UnityTransport>();
        set => transport = value;
    }

    public static void ResetStatics()
    {
        if (Transport != null)
        {
            Transport.Shutdown();
            Transport = null;
        }

        currentLobby = null;
    }

    public static async Task CreateLobby(LobbyConfig config)
    {
        transport = Object.FindObjectOfType<UnityTransport>();
        var alloc = await RelayService.Instance.CreateAllocationAsync(config.maxPlayerAmount);
        var joinCode = await RelayService.Instance.GetJoinCodeAsync(alloc.AllocationId);

        var lobbyOptions = new CreateLobbyOptions
        {
            Data = new Dictionary<string, DataObject>
            {
                { Constants.JOINCODEKEY, new DataObject(DataObject.VisibilityOptions.Member,joinCode)},
                { Constants.LOBBYNAMEKEY, new DataObject(DataObject.VisibilityOptions.Public,config.lobbyName)}
            }
        };

        if(CrossSceneNetworkData.privatLobby)
            lobbyOptions.IsPrivate = true;
  
        currentLobby = await Lobbies.Instance.CreateLobbyAsync(config.lobbyName,config.maxPlayerAmount,lobbyOptions);

        transport.SetHostRelayData(alloc.RelayServer.IpV4, (ushort)alloc.RelayServer.Port,alloc.AllocationIdBytes,alloc.Key,alloc.ConnectionData);
        Debug.Log("Lobbyname: " + currentLobby.Name);
        Debug.Log("LobbyCode: " + currentLobby.LobbyCode);
        Debug.Log("Private: "+ currentLobby.IsPrivate);
        Debug.Log("Slots: " + (currentLobby.MaxPlayers - currentLobby.AvailableSlots)+"/" + currentLobby.MaxPlayers);
        PingCurrentLobby(); //Heartbeat 
        RefreshLobby();
    }

    private static async void PingCurrentLobby()
    {
        lobbyPingSource = new CancellationTokenSource();
        while(lobbyPingSource.IsCancellationRequested && currentLobby != null)
        {
            await Lobbies.Instance.SendHeartbeatPingAsync(currentLobby.Id);
            Debug.Log("Heartbeat");
            await Task.Delay(lobbyPingTimer * 1000);
            
        }
    }

    private static async void RefreshLobby()
    {
        updateLobbySource = new CancellationTokenSource();
        await Task.Delay(lobbyRefreshRate * 1000);
        while(!updateLobbySource.IsCancellationRequested && currentLobby != null)
        {
            currentLobby = await Lobbies.Instance.GetLobbyAsync(currentLobby.Id);
            CurrentLobbyRefreshed?.Invoke(currentLobby);
            Debug.Log("Refresh");
            await Task.Delay(lobbyRefreshRate * 1000);
            
        }
    }

    public static async Task<List<Lobby>> SearchLobbies(string filter)
    {
          QueryLobbiesOptions options = new QueryLobbiesOptions();
          options.Count = 25;

          // Filter for open lobbies only
          options.Filters = new List<QueryFilter>()
            {
              new QueryFilter(
                  field: QueryFilter.FieldOptions.AvailableSlots,
                  op: QueryFilter.OpOptions.GT,
                  value: "0")
           };

          // Order by newest lobbies first
          options.Order = new List<QueryOrder>()
          {
              new QueryOrder(
                  asc: false,
                  field: QueryOrder.FieldOptions.Created)
          };

          QueryResponse foundLobbies = await Lobbies.Instance.QueryLobbiesAsync(options);
          Debug.Log("LobbyCountPreFilter: " + foundLobbies.Results.Count);
          foreach(Lobby lobby in foundLobbies.Results)
          {
              if (!lobby.Name.Contains(filter)) 
                  foundLobbies.Results.Remove(lobby);
          }

          return foundLobbies.Results;
    }

    public static async Task JoinLobby(string lobbyData,bool isJoincode = false)
    {
        transport = Object.FindObjectOfType<UnityTransport>();
        if (!isJoincode)
            currentLobby = await Lobbies.Instance.JoinLobbyByIdAsync(lobbyData);
        else
            currentLobby = await Lobbies.Instance.JoinLobbyByCodeAsync(lobbyData);


        var alloc = await RelayService.Instance.JoinAllocationAsync(currentLobby.Data[Constants.JOINCODEKEY].Value);
        transport.SetClientRelayData(alloc.RelayServer.IpV4, (ushort)alloc.RelayServer.Port, alloc.AllocationIdBytes, alloc.Key, alloc.ConnectionData, alloc.HostConnectionData);
        Debug.Log("Lobbyname: " + currentLobby.Name);
        Debug.Log("LobbyCode: " + currentLobby.LobbyCode);
        Debug.Log("Private: " + currentLobby.IsPrivate);
        RefreshLobby();
    }

    public static async Task LeaveLobby()
    {
        lobbyPingSource?.Cancel();
        updateLobbySource?.Cancel();

        if(currentLobby != null)
        {
            try
            {
                if(currentLobby.HostId == Authentication.PlayerId) await Lobbies.Instance.DeleteLobbyAsync(currentLobby.Id);
                else await Lobbies.Instance.RemovePlayerAsync(currentLobby.Id, Authentication.PlayerId);
                currentLobby = null;
            }catch (Exception ex)
            {
                Debug.Log(ex);
            }
        }
    }

    public Lobby GetCurrentLobby()
    {
        return currentLobby;
    }


}
