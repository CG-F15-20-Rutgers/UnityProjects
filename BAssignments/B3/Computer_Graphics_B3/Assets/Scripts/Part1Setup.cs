using UnityEngine;
using System.Collections;

public class Part1Setup : MonoBehaviour {

	// Use this for initialization
	void Start () {
        GameObject[] lights = GameObject.FindGameObjectsWithTag("P2Lights");
        Debug.Log("Request to weaken " + lights.Length + " lights from part 3");
        for (int i = 0; i < lights.Length; i++)
        {
            GameObject lightComponent = lights[i].transform.FindChild("Point light").gameObject;
            lightComponent.GetComponent<Light>().intensity = 0.4f;
            Material mat = lights[i].GetComponent<Renderer>().material;
            mat.SetColor("_EmissionColor", Color.black);
        }
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
