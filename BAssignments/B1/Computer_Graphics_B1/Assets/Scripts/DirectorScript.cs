using UnityEngine;
using System.Collections;

public class DirectorScript : MonoBehaviour {
    // To be called by the MouseSelectionScript.
    public static void DirectAgents(Vector3 target) {
        GameObject[] selectedObjects = GameObject.FindGameObjectsWithTag("Selected");
        foreach (GameObject gameObject in selectedObjects) {
            NavMeshAgent meshAgent = gameObject.GetComponent<NavMeshAgent>();
            meshAgent.SetDestination(target);
        }
    }
}
