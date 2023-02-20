using System;
using System.Collections;
using System.Collections.Generic;
using FishNet;
using UnityEngine;

namespace StormDreams
{
    public class GameManager : Singleton<GameManager>
    {
        private void Awake()
        {
            DontDestroyOnLoad(gameObject);
        }

        private void Start()
        {
            // NetworkManager.Singleton.NetworkConfig.ConnectionApproval = true;

            // if (RelayManager.Instance.IsHost)
            // {
            //     NetworkManager.Singleton.ConnectionApprovalCallback = NetworkManager_ConnectionApprovalCallback;
            //     (byte[] allocationId, byte[] key, byte[] connectionData, string ipAddress, int port) = RelayManager.Instance.GetHostConnectionInfo();
            //     NetworkManager.Singleton.GetComponent<UnityTransport>().SetHostRelayData(ipAddress, (ushort)port, allocationId, key, connectionData, true);
            //     NetworkManager.Singleton.StartHost();
            // }
            // else
            // {
            //     (byte[] allocationId, byte[] key, byte[] connectionData, byte[] hostConnectionData, string ipAddress, int port) = RelayManager.Instance.GetClientConnectionInfo();
            //     NetworkManager.Singleton.GetComponent<UnityTransport>().SetClientRelayData(ipAddress, (ushort)port, allocationId, key, connectionData, hostConnectionData, true);
            //     NetworkManager.Singleton.StartClient();
            // }
        }

        // private void NetworkManager_ConnectionApprovalCallback(NetworkManager.ConnectionApprovalRequest request, NetworkManager.ConnectionApprovalResponse response)
        // {
        //     response.Approved = true;
        //     response.CreatePlayerObject = true;
        //     response.Pending = false;
        // }
    }
}
