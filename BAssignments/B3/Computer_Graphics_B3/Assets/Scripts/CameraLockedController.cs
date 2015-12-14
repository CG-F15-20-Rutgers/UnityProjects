using UnityEngine;
using System.Collections;

public class CameraLockedController : MonoBehaviour
{

    // Use this for initialization
    void Start()
    {

    }

    // Update is called once per frame
    void FixedUpdate()
    {
        float VerticalMovement = Input.GetAxis("Vertical");
        Vector3 movement = new Vector3(0.0f, 0.0f, VerticalMovement);
        Quaternion currentRotation = transform.rotation;
        float speed = 20f;
        movement = currentRotation * movement * speed;
        gameObject.GetComponent<Rigidbody>().velocity = movement;
    }
}
