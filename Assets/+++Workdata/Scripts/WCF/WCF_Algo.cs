using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Random = UnityEngine.Random;

public class WCF_Algo : MonoBehaviour
{
   private WorldGrid worldGrid;

   [SerializeField] private SO_Rules rules;

   private void Start()
   {
      worldGrid = FindObjectOfType<WorldGrid>();

      BaseTile startingTile = worldGrid.GetRandomBaseTile();
      startingTile.InitTile(startingTile.possibleTiles[Random.Range(0, startingTile.possibleTiles.Count)]);
      //startingTile.possibleTiles.Remove(startingTile.tileData);
      
      foreach (SO_TileData tileData in startingTile.possibleTiles)
      {
         if (tileData == startingTile.tileData)
         {
            continue;
         }

         startingTile.possibleTiles.Remove(tileData);
         HashSet<SO_TileData.TileType> possibleNeighbors = CalculatePossibleNeighbors(startingTile.possibleTiles);
      }
   }

   private void CheckNeighbors(Vector2Int currentPos, HashSet<SO_TileData.TileType> possibleNeighbors)
   {
      //one to the left
      BaseTile neighborTile = worldGrid.GetBaseTileAt(currentPos + Vector2Int.left);
      foreach (SO_TileData tileData in neighborTile.possibleTiles)
      {
         if (possibleNeighbors.Contains(tileData.type))
         {
            continue;
         }

         neighborTile.possibleTiles.Remove(tileData);
         HashSet<SO_TileData.TileType> tempHash = neighborTile.possibleTiles.Select(tile => tile.type).ToHashSet();
         CheckNeighbors(neighborTile.gridPosition, tempHash);
      }
      
      for (int i = 0; i < 4; i++)
      {
         
      }
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
