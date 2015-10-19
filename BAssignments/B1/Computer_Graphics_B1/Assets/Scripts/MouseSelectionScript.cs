using UnityEngine;
using System.Collections;

public class MouseSelectionScript : MonoBehaviour {

    public Material activeObstacleMaterial;
    public Material inactiveObstacleMaterial;
    public bool isPartThree;

    private float LastClickTime;
    private bool IsClicked;

    void Start()
    {
        IsClicked = false;
        LastClickTime = 0;
    }
	
	// Update is called once per frame
	void Update () {
        bool DoubleClick = false;
        if (Input.GetMouseButtonDown(0))
        {
            // Check for dbl click
            IsClicked = true;
            if (Time.time - LastClickTime < 2) DoubleClick = true;

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
                else if (targGameObject.CompareTag("MoveableObstacle"))
                {
                    targGameObject.tag = "ObstaclesToMove";
                    targGameObject.GetComponent<Renderer>().sharedMaterial = activeObstacleMaterial;
                }
                else if (targGameObject.CompareTag("ObstaclesToMove"))
                {
                    targGameObject.tag = "MoveableObstacle";
                    targGameObject.GetComponent<Renderer>().sharedMaterial = inactiveObstacleMaterial;
                }
                else
                {
                    // Send the coordinates to the director script.
                    NavMeshHit navMeshHit;
                    bool ClickedNavMesh = NavMesh.SamplePosition(hit.point, out navMeshHit, 0.1f, NavMesh.AllAreas);
                    if (ClickedNavMesh)
                    {
                        DirectorScript.DirectAgents(navMeshHit.position, isPartThree, (DoubleClick)? 0.3f : 0.7f);
                    }
                }
            }
            else
            {
                if (IsClicked)
                {
                    LastClickTime = Time.time;
                    IsClicked = false;
                }
            }
        }
	}
}
