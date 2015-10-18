using UnityEngine;
using System.Collections;

public class DirectorScript : MonoBehaviour {

    // Called every frame
    public void FixedUpdate()
    {
        float HorizontalObstacleMove = Input.GetAxis("ObstacleHorizontal");
        float VerticalObstacleMove = Input.GetAxis("ObstacleVertical");
        GameObject[] selectedObjects = GameObject.FindGameObjectsWithTag("ObstaclesToMove");
        foreach (GameObject gameObject in selectedObjects) {
            Vector3 movement = new Vector3(HorizontalObstacleMove, 0, VerticalObstacleMove);
            gameObject.GetComponent<Rigidbody>().freezeRotation = true;
            gameObject.GetComponent<Rigidbody>().velocity = (movement * 10);
        }
    }

    // To be called by the MouseSelectionScript.
    public static void DirectAgents(Vector3 target) {
        GameObject[] selectedObjects = GameObject.FindGameObjectsWithTag("Selected");
        foreach (GameObject gameObject in selectedObjects) {
            NavMeshAgent meshAgent = gameObject.GetComponent<NavMeshAgent>();
            meshAgent.SetDestination(target);
        }
    }
}
