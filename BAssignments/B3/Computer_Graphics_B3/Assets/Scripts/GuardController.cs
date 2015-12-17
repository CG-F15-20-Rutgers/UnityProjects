using UnityEngine;
using System.Collections;

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
            hint.gameObject.SetActive(isPossessed && dismissableNPC != null);
        }
        wasPossessed = isPossessed;
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Shopper") || other.gameObject.CompareTag("Thief"))
        {
            dismissableNPC = other.gameObject;
            wasPossessed = IsPossessed();
            hint.gameObject.SetActive(wasPossessed);
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

    bool IsPossessed()
    {
        PossessionScript ps = GetComponent<PossessionScript>();
        return (ps != null && ps.IsPossessed);
    }
}
