using UnityEngine;
using System.Collections;

// Controls for the car.
public class Driver : MonoBehaviour {

	// The car's rigidbody.
	Rigidbody body;
	// The audio controller for the UI.
	AudioController audioController;
	[Tooltip("The current speed of the car.")]
	public float moveSpeed = 0;
	[Tooltip("The maximum speed of the car.")]
	public float maxSpeed;
	[Tooltip("The acceleration of the car per frame.")]
	public float acceleration;
	// The current floor the car is on.
	//[HideInInspector]
	public int floor = 1;
	// The floor the car needs to get to.
	//[HideInInspector]
	public int targetFloor = 0;
	[Tooltip("The index of the location the car wants to get to.")]
	public int targetIndex = 0;
	[Tooltip("The locations the car wants to get to.")]
	public GameObject[] target;

	// The index of the location the car wanted to get to on the previous frame.
	int prevIndex = -1;

	// Use this for initialization.
	void Start () {
		audioController = GetComponentInChildren<AudioController> ();
		body = GetComponent<Rigidbody> ();
	}
	
	// Update is called once per frame.
	void Update () {
		if (GetKey (KeyCode.W, KeyCode.UpArrow)) {
			// Drive forward.
			moveSpeed = Mathf.MoveTowards (moveSpeed, maxSpeed, acceleration);
		} else if (GetKey (KeyCode.S, KeyCode.DownArrow)) {
			// Reverse.
			moveSpeed = Mathf.MoveTowards (moveSpeed, -maxSpeed / 2, acceleration / 2);
		} else if (GetKey (KeyCode.Space)) {
			// Brake.
			moveSpeed = Mathf.MoveTowards (moveSpeed, 0, acceleration);
		} else {
			// Friction.
			moveSpeed = Mathf.MoveTowards (moveSpeed, 0, 0.1f);
		}
		if (Mathf.Abs (moveSpeed) > 0.5f) {
			// Handle turn input.
			if (GetKey (KeyCode.A, KeyCode.LeftArrow)) {
				transform.eulerAngles += Vector3.down * moveSpeed * 2 / maxSpeed;
			}
			if (GetKey (KeyCode.D, KeyCode.RightArrow)) {
				transform.eulerAngles += Vector3.up * moveSpeed * 2 / maxSpeed;
			}
		}
		float angle = Mathf.Deg2Rad * transform.eulerAngles.y;
		body.velocity = new Vector3 (-moveSpeed * Mathf.Cos (angle), body.velocity.y, moveSpeed * Mathf.Sin (angle));
		
		int prevFloor = floor;
		floor = GetFloor (gameObject);
		if (floor != prevFloor) {
			audioController.ChangeFloor ();
		}
		CheckTargetFloor ();
	}

	// Returns true if any of the given keys are pressed.
	bool GetKey (params KeyCode[] keys) {
		foreach (KeyCode key in keys) {
			if (Input.GetKey (key)) {
				return true;
			}
		}
		return false;
	}

	// Plays sounds when the player collides with obstacles.
	void OnCollisionEnter (Collision collision) {
		audioController.Collide (collision);
	}

	// Checks if the target has changed, and changes the target floor if it has.
	void CheckTargetFloor () {
		if (prevIndex != targetIndex) {
			prevIndex = targetIndex;
			int prevFloor = targetFloor;
			targetFloor = GetFloor (target [targetIndex]);
			audioController.ChangeTarget (targetIndex);
		}
	}

	// Calculates the distance between the player and the target.
	public float GetTargetDistance () {
		return Vector3.Distance (transform.position, target [targetIndex].transform.position);
	}

	// Gets the current floor of an object.
	int GetFloor (GameObject gameObject) {
		return (int)(gameObject.transform.position.y - 0.5f) / 8 + 1;
	}
}