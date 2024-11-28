using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class DungeonTile : MonoBehaviour
{
    public enum TileType
    {
        Water,
        Gras,
        Mountain,
        Tree,
        Floor,
        Wall,
        Background
    }
    
    [SerializeField] private Sprite[] possibleTileSprite;
    [SerializeField, Range(0, 1)] private float probalilityDifferentSprite;
    public bool walkable;
    public TileType tileType;

    private void Start()
    {
        if (possibleTileSprite.Length > 0)
        {
            if (Random.value < probalilityDifferentSprite)
            {
                GetComponent<SpriteRenderer>().sprite = possibleTileSprite[Random.Range(1, possibleTileSprite.Length)];
            }
            else
            {
                GetComponent<SpriteRenderer>().sprite = possibleTileSprite[0];
            }
        }
    }
}
