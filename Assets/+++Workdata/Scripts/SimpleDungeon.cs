using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;
using Random = UnityEngine.Random;

public class SimpleDungeon : MonoBehaviour
{
    enum HeavenDirection
    {
        North,
        East,
        South,
        West
    }
    
    //important parameters
    /*
     * roomsize (Vector2int)
     * room exits (Vector2int)
     * corridor curve (bool)
     * branching corridors (bool)
     * probabillity (if a corridor branches - float 0-1)
     *
     * corridor length (Vector2int)
     *
     * start in room or corridor
     *
     * non walkable areas in rooms (float) -> but all exits have to be reachable
     *
     * door location
     */

    [SerializeField] private DungeonTile floorTile;

    [SerializeField, Tooltip("X : From; Y: To")] private Vector2Int roomSize;

    private Transform parentDungeon;

    private void Start()
    {
        parentDungeon = new GameObject().transform;
        parentDungeon.gameObject.name = "DungeonParent";
        
        StartCoroutine(GenerateDungeonRoom(new Vector2Int(0,0),
            Random.Range(roomSize.x, roomSize.y), Random.Range(roomSize.x, roomSize.y))); 
    }

    IEnumerator GenerateDungeonRoom(Vector2Int startingPosition, int xSize, int ySize)
    {
        for (int x = 0; x < xSize; x++)
        {
            for (int y = 0; y < ySize; y++)
            {
                DungeonTile dungeonTile = Instantiate(floorTile, new Vector3(x, y), Quaternion.identity, parentDungeon);
                dungeonTile.walkable = true;
                yield return null; // if you want a longer pause use new WaifForSeconds(0.2f)
            }
        }

        for (int i = 0; i < 100; i++)
        {
            //find possible exits
            //corridors are not allowed next to each other
            (Vector2Int, HeavenDirection) value = GetPossibleCorridorPosition(xSize, ySize);
            Vector2Int corridorPosition = startingPosition + value.Item1;
            HeavenDirection direction = value.Item2;

            if (Physics2D.Raycast(corridorPosition.ToVector3() + Vector3.back * 0.2f, Vector3.forward))
            {
                Debug.Log("here is already an exit");
            }
            else
            {
                Vector3 checkDirection = (direction == HeavenDirection.North || direction == HeavenDirection.South)
                    ? Vector3.right
                    : Vector3.up;
                if (Physics2D.Raycast(corridorPosition.ToVector3() + checkDirection + Vector3.back * 0.2f, Vector3.forward)
                    || Physics2D.Raycast(corridorPosition.ToVector3() - checkDirection + Vector3.back * 0.2f, Vector3.forward))
                {
                    Debug.Log("here is already an exit next to the position");
                }
                else
                {
                    Instantiate(floorTile, new Vector3(corridorPosition.x, corridorPosition.y), Quaternion.identity, parentDungeon);
                }
            }
        }
        
    }

    (Vector2Int, HeavenDirection) GetPossibleCorridorPosition(int x, int y)
    {
        Vector2Int[] possiblePositions =
        {
            new (Random.Range(0, x), y),
            new (x , Random.Range(0, y)),
            new (Random.Range(0, x),  - 1),
            new (- 1, Random.Range(0, y))
        };
        int ranNumber = Random.Range(0, possiblePositions.Length);
        
        return (possiblePositions[ranNumber], (HeavenDirection) ranNumber);
    }
    
}

public static class Extentions
{
    public static Vector3 ToVector3(this Vector2Int vector2Int)
    {
        return new Vector3(vector2Int.x, vector2Int.y);
    }
}
