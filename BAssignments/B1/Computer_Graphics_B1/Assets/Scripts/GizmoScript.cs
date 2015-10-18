using UnityEngine;
using System.Collections;

public class GizmoScript : MonoBehaviour {

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

    // Called by the engine to draw shapes only visible in the editor
    void OnDrawGizmos()
    {
        if (gameObject.CompareTag("CameraWall"))
        {
            BoxCollider collider = gameObject.GetComponent<BoxCollider>();
            Vector3 cubeSize = collider.size;
            Gizmos.color = Color.cyan;
            Gizmos.DrawCube(transform.position, cubeSize);
        }
        else
        {
            Gizmos.color = Color.cyan;
            Gizmos.DrawCube(transform.position, new Vector3(0.3f, 5.0f, 0.3f));
        } 
    }
}
