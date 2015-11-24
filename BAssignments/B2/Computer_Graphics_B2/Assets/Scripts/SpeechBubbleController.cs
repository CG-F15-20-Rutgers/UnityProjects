using UnityEngine;
using System.Collections;

public class SpeechBubbleController : MonoBehaviour {

    public float timeToDisplaySpeechBubble;
    public Texture speechBubble;
    private float currTime;
    private bool isBubbleVisible;
    private string message;

	// Use this for initialization
	void Start () {
        currTime = 0;
        isBubbleVisible = false;
        message = "Hey there!";
	}
	
	// Update is called once per frame
	void Update () {
        if (currTime > 0) currTime = Mathf.Max(0, currTime - Time.deltaTime);
        if (isBubbleVisible && currTime == 0)
        {
            isBubbleVisible = false;
        }
	}

    void OnGUI()
    {
        if (isBubbleVisible)
        {
            Vector2 position = Camera.main.WorldToScreenPoint(gameObject.transform.position);
            position.y = Screen.height - position.y - 150;
            position.x = position.x - 10;
            GUI.DrawTexture(new Rect(position, new Vector2(100, 100)), speechBubble);

            position.y += 10;
            position.x += 10;
            GUIStyle gs = new GUIStyle();
            gs.alignment = TextAnchor.MiddleCenter;
            gs.normal.textColor = Color.black;
            gs.wordWrap = true;
            gs.fontSize = 10;
            GUI.Label(new Rect(position, new Vector2(80, 45)), message, gs);
        }
    }

    public void DisplaySpeechBubble(string message)
    {
        this.message = message;
        currTime = timeToDisplaySpeechBubble;
        isBubbleVisible = true;
    }

    public bool IsBubbleVisible()
    {
        return isBubbleVisible;
    }
}
