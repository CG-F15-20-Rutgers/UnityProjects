using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class MenuController : MonoBehaviour {

    public Button part1Button;
    public Button part2Button;
    public Button part3Button;
    public Button quitButton;

	// Use this for initialization
	void Start () {
        if(part1Button) part1Button.onClick.AddListener(loadPart1);
        if(part2Button) part2Button.onClick.AddListener(loadPart2);
        if(part3Button) part3Button.onClick.AddListener(loadPart3);
        if(quitButton) quitButton.onClick.AddListener(quit);
	}
	
	// Update is called once per frame
	void Update () {
	
	}

    public void loadPart1()
    {
        Application.LoadLevel("Part1-Scene");
    }

    public void loadPart2()
    {
        Application.LoadLevel("Part2-Scene");
    }

    public void loadPart3()
    {
        Application.LoadLevel("Part3-Scene");
    }

    public void quit()
    {
        Application.LoadLevel("Menu");
    }
}
