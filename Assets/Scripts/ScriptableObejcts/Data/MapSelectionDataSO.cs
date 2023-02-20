using System;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]
public class MapSelectionDataSO : ScriptableObject
{
    public List<MapInfo> Maps;
}

[Serializable]
public struct MapInfo
{
    public Color Thumbnail;
    public string Name;
    public string SceneName;
}