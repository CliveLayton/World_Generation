using System;
using UnityEngine;
using Random = UnityEngine.Random;

public class WorldGrid : MonoBehaviour
{
    [SerializeField] private Vector2Int size;
    private BaseTile[,] worldGrid;

    private BaseTile prefabBaseTile;

    private void Start()
    {
        worldGrid = new BaseTile[size.x, size.y];
        for (int x = 0; x < size.x; x++)
        {
            for (int y = 0; y < size.y; y++)
            {
                worldGrid[x, y] = Instantiate(prefabBaseTile, new Vector3(x,y), Quaternion.identity);
                worldGrid[x, y].gridPosition = new Vector2Int(x, y);
            }
        }
    }

    public BaseTile GetBaseTileAt(Vector2Int pos)
    {
        return worldGrid[pos.x, pos.y];
    }

    public BaseTile GetRandomBaseTile()
    {
        return worldGrid[Random.Range(0, size.x), Random.Range(0, size.y)];
    }
}
