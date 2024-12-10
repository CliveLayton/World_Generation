using System;
using UnityEngine;

[CreateAssetMenu(fileName = "SO_Rules", menuName = "Scriptable Objects/SO_Rules")]
public class SO_Rules : ScriptableObject
{
    [Serializable]
    public struct Rule
    {
        public SO_TileData.TileType tile1;
        public SO_TileData.TileType tile2;
    }

    public Rule[] Rules;
}
