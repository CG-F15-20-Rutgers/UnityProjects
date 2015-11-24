using UnityEngine;
using System.Collections;

public class TestController : MonoBehaviour {

    public GameObject pinPad;
    public GameObject egg;

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

        if(Input.GetKeyDown(KeyCode.C))
        {
            IKController ikc = GetComponent<IKController>();
            ikc.StartPrayer(egg.transform);
        }

        if (Input.GetKeyDown(KeyCode.D))
        {
            IKController ikc = GetComponent<IKController>();
            ikc.EndPrayer();
        }
	
	}
}
