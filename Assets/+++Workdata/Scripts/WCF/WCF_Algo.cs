using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Random = UnityEngine.Random;

public class WCF_Algo : MonoBehaviour
{
   private WorldGrid worldGrid;

   [SerializeField] private SO_Rules rules;

   private int countSetTiles = 0;

   private void Start()
   {
      StartGeneration();
   }

   private void StartGeneration()
   {
      worldGrid = FindObjectOfType<WorldGrid>();
      
      //add loop - check if random tile already set - check if all are set
      while (countSetTiles < worldGrid.GetTotalTileCount())
      {
         BaseTile startingTile = worldGrid.GetRandomBaseTile();
         while (startingTile.tileData)
         {
            startingTile = worldGrid.GetRandomBaseTile();
         }
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
            HashSet<SO_TileData.TileType> possibleNeighbors = CalculatePossibleNeighbors(currentPossibleTileDatas);
            CheckNeighbors(startingTile.gridPosition, possibleNeighbors);
         }
      }
   }

   private void CheckNeighbors(Vector2Int currentPos, HashSet<SO_TileData.TileType> possibleNeighbors)
   {
      CheckNeighborPossibleTiles(currentPos + Vector2Int.up, possibleNeighbors);
      CheckNeighborPossibleTiles(currentPos + Vector2Int.right, possibleNeighbors);
      CheckNeighborPossibleTiles(currentPos + Vector2Int.down, possibleNeighbors);
      CheckNeighborPossibleTiles(currentPos + Vector2Int.left, possibleNeighbors);
   }

   private void CheckNeighborPossibleTiles(Vector2Int currentPos, HashSet<SO_TileData.TileType> possibleNeighbors)
   {
      //one to the left
      BaseTile neighborTile = worldGrid.GetBaseTileAt(currentPos);
      if (!neighborTile || neighborTile.tileData != null)
      {
         return;
      }

      List<SO_TileData> currentPossibleTileDatas = new List<SO_TileData>(neighborTile.possibleTiles);
      
      foreach (SO_TileData tileData in neighborTile.possibleTiles)
      {
         if (possibleNeighbors.Contains(tileData.type))
         {
            continue;
         }

         currentPossibleTileDatas.Remove(tileData);
         
         CheckNeighbors(neighborTile.gridPosition, CalculatePossibleNeighbors(currentPossibleTileDatas));

         if (currentPossibleTileDatas.Count == 1)
         {
            neighborTile.InitTile(tileData);
            countSetTiles++;
            worldGrid.TileWasSet(neighborTile);
         }
      }

      neighborTile.possibleTiles = new List<SO_TileData>(currentPossibleTileDatas);
   }

   HashSet<SO_TileData.TileType> CalculatePossibleNeighbors(List<SO_TileData> listTileData)
   {
      HashSet<SO_TileData.TileType> possibleTileTypes = new HashSet<SO_TileData.TileType>();
      foreach (SO_TileData tileData in listTileData)
      {
         foreach (SO_Rules.Rule rule in rules.Rules)
         {
            if (rule.tile1 == tileData.type)
            {
               possibleTileTypes.Add(rule.tile2);
            }
            else if (rule.tile2 == tileData.type)
            {
               possibleTileTypes.Add(rule.tile1);
            }
         }
      }

      return possibleTileTypes;
   }
}
