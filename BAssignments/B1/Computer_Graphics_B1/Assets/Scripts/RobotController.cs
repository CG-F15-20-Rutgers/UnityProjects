using UnityEngine;
using System.Collections;

public class RobotController : MonoBehaviour {

    private NavMeshAgent agent;
    private Animator animator;
    private Rigidbody rb;

    private float previousYRotation;

	// Use this for initialization
	void Start () {
        agent = gameObject.GetComponent<NavMeshAgent>();
        animator = gameObject.GetComponent<Animator>();
        rb = gameObject.GetComponent<Rigidbody>();
        previousYRotation = transform.rotation.y;
	}
	
	// Update is called once per frame
	void Update () {
        float diffYRotation = transform.rotation.y - previousYRotation;
        previousYRotation = transform.rotation.y;

        Debug.Log(diffYRotation);

        animator.SetFloat("Speed", agent.velocity.magnitude);
	}
}
