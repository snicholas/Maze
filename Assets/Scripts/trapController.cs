using UnityEngine;
using System.Collections;

public class trapController : MonoBehaviour {
    // ToDo: creare enum per tipi trappola
    public int type;
    private SpriteRenderer spRnd;
    //private bool fadeIn = true;
    private Color precColor;
    private Vector2 movDir;
    float speed = 3.5f;
    Rigidbody2D r2body;
    float lastChange = -1000f;
    // Use this for initialization
    void Awake()
    {
        spRnd = gameObject.GetComponent<SpriteRenderer>();
        precColor = spRnd.color;
        precColor.a = 1f;
        spRnd.color = precColor;
        speed = Random.Range(2.5f, 5.5f);
        float xSpeed = ((int)Random.Range(0,100) % 2 == 0) ? speed : -speed;
        float ySpeed = ((int)Random.Range(0, 100) % 2 == 0) ? speed : -speed;
        movDir = new Vector2(xSpeed, ySpeed);
        r2body = GetComponent<Rigidbody2D>();
    }
    void Update () {
        transform.Rotate(new Vector3(0, 0, 45) * Time.deltaTime);
        /*if (fadeIn)
        {
            precColor.a += 0.5f * Time.deltaTime;
            if (precColor.a >= 1)
            {
                precColor.a = 1f;
                fadeIn = false;
            }
        }
        if (!fadeIn)
        {
            precColor.a -= 0.75f * Time.deltaTime;
            if (precColor.a <= 0)
            {
                precColor.a = 0f;
                fadeIn = true;
            }
        }
        spRnd.color = precColor;    */    
    }
    void FixedUpdate()
    {
        if ((Time.time - lastChange) > 1)
        {
            lastChange = Time.time;
            if (((int)lastChange) % 2 == 0)
            {
                movDir.x *= -1;
            }
            else
            {
                movDir.y *= -1;
            }
            r2body.velocity = movDir;
        }
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player") && precColor.a>=0.25)
        {
            if (type == 1)
            {
                other.gameObject.SendMessage("lifeUpDown", -1);
                Destroy(gameObject);
            }else if (type == 2)
            {
                other.gameObject.SendMessage("lifeUpDown", 1);
                Destroy(gameObject);
            }
        }
    }
}
