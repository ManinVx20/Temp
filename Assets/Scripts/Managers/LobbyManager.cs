using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using Unity.Services.Authentication;
using Unity.Services.Lobbies;
using Unity.Services.Lobbies.Models;
using UnityEngine;

namespace StormDreams
{
    public class LobbyManager : Singleton<LobbyManager>
    {
        public static event Action<Lobby> OnLobbyUpdated;

        private Lobby _lobby;
        private Coroutine _heartbeatLobbyCoroutine;
        private Coroutine _refreshLobbyCoroutine;

        private void Awake()
        {
            DontDestroyOnLoad(gameObject);
        }

        public string GetHostId()
        {
            return _lobby.HostId;
        }

        public string GetLobbyCode()
        {
            return _lobby?.LobbyCode;
        }

        public List<Dictionary<string, PlayerDataObject>> GetPlayersData()
        {
            List<Dictionary<string, PlayerDataObject>> playersData = new List<Dictionary<string, PlayerDataObject>>();
            foreach (Player player in _lobby.Players)
            {
                playersData.Add(player.Data);
            }

            return playersData;
        }

        // public async void ListLobbies()
        // {
        //     try
        //     {
        //         QueryLobbiesOptions queryLobbiesOptions = new QueryLobbiesOptions();
        //         queryLobbiesOptions.Count = 25;

        //         // Filter for open lobbies only
        //         queryLobbiesOptions.Filters = new List<QueryFilter>
        //         {
        //             new QueryFilter(QueryFilter.FieldOptions.AvailableSlots, "0", QueryFilter.OpOptions.GT)
        //         };

        //         // Order by newest lobbies first
        //         queryLobbiesOptions.Order = new List<QueryOrder>
        //         {
        //             new QueryOrder(false, QueryOrder.FieldOptions.Created)
        //         };

        //         QueryResponse queryResponse = await LobbyService.Instance.QueryLobbiesAsync(queryLobbiesOptions);

        //         Debug.Log($"Lobbies found: {queryResponse.Results.Count}");
        //         foreach (Lobby lobby in queryResponse.Results)
        //         {
        //             Debug.Log($"{lobby.Name} | {lobby.MaxPlayers}");
        //         }
        //     }
        //     catch (LobbyServiceException e)
        //     {
        //         Debug.Log(e);
        //     }
        // }

        public async Task<bool> CreateLobby(int maxPlayers, bool isPrivate, Dictionary<string, string> playerData, Dictionary<string, string> lobbyData)
        {
            Dictionary<string, PlayerDataObject> data = SerializePlayerData(playerData);
            Player player = new Player(AuthenticationService.Instance.PlayerId, null, data);

            CreateLobbyOptions createLobbyOptions = new CreateLobbyOptions()
            {
                Data = SerializeLobbyData(lobbyData),
                IsPrivate = isPrivate,
                Player = player
            };

            try
            {
                _lobby = await LobbyService.Instance.CreateLobbyAsync("Lobby", maxPlayers, createLobbyOptions);
            }
            catch (LobbyServiceException e)
            {
                Debug.Log(e);

                return false;
            }

            Debug.Log($"Created lobby \"{_lobby.Name}\" ({_lobby.Id}) with join code {_lobby.LobbyCode}.");

            _heartbeatLobbyCoroutine = StartCoroutine(HeartbeatLobbyCoroutine(_lobby.Id, 15.0f));
            _refreshLobbyCoroutine = StartCoroutine(RefreshLobbyCoroutine(_lobby.Id, 1.0f));

            return true;
        }

        public async Task<bool> JoinLobby(string lobbyCode, Dictionary<string, string> playerData)
        {
            Dictionary<string, PlayerDataObject> data = SerializePlayerData(playerData);
            Player player = new Player(AuthenticationService.Instance.PlayerId, null, data);

            JoinLobbyByCodeOptions joinLobbyByCodeOptions = new JoinLobbyByCodeOptions()
            {
                Player = player
            };

            try
            {
                _lobby = await LobbyService.Instance.JoinLobbyByCodeAsync(lobbyCode, joinLobbyByCodeOptions);
            }
            catch (LobbyServiceException e)
            {
                Debug.Log(e);

                return false;
            }

            Debug.Log($"Joined in lobby \"{_lobby.Name}\" ({_lobby.Id}).");

            _refreshLobbyCoroutine = StartCoroutine(RefreshLobbyCoroutine(_lobby.Id, 1.0f));

            return true;
        }

        public async Task<bool> UpdatePlayerData(string playerId, Dictionary<string, string> playerData, string allocationId = default, string connectionData = default)
        {
            Dictionary<string, PlayerDataObject> data = SerializePlayerData(playerData);

            UpdatePlayerOptions updatePlayerOptions = new UpdatePlayerOptions()
            {
                Data = data,
                AllocationId = allocationId,
                ConnectionInfo = connectionData
            };

            try
            {
                _lobby = await LobbyService.Instance.UpdatePlayerAsync(_lobby.Id, playerId, updatePlayerOptions);
            }
            catch (LobbyServiceException e)
            {
                Debug.Log(e);

                return false;
            }

            OnLobbyUpdated?.Invoke(_lobby);

            return true;
        }

        public async Task<bool> UpdateLobbyData(Dictionary<string, string> lobbyData)
        {
            Dictionary<string, DataObject> data = SerializeLobbyData(lobbyData);

            UpdateLobbyOptions updateLobbyOptions = new UpdateLobbyOptions()
            {
                Data = data
            };

            try
            {
                _lobby = await LobbyService.Instance.UpdateLobbyAsync(_lobby.Id, updateLobbyOptions);
            }
            catch (LobbyServiceException e)
            {
                Debug.Log(e);

                return false;
            }

            OnLobbyUpdated?.Invoke(_lobby);

            return true;
        }

        // public async void LeaveLobby()
        // {
        //     try
        //     {
        //         await LobbyService.Instance.RemovePlayerAsync(_joinedLobby.Id, AuthenticationService.Instance.PlayerId);
        //     }
        //     catch (LobbyServiceException e)
        //     {
        //         Debug.Log(e);
        //     }
        // }

        // public async void KickPlayer(string playerId)
        // {
        //     try
        //     {
        //         await LobbyService.Instance.RemovePlayerAsync(_joinedLobby.Id, playerId);
        //     }
        //     catch (LobbyServiceException e)
        //     {
        //         Debug.Log(e);
        //     }
        // }

        // public async void MigrateLobbyHost()
        // {
        //     try
        //     {
        //         UpdateLobbyOptions updateLobbyOptions = new UpdateLobbyOptions
        //         {
        //             HostId = _joinedLobby.Players[1].Id
        //         };


        //         _hostedLobby = await LobbyService.Instance.UpdateLobbyAsync(_joinedLobby.Id, updateLobbyOptions);
        //         _joinedLobby = _hostedLobby;
        //     }
        //     catch (LobbyServiceException e)
        //     {
        //         Debug.Log(e);
        //     }
        // }

        private void OnApplicationQuit()
        {
            if (_lobby != null && _lobby.HostId == AuthenticationService.Instance.PlayerId)
            {
                LobbyService.Instance.DeleteLobbyAsync(_lobby.Id);
            }
        }

        private Dictionary<string, PlayerDataObject> SerializePlayerData(Dictionary<string, string> playerData)
        {
            Dictionary<string, PlayerDataObject> data = new Dictionary<string, PlayerDataObject>();
            foreach (var (key, value) in playerData)
            {
                data.Add(key, new PlayerDataObject(PlayerDataObject.VisibilityOptions.Member, value));
            }

            return data;
        }

        private Dictionary<string, DataObject> SerializeLobbyData(Dictionary<string, string> lobbyData)
        {
            Dictionary<string, DataObject> data = new Dictionary<string, DataObject>();
            foreach (var (key, value) in lobbyData)
            {
                data.Add(key, new DataObject(DataObject.VisibilityOptions.Member, value));
            }

            return data;
        }

        private IEnumerator HeartbeatLobbyCoroutine(string lobbyId, float waitTimeSeconds)
        {
            while (true)
            {
                LobbyService.Instance.SendHeartbeatPingAsync(lobbyId);

                yield return new WaitForSecondsRealtime(waitTimeSeconds);
            }
        }

        private IEnumerator RefreshLobbyCoroutine(string lobbyId, float waitTimeSeconds)
        {
            while (true)
            {
                Task<Lobby> task = LobbyService.Instance.GetLobbyAsync(lobbyId);

                yield return new WaitUntil(() => task.IsCompleted);

                _lobby = task.Result;

                OnLobbyUpdated?.Invoke(_lobby);

                yield return new WaitForSecondsRealtime(waitTimeSeconds);
            }
        }
    }
}
