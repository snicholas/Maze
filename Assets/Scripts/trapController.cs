using UnityEngine;
using System.Collections;

public class trapController : MonoBehaviour {
    ParticleCollisionEvent[] collisionEvents;
    ParticleSystem part;
    // ToDo: creare enum per tipi trappola
    public int type;
    // Use this for initialization
    void Start () {
        part = GetComponent<ParticleSystem>();
        collisionEvents = new ParticleCollisionEvent[16];
    }

    void OnParticleCollision(GameObject other)
    {
        if (other.CompareTag("Player"))
        {
            if (type == 1)
            {
                other.SendMessage("lifeUpDown", -10);
                Destroy(gameObject);
            }else if (type == 2)
            {
                other.SendMessage("lifeUpDown", 10);
                Destroy(gameObject);
            }
        }
    }
}
