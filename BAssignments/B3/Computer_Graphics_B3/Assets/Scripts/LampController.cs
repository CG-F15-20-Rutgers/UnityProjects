using UnityEngine;
using System.Collections;

public class LampController : MonoBehaviour {

    float DistWeight = 0f;
    float theta = 0f;
    bool HasLamp = false;

    public GameObject thief;
    public Vector3 primary;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
    void Update()
    {
        Vector3 toAdd = new Vector3(0, 1.85f, 0.6f);
        Quaternion rotation = Quaternion.Euler(new Vector3(0, theta, 0));
        Vector3 secondary = thief.transform.position + (rotation * toAdd);
        if (!HasLamp)
        {
            DistWeight += Time.deltaTime;
            if (DistWeight > 1)
            {
                DistWeight = 1;
                HasLamp = true;
            }
            transform.position = primary * (1 - DistWeight) + secondary * DistWeight;
        }
        else
        {
            theta += Time.deltaTime * 300;
            transform.position = secondary;
        }

	}
}