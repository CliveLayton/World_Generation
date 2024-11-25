using System;
using System.Collections;
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
     * roomcount (int)
     * 
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
    [SerializeField] private DungeonTile wallTile;
    [SerializeField] private DungeonTile backgroundTile;

    [SerializeField] private bool useSeed;
    [SerializeField] private int seed;

    [SerializeField, Tooltip("X : From; Y: To")] private Vector2Int roomCount;
    
    [SerializeField, Tooltip("X : From; Y: To")] private Vector2Int roomSize;

    [SerializeField, Tooltip("X : From; Y: To")] private Vector2Int possibleExitsRoom;
    
    [SerializeField, Tooltip("X : From; Y: To")] private Vector2Int corridorLength;
    [SerializeField, Range(0f, 1f)] private float corridorCurvines;
    [SerializeField, Range(0f, 1f)] private float corridorBranchingProbability;

    private Transform parentDungeon;
    private int totalRoomCount;
    private int roomCounter = 0;

    private int runningCorountine = 0;
    private bool isGenerationFinished = false;

    [SerializeField] private Vector2Int upperRightCorner;
    [SerializeField] private Vector2Int lowerLeftCorner;

    struct RoomInfo
    {
        public Vector2Int startingPosition;
        public Vector2Int size;
        public int exitCount;
    }
    
    struct CorridorInfo
    {
        public Vector2Int startingPosition;
        public int length;
        public HeavenDirection direction;
        
        /*
        public CorridorInfo(Vector2Int newStartingPoint, HeavenDirection newHeavenDirection)
        {
            startingPosition = newStartingPoint;
            direction = newHeavenDirection;
            length = 0;
        }
        */
    }

    private void Start()
    {
        GenerateDungeon();
    }

    private void GenerateDungeon()
    {
        if (useSeed)
        {
            Random.InitState(seed);
        }

        parentDungeon = new GameObject().transform;
        parentDungeon.gameObject.name = "DungeonParent";

        totalRoomCount = Random.Range(roomCount.x, roomCount.y + 1);

        StartCoroutine(GenerateDungeonRoom(RandomRoomInfo(new Vector2Int(0,0))));
    }

    private void Reset()
    {
        upperRightCorner = new Vector2Int();
        lowerLeftCorner = new Vector2Int();
        StopAllCoroutines();
        Destroy(parentDungeon.gameObject);
        roomCounter = 0;
        isGenerationFinished = false;
    }

    private void Update()
    {
        if (!isGenerationFinished && runningCorountine == 0)
        {
            if (totalRoomCount == roomCounter)
            {
                isGenerationFinished = true;
                StartCoroutine(GenerateBackground());
            }
            else
            {
                Reset();
                GenerateDungeon();
            }
        }
    }

    IEnumerator GenerateDungeonRoom(RoomInfo roomInfo)
    {
        runningCorountine++;
        for (int x = 0; x < roomInfo.size.x; x++)
        {
            for (int y = 0; y < roomInfo.size.y; y++)
            {
                Vector2Int newTilePos = new Vector2Int(x, y) + roomInfo.startingPosition;
                if (TileCheck(newTilePos))
                {
                    continue;
                }
                
                Instantiate(floorTile, new Vector3(x, y) + roomInfo.startingPosition.ToVector3(), Quaternion.identity, parentDungeon);
                
                AdjustMapBoundries(newTilePos);
            }
        }
        yield return null; // if you want a longer pause use new WaifForSeconds(0.2f)

        roomCounter++;
        if (roomCounter > totalRoomCount)
        {
            StopAllCoroutines();
            runningCorountine = 0;
            
            yield break;
        }

        int maxAttempts = 100;  //if time -> add function that calculates possible exits
        for (int i = 0; i < roomInfo.exitCount;)
        {
            CorridorInfo? newCorridorInfo = GetCorridorStartingPoint(roomInfo);

            if (newCorridorInfo != null)
            {
                CorridorInfo ci = (CorridorInfo)newCorridorInfo; //csiMiami
                ci.length = Random.Range(corridorLength.x, corridorLength.y + 1);
                StartCoroutine(GenerateCorridor(ci));
                i++;
            }
            
            maxAttempts--;
            if (maxAttempts <= 0)
            {
                break;
            }
        }

        runningCorountine--;
    }

    CorridorInfo? GetCorridorStartingPoint(RoomInfo roomInfo)
    {
        CorridorInfo corridorInfo = new CorridorInfo();

        //find possible exits
        //corridors are not allowed next to each other
        (Vector2Int, HeavenDirection) value = GetPossibleCorridorPosition(roomInfo.size.x, roomInfo.size.y);
        Vector2Int corridorPosition = roomInfo.startingPosition + value.Item1;
        HeavenDirection direction = value.Item2;
        
        corridorInfo.direction = direction;
        corridorInfo.startingPosition = corridorPosition;

        //CorridorInfo? corridorInfo = new CorridorInfo(corridorPosition, direction);

        if (TileCheck(corridorPosition))
        {
            Debug.Log("here is already an exit");
        }
        else
        {
            Vector2Int checkDirection = (direction == HeavenDirection.North || direction == HeavenDirection.South)
                ? Vector2Int.right
                : Vector2Int.up;
            if (TileCheck(corridorPosition + checkDirection) || TileCheck(corridorPosition - checkDirection))
            {
                Debug.Log("here is already an exit next to the position");
            }
            else
            {
                return corridorInfo;
            }
        }

        return null;
    }

    IEnumerator GenerateCorridor(CorridorInfo corridorInfo)
    {
        runningCorountine++;
        Vector2Int currentPos = corridorInfo.startingPosition;
        HeavenDirection currentDirection = corridorInfo.direction;
        
        for (int i = 0; i < corridorInfo.length; i++)
        {
            if (TileCheck(currentPos))
            {
                runningCorountine--;
                yield break;
            }
            
            Instantiate(floorTile, currentPos.ToVector3(), Quaternion.identity, parentDungeon);
            
            AdjustMapBoundries(currentPos);

            if (currentDirection == HeavenDirection.East || currentDirection == HeavenDirection.West)
            {
                Instantiate(wallTile, (currentPos + Vector2Int.up).ToVector3(), Quaternion.identity, parentDungeon);
            }

            if (Random.value < corridorCurvines)
            {
                if (Random.value > 0.5f)
                {
                    //Curve to right
                    currentDirection++;
                }
                else
                {
                    //Curve to left
                    currentDirection--;
                }

                if ((int)currentDirection == 4)
                {
                    currentDirection = HeavenDirection.North;
                }
                else if ((int) currentDirection == -1)
                {
                    currentDirection = HeavenDirection.West;
                }
            }

            currentPos += HeavenDirectionToVector(currentDirection);
            
        }

        yield return null; 
        StartCoroutine(GenerateDungeonRoom(RandomRoomInfo(currentPos)));
        runningCorountine--;
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

    Vector2Int HeavenDirectionToVector(HeavenDirection heavenDirection)
    {
        switch (heavenDirection)
        {
            case HeavenDirection.North:
                return Vector2Int.up;
            case HeavenDirection.East:
                return Vector2Int.right;
            case HeavenDirection.South:
                return Vector2Int.down;
            case HeavenDirection.West:
                return Vector2Int.left;
            default:
                return Vector2Int.zero;
        }

        //another possible way to write the switch statement above
        /*
         return heavenDirection switch
        {
            HeavenDirection.North => Vector2Int.up,
            HeavenDirection.East => Vector2Int.right,
            HeavenDirection.South => Vector2Int.down,
            HeavenDirection.West => Vector2Int.left,
            _ => Vector2Int.zero
        };
        */
    }

    DungeonTile TileCheck(Vector2Int checkPosition)
    {
        RaycastHit2D hit = Physics2D.Raycast(checkPosition.ToVector3() + Vector3.back * 0.2f, Vector3.forward);

        return hit ? hit.transform.GetComponent<DungeonTile>() : null;
    }

    RoomInfo RandomRoomInfo(Vector2Int startingPoint)
    {
        return new RoomInfo {
            startingPosition = startingPoint,
            size = new Vector2Int(Random.Range(roomSize.x, roomSize.y), Random.Range(roomSize.x, roomSize.y)),
            exitCount = Random.Range(possibleExitsRoom.x, possibleExitsRoom.y +1) //we add one on the y because random range is max excluded
        };
    }

    private void AdjustMapBoundries(Vector2Int currentPoint)
    {
        upperRightCorner.x = Mathf.Max(upperRightCorner.x, currentPoint.x);
        lowerLeftCorner.x = Mathf.Min(lowerLeftCorner.x, currentPoint.x);
        
        upperRightCorner.y = Mathf.Max(upperRightCorner.y, currentPoint.y);
        lowerLeftCorner.y = Mathf.Min(lowerLeftCorner.y, currentPoint.y);
    }

    IEnumerator GenerateBackground()
    {
        for (int x = lowerLeftCorner.x - 5; x < upperRightCorner.x + 5; x++)
        {
            for (int y = lowerLeftCorner.y - 5; y < upperRightCorner.y + 5; y++)
            {
                Vector2Int currentPos = new Vector2Int(x, y);
                DungeonTile dt = TileCheck(currentPos);
                if (dt && dt.walkable)
                {
                    if (!TileCheck(currentPos + Vector2Int.up))
                    { 
                        Instantiate(wallTile, (currentPos + Vector2Int.up).ToVector3(), Quaternion.identity, parentDungeon);
                    }
                }
                else if(!dt)
                {
                    Instantiate(backgroundTile, currentPos.ToVector3(), Quaternion.identity, parentDungeon);
                }
            }
            yield return null;
        }
        
        SetCameraToCenter();
    }

    private void SetCameraToCenter()
    {
        Vector3 sumVector = new Vector3(0f, 0f, 0f);

        foreach (Transform child in parentDungeon.transform)
        {
            sumVector += child.position;
        }

        Vector3 groupCenter = sumVector / parentDungeon.transform.childCount;
        Camera.main.transform.position = groupCenter;
        Camera.main.transform.position = new Vector3(groupCenter.x, groupCenter.y, -10);
    }
}

public static class Extentions
{
    public static Vector3 ToVector3(this Vector2Int vector2Int)
    {
        return new Vector3(vector2Int.x, vector2Int.y);
    }
}
