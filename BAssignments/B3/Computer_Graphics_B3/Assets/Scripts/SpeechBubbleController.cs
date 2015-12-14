using UnityEngine;
using System.Collections;

public class SpeechBubbleController : MonoBehaviour {

	public float defaultTimeToDisplaySpeechBubble;
    public Font font;
    private Texture speechBubble;
    private float currTime;
    private bool isBubbleVisible;
    private string message;

	// Use this for initialization
	void Start () {
        currTime = 0;
        isBubbleVisible = false;
		defaultTimeToDisplaySpeechBubble = 3;
        speechBubble = (Texture)Resources.Load("speech_bubble");
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
            Vector2 position = Camera.main.WorldToScreenPoint(gameObject.transform.position + new Vector3(0, 2.8f, 0));
            GUIStyle gs = new GUIStyle();
            position.y = Screen.height - position.y;
            gs.alignment = TextAnchor.MiddleCenter;
            gs.font = font;
            gs.fontSize = 12;
            gs.fontStyle = FontStyle.Bold;
            gs.normal.textColor = new Color(0.3f, 0.3f, 0.3f);

            position.x -= 1;
            position.y -= 1;
            GUI.Label(new Rect(position, new Vector2(0, 0)), message, gs);
            position.x += 2;
            GUI.Label(new Rect(position, new Vector2(0, 0)), message, gs);
            position.y += 2;
            GUI.Label(new Rect(position, new Vector2(0, 0)), message, gs);
            position.x -= 2;
            GUI.Label(new Rect(position, new Vector2(0, 0)), message, gs);
            gs.normal.textColor = Color.white;
            position.x += 1;
            position.y -= 1;
            GUI.Label(new Rect(position, new Vector2(0, 0)), message, gs);
        }
    }

    public void DisplaySpeechBubble(string message, long duration) {
		DisplaySpeechBubble(message, duration / 1000f);
    }

	public void DisplaySpeechBubble(string message) {
		DisplaySpeechBubble(message, defaultTimeToDisplaySpeechBubble);
	}

	public void DisplaySpeechBubble(string message, float durationInSeconds) {
		this.message = message;
		currTime = durationInSeconds;
		isBubbleVisible = true;
	}

    public bool IsBubbleVisible()
    {
        return isBubbleVisible;
    }
}
