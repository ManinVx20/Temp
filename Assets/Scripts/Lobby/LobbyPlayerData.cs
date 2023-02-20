using System.Collections.Generic;
using Unity.Services.Lobbies.Models;

public class LobbyPlayerData
{
    private string _id;
    private string _gamerTag;
    private bool _isReady;

    public string Id => _id;
    public string GamerTag => _gamerTag;
    public bool IsReady
    {
        get => _isReady;
        set => _isReady = value;
    }

    public LobbyPlayerData(string id, string gamerTag)
    {
        _id = id;
        _gamerTag = gamerTag;
    }

    public LobbyPlayerData(Dictionary<string, PlayerDataObject> data)
    {
        UpdateState(data);
    }

    public void UpdateState(Dictionary<string, PlayerDataObject> data)
    {
        if (data.ContainsKey("Id"))
        {
            _id = data["Id"].Value;
        }
        if (data.ContainsKey("GamerTag"))
        {
            _gamerTag = data["GamerTag"].Value;
        }
        if (data.ContainsKey("IsReady"))
        {
            _isReady = data["IsReady"].Value == "True";
        }
    }

    public Dictionary<string, string> Serialize()
    {
        return new Dictionary<string, string>()
        {
            { "Id", _id },
            { "GamerTag", _gamerTag },
            { "IsReady", _isReady.ToString() }
        };
    }
}
