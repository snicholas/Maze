using UnityEngine;
using System.Collections;

public class trapController : MonoBehaviour {
    // ToDo: creare enum per tipi trappola
    public int type;
    private SpriteRenderer spRnd;
    private bool fadeIn = true;
    private Color precColor;
    private Vector2 movDir;
    float speed = 2.5f;
    Rigidbody2D r2body;
    float lastChange = -1000f;
    // Use this for initialization
    void Awake()
    {
        spRnd = gameObject.GetComponent<SpriteRenderer>();
        precColor = spRnd.color;
        precColor.a = 0f;
        spRnd.color = precColor;
        movDir = new Vector2(speed, speed);
        r2body = GetComponent<Rigidbody2D>();
    }
    // Use this for initialization
    void Update () {
        transform.Rotate(new Vector3(0, 0, 45) * Time.deltaTime);
        if (fadeIn)
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
        spRnd.color = precColor;        
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
        //else
        //{
        /*r2body.AddForce(movDir);
        if (Mathf.Abs(r2body.velocity.x) > maxSpeed || Mathf.Abs(r2body.velocity.y) > maxSpeed)
        {
            Vector3 newVelocity = r2body.velocity.normalized;
            newVelocity *= maxSpeed;
            r2body.velocity = newVelocity;
        }*/
        //}
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player") && precColor.a>=0.25)
        {
            if (type == 1)
            {
                other.gameObject.SendMessage("lifeUpDown", -10);
                Destroy(gameObject);
            }else if (type == 2)
            {
                other.gameObject.SendMessage("lifeUpDown", 10);
                Destroy(gameObject);
            }
        }
        //Debug.Log("Trap trig");
    }
}
