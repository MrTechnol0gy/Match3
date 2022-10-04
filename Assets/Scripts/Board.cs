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

    [HideInInspector]
    public MatchFinder matchFind;

    public enum BoardState { wait, move}
    public BoardState currentState = BoardState.move;

    public Gem bomb;
    public float bombChance = 2f;

    [HideInInspector]
    public RoundManager roundMan;

    private float bonusMulti;
    public float bonusAmount = .5f;

    private BoardLayout boardLayout;
    private Gem[,] layoutStore;

    private void Awake()
    {
        matchFind = FindObjectOfType<MatchFinder>(); //finds the hierarchy matchfinding item
        roundMan = FindObjectOfType<RoundManager>();
        boardLayout = GetComponent<BoardLayout>();
    }

    // Start is called before the first frame update
    void Start()
    {
        allGems = new Gem[width, height];

        layoutStore = new Gem[width, height];

        Setup();

    }

    private void Update()
    {
        //matchFind.FindAllMatches();

        //checks for a keypress and calls the ShuffleBoard function
        if(Input.GetKeyDown(KeyCode.S))
        {
            ShuffleBoard();
        }
    }

    private void Setup()
    {
        if(boardLayout != null)
        {
            layoutStore = boardLayout.GetLayout();
        }

        for(int x = 0; x < width; x++)
        {
            for(int y = 0; y < height; y++)
            {
                Vector2 pos = new Vector2(x, y);
                GameObject bgTile = Instantiate(bgTilePrefab, pos, Quaternion.identity);
                bgTile.transform.parent = transform;
                bgTile.name = "BG Tile - " + x + ", " + y; //names the background tiles in the hierarchy based on their location

                if (layoutStore[x, y] != null) //checks for a custom layout
                {
                    SpawnGem(new Vector2Int(x, y), layoutStore[x, y]); //spawns the gems from the custom layout
                }
                else
                {

                    int gemToUse = Random.Range(0, gems.Length); //picks a random number

                    int iterations = 0; //an escape clause for the while(MatchesAt) code below
                                        //checks for matches at board creation and prevents them from occurring 
                    while (MatchesAt(new Vector2Int(x, y), gems[gemToUse]) && iterations < 100)
                    {
                        gemToUse = Random.Range(0, gems.Length);
                        iterations++;
                    }

                    SpawnGem(new Vector2Int(x, y), gems[gemToUse]);
                }
            }
        }
    } 

    private void SpawnGem(Vector2Int pos, Gem gemToSpawn)
    {
        if(Random.Range(0f, 100f) <= bombChance)
        {
            gemToSpawn = bomb;
        }

        Gem gem = Instantiate(gemToSpawn, new Vector3(pos.x, pos.y + height, 0f), Quaternion.identity); //spawns gems from the top of the screen
        gem.transform.parent = transform;
        gem.name = "Gem - " + pos.x + ", " + pos.y; //names the gem in the hierarchy based on its location
        allGems[pos.x, pos.y] = gem; //stores the location of the gem that just spawned

        gem.SetupGem(pos, this);
    }

    bool MatchesAt(Vector2Int posToCheck, Gem gemToCheck)
    {
        if(posToCheck.x > 1)
        {
            if (allGems[posToCheck.x - 1, posToCheck.y].type == gemToCheck.type && allGems[posToCheck.x - 2, posToCheck.y].type == gemToCheck.type)
            {
                return true;
            }
        }
        if (posToCheck.y > 1)
        {
            if (allGems[posToCheck.x, posToCheck.y - 1].type == gemToCheck.type && allGems[posToCheck.x, posToCheck.y - 2].type == gemToCheck.type)
            {
                return true;
            }
        }

        return false;
    }

    private void DestroyMatchedGemAt(Vector2Int pos)
    {
        if (allGems[pos.x, pos.y] != null) //checks to see if the gem exists
        {
            if (allGems[pos.x, pos.y].isMatched)
            {
                if (allGems[pos.x, pos.y].type == Gem.GemType.bomb)
                {
                    SFXManager.instance.PlayExplode();
                }
                else if (allGems[pos.x, pos.y].type == Gem.GemType.stone)
                {
                    SFXManager.instance.PlayStoneBreak();
                }
                else
                {
                    SFXManager.instance.PlayGemBreak();
                }

                Instantiate(allGems[pos.x, pos.y].destroyEffect, new Vector2(pos.x, pos.y), Quaternion.identity);

                Destroy(allGems[pos.x, pos.y].gameObject);
                allGems[pos.x, pos.y] = null; //removes references to the destroyed gems
            }
        }
    }

    public void DestroyMatches()
    {
        for(int i = 0; i < matchFind.currentMatches.Count; i++)
        {
            if(matchFind.currentMatches[i] != null)
            {
                ScoreCheck(matchFind.currentMatches[i]); //adds score for destroyed gem

                DestroyMatchedGemAt(matchFind.currentMatches[i].posIndex);
            }
        }

        StartCoroutine(DecreaseRowCo());
    }

    private IEnumerator DecreaseRowCo()
    {
        yield return new WaitForSeconds(.2f); //puts a delay after gems are destroyed before gems fall to fill spaces

        int nullCounter = 0;

        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                if (allGems[x,y] == null)
                {
                    nullCounter++;
                }
                else if (nullCounter > 0)
                {
                    allGems[x, y].posIndex.y -= nullCounter;
                    yield return new WaitForSeconds(.005f); //adds a slight delay between each gem falling to fill spaces below
                    allGems[x, y - nullCounter] = allGems[x, y];
                    allGems[x, y] = null;
                }
            }

            nullCounter = 0;
        }

        StartCoroutine(FillBoardCo());
    }

    private IEnumerator FillBoardCo()
    {
        yield return new WaitForSeconds(.5f);
        RefillBoard();

        yield return new WaitForSeconds(.5f);

        //checks for new matches created by filling the board
        matchFind.FindAllMatches();

        if(matchFind.currentMatches.Count > 0)
        {
            bonusMulti++; //increase the bonus multiplier

            yield return new WaitForSeconds(.25f); //creates a small delay before gems fill the board
            DestroyMatches();
        }
        else
        {
            yield return new WaitForSeconds(.5f);
            currentState = BoardState.move;

            bonusMulti = 0f; //resets the bonus multiplier
        }
    }

    private void RefillBoard()
    {
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                if (allGems[x, y] == null)
                {
                    int gemToUse = Random.Range(0, gems.Length);

                    SpawnGem(new Vector2Int(x, y), gems[gemToUse]);
                }
            }
        }
    }

    //checks for edge cases of gems that shouldn't exist
    private void CheckMisplacedGems()
    {
        List<Gem> foundGems = new List<Gem>();

        foundGems.AddRange(FindObjectsOfType<Gem>());

        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                if (foundGems.Contains(allGems[x, y]))
                {
                    foundGems.Remove(allGems[x, y]);
                }
            }
        }

        foreach(Gem ge in foundGems)
        {
            Destroy(ge.gameObject);
        }
    }

    public void ShuffleBoard()
    {
        if (currentState != BoardState.wait)
        {
            currentState = BoardState.wait;

            List<Gem> gemsFromBoard = new List<Gem>();

            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < height; y++)
                {
                    gemsFromBoard.Add(allGems[x, y]);
                    allGems[x, y] = null;
                }
            }

            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < height; y++)
                {
                    int gemToUse = Random.Range(0, gemsFromBoard.Count);

                    int iterations = 0;

                    while(MatchesAt(new Vector2Int(x, y), gemsFromBoard[gemToUse]) && iterations < 100 && gemsFromBoard.Count > 1)
                    {
                        gemToUse = Random.Range(0, gemsFromBoard.Count);
                        iterations++;
                    }

                    gemsFromBoard[gemToUse].SetupGem(new Vector2Int(x, y), this);
                    allGems[x, y] = gemsFromBoard[gemToUse];
                    gemsFromBoard.RemoveAt(gemToUse);
                }
            }

            StartCoroutine(FillBoardCo());
        }
    }

    public void ScoreCheck(Gem gemToCheck)
    {
        roundMan.currentScore += gemToCheck.scoreValue;

        if(bonusMulti > 0)
        {
            float bonusToAdd = gemToCheck.scoreValue * bonusMulti * bonusAmount; //calculates the amount of bonus points earned
            roundMan.currentScore += Mathf.RoundToInt(bonusToAdd); //adds the bonus amount to the score and converts it back into a readable integer
        }
    }
}
