using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Services.Lobbies.Models;
using UnityEngine;

public class LobbyData
{
    private int _mapIndex;
    private string _sceneName;
    private string _relayJoinCode;

    public int MapIndex
    {
        get => _mapIndex;
        set => _mapIndex = value;
    }

    public string SceneName
    {
        get => _sceneName;
        set => _sceneName = value;
    }

    public string RelayJoinCode
    {
        get => _relayJoinCode;
        set => _relayJoinCode = value;
    }

    public LobbyData(int mapIndex)
    {
        _mapIndex = mapIndex;
    }

    public LobbyData(Dictionary<string, DataObject> lobbyData)
    {
        UpdateState(lobbyData);
    }

    public void UpdateState(Dictionary<string, DataObject> lobbyData)
    {
        if (lobbyData.ContainsKey("MapIndex"))
        {
            _mapIndex = Int32.Parse(lobbyData["MapIndex"].Value);
        }

        if (lobbyData.ContainsKey("SceneName"))
        {
            _sceneName = lobbyData["SceneName"].Value;
        }

        if (lobbyData.ContainsKey("RelayJoinCode"))
        {
            _relayJoinCode = lobbyData["RelayJoinCode"].Value;
        }
    }

    public Dictionary<string, string> Serialize()
    {
        return new Dictionary<string, string>()
        {
            { "MapIndex", _mapIndex.ToString() },
            { "SceneName", _sceneName },
            { "RelayJoinCode", _relayJoinCode }
        };
    }
}
