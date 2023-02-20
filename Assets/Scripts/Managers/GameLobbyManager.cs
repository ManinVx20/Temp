using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using FishNet;
using FishNet.Transporting;
using FishNet.Transporting.FishyUTPPlugin;
using FishNet.Transporting.Tugboat;
using Unity.Services.Authentication;
using Unity.Services.Lobbies.Models;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace StormDreams
{
    public class GameLobbyManager : Singleton<GameLobbyManager>
    {
        public static event Action OnGameLobbyUpdated;
        public static event Action<bool> OnGameLobbyReady;

        private List<LobbyPlayerData> _lobbyPlayersData = new List<LobbyPlayerData>();
        private LobbyPlayerData _lobbyPlayerData;
        private LobbyData _lobbyData;
        private int _maxPlayersPerLobby = 2;
        private bool _inGame = false;

        public bool IsHost => _lobbyPlayerData.Id == LobbyManager.Instance.GetHostId();

        private void Awake()
        {
            DontDestroyOnLoad(gameObject);
        }

        private void OnEnable()
        {
            LobbyManager.OnLobbyUpdated += LobbyManager_OnLobbyUpdated;
            FishyRelayManager.OnJoinCode += FishyRelayManager_OnJoinCode;
        }

        private void OnDisable()
        {
            LobbyManager.OnLobbyUpdated -= LobbyManager_OnLobbyUpdated;
            FishyRelayManager.OnJoinCode -= FishyRelayManager_OnJoinCode;
        }

        public string GetLobbyCode()
        {
            return LobbyManager.Instance.GetLobbyCode();
        }

        public List<LobbyPlayerData> GetPlayers()
        {
            return _lobbyPlayersData;
        }

        public int GetMapIndex()
        {
            return _lobbyData.MapIndex;
        }

        public async Task<bool> CreateLobby()
        {
            _lobbyPlayerData = new LobbyPlayerData(AuthenticationService.Instance.PlayerId, "Host");

            _lobbyData = new LobbyData(0);

            bool status = await LobbyManager.Instance.CreateLobby(_maxPlayersPerLobby, true, _lobbyPlayerData.Serialize(), _lobbyData.Serialize());

            return status;
        }

        public async Task<bool> JoinLobby(string lobbyCode)
        {
            _lobbyPlayerData = new LobbyPlayerData(AuthenticationService.Instance.PlayerId, "Guest");

            bool status = await LobbyManager.Instance.JoinLobby(lobbyCode, _lobbyPlayerData.Serialize());

            return status;
        }

        public async Task<bool> SetPlayerReady()
        {
            _lobbyPlayerData.IsReady = !_lobbyPlayerData.IsReady;

            return await LobbyManager.Instance.UpdatePlayerData(_lobbyPlayerData.Id, _lobbyPlayerData.Serialize());
        }

        public async Task<bool> SetSelectedMap(int mapIndex, string sceneName)
        {
            _lobbyData.MapIndex = mapIndex;
            _lobbyData.SceneName = sceneName;

            return await LobbyManager.Instance.UpdateLobbyData(_lobbyData.Serialize());
        }

        public async Task StartGame()
        {
            // string relayJoinCode = await RelayManager.Instance.CreateRelay(_maxPlayersPerLobby);

            // _lobbyData.RelayJoinCode = relayJoinCode;
            // await LobbyManager.Instance.UpdateLobbyData(_lobbyData.Serialize());

            // string allocationId = RelayManager.Instance.GetAllocationId();
            // string connectionData = RelayManager.Instance.GetConnectionData();
            // await LobbyManager.Instance.UpdatePlayerData(_lobbyPlayerData.Id, _lobbyPlayerData.Serialize(), allocationId, connectionData);

            AsyncOperation op = SceneManager.LoadSceneAsync(_lobbyData.SceneName);
            op.completed += SceneManager_OnSceneLoaded;

            _inGame = true;
        }

        public async Task JoinGame()
        {
            // await RelayManager.Instance.JoinRelay(_lobbyData.RelayJoinCode);

            FishyRelayManager relayManager = ((FishyUTP)InstanceFinder.TransportManager.Transport).relayManager;
            relayManager.joinCode = _lobbyData.RelayJoinCode;

            AsyncOperation op = SceneManager.LoadSceneAsync(_lobbyData.SceneName);
            op.completed += SceneManager_OnSceneLoaded;

            _inGame = true;
        }

        private async void LobbyManager_OnLobbyUpdated(Lobby lobby)
        {
            List<Dictionary<string, PlayerDataObject>> playersData = LobbyManager.Instance.GetPlayersData();
            _lobbyPlayersData.Clear();

            int playersReadyCount = 0;
            foreach (Dictionary<string, PlayerDataObject> data in playersData)
            {
                LobbyPlayerData lobbyPlayerData = new LobbyPlayerData(data);

                if (lobbyPlayerData.IsReady)
                {
                    playersReadyCount += 1;
                }

                if (lobbyPlayerData.Id == AuthenticationService.Instance.PlayerId)
                {
                    _lobbyPlayerData = lobbyPlayerData;
                }

                _lobbyPlayersData.Add(lobbyPlayerData);
            }

            _lobbyData = new LobbyData(lobby.Data);

            OnGameLobbyUpdated?.Invoke();

            if (IsHost)
            {
                OnGameLobbyReady?.Invoke(playersReadyCount == lobby.Players.Count);
            }
            else
            {
                if (!_inGame && !string.IsNullOrEmpty(_lobbyData.RelayJoinCode))
                {
                    await JoinGame();
                }
            }
        }

        private void SceneManager_OnSceneLoaded(AsyncOperation op)
        {
            StartCoroutine(StartConnectionCoroutine());
        }

        private async void FishyRelayManager_OnJoinCode(string relayCode)
        {
            _lobbyData.RelayJoinCode = relayCode;
            await LobbyManager.Instance.UpdateLobbyData(_lobbyData.Serialize());

            FishyRelayManager relayManager = ((FishyUTP)InstanceFinder.TransportManager.Transport).relayManager;
            string allocationId = relayManager.HostAllocation.AllocationId.ToString();
            string connectionData = relayManager.HostAllocation.ConnectionData.ToString();
            await LobbyManager.Instance.UpdatePlayerData(_lobbyPlayerData.Id, _lobbyPlayerData.Serialize(), allocationId, connectionData);
        }

        private IEnumerator StartConnectionCoroutine()
        {
            FishyUTP transport = (FishyUTP)InstanceFinder.TransportManager.Transport;

            if (IsHost)
            {
                // (byte[] allocationId, byte[] key, byte[] connectionData, string ipAddress, int port) = RelayManager.Instance.GetHostConnectionInfo();

                // transport.SetServerBindAddress(transport.relayManager.HostAllocation.RelayServer.IpV4, IPAddressType.IPv4);
                InstanceFinder.ServerManager.StartConnection();

                yield return new WaitUntil(() => InstanceFinder.ServerManager.Started);

                InstanceFinder.ClientManager.StartConnection();
            }
            else
            {
                // (byte[] allocationId, byte[] key, byte[] connectionData, byte[] hostConnectionData, string ipAddress, int port) = RelayManager.Instance.GetClientConnectionInfo();

                yield return new WaitForSeconds(4.0f);

                InstanceFinder.ClientManager.StartConnection();
            }

            yield return null;
        }
    }
}
