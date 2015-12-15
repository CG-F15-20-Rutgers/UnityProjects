using UnityEngine;
using System.Collections;

public class ShopperMeta : MonoBehaviour {

    public GameObject stallSpot;
    public GameObject salesman;
    public bool StillActive;

	// Use this for initialization
	void Start () {
	    StillActive = true;
	}
	
	// Update is called once per frame
	void Update () {
	
	}

    public bool IsActiveForTree()
    {
        return StillActive;
    }
}
