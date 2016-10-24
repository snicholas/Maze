using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

public class mainMenu : MonoBehaviour {
    public GameObject continueBtn;
    public GameObject startBtn;
    // Use this for initialization
    void Start () {
        Screen.sleepTimeout = SleepTimeout.SystemSetting;
        gameManager.getInstance().readGameData();
        if (gameManager.getInstance().getCanContinue())
        {
            continueBtn.gameObject.SetActive(false);
            startBtn.transform.position = new Vector3(startBtn.transform.position.x, 0, startBtn.transform.position.z);
        }

    }
	
	// Update is called once per frame
	void Update () {
	
	}
    public void startNewGame()
    {
        gameManager.getInstance().setLevel(1);
        gameManager.getInstance().setScore(0);
        gameManager.getInstance().setHp(20);
        gameManager.getInstance().writeGameData(true);
        SceneManager.LoadScene("level0");
    }
    public void continueCurrentGame()
    {
        SceneManager.LoadScene("level0");
    }
    public void toMainMenu()
    {
        SceneManager.LoadScene("MainMenu");
    }
}
