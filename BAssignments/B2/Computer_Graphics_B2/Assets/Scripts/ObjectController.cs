using UnityEngine;
using System.Collections;

public class ObjectController : MonoBehaviour {

    Rigidbody eggrb;
    float currTime;
    int prayerCount;
    float startY;

	// Use this for initialization
	void Start () {
        foreach(Transform t in transform) {
            if(t.name.Equals("egg")) {
                eggrb = t.gameObject.GetComponent<Rigidbody>();
            }
        }
        currTime = 0;
        prayerCount = 0;
        startY = transform.position.y;
	}

    public void StartPrayer()
    {
        prayerCount++;
    }
    public void EndPrayer()
    {
        prayerCount--;
    }
	
	// Update is called once per frame
	void FixedUpdate () {

        // floating egg
        if (prayerCount == 0)
        {
            Vector3 currPos = eggrb.position;
            float y = currPos.y;
            float newCurrTime = currTime + (Time.deltaTime) * 3f;
            y += (Mathf.Sin(newCurrTime) - Mathf.Sin(currTime)) / 3f;
            currPos.y = y;
            eggrb.position = currPos;
            currTime = newCurrTime;
        }
        else
        {
            Vector3 pos = eggrb.position;
            pos.y = startY;
            eggrb.position = pos;
            currTime = 0;
        }

        // handle movement
        float Horizontal = Input.GetAxis("Hor2");
        float Vertical = Input.GetAxis("Ver2");
        Vector3 velocity = new Vector3(Horizontal, 0, Vertical);
        eggrb.velocity = velocity * 8f;
	}
}
