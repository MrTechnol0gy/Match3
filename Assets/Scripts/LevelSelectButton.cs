using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelSelectButton : MonoBehaviour
{
    public string levelToLoad;

    public GameObject star1, star2, star3;

    // Start is called before the first frame update
    void Start()
    {
        star1.SetActive(false); //sets the star to be deactive when the scene loads
        star2.SetActive(false);
        star3.SetActive(false);

        if(PlayerPrefs.HasKey(levelToLoad + "_Star1")) //checks to see if the key exists
        {
            star1.SetActive(true);
        }
        if(PlayerPrefs.HasKey(levelToLoad + "_Star2")) //checks to see if the key exists
        {
            star2.SetActive(true);
        }
        if(PlayerPrefs.HasKey(levelToLoad + "_Star3")) //checks to see if the key exists
        {
            star3.SetActive(true);
        }
    }

    public void LoadLevel()
    {
        SceneManager.LoadScene(levelToLoad);
    }

}
