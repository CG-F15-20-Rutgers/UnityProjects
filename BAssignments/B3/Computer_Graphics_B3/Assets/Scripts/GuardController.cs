using UnityEngine;
using System.Collections;
using TreeSharpPlus;

public class GuardController : MonoBehaviour {

    [HideInInspector]
    public GameObject dismissableNPC;
    public UnityEngine.UI.Text hint;

    private bool wasPossessed;

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
        // Control code copied from BehaviorTree3.cs@PossessedShopArc(GameObject).
        float v = Input.GetAxis("Vertical");
        float h = Input.GetAxis("Horizontal");
        Vector3 target = transform.position + v * (Quaternion.Euler(0, h * 20, 0) * transform.forward);
        Vector3 targetDirection = transform.position + 10 * (Quaternion.Euler(0, h * 40, 0) * transform.forward);
        if (mec(gameObject).Character.NavGoTo(target) == RunStatus.Failure)
            mec(gameObject).Character.NavStop();
        if (mec(gameObject).Character.NavTurn(targetDirection) == RunStatus.Failure) ;
             mec(gameObject).Character.NavOrientBehavior(OrientationBehavior.LookForward);

        if (Input.GetKeyDown(KeyCode.Space) && hint.gameObject.activeSelf && dismissableNPC != null)
        {
            GetComponent<SpeechBubbleController>().DisplaySpeechBubble("Sir, I'm going to have to ask you to leave.");
            dismissableNPC.GetComponent<PossessionScript>().IsDismissed = true;

            // Hide the Hint b/c we just dismissed him.
            hint.gameObject.SetActive(false);
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Shopper") || other.gameObject.CompareTag("Thief"))
        {
            dismissableNPC = other.gameObject;
            wasPossessed = IsPossessed();
            hint.gameObject.SetActive(wasPossessed && CanDismissNPC());
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.gameObject.Equals(dismissableNPC))
        {
            dismissableNPC = null;
            hint.gameObject.SetActive(false);
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
