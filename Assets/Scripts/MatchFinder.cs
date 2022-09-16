using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MatchFinder : MonoBehaviour
{
    private Board board;

    private void Awake()
    {
        board = FindObjectOfType<Board>(); //finds the board at the very start of the game
    }

    public void FindAllMatches() //checks the board for all matches
    {
        for (int x = 0; x < board.width; x++)
        {
            for (int y = 0; y < board.height; y++)
            {
                Gem currentGem = board.allGems[x, y];
                if(currentGem != null)
                {
                    if(x > 0 && x < board.width - 1) //checks to see if player is at the board edge
                    {
                        //checks for horizontal matches
                        Gem leftGem = board.allGems[x - 1, y];
                        Gem rightGem = board.allGems[x + 1, y];
                        if(leftGem != null && rightGem != null)
                        {
                            if(leftGem.type == currentGem.type && rightGem.type == currentGem.type)
                            {
                            currentGem.isMatched = true;
                            leftGem.isMatched = true;
                            rightGem.isMatched = true;
                            }
                        }
                    }

                    if (y > 0 && y < board.height - 1) //checks to see if player is at the board edge
                    {
                        //checks for vertical matches
                        Gem aboveGem = board.allGems[x, y + 1];
                        Gem belowGem = board.allGems[x, y - 1];
                        if (aboveGem != null && belowGem != null)
                        {
                            if (aboveGem.type == currentGem.type && belowGem.type == currentGem.type)
                            {
                                currentGem.isMatched = true;
                                aboveGem.isMatched = true;
                                belowGem.isMatched = true;
                            }
                        }
                    }
                }
            }
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
