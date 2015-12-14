using UnityEngine;
using System.Collections;

public class Cam_Easy : MonoBehaviour {

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
        float moveX = Input.GetAxis("Horizontal");
        float moveZ = Input.GetAxis("Vertical");

        Vector3 pos = transform.position;
        pos.x += moveX;
        pos.z += moveZ;
        transform.position = pos;
	}
}
