using UnityEngine;
using System.Collections;

public class QuitButton : MonoBehaviour {

	// Use this for initialization
	void Start () {
        GetComponent<UnityEngine.UI.Button>().onClick.AddListener(onClick);
	}

    public void onClick()
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene("Menu");
    }
	
	// Update is called once per frame
	void Update () {
	
	}
}
