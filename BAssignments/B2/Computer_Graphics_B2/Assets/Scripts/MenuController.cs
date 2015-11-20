using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class MenuController : MonoBehaviour {

    public Button part1Button = null;
    public Button part2Button = null;
    public Button part3Button = null;
    public Button quitButton = null;

	// Use this for initialization
	void Start () {
        if(part1Button) part1Button.onClick.AddListener(loadPart1);
        if(part2Button) part2Button.onClick.AddListener(loadPart2);
        if(part3Button) part3Button.onClick.AddListener(loadPart3);
        if(quitButton) quitButton.onClick.AddListener(loadMenu);
	}
	
	// Update is called once per frame
	void Update () {
	
	}

    public void loadPart1()
    {
        Application.LoadLevel(1);
    }

    public void loadPart2()
    {
        Application.LoadLevel(2);
    }

    public void loadPart3()
    {
        Application.LoadLevel(3);
    }

    public void loadMenu()
    {
        Debug.Log("Throwing to main menu");
        Application.LoadLevel(0);
    }
}
