using UnityEngine;
using System.Collections;

public class ObjectController : MonoBehaviour {

    Rigidbody rb;

	// Use this for initialization
	void Start () {
        foreach(Transform t in transform) {
            if(t.name.Equals("egg")) {
                rb = t.gameObject.GetComponent<Rigidbody>();
            }
        }
	}
	
	// Update is called once per frame
	void FixedUpdate () {
        float Horizontal = Input.GetAxis("Hor2");
        float Vertical = Input.GetAxis("Ver2");
        Vector3 velocity = new Vector3(Horizontal, 0, Vertical);
        rb.velocity = velocity * 8f;
	}
}
