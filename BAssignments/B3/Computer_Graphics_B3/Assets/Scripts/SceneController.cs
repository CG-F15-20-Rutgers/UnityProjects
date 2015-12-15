using UnityEngine;
using System.Collections;

public enum SceneState
{
    ACTIVE,
    ENDING,
    ENDED,
    DISPLAYINGTEXT,
    FADINGTEXT
}

public class SceneController : MonoBehaviour {

    public SceneState state;
    public Font font;
    public Texture tex;

    private float opScreenCover;
    private float opText;
    private float currAnimTick;
    private float maxAnimTime;

    public string endMessage = "That's how the lamp was stolen...";

	// Use this for initialization
	void Start () {
        state = SceneState.ACTIVE;
        opScreenCover = 0.0f;
        opText = 0.0f;
        currAnimTick = 0.0f;
        maxAnimTime = 2.4f;
	}
	
	// Update is called once per frame
	void Update () {
        switch (state)
        {
            case SceneState.ENDING:
                opScreenCover = opScreenCover + Time.deltaTime;
                if (opScreenCover > maxAnimTime)
                {
                    opScreenCover = maxAnimTime;
                    state = SceneState.ENDED;
                }
                break;
            case SceneState.ENDED:
                opText = opText + Time.deltaTime;
                if(opText > maxAnimTime)
                {
                    opText = maxAnimTime;
                    state = SceneState.DISPLAYINGTEXT;
                }
                break;
            case SceneState.DISPLAYINGTEXT:
                currAnimTick = currAnimTick + Time.deltaTime;
                if (currAnimTick > maxAnimTime)
                {
                    currAnimTick = maxAnimTime;
                    state = SceneState.FADINGTEXT;
                }
                break;
            case SceneState.FADINGTEXT:
                opText -= Time.deltaTime;
                if(opText < 0)
                {
                    opText = 0;
                    Application.LoadLevel(0);
                }
                break;
            default:
                break;
        }
	}

    void OnGUI()
    {

        float oscn = opScreenCover / maxAnimTime;
        GUI.color = new Color(0.0f, 0.0f, 0.0f, oscn);
        GUI.depth = 1;

        GUI.DrawTexture(new Rect(0, 0, Screen.width, Screen.height), tex);


        float otn = opText / maxAnimTime;
        GUI.color = new Color(1.0f, 1.0f, 1.0f, otn);
        GUI.depth = 0;

        GUIStyle gs = new GUIStyle();
        gs.alignment = TextAnchor.MiddleCenter;
        gs.font = font;
        gs.fontSize = 36;
        gs.fontStyle = FontStyle.Bold;
        gs.normal.textColor = new Color(1.0f, 1.0f, 1.0f, 1.0f);

        GUI.Label(new Rect(Screen.width / 2, Screen.height / 2, 0, 0), endMessage, gs);

    }
}
