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

    public Gem[,] allGems; //stores an x and y value for a gem

    public float gemSpeed;

    private MatchFinder matchFind;

    private void Awake()
    {
        matchFind = FindObjectOfType<MatchFinder>();
    }

    // Start is called before the first frame update
    void Start()
    {
        allGems = new Gem[width, height];

        Setup();

        
    }

    private void Update()
    {
        matchFind.FindAllMatches();
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
        gem.transform.parent = transform;
        gem.name = "Gem - " + pos.x + ", " + pos.y; //names the gem in the hierarchy based on its location
        allGems[pos.x, pos.y] = gem; //stores the location of the gem that just spawned

        gem.SetupGem(pos, this);
    }
}
