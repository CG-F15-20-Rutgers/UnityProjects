using UnityEngine;
using System.Collections;

public class TestController : MonoBehaviour {

    public GameObject pinPad;
    public GameObject pinPadB;
    public GameObject pinPadC;
    public GameObject egg;

    int status;

    NavMeshAgent nma;
    IKController ikc;

	// Use this for initialization
	void Start () {
        status = 0;
        nma = GetComponent<NavMeshAgent>();
        ikc = GetComponent<IKController>();
	}
	
	// Update is called once per frame
    void Update()
    {
        if (status == 0)
        {
            nma.SetDestination(pinPad.transform.position - new Vector3(-1, 0, 1));
            status++;
        }
        else if (status == 1)
        {
            if (nma.pathStatus == NavMeshPathStatus.PathComplete && nma.remainingDistance == 0)
            {
                ikc.PressButton(pinPad.transform);
                status++;
            }
        }
        else if (status == 2)
        {
            if (!ikc.IsPressingButton()) status++;
        }
        else if (status == 3)
        {
            nma.SetDestination(pinPadB.transform.position - new Vector3(-1, 0, 1));
            status++;
        }
        else if (status == 4)
        {
            if (nma.pathStatus == NavMeshPathStatus.PathComplete && nma.remainingDistance == 0)
            {
                ikc.PressButton(pinPadB.transform);
                status++;
            }
        }
        else if (status == 5)
        {
            if (!ikc.IsPressingButton()) status++;
        }
        else if (status == 6)
        {
            nma.SetDestination(pinPadC.transform.position - new Vector3(-1, 0, 1));
            status++;
        }
        else if (status == 7)
        {
            if (nma.pathStatus == NavMeshPathStatus.PathComplete && nma.remainingDistance == 0)
            {
                ikc.PressButton(pinPadC.transform);
                status++;
            }
        }
        else if (status == 8)
        {
            if (!ikc.IsPressingButton())
            {
                nma.SetDestination(egg.transform.position - new Vector3(0, 0, 2));
                status++;
            }
        }
        else if (status == 9)
        {
            if (nma.pathStatus == NavMeshPathStatus.PathComplete && nma.remainingDistance == 0)
            {
                ikc.StartPrayer(gameObject.transform, egg.transform);
            }
            else
            {
                nma.SetDestination(egg.transform.position - new Vector3(0, 0, 2));
            }
        }

        /*if (Input.GetKeyDown(KeyCode.B))
        {
            IKController ikc = GetComponent<IKController>();
            ikc.PressButton(pinPad.transform);
        }

        if(Input.GetKeyDown(KeyCode.C))
        {
            IKController ikc = GetComponent<IKController>();
            ikc.StartPrayer(gameObject.transform, egg.transform);
        }

        if (Input.GetKeyDown(KeyCode.D))
        {
            IKController ikc = GetComponent<IKController>();
            ikc.EndPrayer(egg.transform);
        }*/
	
	}
}
