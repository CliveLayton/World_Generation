using System;
using Cinemachine;
using UnityEngine;
using UnityEngine.Serialization;

public class SimpleWorld : MonoBehaviour
{
    [SerializeField] private GameObject prefabCharacter;
    
    [SerializeField] private DungeonTile prefabGras;
    [SerializeField] private DungeonTile prefabMountain;
    [SerializeField] private DungeonTile prefabWater;
    [SerializeField] private DungeonTile prefabTree;

    [SerializeField] private Vector2Int sizeWorld;
    [SerializeField] private float scaleNoise = 0.1f;

    [SerializeField] private float treeNoiseScale = 0.1f;
    
    private void Start()
    {
        GenerateWorld();
    }

    private void GenerateWorld()
    {
        for (int x = 0; x < sizeWorld.x; x++)
        {
            for (int y = 0; y < sizeWorld.y; y++)
            {
                float value = Mathf.PerlinNoise(x * scaleNoise, y *scaleNoise);
                if (value >= 0.3f && value <= 0.7f)
                {
                    Instantiate(prefabGras, new Vector3(x, y), Quaternion.identity);
                }
                else if(value > 0.7f)
                {
                    Instantiate(prefabMountain, new Vector3(x, y), Quaternion.identity);
                }
                else if (value < 0.3f)
                {
                    Instantiate(prefabWater, new Vector3(x, y), Quaternion.identity);
                }
            }
        }
        
        GenerateTrees();
    }

    private void GenerateTrees()
    {
        for (int x = 0; x < sizeWorld.x; x++)
        {
            for (int y = 0; y < sizeWorld.y; y++)
            {
                float treeValue = Mathf.PerlinNoise(x * treeNoiseScale, y * treeNoiseScale);
                if (treeValue >= 0.5f && treeValue <= 0.6f)
                {
                    if (TileCheck(new Vector2Int(x, y)).tileType == DungeonTile.TileType.Gras)
                    {
                        Instantiate(prefabTree, new Vector3(x, y), Quaternion.identity);
                    }
                }
            }
        }
        

        FindObjectOfType<CinemachineVirtualCamera>().m_Follow = Instantiate(prefabCharacter).transform;
    }

    DungeonTile TileCheck(Vector2Int checkPosition)
    {
        RaycastHit2D hit = Physics2D.Raycast(checkPosition.ToVector3() + Vector3.back * 0.2f, Vector3.forward);

        return hit ? hit.transform.GetComponent<DungeonTile>() : null;
    }
}
