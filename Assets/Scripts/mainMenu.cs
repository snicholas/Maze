using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

public class mainMenu : MonoBehaviour {

	// Use this for initialization
	void Start () {
        Screen.sleepTimeout = SleepTimeout.SystemSetting;
    }
	
	// Update is called once per frame
	void Update () {
	
	}
    public void loadLevel0()
    {
        SceneManager.LoadScene("level0");
    }
    public void toMainMenu()
    {
        SceneManager.LoadScene("MainMenu");
    }
}
