using UnityEngine;
using System.Collections;

public class CameraP1Controller : MonoBehaviour {

    private float minX = -64.0f;
    private float maxX = 32.0f;
    private float minZ = -76.0f;
    private float maxZ = 20.0f;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
        float MoveX = Input.GetAxis("Horizontal") / 2;
        float MoveZ = Input.GetAxis("Vertical") / 2;
        float RotY = Input.GetAxis("RotHoriz") * 10;
        Quaternion rotationForward = transform.rotation;
        rotationForward.x = 0;
        rotationForward.z = 0;
        Vector3 pos = transform.position + rotationForward * new Vector3(MoveX, 0, MoveZ);
        pos.x = Mathf.Clamp(pos.x, minX, maxX);
        pos.z = Mathf.Clamp(pos.z, minZ, maxZ);
        Quaternion rot = Quaternion.Euler(0, RotY, 0);
        transform.position = pos;
        transform.rotation = transform.rotation * rot;
	}
}
