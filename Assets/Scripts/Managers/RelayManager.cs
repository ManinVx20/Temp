using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FishNet;
using FishNet.Transporting.FishyUTPPlugin;
using Unity.Services.Relay;
using Unity.Services.Relay.Models;
using UnityEngine;

namespace StormDreams
{
    public class RelayManager : Singleton<RelayManager>
    {
        private string _joinCode;
        private string _ip;
        private int _port;
        private byte[] _key;
        private System.Guid _allocationId;
        private byte[] _allocationIdBytes;
        private byte[] _connectionData;
        private byte[] _hostConnectionData;

        private void Awake()
        {
            DontDestroyOnLoad(gameObject);
        }

        public string GetAllocationId()
        {
            return _allocationId.ToString();
        }

        public string GetConnectionData()
        {
            return _connectionData.ToString();
        }

        public (byte[] AllocationId, byte[] Key, byte[] ConnectionData, string IpAddress, int Port) GetHostConnectionInfo()
        {
            return (_allocationIdBytes, _key, _connectionData, _ip, _port);
        }

        public (byte[] AllocationId, byte[] Key, byte[] ConnectionData, byte[] HostConnectionData, string IpAddress, int Port) GetClientConnectionInfo()
        {
            return (_allocationIdBytes, _key, _connectionData, _hostConnectionData, _ip, _port);
        }

        public async Task<string> CreateRelay(int maxConnections)
        {
            try
            {
                Allocation allocation = await RelayService.Instance.CreateAllocationAsync(maxConnections);

                _joinCode = await RelayService.Instance.GetJoinCodeAsync(allocation.AllocationId);

                RelayServerEndpoint dtlsEndpoint = allocation.ServerEndpoints.First(conn => conn.ConnectionType == "dtls");
                _ip = dtlsEndpoint.Host;
                _port = dtlsEndpoint.Port;

                _port = allocation.RelayServer.Port;

                _key = allocation.Key;
                _allocationId = allocation.AllocationId;
                _allocationIdBytes = allocation.AllocationIdBytes;
                _connectionData = allocation.ConnectionData;
            }
            catch (RelayServiceException e)
            {
                Debug.Log(e);

                return null;
            }
            
            return _joinCode;
        }

        public async Task<bool> JoinRelay(string joinCode)
        {
            try
            {
                JoinAllocation joinAllocation = await RelayService.Instance.JoinAllocationAsync(joinCode);

                _joinCode = joinCode;

                RelayServerEndpoint dtlsEndpoint = joinAllocation.ServerEndpoints.First(conn => conn.ConnectionType == "dtls");
                _ip = dtlsEndpoint.Host;
                _port = dtlsEndpoint.Port;

                _key = joinAllocation.Key;
                _allocationId = joinAllocation.AllocationId;
                _allocationIdBytes = joinAllocation.AllocationIdBytes;
                _connectionData = joinAllocation.ConnectionData;
                _hostConnectionData = joinAllocation.HostConnectionData;
            }
            catch (RelayServiceException e)
            {
                Debug.Log(e);

                return false;
            }

            return true;
        }
    }
}
