using UnityEngine;
using System.Collections;
using GooglePlayGames;
using UnityEngine.SocialPlatforms;

public class gameManager : MonoBehaviour {
    public static gameManager instance = null;              //Static instance of GameManager which allows it to be accessed by any other script.
    private int currentScore = 0;
    private int currentLevel = 1;
    private int currentHp = 1;
    private bool canContinue = false;
    private int highesScore = 0;
    private int levelWithoutHit = 0;
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
            PlayGamesPlatform.Activate();
            Debug.Log("PlayGamesPlatform.Activate()");
            this.currentScore = 0;
            this.currentLevel = 1;
            this.currentHp = 2;
            this.canContinue = false;
            this.highesScore = 0;
            this.levelWithoutHit = 0;
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
    void start()
    {
        
    }
    public void writeGameData(bool saveStats)
    {
        PlayerPrefs.SetInt("currentHp", saveStats ? currentHp : 2);
        PlayerPrefs.SetInt("currentLevel", saveStats ? currentLevel : 0);
        PlayerPrefs.SetInt("currentScore", saveStats ? currentScore : 0);
        PlayerPrefs.SetInt("canContinue", saveStats ? (canContinue ? 1 : 0) : 0);
        PlayerPrefs.SetInt("levelWithoutHit", saveStats ? levelWithoutHit : 0); 
        if (!saveStats)
        {
            int highScore = 0;
            if (PlayerPrefs.HasKey("highestScore"))
            {
                highScore = PlayerPrefs.GetInt("highestScore");
                highesScore = highScore;
            }
            if (currentScore > highScore)
            {
                PlayerPrefs.SetInt("highestScore", currentScore);
                highesScore = currentScore;
                Social.ReportScore(highesScore, "CgkIldzv_8IEEAIQBg", (bool success) => {
                    // handle success or failure
                });
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
                currentHp = 2;
            }
        }
        //levelWithoutHit
        if (PlayerPrefs.HasKey("levelWithoutHit"))
        {
            levelWithoutHit = PlayerPrefs.GetInt("levelWithoutHit");
        }
        if (PlayerPrefs.HasKey("currentLevel"))
        {
            currentLevel = PlayerPrefs.GetInt("currentLevel");
        }
        if (PlayerPrefs.HasKey("currentScore"))
        {
            currentScore = PlayerPrefs.GetInt("currentScore");
        }
        if (PlayerPrefs.HasKey("canContinue"))
        {
            canContinue = PlayerPrefs.GetInt("canContinue") == 1;
        }

    }
    //levelWithoutHit
    public int getLevelWithoutHit()
    {
        return levelWithoutHit;
    }
    public bool getCanContinue()
    {
        return canContinue;
    }
    public int getScore()
    {
        return currentScore;
    }
    public int getHighestScore()
    {
        return highesScore;
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
    
    public void setLevelWithoutHit(int levelWithoutHit)
    {
        this.levelWithoutHit = levelWithoutHit;
    }
    public void setLevel(int lvl)
    {
        currentLevel = lvl;
    }
    public void setHp(int hp)
    {
        currentHp = hp;
    }
    public void setCanContinue(bool can)
    {
        canContinue = can;
    }
    public void tweetBestScore()
    {
        /*string TWITTER_ADDRESS = "http://twitter.com/intent/tweet";
        Debug.Log(instance.getHighestScore());
        Application.OpenURL(TWITTER_ADDRESS + "?text=" + WWW.EscapeURL("I Just scored "+instance.getScore()+ " in #UFOMaze <link rel=\"canonical\" href =\"https://play.google.com/store/apps/details?id=com.NicholasSpadaro.UFOMaze\">Download it</link> and beat me!"));*/

    }
}
