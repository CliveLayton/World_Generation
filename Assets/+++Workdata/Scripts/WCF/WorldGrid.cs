using System;
using System.Collections.Generic;
using Cinemachine;
using UnityEngine;
using Random = UnityEngine.Random;

public class WorldGrid : MonoBehaviour
{
    [SerializeField] private Vector2Int size;
    [SerializeField] private Transform parent;
    [SerializeField] private CinemachineVirtualCamera vc;
    private BaseTile[,] worldGrid;

    private List<BaseTile> allNotSetTiles = new List<BaseTile>();

    [SerializeField] private BaseTile prefabBaseTile;

    private void Awake()
    {
        worldGrid = new BaseTile[size.x, size.y];
        for (int x = 0; x < size.x; x++)
        {
            for (int y = 0; y < size.y; y++)
            {
                worldGrid[x, y] = Instantiate(prefabBaseTile, new Vector3(x,y), Quaternion.identity, parent);
                worldGrid[x, y].gridPosition = new Vector2Int(x, y);
                allNotSetTiles.Add(worldGrid[x,y]);
            }
        }
        
        Vector3 sumVector = new Vector3(0f, 0f, 0f);

        foreach (Transform child in parent.transform)
        {
            sumVector += child.position;
        }

        Vector3 groupCenter = sumVector / parent.transform.childCount;
        vc.gameObject.transform.position = groupCenter;
        vc.gameObject.transform.position = new Vector3(groupCenter.x, groupCenter.y, -10);
        vc.m_Lens.OrthographicSize = (float)size.x / 2;
    }

    public void TileWasSet(BaseTile setBaseTile)
    {
        allNotSetTiles.Remove(setBaseTile);
    }

    public BaseTile GetBaseTileAt(Vector2Int pos)
    {
        if (pos.x < 0 || pos.y < 0 || pos.x >= size.x || pos.y >= size.y)
        {
            return null;
        }
        return worldGrid[pos.x, pos.y];
    }

    public BaseTile GetRandomBaseTile()
    {
        return allNotSetTiles[Random.Range(0, allNotSetTiles.Count)];
    }

    public int GetTotalTileCount()
    {
        return size.x * size.y;
    }
}
