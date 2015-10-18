using UnityEngine;
using System.Collections;

public class CameraController : MonoBehaviour {

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void FixedUpdate () {
        float VerticalMovement = Input.GetAxis("Vertical");
        float HorizontalMovement = Input.GetAxis("Horizontal");
        if (VerticalMovement != 0.0f || HorizontalMovement != 0.0f)
        {
            Vector3 movement = new Vector3(HorizontalMovement, 0.0f, VerticalMovement);
            gameObject.GetComponent<Rigidbody>().velocity = movement;
        }
	}
}
