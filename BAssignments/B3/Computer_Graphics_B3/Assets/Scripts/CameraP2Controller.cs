using UnityEngine;
using System.Collections;

public class CameraP2Controller : MonoBehaviour {

    public float animSpeed = 1.5f;
    public float lookSmoother = 3f;

    private Animator anim; // a reference to the animator on the character

    private Rigidbody rb;

    void Start()
    {
        // initialising reference variables
        anim = GetComponent<Animator>();
        rb = GetComponent<Rigidbody>();
        if (anim.layerCount == 2)
        {
            anim.SetLayerWeight(1, 1);
        }
    }
	
	// Update is called once per frame
	void Update () {
        // Script from our B1 modified VERY slightly for this assignment (no jump / fall)
        float h = Input.GetAxis("Horizontal");
        float v = Input.GetAxis("Vertical");

        if (v > 0 && Input.GetKey(KeyCode.LeftShift))
        {
            v = v * 6.5f;
        }
        else if(v < 0)
        {
            v = v * 0.5f;
            h = h * -1;
        }

        if (v != 0)
        {
            Quaternion rotation = Quaternion.Euler(new Vector3(0, h * 10, 0));
            transform.rotation = transform.rotation * rotation;
        }
        else if(v == 0)
        {
            h = h * 75;
        }

        anim.SetFloat("Speed", v);
        anim.SetFloat("Direction", h);
        anim.speed = animSpeed;

	}
}
