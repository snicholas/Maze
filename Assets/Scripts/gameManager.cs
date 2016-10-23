using UnityEngine;
using System.Collections;

public class gameManager : MonoBehaviour {
    public static gameManager instance = null;              //Static instance of GameManager which allows it to be accessed by any other script.
    private int currentScore = 0;
    private int currentLevel = 1;
    private int currentHp = 1;
    //private bool canContinue = false;
    // Use this for initialization
    public static gameManager getInstance()
    {
        return instance;
    }
    //Awake is always called before any Start functions
    void Awake()
    {
        //Check if instance already exists
        if (instance == null)
        {
            //if not, set instance to this
            instance = this;
            currentScore = 0;
            currentLevel = 1;
            currentHp = 20;
            readGameData();
        }
        //If instance already exists and it's not this:
        else if (instance != this)
        {

            //Then destroy this. This enforces our singleton pattern, meaning there can only ever be one instance of a GameManager.
            Destroy(gameObject);
        }
        
        //Sets this to not be destroyed when reloading scene
        DontDestroyOnLoad(gameObject);
    }
    public void writeGameData(bool saveStats)
    {
        PlayerPrefs.SetInt("currentHp", saveStats ? currentHp : 20);
        PlayerPrefs.SetInt("currentLevel", saveStats ? currentLevel : 0);
        PlayerPrefs.SetInt("currentScore", saveStats ? currentScore : 0);
        if (!saveStats)
        {
            int highScore = 0;
            if (PlayerPrefs.HasKey("highestScore"))
            {
                highScore = PlayerPrefs.GetInt("highestScore");
            }
            if (currentScore > highScore)
            {
                PlayerPrefs.SetInt("highestScore", currentScore);
            }
        }
        PlayerPrefs.Save();
    }
    public void readGameData()
    {
        if (PlayerPrefs.HasKey("currentHp"))
        {
            currentHp = PlayerPrefs.GetInt("currentHp");
            if (currentHp == 0)
            {
                currentHp = 50;
            }
        }
        if (PlayerPrefs.HasKey("currentLevel"))
        {
            currentLevel = PlayerPrefs.GetInt("currentLevel");
        }
        if (PlayerPrefs.HasKey("currentScore"))
        {
            currentScore = PlayerPrefs.GetInt("currentScore");
        }
        
    }
    public int getScore()
    {
        return currentScore;
    }
    public int getHp()
    {
        return currentHp;
    }
    public int getLevel()
    {
        return currentLevel;
    }
    public void setScore(int score)
    {
        currentScore = score;
    }
    public void setLevel(int lvl)
    {
        currentLevel = lvl;
    }
    public void setHp(int hp)
    {
        currentHp = hp;
    }
    /*void setCanContinue(bool can)
    {
        canContinue = can;
    }*/

}
