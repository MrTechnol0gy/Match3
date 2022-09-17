using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;
using UnityEngine;
using Debug = UnityEngine.Debug;

public class Gem : MonoBehaviour
{
    [HideInInspector] //hides the below line from the unity inspector
    public Vector2Int posIndex;
    [HideInInspector]
    public Board board;

    private Vector2 firstTouchPosition;
    private Vector2 finalTouchPosition;

    private bool mousePressed; //if they've grabbed a gem
    private float swipeAngle = 0; //the direction the gem is going

    private Gem otherGem;

    public enum GemType { blue, green, red, yellow, purple, bomb}
    public GemType type;

    public bool isMatched;
    private Vector2Int previousPos;

    public GameObject destroyEffect;

    public int blastSize = 2;   //bomb explosion size

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if (Vector2.Distance(transform.position, posIndex) > .01f)
        {
            transform.position = Vector2.Lerp(transform.position, posIndex, board.gemSpeed * Time.deltaTime);
        }
        else
        {
            transform.position = new Vector3(posIndex.x, posIndex.y, 0f);
            board.allGems[posIndex.x, posIndex.y] = this;
        }
        if (mousePressed && Input.GetMouseButtonUp(0))
        {
            mousePressed = false; //prevents multiple calls once the mouse is pressed and released
            if (board.currentState == Board.BoardState.move)
            {
                finalTouchPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                CalculateAngle();
            }
        }
    }

    public void SetupGem(Vector2Int pos, Board theBoard)
    {
        posIndex = pos;
        board = theBoard;
    }

    private void OnMouseDown()
    {
        if (board.currentState == Board.BoardState.move)
        {
            firstTouchPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition); //converts the mouse position to a location within the world using the camera
            mousePressed = true;
        }
    }

    private void CalculateAngle()
    {
        swipeAngle = Mathf.Atan2(finalTouchPosition.y - firstTouchPosition.y, finalTouchPosition.x - firstTouchPosition.x);
        swipeAngle = swipeAngle * 180 / Mathf.PI;

        if (Vector3.Distance(firstTouchPosition, finalTouchPosition) > .5f) //checks to see if the intended player movement is large enough to be considered a move
        {
            MovePieces();
        }

    }

    private void MovePieces()
    {
        previousPos = posIndex;

        if (swipeAngle < 45 && swipeAngle > -45 && posIndex.x < board.width - 1) //checks the movement angle and makes sure the movement isn't outside of the right side board range.
        {
            otherGem = board.allGems[posIndex.x + 1, posIndex.y];
            otherGem.posIndex.x--;
            posIndex.x++;
        }
        else if (swipeAngle > 45 && swipeAngle <= 135 && posIndex.y < board.height - 1) //checks the movement angle and makes sure the movement isn't outside of the top of the board range.
        {
            otherGem = board.allGems[posIndex.x, posIndex.y + 1];
            otherGem.posIndex.y--;
            posIndex.y++;
        }
        else if (swipeAngle < -45 && swipeAngle >= -135 && posIndex.y > 0) //checks the movement angle and makes sure the movement isn't outside of the bottom of the board range.
        {
            otherGem = board.allGems[posIndex.x, posIndex.y - 1];
            otherGem.posIndex.y++;
            posIndex.y--;
        }
        else if (swipeAngle > 135 || swipeAngle < -135 && posIndex.x > 0) //checks the movement angle and makes sure the movement isn't outside of the left side board range.
        {
            otherGem = board.allGems[posIndex.x - 1, posIndex.y];
            otherGem.posIndex.x++;
            posIndex.x--;
        }

        board.allGems[posIndex.x, posIndex.y] = this;
        board.allGems[otherGem.posIndex.x, otherGem.posIndex.y] = otherGem;

        StartCoroutine(CheckMoveCo());
    }

    public IEnumerator CheckMoveCo()
    {
        board.currentState = Board.BoardState.wait; //sets board state to wait in order to prevent player making moves during the board change state

        yield return new WaitForSeconds(.5f);

        board.matchFind.FindAllMatches();

        if(otherGem != null)
        {
            if(!isMatched && !otherGem.isMatched)
            {
                otherGem.posIndex = posIndex;
                posIndex = previousPos;

                board.allGems[posIndex.x, posIndex.y] = this;
                board.allGems[otherGem.posIndex.x, otherGem.posIndex.y] = otherGem;

                yield return new WaitForSeconds(.5f);

                board.currentState = Board.BoardState.move; //sets board state to move in order to allow the player to make moves again
            }
            else
            {
                board.DestroyMatches();
            }
        }
    }

}
