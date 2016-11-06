using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
//using UnityEngine.Advertisements;

public class playerController : MonoBehaviour
{
    public GameObject pausePanel;
    public float speedMult;
    public float maxSpeed;
    public gameController gController;
    public Text scoreTxt;
    public Text hpTxt;
    public Text pickupTxt;
    public Text timeTxt;
    public int healt = 2;
    public int maxHealt = 5;
    public Animator animHurry, animNextLvl, animCountDown;
    
    int curPickup = 0;
    int totPickup = 0;
    Rigidbody2D r2body;
    public int score = 0;
    public int level = 0;
    Matrix4x4 calibrationMatrix;
    Vector3 wantedDeadZone = Vector3.zero;
    public bool isInPlay = false;
    int remainingTime = 0;
    void setTimeText(int sec)
    {
        remainingTime = sec;
        timeTxt.text = "Time: " + sec.ToString();
        timeTxt.color = Color.blue;
        if (sec < 10)
        {
            timeTxt.color = Color.red;
            StartCoroutine(playAnimation(animHurry, "showHurryUp", 2f));
        }
    }
    void showNL()
    {
        StartCoroutine(playAnimation(animNextLvl, "showNL", 2f));
    }

    private IEnumerator playAnimation(Animator anim, string trigger, float waitTime)
    {
        anim.SetBool(trigger, true);
        yield return new WaitForSeconds(waitTime);
        anim.SetBool(trigger, false);
        isInPlay = true;
        yield return null;
    }


    void lifeUpDown(int hp)
    {
        healt += hp;
        if (healt <= 0)
        {
            //gameOver
            //ShowAd();
            //gameManager.getInstance()
            gameManager.getInstance().setLevel(0);
            gameManager.getInstance().setScore(score);
            gameManager.getInstance().setHp(0);
            gameManager.getInstance().setCanContinue(false);
            gameManager.getInstance().writeGameData(false);
            SceneManager.LoadScene("gameOver");
        }
        else if (healt > maxHealt)
        {
            healt = maxHealt;
            score += 5;
        }
        updateHealt();
    }
    void updateHealt()
    {
        hpTxt.text = "X " + healt;
    }
    void updatePickUp()
    {
        pickupTxt.text = curPickup + "/" + totPickup;
    }
    void setStartPickup()
    {
        curPickup = 0;
        totPickup = GameObject.FindGameObjectsWithTag("PickUp").Length;
        updatePickUp();
    }
    //public void ShowAd()
    //{
    //    if (Advertisement.IsReady())
    //    {
    //        Advertisement.Show();
    //    }
    //}
    void startCountDown()
    {
        isInPlay = false;
        animCountDown.playbackTime = 0;
        StartCoroutine(playAnimation(animCountDown, "showCountDown",3.5f));
        
    }

    public void pauseGame()
    {
        isInPlay = false;
        animCountDown.SetBool("showCountDown", false);
        gameObject.SetActive(false);
        pausePanel.SetActive(true);
        
    }
    public void resumeGame()
    {
        /*gameManager.getInstance().readGameData();
        score = gameManager.getInstance().getScore();
        healt = gameManager.getInstance().getHp();*/
        scoreTxt.text = "Score " + score;
        gameObject.SetActive(true);
        pausePanel.SetActive(false);
        calibrateAccelerometer();
        startCountDown();
    }
    public void toMainMenu()
    {
        gameManager.getInstance().setLevel(level);
        score = gameManager.getInstance().getScore();
        gameManager.getInstance().setScore(score);
        healt = gameManager.getInstance().getHp();
        gameManager.getInstance().setHp(healt);
        if (level > 1)
        {
            gameManager.getInstance().setCanContinue(true);
        }
        else
        {
            gameManager.getInstance().setCanContinue(false);
        }
        gameManager.getInstance().writeGameData(true);
        SceneManager.LoadScene("MainMenu");
    }
    void OnApplicationPause(bool pause)
    {
        if (pause)
        {
            pauseGame();
        }
        else
        {
            resumeGame();
        }
    }
    void calibrateAccelerometer()
    {
        wantedDeadZone = Input.acceleration;
        Quaternion rotateQuaternion = Quaternion.FromToRotation(new Vector3(0f, 0f, -1f), wantedDeadZone);
        //create identity matrix ... rotate our matrix to match up with down vec
        Matrix4x4 matrix = Matrix4x4.TRS(Vector3.zero, rotateQuaternion, new Vector3(1f, 1f, 1f));
        //get the inverse of the matrix
        calibrationMatrix = matrix.inverse;
        isInPlay = true;
    }
    Vector3 getAccelerometer(Vector3 accelerator)
    {
        Vector3 accel = this.calibrationMatrix.MultiplyVector(accelerator);
        return accel;
    }

    //Finally how you get the accelerometer input
    Vector3 _InputDir;
    // Use this for initialization
    void Start()
    {
        animHurry.SetBool("showHurryUp", false);
        animNextLvl.SetBool("showNL", false);
        gController.SendMessage("generateNextLevel",false);
        setStartPickup();
        r2body = GetComponent<Rigidbody2D>();
        gameManager.getInstance().readGameData();
        score = gameManager.getInstance().getScore();
        healt = gameManager.getInstance().getHp();
        scoreTxt.text = "Score " + score;
        calibrateAccelerometer();
        gameManager.getInstance().setCanContinue(true);
        updatePickUp();
        updateHealt();
        startCountDown();
    }
    void LateUpdate()
    {
        scoreTxt.text = "Score " + score;
        updatePickUp();
        updateHealt();
    }
    void Update()
    {
        transform.Rotate(new Vector3(0, 0, 0));
        transform.Rotate(new Vector3(0, 0, 45) * Time.deltaTime);
    }
    // Update is called once per frame
    void FixedUpdate()
    {
        if (isInPlay)
        {
#if UNITY_ANDROID || UNITY_IPHONE

            _InputDir = getAccelerometer(Input.acceleration);
            Vector3 movement = new Vector3(_InputDir.x * speedMult, _InputDir.y * speedMult, 0.0f);
            r2body.velocity = movement * speedMult;

#else
            float horizMov = Input.GetAxisRaw("Horizontal");
            float vertMov = Input.GetAxisRaw("Vertical");
            r2body.AddForce(new Vector2(horizMov * speedMult, vertMov * speedMult));
            
#endif
            if (Mathf.Abs(r2body.velocity.x) > maxSpeed || Mathf.Abs(r2body.velocity.y) > maxSpeed)
            {
                Vector3 newVelocity = r2body.velocity.normalized;
                newVelocity *= maxSpeed;
                r2body.velocity = newVelocity;
            }
        }

    }
    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("PickUp"))
        {
            //DestroyImmediate(other.gameObject);
            other.gameObject.SetActive(false);
            score++;
            curPickup++;
            updatePickUp();
        }
        else if (other.CompareTag("Wall"))
        {
            if (score > 5) { score -= 5; }
        }
        else if (other.CompareTag("Finish"))
        {
            if (gController.levelEndEnabled)
            {
                animHurry.SetBool("showHurryUp", false);
                animNextLvl.SetBool("showNL", false);
                score += 5;
                if (curPickup == totPickup)
                {
                    score += remainingTime;
                }

                gameManager.getInstance().setLevel(level);
                gameManager.getInstance().setScore(score);
                gameManager.getInstance().setHp(healt);
                gameManager.getInstance().setCanContinue(true);
                gameManager.getInstance().writeGameData(true);
                gController.SendMessage("generateNextLevel", true);
                setStartPickup();
                startCountDown();
            }
        }
    }

}
