using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class PossessionScript : MonoBehaviour {

    public bool IsPossessed;
    public bool IsDismissed;
    public Text hint;

    private UnitySteeringController steering;
    private GameObject StallToRob;
    private bool IsMovementBlocked;
    private float tick;
    private float thresh;
    private bool DidSteal;

    private GameObject escapePoint;

    public bool canSpeak;
    public bool waitForSpeak;

	// Use this for initialization
	void Start () {
        IsPossessed = false;
        IsDismissed = false;

        canSpeak = false;
        waitForSpeak = false;

        StallToRob = null;
        IsMovementBlocked = false;
        tick = 0.0f;
        thresh = 1.0f;
        DidSteal = false;
        steering = GetComponent<UnitySteeringController>();
	}
	
	// Update is called once per frame
	void Update () {

        if ((IsPossessed && gameObject.CompareTag("Salesman")))
        {
            if (waitForSpeak && !canSpeak)
            {
                hint.gameObject.SetActive(true);
                if (Input.GetKey(KeyCode.Space))
                {
                    canSpeak = true;
                }
            }
            else
            {
                hint.gameObject.SetActive(false);
            }
        }

        if ((IsPossessed && gameObject.CompareTag("Shopper")) || (IsPossessed && gameObject.CompareTag("Thief")))
        {
            if (!IsMovementBlocked)
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
                if (gameObject.CompareTag("Thief"))
                {
                    GetComponent<NavMeshAgent>().obstacleAvoidanceType = ObstacleAvoidanceType.NoObstacleAvoidance;
                }
                if (gameObject.CompareTag("Thief") && !DidSteal)
                {
                    if (Input.GetKey(KeyCode.Space) && StallToRob != null)
                    {

                        GameObject[] escapePoints = GameObject.FindGameObjectsWithTag("ThiefEscapePoint");
                        int minInd = 0;
                        float min = 0;
                        for (int i = 0; i < escapePoints.Length; i++)
                        {
                            float dist = (escapePoints[i].transform.position - transform.position).sqrMagnitude;
                            if (dist < min || i == 0)
                            {
                                minInd = i;
                                min = dist;
                            }
                        }
                        escapePoint = escapePoints[minInd];
                        tick = 0.0f;

                        IsMovementBlocked = true;
                        GameObject goLamp = StallToRob.transform.parent.FindChild("Lamps").FindChild("L2").gameObject;
                        ThiefMeta tm = gameObject.GetComponent<ThiefMeta>();
                        tm.lamp = goLamp;
                        LampController lc = goLamp.AddComponent<LampController>();
                        lc.thief = this.gameObject;
                        lc.primary = goLamp.transform.position;
                        hint.gameObject.SetActive(false);
                        StallToRob = null;
                    }
                }
            }
            else
            {
                tick += Time.deltaTime;
                if (tick > thresh)
                {
                    tick = thresh;
                    steering.Target = escapePoint.transform.position;
                    steering.maxSpeed = 8.0f;
                    if ((escapePoint.transform.position - transform.position).sqrMagnitude < 2.0f)
                    {
                        SceneController sc = gameObject.GetComponent<SceneController>();
                        if(sc.state == SceneState.ACTIVE) sc.state = SceneState.ENDING;
                    }
                }
            }
        }
	}

    void OnTriggerEnter(Collider other)
    {
        if (IsPossessed && gameObject.CompareTag("Thief"))
        {
            Debug.Log(other.gameObject.tag);
            if (other.gameObject.CompareTag("Marker"))
            {
                StallToRob = other.gameObject;
                hint.gameObject.SetActive(true);
            }
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (gameObject.CompareTag("Thief") && IsPossessed)
        {
            if (other.gameObject.CompareTag("Marker"))
            {
                StallToRob = null;
                hint.gameObject.SetActive(false);
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
        if (gameObject.CompareTag("Shopper") || gameObject.CompareTag("Thief"))
        {
            steering.stoppingRadius = 0.4f;
            steering.arrivingRadius = 3;
            steering.SlowArrival = true;
            if (gameObject.CompareTag("Thief"))
            {
                GetComponent<NavMeshAgent>().obstacleAvoidanceType = ObstacleAvoidanceType.GoodQualityObstacleAvoidance;
                StallToRob = null;
                hint.gameObject.SetActive(false);
            }
        }
        if (gameObject.CompareTag("Salesman"))
        {
            hint.gameObject.SetActive(false);
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
