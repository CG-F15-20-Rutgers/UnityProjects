using UnityEngine;
using System.Collections;

public class RobotController : MonoBehaviour {

    private NavMeshAgent agent;
    private Animator animator;
    private Rigidbody rb;

    private float previousYRotation;
    private bool isJumping;
    private float time;
    private Vector3 jumpStart;

	// Use this for initialization
	void Start () {
        agent = gameObject.GetComponent<NavMeshAgent>();
        animator = gameObject.GetComponent<Animator>();
        rb = gameObject.GetComponent<Rigidbody>();
        previousYRotation = transform.rotation.y;
        isJumping = false;
	}
	
	// Update is called once per frame
	void Update () {
        float diffYRotation = transform.rotation.y - previousYRotation;
        previousYRotation = transform.rotation.y;

        animator.SetFloat("Speed", agent.velocity.magnitude);

        if (agent.isOnOffMeshLink)
        {
            if (!isJumping)
            {
                animator.CrossFade(Animator.StringToHash("Base Layer.Jump Across"), 0.0f);
                isJumping = true;
                time = 0.0f;
                jumpStart = transform.position;
            }
            else
            {
                time += Time.deltaTime;
            }
            float normal = time / 0.867f;
            Vector3 pos = Vector3.Lerp(jumpStart, agent.currentOffMeshLinkData.endPos, normal);
            transform.position = pos;
            if (time >= 0.867f)
            {
                transform.position = agent.currentOffMeshLinkData.endPos;
                agent.CompleteOffMeshLink();
                agent.Resume();
                isJumping = false;
                animator.SetBool("NavMeshJump", false);
            }
        }
	}
}
