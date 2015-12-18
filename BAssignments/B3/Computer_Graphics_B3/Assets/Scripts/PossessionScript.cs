using UnityEngine;
using System.Collections;

public class PossessionScript : MonoBehaviour {

    public bool IsPossessed;
    public bool IsDismissed;

    private UnitySteeringController steering;
    private CharacterMecanim characterMecanim;
    private BodyMecanim bodyMecanim;
    private IKController ikc;
    private _NavigatorScript navScript;
    private NavMeshAgent agent;
    private CharacterController charController;
    private BehaviorMecanim behaviorMecanim;
    private Rigidbody rb;
    private CapsuleCollider capsule;
    private Animator anim;

	// Use this for initialization
	void Start () {
        IsPossessed = false;
        IsDismissed = false;

        steering = GetComponent<UnitySteeringController>();
        characterMecanim = GetComponent<CharacterMecanim>();
        bodyMecanim = GetComponent<BodyMecanim>();
        ikc = GetComponent<IKController>();
        navScript = GetComponent<_NavigatorScript>();
        agent = GetComponent<NavMeshAgent>();
        charController = GetComponent<CharacterController>();
        behaviorMecanim = GetComponent<BehaviorMecanim>();
        rb = GetComponent<Rigidbody>();
        capsule = GetComponent<CapsuleCollider>();
        anim = GetComponent<Animator>();
	}
	
	// Update is called once per frame
	void Update () {
        if ((IsPossessed && gameObject.CompareTag("Shopper")) || (IsPossessed && gameObject.CompareTag("Thief")))
        {
            float Vertical = Input.GetAxis("Vertical");
            float Horizontal = Input.GetAxis("Horizontal");
            steering.Target = transform.position + Quaternion.LookRotation(transform.forward) * new Vector3(Horizontal / 2, 0, Vertical) * 5;
            steering.maxSpeed = 3;
            if (Input.GetKey(KeyCode.LeftShift)) steering.maxSpeed = 6;
            if (Vertical == 0 && Horizontal != 0)
            {
                steering.orientationBehavior = OrientationBehavior.None;
                steering.SetDesiredOrientation(transform.position + Quaternion.Euler(0, Horizontal * 50, 0) * transform.forward * 5);
                if (steering.IsFacing()) steering.orientationBehavior = OrientationBehavior.LookForward;
            }
        }
	}

    // Swap over to normal walking mode by disabling all components related to the tree and enabling the movement components
    public void Possess()
    {
        this.IsPossessed = true;
        if (gameObject.CompareTag("Shopper"))
        {
            steering.stoppingRadius = 0;
            steering.arrivingRadius = 0;
            steering.SlowArrival = false;
        }
    }

    public void Depossess()
    {
        this.IsPossessed = false;
        if (gameObject.CompareTag("Shopper"))
        {
            steering.stoppingRadius = 0.4f;
            steering.arrivingRadius = 3;
            steering.SlowArrival = true;
        }
    }

    public bool IsTreeControlled()
    {
        return !IsPossessed && !IsDismissed;
    }

    public bool IsDismissable()
    {
        return IsDismissed;
    }
}
