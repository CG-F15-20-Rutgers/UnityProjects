using UnityEngine;
using System.Collections;
using TreeSharpPlus;

public class GuardController : MonoBehaviour {

    [HideInInspector]
    public GameObject dismissableNPC;
    public UnityEngine.UI.Text hint;

    private bool wasPossessed;
    private UnitySteeringController steering;

    void Start()
    {
        steering = GetComponent<UnitySteeringController>();
    }

    void Update()
    {
        bool isPossessed = IsPossessed();
        if (isPossessed != wasPossessed)
        {
            hint.gameObject.SetActive(isPossessed && CanDismissNPC());
        }
        if (isPossessed)
        {
            HandleInput();
        }
        wasPossessed = isPossessed;
    }

    BehaviorMecanim mec(GameObject g)
    {
        return g.GetComponent<BehaviorMecanim>();
    }

    void HandleInput()
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

        if (Input.GetKeyDown(KeyCode.Space) && hint.gameObject.activeSelf && dismissableNPC != null)
        {
            GetComponent<SpeechBubbleController>().DisplaySpeechBubble("Sir, I'm going to have to ask you to leave.");
            dismissableNPC.GetComponent<PossessionScript>().IsDismissed = true;
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Shopper") || other.gameObject.CompareTag("Thief"))
        {
            dismissableNPC = other.gameObject;
            wasPossessed = IsPossessed();
            if (wasPossessed)
            {
                hint.gameObject.SetActive(CanDismissNPC());
            }
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.gameObject.Equals(dismissableNPC))
        {
            dismissableNPC = null;
            if (wasPossessed)
            {
                hint.gameObject.SetActive(false);
            }
        }
    }

    bool CanDismissNPC()
    {
        return (dismissableNPC != null && !dismissableNPC.GetComponent<PossessionScript>().IsDismissed);
    }

    bool IsPossessed()
    {
        PossessionScript ps = GetComponent<PossessionScript>();
        return (ps != null && ps.IsPossessed);
    }
}
