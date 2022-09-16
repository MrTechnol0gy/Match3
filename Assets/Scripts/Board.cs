using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using UnityEngine;
using Random = UnityEngine.Random;

public class Board : MonoBehaviour
{
    public int width; //sets board width
    public int height; //sets board height

    public GameObject bgTilePrefab;

    public Gem[] gems; //An array to collect gems

    // Start is called before the first frame update
    void Start()
    {
        Setup();
    }

    private void Setup()
    {
        for(int x = 0; x < width; x++)
        {
            for(int y = 0; y < height; y++)
            {
                Vector2 pos = new Vector2(x, y);
                GameObject bgTile = Instantiate(bgTilePrefab, pos, Quaternion.identity);
                bgTile.transform.parent = transform;
                bgTile.name = "BG Tile - " + x + ", " + y; //names the background tiles in the hierarchy based on their location

                int gemToUse = Random.Range(0, gems.Length); //picks a random number

                SpawnGem(new Vector2Int(x, y), gems[gemToUse]);
            }
        }
    } 

    private void SpawnGem(Vector2Int pos, Gem gemToSpawn)
    {
        Gem gem = Instantiate(gemToSpawn, new Vector3(pos.x, pos.y, 0f), Quaternion.identity);
        gem.transform.parent = this.transform;
        gem.name = "Gem - " + pos.x + ", " + pos.y;
    }
}
