using UnityEngine;
using System.Collections;

public class CameraP1Controller : MonoBehaviour {

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
        float MoveX = Input.GetAxis("Horizontal") / 2;
        float MoveZ = Input.GetAxis("Vertical") / 2;
        float RotY = Input.GetAxis("RotHoriz") * 4;
        Quaternion rotationForward = transform.rotation;
        rotationForward.x = 0;
        rotationForward.z = 0;
        Vector3 pos = transform.position + rotationForward * new Vector3(MoveX, 0, MoveZ);
        Quaternion rot = Quaternion.Euler(0, RotY, 0);
        transform.position = pos;
        transform.rotation = transform.rotation * rot;
	}
}
