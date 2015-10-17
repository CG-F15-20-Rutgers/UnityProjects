using UnityEngine;
using System.Collections;

public class DirectorScript : MonoBehaviour {

    // Use this for initialization
    void Start() {

    }

    // Update is called once per frame
    void Update() {

    }

    // To be called by the MouseSelectionScript.
    public void DirectAgents(Vector3 target) {
        GameObject[] selectedObjects = GameObject.FindGameObjectsWithTag("Selected");
        foreach (GameObject gameObject in selectedObjects) {
            NavMeshAgent meshAgent = gameObject.GetComponent<NavMeshAgent>();
            meshAgent.SetDestination(target);
        }
    }
}
