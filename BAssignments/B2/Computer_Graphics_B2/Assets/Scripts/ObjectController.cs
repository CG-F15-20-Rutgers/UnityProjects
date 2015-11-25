using UnityEngine;
using System.Collections;

public class ObjectController : MonoBehaviour {

    public enum EggState
    {
        INACTIVE,
        UP,
        DOWN
    }

    Rigidbody eggrb;
    float currTime;
    int prayerCount;
    float startY;

    float tick;
    float tickMax;

    float subt;

    EggState state;

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
        tickMax = 0.5f;
        state = EggState.INACTIVE;
        subt = 0;
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
        Vector3 currPos = eggrb.position;
        float y = currPos.y;
        float newCurrTime = currTime + (Time.deltaTime) * 3f;
        y += (Mathf.Sin(newCurrTime) - Mathf.Sin(currTime)) / 3f;
        currPos.y = y;
        currTime = newCurrTime;

        if (Input.GetKeyDown(KeyCode.Space) && state == EggState.INACTIVE)
        {
            state = EggState.UP;
        }

        float extraY;
        switch (state) { 
            case EggState.INACTIVE:
            default:
                extraY = 0;
                break;
            case EggState.UP:
                tick += Time.deltaTime;
                if (tick > tickMax)
                {
                    tick = tickMax;
                    state = EggState.DOWN;
                    extraY = 1f;
                }
                else
                {
                    extraY = 3 * tick / tickMax;
                }
                break;
            case EggState.DOWN:
                tick -= Time.deltaTime;
                if (tick < 0)
                {
                    tick = 0;
                    state = EggState.INACTIVE;
                    extraY = 0f;
                }
                else
                {
                    extraY = 3 * tick / tickMax;
                }
                break;
        }
        currPos.y -= subt;
        currPos.y += extraY;
        subt = extraY;
        eggrb.position = currPos;

	}

    public EggState getState()
    {
        return state;
    }
}
