using UnityEngine;

[CreateAssetMenu(fileName = "SO_TileData", menuName = "Scriptable Objects/So_TileData")]
public class SO_TileData : ScriptableObject
{
    public enum TileType
    {
        Water,
        Land,
        Coast,
        ShallowWater,
        InnerLand,
        SpecialLand,
        CliffEdgeTopLeft,
        CliffEdgeTopRight,
        CliffEdgeDownLeft,
        CliffEdgeDownRight,
        CliffEdgeVerticalLeft,
        CliffEdgeVerticalRight,
        CliffEdgeHorizontalTop,
        CliffEdgeHorizontalDown
    }

    public TileType type;
    public Sprite tileSprite;
    public bool walkable;
    
    //ToDo: variant with spritearray
}
