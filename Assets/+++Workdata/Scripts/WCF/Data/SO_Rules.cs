using System;
using UnityEngine;
using UnityEngine.Serialization;

[CreateAssetMenu(fileName = "SO_Rules", menuName = "Scriptable Objects/SO_Rules")]
public class SO_Rules : ScriptableObject
{
    [Serializable]
    public struct RuleTop
    {
        public SO_TileData.TileType tile1;
        public SO_TileData.TileType tile2;
    }

    [Serializable]
    public struct RuleDown
    {
        public SO_TileData.TileType tile1;
        public SO_TileData.TileType tile2;
    }
    
    [Serializable]
    public struct RuleLeft
    {
        public SO_TileData.TileType tile1;
        public SO_TileData.TileType tile2;
    }
    
    [Serializable]
    public struct RuleRight
    {
        public SO_TileData.TileType tile1;
        public SO_TileData.TileType tile2;
    }

    [FormerlySerializedAs("Rules")] public RuleTop[] RulesTop;
    public RuleDown[] RulesDown;
    public RuleLeft[] RulesLeft;
    public RuleRight[] RulesRight;
}
