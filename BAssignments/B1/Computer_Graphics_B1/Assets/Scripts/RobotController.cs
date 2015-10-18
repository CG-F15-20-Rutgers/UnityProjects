using UnityEngine;
using System.Collections;

public class RobotController : MonoBehaviour {

    private NavMeshAgent agent;
    private Animator animator;

	// Use this for initialization
	void Start () {
        agent = gameObject.GetComponent<NavMeshAgent>();
        animator = gameObject.GetComponent<Animator>();
	}
	
	// Update is called once per frame
	void Update () {
        animator.SetFloat("Speed", agent.speed);
        Debug.Log(agent.angularSpeed);
        animator.SetFloat("Direction", agent.angularSpeed);
	}
}
