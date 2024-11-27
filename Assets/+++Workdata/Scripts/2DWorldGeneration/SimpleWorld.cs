using System;
using Cinemachine;
using UnityEngine;

public class SimpleWorld : MonoBehaviour
{
    [SerializeField] private GameObject prefabCharacter;
    
    [SerializeField] private DungeonTile prefabFloor;
    [SerializeField] private DungeonTile prefabWall;
    [SerializeField] private DungeonTile prefabWater;

    [SerializeField] private Vector2Int sizeWorld;
    [SerializeField] private float scaleNoise = 0.1f;

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
                    Instantiate(prefabFloor, new Vector3(x, y), Quaternion.identity);
                }
                else if(value > 0.7f)
                {
                    Instantiate(prefabWall, new Vector3(x, y), Quaternion.identity);
                }
                else if (value < 0.3f)
                {
                    Instantiate(prefabWater, new Vector3(x, y), Quaternion.identity);
                }
            }
        }

        FindObjectOfType<CinemachineVirtualCamera>().m_Follow = Instantiate(prefabCharacter).transform;
    }
}
