using System;
using TMPro;
using Unity.Services.Lobbies.Models;
using UnityEngine;
using UnityEngine.UI;

namespace StormDreams
{
    public class LobbyScreen : MonoBehaviour
    {
        [SerializeField]
        private TMP_Text _lobbyCodeText;
        [SerializeField]
        private Button _readyButton;
        [SerializeField]
        private Button _startButton;
        [SerializeField]
        private Image _mapImage;
        [SerializeField]
        private TMP_Text _mapName;
        [SerializeField]
        private Button _leftButton;
        [SerializeField]
        private Button _rightButton;
        [SerializeField]
        private MapSelectionDataSO _mapSelectionDataSO;

        private int _currentMapIndex = 0;

        private void OnEnable()
        {
            _readyButton.onClick.AddListener(OnReadyButtonClicked);

            if (GameLobbyManager.Instance.IsHost)
            {
                _startButton.onClick.AddListener(OnStartButtonClicked);
                _leftButton.onClick.AddListener(OnLeftButtonClicked);
                _rightButton.onClick.AddListener(OnRightButtonClicked);

                GameLobbyManager.OnGameLobbyReady += GameLobbyManager_OnGameLobbyReady;
            }

            GameLobbyManager.OnGameLobbyUpdated += GameLobbyManager_OnGameLobbyUpdated;
        }

        private void OnDisable()
        {
            _readyButton.onClick.RemoveListener(OnReadyButtonClicked);
            _startButton.onClick.RemoveListener(OnStartButtonClicked);
            _leftButton.onClick.RemoveListener(OnLeftButtonClicked);
            _rightButton.onClick.RemoveListener(OnRightButtonClicked);

            GameLobbyManager.OnGameLobbyUpdated -= GameLobbyManager_OnGameLobbyUpdated;
            GameLobbyManager.OnGameLobbyReady -= GameLobbyManager_OnGameLobbyReady;
        }

        private async void Start()
        {
            _lobbyCodeText.text = $"Lobby code: {GameLobbyManager.Instance.GetLobbyCode()}";

            _startButton.gameObject.SetActive(false);

            if (!GameLobbyManager.Instance.IsHost)
            {
                _leftButton.gameObject.SetActive(false);
                _rightButton.gameObject.SetActive(false);
            }
            else
            {
                await GameLobbyManager.Instance.SetSelectedMap(_currentMapIndex, _mapSelectionDataSO.Maps[_currentMapIndex].SceneName);
            }

            UpdateMap();
        }

        private void UpdateMap()
        {
            _mapImage.color = _mapSelectionDataSO.Maps[_currentMapIndex].Thumbnail;
            _mapName.text = _mapSelectionDataSO.Maps[_currentMapIndex].Name;
        }

        private async void OnReadyButtonClicked()
        {
            await GameLobbyManager.Instance.SetPlayerReady();
        }

        private async void OnStartButtonClicked()
        {
            await GameLobbyManager.Instance.StartGame();
        }

        private async void OnLeftButtonClicked()
        {
            if (_currentMapIndex - 1 > 0)
            {
                _currentMapIndex -= 1;
            }
            else
            {
                _currentMapIndex = 0;
            }

            UpdateMap();

            await GameLobbyManager.Instance.SetSelectedMap(_currentMapIndex, _mapSelectionDataSO.Maps[_currentMapIndex].SceneName);
        }

        private async void OnRightButtonClicked()
        {
            if (_currentMapIndex + 1 < _mapSelectionDataSO.Maps.Count - 1)
            {
                _currentMapIndex += 1;
            }
            else
            {
                _currentMapIndex = _mapSelectionDataSO.Maps.Count - 1;
            }

            UpdateMap();

            await GameLobbyManager.Instance.SetSelectedMap(_currentMapIndex, _mapSelectionDataSO.Maps[_currentMapIndex].SceneName);
        }

        private void GameLobbyManager_OnGameLobbyUpdated()
        {
            _currentMapIndex = GameLobbyManager.Instance.GetMapIndex();

            UpdateMap();
        }

        private void GameLobbyManager_OnGameLobbyReady(bool enable)
        {
            _startButton.gameObject.SetActive(enable);
        }
    }
}
