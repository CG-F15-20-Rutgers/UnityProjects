using UnityEngine;
using System.Collections;

enum State { STATE_OPEN, STATE_OPENING, STATE_CLOSED, STATE_CLOSING };

public class DoorController : MonoBehaviour {

    GameObject left_door;
    GameObject right_door;
    State door_state;

    float current_displacement = 0f;

    public float maxDisplacement = 2f;
    public float doorMoveSpeed = 0.1f;

	// Use this for initialization
	void Start () {
        foreach (Transform child in transform)
        {
            if (child.CompareTag("LDoor"))
            {
                left_door = child.gameObject;
            }
            else if (child.CompareTag("RDoor"))
            {
                right_door = child.gameObject;
            }
        }
        door_state = State.STATE_CLOSED;
	}

    void OpenDoor()
    {
        door_state = State.STATE_OPENING;
    }

    void CloseDoor()
    {
        door_state = State.STATE_CLOSING;
    }
	
	// Update is called once per frame
	void Update () {
        switch (door_state)
        {
            case State.STATE_OPENING:
                float displacement = doorMoveSpeed * Time.deltaTime;
                if (current_displacement + displacement > maxDisplacement)
                {
                    displacement = maxDisplacement - current_displacement;
                    current_displacement = maxDisplacement;
                    door_state = State.STATE_OPEN;
                }
                else
                {
                    current_displacement += displacement;
                }
                
                Vector3 pos = left_door.transform.position;
                pos.x -= displacement;
                left_door.transform.position = pos;
                
                pos = right_door.transform.position;
                pos.x += displacement;
                right_door.transform.position = pos;
                break;
            case State.STATE_CLOSING:
                displacement = doorMoveSpeed * Time.deltaTime;
                if (current_displacement - displacement < 0f)
                {
                    displacement = current_displacement;
                    current_displacement = 0f;
                    door_state = State.STATE_CLOSED;
                }
                else {
                    current_displacement -= displacement;
                }

                pos = left_door.transform.position;
                pos.x += displacement;
                left_door.transform.position = pos;

                pos = right_door.transform.position;
                pos.x -= displacement;
                right_door.transform.position = pos;
                break;
            case State.STATE_CLOSED:
                if (Input.GetKeyDown(KeyCode.A))
                {
                    OpenDoor();
                }
                break;
            case State.STATE_OPEN:
                if (Input.GetKeyDown(KeyCode.A))
                {
                    CloseDoor();
                }
                break;
        }
	}
}
