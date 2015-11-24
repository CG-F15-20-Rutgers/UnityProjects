﻿using UnityEngine;
using System.Collections;

public class TestController : MonoBehaviour {

    public GameObject pinPad;
    public GameObject pinPadB;
    public GameObject pinPadC;
    public GameObject egg;

    public GameObject pointTarget;

    int status;

    NavMeshAgent nma;
    IKController ikc;

	// Use this for initialization
	void Start () {
        status = -3;
        nma = GetComponent<NavMeshAgent>();
        ikc = GetComponent<IKController>();
	}
    float time = 0;
	// Update is called once per frame
    void Update()
    {
        if (status == -3)
        {
            time += Time.deltaTime;
            if (time > 4)
            {
                ikc.Die(transform);
                status = -4;
            }
        }
        if (status == -2)
        {
            ikc.PointAt(transform, pointTarget.transform, false);
            status++;
        }
        if (status == -1)
        {
            if (!ikc.IsPointing())
            {
                nma.SetDestination(transform.position + new Vector3(0, 0, -1));
                GetComponent<SpeechBubbleController>().DisplaySpeechBubble("Hi! I think we should steal the golden egg!");
                status++;
            }
        }
        else if (status == 0)
        {
            if (!GetComponent<SpeechBubbleController>().IsBubbleVisible())
            {
                nma.SetDestination(pinPad.transform.position - new Vector3(-1, 0, 1));
                status++;
            }
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
            if (!ikc.IsPressingButton() && !ikc.IsPraying())
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
                status++;
            }
            else
            {
                nma.SetDestination(egg.transform.position - new Vector3(0, 0, 2));
            }
        }
	
	}
}
