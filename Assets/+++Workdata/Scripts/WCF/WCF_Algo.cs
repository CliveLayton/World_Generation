using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Random = UnityEngine.Random;

public class WCF_Algo : MonoBehaviour
{
   private enum Direction
   {
      Top, 
      Right, 
      Down,
      Left
   }
   
   private WorldGrid worldGrid;

   [SerializeField] private SO_Rules rules;

   private int countSetTiles = 0;
   private List<BaseTile> currentPath = new List<BaseTile>();

   private void Start()
   {
      StartGeneration();
   }

   async Awaitable StartGeneration()
   {
      worldGrid = FindAnyObjectByType<WorldGrid>();
      
      //add loop - check if random tile already set - check if all are set
      while (countSetTiles < worldGrid.GetTotalTileCount())
      {
         BaseTile startingTile = worldGrid.GetRandomBaseTile();
         while (startingTile.tileData)
         {
            startingTile = worldGrid.GetRandomBaseTile();
         }
         
         currentPath.Clear();
         currentPath.Add(startingTile);
         UpdateLine();
         
         startingTile.InitTile(startingTile.possibleTiles[Random.Range(0, startingTile.possibleTiles.Count)]);
         countSetTiles++;
         worldGrid.TileWasSet(startingTile);
         //startingTile.possibleTiles.Remove(startingTile.tileData);

         List<SO_TileData> currentPossibleTileDatas = new List<SO_TileData>(startingTile.possibleTiles);
      
         for (int i = 0; i < startingTile.possibleTiles.Count; i++)
         {
            if (startingTile.possibleTiles[i] == startingTile.tileData)
            {
               continue;
            }

            currentPossibleTileDatas.Remove(startingTile.possibleTiles[i]);
            HashSet<SO_TileData.TileType> possibleNeighbors = CalculatePossibleNeighbors(currentPossibleTileDatas, Direction.Top, true);
            await CheckNeighbors(startingTile.gridPosition, possibleNeighbors);
         }

         //startingTile.possibleTiles = new List<SO_TileData>(currentPossibleTileDatas);
      }
      
      //put things here that should happen, if the generation is finished
      Destroy(lineRenderer.gameObject);
   }

   async Awaitable CheckNeighbors(Vector2Int currentPos, HashSet<SO_TileData.TileType> possibleNeighbors)
   {
      await CheckNeighborPossibleTiles(currentPos + Vector2Int.up, possibleNeighbors, Direction.Top);
      await CheckNeighborPossibleTiles(currentPos + Vector2Int.right, possibleNeighbors, Direction.Right);
      await CheckNeighborPossibleTiles(currentPos + Vector2Int.down, possibleNeighbors, Direction.Down);
      await CheckNeighborPossibleTiles(currentPos + Vector2Int.left, possibleNeighbors, Direction.Left);
   }

   async Awaitable CheckNeighborPossibleTiles(Vector2Int currentPos, HashSet<SO_TileData.TileType> possibleNeighbors, Direction direction)
   {
      BaseTile neighborTile = worldGrid.GetBaseTileAt(currentPos);
      if (!neighborTile || neighborTile.tileData != null || currentPath.Contains(neighborTile))
      {
         return;
      }
      
      currentPath.Add(neighborTile);
      UpdateLine();
      if (visualize)
      {
         await Awaitable.WaitForSecondsAsync(waitTime);
      }

      List<SO_TileData> currentPossibleTileDatas = new List<SO_TileData>(neighborTile.possibleTiles);
      
      foreach (SO_TileData tileData in neighborTile.possibleTiles)
      {
         if (possibleNeighbors.Contains(tileData.type))
         {
            continue;
         }

         currentPossibleTileDatas.Remove(tileData);
         
         await CheckNeighbors(neighborTile.gridPosition, CalculatePossibleNeighbors(currentPossibleTileDatas, direction, false));

         if (currentPossibleTileDatas.Count == 1)
         {
            neighborTile.InitTile(currentPossibleTileDatas[0]);
            countSetTiles++;
            worldGrid.TileWasSet(neighborTile);
         }
      }

      neighborTile.possibleTiles = new List<SO_TileData>(currentPossibleTileDatas);

      currentPath.Remove(neighborTile);
   }

   HashSet<SO_TileData.TileType> CalculatePossibleNeighbors(List<SO_TileData> listTileData, Direction direction, bool fallThrough)
   {
      HashSet<SO_TileData.TileType> possibleTileTypes = new HashSet<SO_TileData.TileType>();
      foreach (SO_TileData tileData in listTileData)
      {
         switch (direction)
         {
            case Direction.Top:
               foreach (SO_Rules.RuleTop rule in rules.RulesTop)
               {
                  if (rule.tile1 == tileData.type)
                  {
                     possibleTileTypes.Add(rule.tile2);
                  }
                  // else if (rule.tile2 == tileData.type)
                  // {
                  //    possibleTileTypes.Add(rule.tile1);
                  // }
               }
               if (fallThrough) goto case Direction.Right;
               
               break;
            case Direction.Right:
               foreach (SO_Rules.RuleRight rule in rules.RulesRight)
               {
                  if (rule.tile1 == tileData.type)
                  {
                     possibleTileTypes.Add(rule.tile2);
                  }
                  // else if (rule.tile2 == tileData.type)
                  // {
                  //    possibleTileTypes.Add(rule.tile1);
                  // }
               }
               if (fallThrough) goto case Direction.Down;
               
               break;
            case Direction.Down:
               foreach (SO_Rules.RuleDown rule in rules.RulesDown)
               {
                  if (rule.tile1 == tileData.type)
                  {
                     possibleTileTypes.Add(rule.tile2);
                  }
                  // else if (rule.tile2 == tileData.type)
                  // {
                  //    possibleTileTypes.Add(rule.tile1);
                  // }
               }
               if (fallThrough) goto case Direction.Left;
               
               break;
            case Direction.Left:
               foreach (SO_Rules.RuleLeft rule in rules.RulesLeft)
               {
                  if (rule.tile1 == tileData.type)
                  {
                     possibleTileTypes.Add(rule.tile2);
                  }
                  // else if (rule.tile2 == tileData.type)
                  // {
                  //    possibleTileTypes.Add(rule.tile1);
                  // }
               }
               break;
         }
         
      }

      return possibleTileTypes;
   }

   [SerializeField] private bool visualize;
   [SerializeField] private float waitTime;
   [SerializeField] private LineRenderer lineRenderer;
   

   void UpdateLine()
   {
      if (!visualize)
      {
         return;
      }
      
      Vector3[] points = currentPath.Select(item => item.gridPosition.ToVector3()).ToArray();

      lineRenderer.positionCount = points.Length;
      lineRenderer.SetPositions(points);
   }
}
