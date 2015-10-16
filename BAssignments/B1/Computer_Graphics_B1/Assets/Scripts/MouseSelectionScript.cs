﻿using UnityEngine;
using System.Collections;

public class MouseSelectionScript : MonoBehaviour {

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
        if (Input.GetMouseButtonDown(0))
        {
            RaycastHit hit;
            Ray ray = gameObject.GetComponent<Camera>().ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out hit))
            {
                Transform target = hit.transform;
                GameObject targGameObject = target.gameObject;
                if (targGameObject.CompareTag("Selectable")) {
                    targGameObject.tag = "Selected";
                    foreach (Transform child in target.transform)
                    {
                        if (child.CompareTag("SelectionIndicator"))
                        {
                            child.gameObject.SetActive(true);
                            break;
                        }
                    }
                }
                else if (targGameObject.CompareTag("Selected"))
                {
                    targGameObject.tag = "Selectable";
                    foreach (Transform child in target.transform)
                    {
                        if (child.CompareTag("SelectionIndicator"))
                        {
                            child.gameObject.SetActive(false);
                            break;
                        }
                    }
                }
                else
                {
                    // Send the coordinates to the director script.
                    NavMeshHit navMeshHit;
                    bool ClickedNavMesh = NavMesh.SamplePosition(hit.point, out navMeshHit, 0.1f, NavMesh.AllAreas);
                    if (ClickedNavMesh)
                    {
                        //TODO: Director Script
                        // Send navMeshHit.position to director script, and call SetDestination on all
                        // "Selected" navMeshAgents ****ONLY ONCE****!!!!!!!
                        // Do not call in an update function.
                    }
                }
            }
        }
	}
}
