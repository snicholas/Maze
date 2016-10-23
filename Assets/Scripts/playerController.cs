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
    public GameObject gameController;
    public Text scoreTxt;
    public Text timeTxt;
    public int healt = 100;
    public int maxHealt = 100;
    Rigidbody2D r2body;
    public int score = 0;
    public int level = 0;
    int totalPickUps;
    Matrix4x4 calibrationMatrix;
    Vector3 wantedDeadZone = Vector3.zero;
    public bool isInPlay = false;
    void setTimeText(int sec)
    {
        timeTxt.text = "Time: " + sec.ToString();
        timeTxt.color = new Color(0, 255, 23, 255);
        if (sec < 10)
        {
            timeTxt.color = Color.red;
        }
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
            gameManager.getInstance().writeGameData(false);
            SceneManager.LoadScene("gameOver");
        }
        else if (healt > maxHealt)
        {
            healt = maxHealt;
        }
    }
    //public void ShowAd()
    //{
    //    if (Advertisement.IsReady())
    //    {
    //        Advertisement.Show();
    //    }
    //}
    public void pauseGame()
    {
        isInPlay = false;
        gameObject.SetActive(false);
        pausePanel.SetActive(true);
        gameManager.getInstance().setLevel(level);
        gameManager.getInstance().setScore(score);
        gameManager.getInstance().setHp(healt);
        gameManager.getInstance().writeGameData(true);
    }
    public void resumeGame()
    {
        gameObject.SetActive(true);
        pausePanel.SetActive(false);
        calibrateAccelerometer();
    }
    void OnApplicationPause(bool pause)
    {
        if (pause)
        {
            gameManager.getInstance().setLevel(level);
            gameManager.getInstance().setScore(score);
            gameManager.getInstance().setHp(healt);
            gameManager.getInstance().writeGameData(true);
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
        gameController.SendMessage("generateNextLevel");
        totalPickUps = GameObject.FindGameObjectsWithTag("PickUp").Length;
        r2body = GetComponent<Rigidbody2D>();
        gameManager.getInstance().readGameData();
        score = gameManager.getInstance().getScore();
        healt = gameManager.getInstance().getHp();
        scoreTxt.text = "Score " + score;
        calibrateAccelerometer();
    }
    void LateUpdate()
    {
        if (totalPickUps == score)//&& Input.GetAxis("Fire1") != 0)
        {
            //SceneManager.LoadScene("MainMenu");
            totalPickUps += GameObject.FindGameObjectsWithTag("PickUp").Length;
        }
        scoreTxt.text = "Score " + score + "\nHP: " + healt;
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
        }
        else if (other.CompareTag("Wall"))
        {
            lifeUpDown(-1);
        }
        else if (other.CompareTag("Finish"))
        {
            lifeUpDown(+5);
            gameController.SendMessage("generateNextLevel");
        }
    }

}
