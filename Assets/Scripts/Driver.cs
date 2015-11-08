using UnityEngine;
using System.Collections;

// Controls for the car.
public class Driver : MonoBehaviour {

	// The car's rigidbody.
	Rigidbody body;
	// The audio controller for the UI.
	AudioController audio;
	[Tooltip("The current speed of the car.")]
	public float moveSpeed = 0;
	[Tooltip("The maximum speed of the car.")]
	public float maxSpeed;
	[Tooltip("The acceleration of the car per frame.")]
	public float acceleration;

	// Use this for initialization.
	void Start () {
		audio = GetComponentInChildren<AudioController> ();
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
			moveSpeed = Mathf.MoveTowards (moveSpeed, 0, acceleration);
		} else {
			// Friction.
			moveSpeed = Mathf.MoveTowards (moveSpeed, 0, 0.1f);
		}
		if (moveSpeed > 0.5f) {
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
		audio.Collide (collision);
	}
}