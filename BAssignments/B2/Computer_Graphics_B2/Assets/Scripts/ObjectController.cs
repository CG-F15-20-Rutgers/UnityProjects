using UnityEngine;
using System.Collections;

public class ObjectController : MonoBehaviour {

    Rigidbody eggrb;
    float currTime;

	// Use this for initialization
	void Start () {
        foreach(Transform t in transform) {
            if(t.name.Equals("egg")) {
                eggrb = t.gameObject.GetComponent<Rigidbody>();
            }
        }
        currTime = 0;
	}
	
	// Update is called once per frame
	void FixedUpdate () {

        // floating egg
        Vector3 currPos = eggrb.position;
        float y = currPos.y;
        float newCurrTime = currTime + (Time.deltaTime) * 3f;
        y += (Mathf.Sin(newCurrTime) - Mathf.Sin(currTime)) / 3f;
        currPos.y = y;
        eggrb.position = currPos;
        currTime = newCurrTime;

        // handle movement
        float Horizontal = Input.GetAxis("Hor2");
        float Vertical = Input.GetAxis("Ver2");
        Vector3 velocity = new Vector3(Horizontal, 0, Vertical);
        eggrb.velocity = velocity * 8f;
	}
}
