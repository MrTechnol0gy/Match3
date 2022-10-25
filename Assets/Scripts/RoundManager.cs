using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class RoundManager : MonoBehaviour
{

    public float roundTurn = 15;
    private UIManager uiMan;

    private bool endingRound = false;
    private Board board;

    public int currentScore;
    public float displayScore;
    public float scoresSpeed;

    public int scoreTarget1, scoreTarget2, scoreTarget3; 

    // Start is called before the first frame update
    void Awake()
    {
        uiMan = FindObjectOfType<UIManager>();
        board = FindObjectOfType<Board>();
    }

    // Update is called once per frame
    void Update()
    {
        if(roundTurn > 0)
        {
            roundTurn -= Time.deltaTime;

            if(roundTurn <= 0) //if turns runs out, prevents turns from going into negative numbers
            {
                roundTurn = 0; 

                endingRound = true;
                
            }
        }

        if(endingRound && board.currentState == Board.BoardState.move)
        {
            WinCheck(); //calls the WinCheck function
            endingRound = false;
        }

        //uiMan.turnsUsedText.text = roundTurn--; //changes the value in our time value UI element

        displayScore = Mathf.Lerp(displayScore, currentScore, scoresSpeed * Time.deltaTime); //gradually increases the score over time, instead of instantly
        uiMan.scoreText.text = displayScore.ToString("0"); //displays the score as a whole number
        
    }

    private void WinCheck()
    {
        uiMan.roundOverScreen.SetActive(true);

        uiMan.winScore.text = currentScore.ToString(); //prints out the score

        if(currentScore >= scoreTarget3)
        {
            uiMan.winText.text = "Congratulations! You earned 3 stars!";
            uiMan.winStars3.SetActive(true);

            PlayerPrefs.SetInt(SceneManager.GetActiveScene().name + "_Star1", 1);
            PlayerPrefs.SetInt(SceneManager.GetActiveScene().name + "_Star2", 1);
            PlayerPrefs.SetInt(SceneManager.GetActiveScene().name + "_Star3", 1);
        }
        else if (currentScore >= scoreTarget2)
        {
            uiMan.winText.text = "Congratulations! You earned 2 stars!";
            uiMan.winStars2.SetActive(true);

            PlayerPrefs.SetInt(SceneManager.GetActiveScene().name + "_Star1", 1);
            PlayerPrefs.SetInt(SceneManager.GetActiveScene().name + "_Star2", 1);
        }
        else if (currentScore >= scoreTarget1)
        {
            uiMan.winText.text = "Congratulations! You earned 1 stars!";
            uiMan.winStars1.SetActive(true);

            PlayerPrefs.SetInt(SceneManager.GetActiveScene().name + "_Star1", 1);
        }
        else
        {
            uiMan.winText.text = "Oh no! No stars for you! Try again?";
        }

        SFXManager.instance.PlayRoundOver();
    }
}
