using UnityEngine;
using System.Collections;

public class cameraController : MonoBehaviour {
    public GameObject player;
    Vector3 startPos;
	// Use this for initialization
	void Start () {
        startPos = player.transform.position;
        transform.position = new Vector3(startPos.x, startPos.y, transform.position.z);
	}
	
	// Update is called once per frame
	void LateUpdate () {
        startPos = player.transform.position;
        transform.position = new Vector3(startPos.x, startPos.y, transform.position.z);

    }
}
