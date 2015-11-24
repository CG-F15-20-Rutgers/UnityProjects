using UnityEngine;
using System.Collections;

public class TestController : MonoBehaviour {

    public GameObject pinPad;

	// Use this for initialization
	void Start () {
	}
	
	// Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.B))
        {
            IKController ikc = GetComponent<IKController>();
            ikc.PressButton(pinPad.transform);
        }
	
	}
}
