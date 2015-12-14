using UnityEngine;
using System.Collections;

public class FollowScript : MonoBehaviour {

    public GameObject followObj;
    private NavMeshAgent nma;

	// Use this for initialization
	void Start () {
        nma = GetComponent<NavMeshAgent>();
	}
	
	// Update is called once per frame
	void Update () {
        Vector3 location = followObj.transform.position;
        Quaternion rotation = Quaternion.LookRotation(followObj.transform.forward);
        location = location + (rotation * new Vector3(0, 0, -2));
        nma.SetDestination(location);
	}
}
