using UnityEngine;
using System.Collections;

public class Part1Setup : MonoBehaviour {

	// Use this for initialization
	void Start () {
        GameObject[] lights = GameObject.FindGameObjectsWithTag("P2Lights");
        Debug.Log("Request to disable " + lights.Length + " lights from part 1");
        for (int i = 0; i < lights.Length; i++)
        {
            GameObject lightComponent = lights[i].transform.FindChild("Point light").gameObject;
            lightComponent.SetActive(false);
            Material mat = lights[i].GetComponent<Renderer>().material;
            mat.SetColor("_EmissionColor", Color.black);
        }
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
