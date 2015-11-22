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
        Vector3 movement = new Vector3(HorizontalMovement, 0.0f, VerticalMovement);
        Quaternion currentRotation = transform.rotation;
        float speed = 20f;
        movement = currentRotation * movement * speed;
        gameObject.GetComponent<Rigidbody>().velocity = movement;

        float Rotation = Input.GetAxis("Rotation");
        Quaternion rotation = transform.rotation;
        rotation *= Quaternion.Euler(0.0f, Rotation, 0.0f);
        transform.rotation = rotation;
	}
}
