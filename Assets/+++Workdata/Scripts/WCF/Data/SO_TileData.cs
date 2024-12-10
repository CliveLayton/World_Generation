using UnityEngine;

[CreateAssetMenu(fileName = "SO_TileData", menuName = "Scriptable Objects/So_TileData")]
public class SO_TileData : ScriptableObject
{
    public enum TileType
    {
        Water,
        Land,
        Coast
    }

    public TileType type;
    public Sprite tileSprite;
    public bool walkable;
    
    //ToDo: variant with spritearray
}
