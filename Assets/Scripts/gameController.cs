using UnityEngine;
using System.Collections;
using MazeGeneration;
using System.Collections.Generic;
using UnityEngine.SceneManagement;

public class gameController : MonoBehaviour
{

    public GameObject player;
    public GameObject mainCamera;
    public GameObject level;
    public Transform[] levels; //0:walls, 1:bkg
    private int currentLevel;
    public float wallOffsetX = 14.58F;
    public float wallOffsetY = 17.5F;

    private playerController pController;
    public int timePerRoom = 4;
    private float elapsedTime = 0;
    private float levelMaxTime = 5;
    private Maze maze = null;

    private Transform levelEnd;
    private ParticleSystem endOnPS, endOffPS;
    public bool levelEndEnabled = false;
    // Use this for initialization
    void Start()
    {
        currentLevel = 0;
        Screen.sleepTimeout = SleepTimeout.NeverSleep;
        pController = player.GetComponent<playerController>();
        currentLevel = gameManager.getInstance().getLevel();
        pController.level = currentLevel;
        levelEndEnabled = false;
        //generateNextLevel();
    }

    // Update is called once per frame
    void Update()
    {
        if (pController.isInPlay)
        {
            elapsedTime += Time.deltaTime;
            player.SendMessage("setTimeText", (int)((levelMaxTime - elapsedTime)));
            if ((int)((levelMaxTime - elapsedTime)) <= 0)
            {
                gameManager.getInstance().setLevel(0);
                gameManager.getInstance().setScore(pController.score);
                gameManager.getInstance().setHp(0);
                gameManager.getInstance().writeGameData(false);
                SceneManager.LoadScene("gameOver");
            }
            else
            {
                //controllo quanti pickup ci sono
                //se 0 sblocco la fine del livello
                GameObject[] pickups = GameObject.FindGameObjectsWithTag("PickUp");
                if (pickups.Length == 0)
                {
                    endOffPS.Stop();
                    endOnPS.Play();
                    levelEndEnabled = true;
                }
            }
        }
    }
    
    void generateNextLevel(bool newLevel)
    {
        Screen.sleepTimeout = SleepTimeout.NeverSleep;
        if (newLevel) { currentLevel++; }
        pController.level = currentLevel;
        foreach (Transform child in level.transform)
        {
            Destroy(child.gameObject);
        }
        generateMaze(currentLevel);
        player.GetComponent<Rigidbody2D>().velocity = new Vector2(0, 0);
        player.transform.position = new Vector2(0, 0);
        mainCamera.transform.position = new Vector3(0, 0, mainCamera.transform.position.z);
        player.SetActive(false);
        level.SetActive(false);
        player.SetActive(true);
        level.SetActive(true);
        elapsedTime = 0;
    }
    void getNewXY(ref List<float> xUsed, ref List<float> yUsed, ref float newX, ref float newY)
    {
        newX = Random.Range(-wallOffsetX * 0.65F, wallOffsetX * 0.65F);
        while (xUsed.Contains(newX))
        {
            newX = Random.Range(-wallOffsetX * 0.65F, wallOffsetX * 0.65F);
        }
        xUsed.Add(newX);
        while (yUsed.Contains(newX))
        {
            newY = Random.Range(-wallOffsetY * 0.65F, wallOffsetY * 0.65F);
        }
        yUsed.Add(newY);
    }
    void generateMaze(int currentLevel)
    {
        int xSize = 1;
        int ySize = 2;
        for (int i = 0; i < currentLevel; i++)
        {
            if (i % 2 == 0) { xSize++; }
            else { ySize++; }
        }
        levelMaxTime = timePerRoom * xSize * ySize + timePerRoom;
        maze = new Maze(xSize, ySize);
        float xOff = 0;
        float yOff = 0;
        float sy = levels[1].GetComponent<Renderer>().bounds.size.y;
        float sx = levels[1].GetComponent<Renderer>().bounds.size.x;
        int traps = xSize + ySize;
        int hrec = (int)(currentLevel * 0.1) + 1;
        bool levelEndPlaced = false;
        for (int y = 0; y < ySize; y++)
        {
            yOff = (y * sy);
            xOff = 0;
            for (int x = 0; x < xSize; x++)
            {
                List<float> xUsed = new List<float>();
                List<float> yUsed = new List<float>();
                xOff = (x * sx);
                Transform bkgCur = Instantiate(levels[1]);
                bkgCur.parent = level.transform;
                bkgCur.position = new Vector2(xOff, yOff);
                if (maze[x, y].HasFlag(CellState.Top))
                {
                    Transform wall = Instantiate(levels[0]);
                    wall.position = new Vector2(xOff, -wallOffsetY + yOff);
                    wall.Rotate(new Vector3(0, 0, 90));
                    wall.parent = level.transform;
                }
                if (maze[x, y].HasFlag(CellState.Left))
                {
                    Transform wall = Instantiate(levels[0]);
                    wall.position = new Vector2(-wallOffsetX + xOff, yOff);
                    wall.parent = level.transform;
                }

                if (x == xSize - 1)
                {
                    Transform wall = Instantiate(levels[0]);
                    wall.position = new Vector2(wallOffsetX + xOff, yOff);
                    wall.parent = level.transform;
                }
                if (y == ySize - 1)
                {
                    Transform wallTop = Instantiate(levels[0]);
                    wallTop.position = new Vector2(xOff, wallOffsetY + yOff);
                    wallTop.Rotate(new Vector3(0, 0, 90));
                    wallTop.parent = level.transform;
                }
                if (x < xSize - 1 || y < ySize - 1)
                {
                    float rx = -1;// 
                    float ry = -1;// 
                    getNewXY(ref xUsed, ref yUsed, ref rx, ref ry);

                    for (int i = 0; i < 2; i++)
                    {
                        Transform pickup = Instantiate(levels[2]);
                        pickup.position = new Vector2(i == 0 ? rx + xOff : -rx + xOff, i == 0 ? ry + yOff : -ry + yOff);
                        pickup.parent = level.transform;
                    }
                    int _rnd = x == 0 && y == 0 ? 0 : Random.Range(0, 100);
                    if (_rnd % 5 < 3 && traps > 0 && (x > 0 || y > 0))
                    {
                        Transform trap1 = Instantiate(levels[3]);
                        float rtx = -1; //Random.Range(-wallOffsetX * 0.65F, wallOffsetX * 0.65F) + xOff;
                        float rty = -1; //Random.Range(-wallOffsetY * 0.65F, wallOffsetY * 0.65F) + yOff;
                        getNewXY(ref xUsed, ref yUsed, ref rx, ref ry);
                        trap1.position = new Vector2(rtx + xOff, rty + yOff);
                        trap1.parent = level.transform;
                        traps--;
                    }
                    else if (_rnd % 7 < 4 && hrec > 0 && (x > 0 || y > 0))
                    {
                        Transform healtCharge = Instantiate(levels[4]);
                        float rtx = -1; //Random.Range(-wallOffsetX * 0.65F, wallOffsetX * 0.65F) + xOff;
                        float rty = -1; //Random.Range(-wallOffsetY * 0.65F, wallOffsetY * 0.65F) + yOff;
                        getNewXY(ref xUsed, ref yUsed, ref rx, ref ry);
                        healtCharge.position = new Vector2(rtx + xOff, rty + yOff);
                        healtCharge.parent = level.transform;
                        hrec--;
                    }
                }
                /*else if(!levelEndPlaced && (_rnd%7>4 || (x==xSize-1 && y == ySize - 1))){
                    Transform levelEnd = Instantiate(levels[5]);
                    float rtx =  xOff;
                    float rty =  yOff;
                    levelEnd.position = new Vector2(rtx, rty);
                    levelEnd.parent = level.transform;
                    levelEndPlaced = true;
                }*/
            }
        }
        if (!levelEndPlaced)
        {
            levelEnd = Instantiate(levels[5]);
            float rtx = xOff;
            float rty = yOff;
            levelEnd.position = new Vector2(rtx, rty);
            levelEnd.parent = level.transform;
            levelEndPlaced = true;
            //levelEnd.gameObject.GetComponent<CircleCollider2D>().enabled = false;
            //levelEnd.gameObject.SetActive(false);
            endOnPS = (ParticleSystem)GameObject.FindGameObjectWithTag("greenFinishLane").GetComponent<ParticleSystem>();
            endOffPS = (ParticleSystem)GameObject.FindGameObjectWithTag("redFinishLane").GetComponent<ParticleSystem>();
            endOnPS.Stop();
            endOffPS.Play();
        }
    }
    void OnApplicationQuit()
    {
        if(maze!=null && player != null)
        {
            
        }
    }
    void OnApplicationPause(bool pauseStatus)
    {
        //isPaused = pauseStatus;
    }
}
