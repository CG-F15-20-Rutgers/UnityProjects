using UnityEngine;
using System.Collections;

public enum CameraState
{
    FREE_FLOW,
    ATTACHING,
    ATTACHED,
    DETACHING
}

public enum CharacterClass
{
    SALESMAN,
    SHOPPER,
    THIEF,
    GUARD
}

public class CameraP3Controller : MonoBehaviour {

    public GameObject possessedCharacter;

    private float tick;
    private float maxAnim;

    private Vector3 freeFlyLocation;
    private Quaternion freeFlyRotation;

    private Vector3 attachedLocation;
    private Quaternion attachedRotation;

    private CameraState state;
    private CharacterClass character;

	// Use this for initialization
	void Start () {
        possessedCharacter = null;
        tick = 0;
        maxAnim = 0.8f;
        state = CameraState.FREE_FLOW;
	}
	
	void FixedUpdate () {
        switch (state)
        {
            case CameraState.FREE_FLOW:
                UpdateFreeFlow();
                break;
            case CameraState.ATTACHING:
                UpdateAttaching();
                break;
            case CameraState.ATTACHED:
                UpdateAttached();
                break;
            case CameraState.DETACHING:
                UpdateDetaching();
                break;
        }
	}

    void UpdateFreeFlow()
    {
        float MoveX = Input.GetAxis("Horizontal") / 2;
        float MoveZ = Input.GetAxis("Vertical") / 2;
        float RotY = Input.GetAxis("RotHoriz") * 2;
        Quaternion rotationForward = transform.rotation;
        rotationForward.x = 0;
        rotationForward.z = 0;
        Vector3 pos = transform.position + rotationForward * new Vector3(MoveX, 0, MoveZ);
        Quaternion rot = Quaternion.Euler(0, RotY, 0);
        transform.position = pos;
        transform.rotation = transform.rotation * rot;

        if (Input.GetMouseButtonDown(0))
        {
            RaycastHit hit;
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out hit))
            {
                Transform targ = hit.transform;
                GameObject targGO = targ.gameObject;
                if (targGO.CompareTag("Salesman") || targGO.CompareTag("Shopper") || targGO.CompareTag("Guard") || targGO.CompareTag("Thief"))
                {
                    PossessPlayer(targGO);
                }
                else if (targGO.CompareTag("GuardChild"))
                {
                    PossessPlayer(targGO.transform.parent.gameObject);
                }
                else
                {
                    Debug.Log("Clicked on " + targGO);
                }
            }
        }
    }

    void UpdateAttaching()
    {
        tick += Time.deltaTime;
        if (tick > maxAnim)
        {
            tick = maxAnim;
            state = CameraState.ATTACHED; // Animation is done, let's just dock the camera
                                          // properly
        }

        float norm = tick / maxAnim;      // A normalized time parameter for lerps

        Quaternion cameraRot = Quaternion.Euler(Mathf.Lerp(30, 0, norm), 0, 0);
        Camera.main.transform.localRotation = cameraRot;

        Vector3 location = Vector3.Lerp(freeFlyLocation, attachedLocation, norm);
        transform.position = location;

        Quaternion rotation = Quaternion.Lerp(freeFlyRotation, attachedRotation, norm);
        transform.rotation = rotation;

        if (state != CameraState.ATTACHING)
        {
            tick = 0;
        }
    }

    void UpdateAttached()
    {
        if (Input.GetKeyDown(KeyCode.Q))
        {
            ReleasePlayer();
        }
        else
        {
            switch (character)
            {
                case CharacterClass.SALESMAN:
                    UpdateSalesman();
                    break;
            }
            transform.rotation = Quaternion.LookRotation(possessedCharacter.transform.forward);
            transform.position = possessedCharacter.transform.position + transform.rotation * new Vector3(0, 1.85f, -2);
        }
    }

    void UpdateDetaching()
    {
        tick += Time.deltaTime;
        if (tick > maxAnim)
        {
            tick = maxAnim;
            state = CameraState.FREE_FLOW; // Animation is done, let's just dock the camera
            // properly
        }

        float norm = tick / maxAnim;      // A normalized time parameter for lerps

        Quaternion cameraRot = Quaternion.Euler(Mathf.Lerp(0, 30, norm), 0, 0);
        Camera.main.transform.localRotation = cameraRot;

        Vector3 location = Vector3.Lerp(attachedLocation, freeFlyLocation, norm);
        transform.position = location;

        Quaternion rotation = Quaternion.Lerp(attachedRotation, freeFlyRotation, norm);
        transform.rotation = rotation;

        if (state != CameraState.DETACHING)
        {
            tick = 0;
        }
    }

    void UpdateSalesman()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            SpeechBubbleController sbc = possessedCharacter.GetComponent<SpeechBubbleController>();
            if (!sbc.IsBubbleVisible())
            {
                sbc.DisplaySpeechBubble("Hi there!");
            }
        }
    }

    public void PossessPlayer(GameObject player)
    {
        if (state == CameraState.FREE_FLOW)
        {
            if (player.CompareTag("Salesman"))
            {
                character = CharacterClass.SALESMAN;
            }
            else if (player.CompareTag("Shopper"))
            {
                character = CharacterClass.SHOPPER;
            }
            else if (player.CompareTag("Guard"))
            {
                character = CharacterClass.GUARD;
            }
            else if (player.CompareTag("Thief"))
            {
                character = CharacterClass.THIEF;
            }
            else return;

            PossessionScript ps = player.GetComponent<PossessionScript>();
            ps.IsPossessed = true;

            possessedCharacter = player;
            attachedRotation = player.transform.rotation;
            attachedLocation = player.transform.position + attachedRotation * new Vector3(0, 1.85f, -2);
            freeFlyLocation = transform.position;
            freeFlyRotation = transform.rotation;

            state = CameraState.ATTACHING;
            tick = 0;
        }
    }

    public GameObject ReleasePlayer()
    {
        GameObject retval = possessedCharacter;
        possessedCharacter = null;
        PossessionScript ps = retval.GetComponent<PossessionScript>();
        ps.IsPossessed = false;
        attachedLocation = transform.position;
        attachedRotation = transform.rotation;
        state = CameraState.DETACHING;
        tick = 0;
        return retval;
    }
}
