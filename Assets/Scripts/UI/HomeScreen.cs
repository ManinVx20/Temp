using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace StormDreams
{
    public class HomeScreen : MonoBehaviour
    {
        [SerializeField]
        private TMP_InputField _lobbyCodeInputField;
        [SerializeField]
        private Button _hostButton;
        [SerializeField]
        private Button _joinButton;

        private void OnEnable()
        {
            _hostButton.onClick.AddListener(OnHostButtonClicked);
            _joinButton.onClick.AddListener(OnJoinButtonClicked);
        }

        private void OnDisable()
        {
            _hostButton.onClick.RemoveListener(OnHostButtonClicked);
            _joinButton.onClick.RemoveListener(OnJoinButtonClicked);
        }

        private async void OnHostButtonClicked()
        {
            bool status = await GameLobbyManager.Instance.CreateLobby();
            if (status)
            {
                SceneManager.LoadSceneAsync("Lobby");
            }

        }

        private async void OnJoinButtonClicked()
        {
            string lobbyCode = _lobbyCodeInputField.text;
            // lobbyCode = lobbyCode.Substring(0, lobbyCode.Length - 1);

            bool status = await GameLobbyManager.Instance.JoinLobby(lobbyCode);
            if (status)
            {
                SceneManager.LoadSceneAsync("Lobby");
            }
        }
    }
}
