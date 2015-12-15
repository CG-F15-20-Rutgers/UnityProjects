using UnityEngine;
using System.Collections;

public class PossessionScript : MonoBehaviour {

    public bool IsPossessed;
    public bool IsDismissed;

	// Use this for initialization
	void Start () {
        IsPossessed = false;
        IsDismissed = false;
	}
	
	// Update is called once per frame
	void Update () {
	
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
