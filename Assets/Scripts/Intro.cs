using ParrelSync;
using Unity.Services.Authentication;
using Unity.Services.Core;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace StormDreams
{
    public class Intro : MonoBehaviour
    {
        private async void Start()
        {
            string profileName = "Server";

#if UNITY_EDITOR
            if (ClonesManager.IsClone())
            {
                profileName = ClonesManager.GetArgument();
            }
#endif

            InitializationOptions initializationOptions = new InitializationOptions();
            initializationOptions.SetProfile(profileName);

            await UnityServices.InitializeAsync(initializationOptions);

            if (UnityServices.State == ServicesInitializationState.Initialized)
            {
                AuthenticationService.Instance.SignedIn += AuthenticationService_SignedIn;

                await AuthenticationService.Instance.SignInAnonymouslyAsync();

                if (AuthenticationService.Instance.IsSignedIn)
                {
                    string username = PlayerPrefs.GetString("Username");
                    if (string.IsNullOrEmpty(username))
                    {
                        username = $"Player_{AuthenticationService.Instance.Profile}";
                        PlayerPrefs.SetString("Username", username);
                    }
                }
            }
        }

        private void AuthenticationService_SignedIn()
        {
            Debug.Log("Signed in.");
            Debug.Log($"Profile: {AuthenticationService.Instance.Profile}");
            Debug.Log($"PlayerId: {AuthenticationService.Instance.PlayerId}");

            SceneManager.LoadSceneAsync("Home");
        }
    }
}
