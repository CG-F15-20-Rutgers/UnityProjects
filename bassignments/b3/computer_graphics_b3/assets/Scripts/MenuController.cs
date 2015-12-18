using UnityEngine;
using System.Collections;

public class MenuController : MonoBehaviour {

    public GameObject part1;
    public GameObject part2;
    public GameObject part3;

    public float startHeight;
    public float endHeight;

    public Font font;
    public Texture tex;

    private bool selected;
    private int finalPart = 1;

	// Use this for initialization
	void Start () {
        selected = false;
	}
	
	// Update is called once per frame
	void Update () {
        int selectedPart = -1;
        RaycastHit hit;
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out hit))
        {
            Transform selected = hit.transform;
            if (selected.gameObject.CompareTag("Marker"))
            {
                string name = selected.gameObject.name;
                selectedPart = (int)(name.ToCharArray()[4] - '0');
            }
        }
        if (selectedPart == 1)
        {
            Vector3 pos = part1.transform.position;
            if (pos.y < endHeight)
            {
                pos.y += Time.deltaTime / 2;
                if (pos.y > endHeight) pos.y = endHeight;
            }
            part1.transform.position = pos;
        }
        else
        {
            Vector3 pos = part1.transform.position;
            if (pos.y > startHeight)
            {
                pos.y -= Time.deltaTime / 2;
                if (pos.y < startHeight) pos.y = startHeight;
            }
            part1.transform.position = pos;
        }
        if(selectedPart == 2)
        {
            Vector3 pos = part2.transform.position;
            if (pos.y < endHeight)
            {
                pos.y += Time.deltaTime / 2;
                if (pos.y > endHeight) pos.y = endHeight;
            }
            part2.transform.position = pos;
        }
        else
        {
            Vector3 pos = part2.transform.position;
            if (pos.y > startHeight)
            {
                pos.y -= Time.deltaTime / 2;
                if (pos.y < startHeight) pos.y = startHeight;
            }
            part2.transform.position = pos;
        }
        if (selectedPart == 3)
        {
            Vector3 pos = part3.transform.position;
            if (pos.y < endHeight)
            {
                pos.y += Time.deltaTime / 2;
                if (pos.y > endHeight) pos.y = endHeight;
            }
            part3.transform.position = pos;
        }
        else
        {
            Vector3 pos = part3.transform.position;
            if (pos.y > startHeight)
            {
                pos.y -= Time.deltaTime / 2;
                if (pos.y < startHeight) pos.y = startHeight;
            }
            part3.transform.position = pos;
        }
        if (selectedPart != -1 && Input.GetMouseButtonDown(0))
        {
            finalPart = selectedPart;
            selected = true;
        }
	}

    void OnGUI()
    {
        if (selected)
        {
            GUI.depth = 1;
            GUI.DrawTexture(new Rect(0, 0, Screen.width, Screen.height), tex);

            GUI.color = new Color(1.0f, 1.0f, 1.0f, 1.0f);
            GUI.depth = 0;

            GUIStyle gs = new GUIStyle();
            gs.alignment = TextAnchor.MiddleCenter;
            gs.font = font;
            gs.fontSize = 36;
            gs.fontStyle = FontStyle.Bold;
            gs.normal.textColor = new Color(1.0f, 1.0f, 1.0f, 1.0f);

            GUI.Label(new Rect(Screen.width / 2, Screen.height / 2, 0, 0), "Please wait, loading scene...", gs);
            UnityEngine.SceneManagement.SceneManager.LoadScene("Part" + finalPart);
        }
    }
}
