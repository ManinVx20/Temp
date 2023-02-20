using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace StormDreams
{
    public class LobbySpawner : MonoBehaviour
    {
        [SerializeField]
        private List<LobbyPlayer> _lobbyPlayers = new List<LobbyPlayer>();

        private void OnEnable()
        {
            GameLobbyManager.OnGameLobbyUpdated += GameLobbyManager_OnGameLobbyUpdated;
        }

        private void OnDisable()
        {
            GameLobbyManager.OnGameLobbyUpdated += GameLobbyManager_OnGameLobbyUpdated;
        }

        private void GameLobbyManager_OnGameLobbyUpdated()
        {
            List<LobbyPlayerData> lobbyPlayersData = GameLobbyManager.Instance.GetPlayers();
            for (int i = 0; i < lobbyPlayersData.Count; i++)
            {
                LobbyPlayerData lobbyPlayerData = lobbyPlayersData[i];
                _lobbyPlayers[i].UpdateData(lobbyPlayerData);
            }
        }
    }
}
