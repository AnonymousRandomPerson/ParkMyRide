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
	// The player camera.
	Transform playerCamera;
	// Camera rotation centered at 0.
	float rotation;
	// The maximum amount that the camera can rotate.
	const float MAXROTATION = 130;
	// The initial rotation of the camera.
	Vector3 initRotation;


	// Use this for initialization.
	void Start () {
		audioController = GetComponentInChildren<AudioController> ();
		body = GetComponent<Rigidbody> ();
		playerCamera = transform.FindChild ("Camera Holder").FindChild("Main Camera");
		initRotation = playerCamera.localEulerAngles;
		Cursor.lockState = CursorLockMode.Locked;
		Cursor.visible = false;
	}
	
	// Update is called once per frame.
	void Update () {
		// Camera movement.
		if (Input.GetMouseButton (0)) {
			// Hold left mouse button to change camera.
			float mouseInput = Input.GetAxis ("Mouse X") * 3;
			if (mouseInput > 0 && rotation < MAXROTATION ||
				mouseInput < 0 && rotation > -MAXROTATION) {
				playerCamera.localEulerAngles = new Vector3 (playerCamera.localEulerAngles.x, playerCamera.localEulerAngles.y + mouseInput, playerCamera.localEulerAngles.z);
				rotation += mouseInput;
			}
		} else {
			playerCamera.localEulerAngles = initRotation;
			rotation = 0;
		}

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
				transform.eulerAngles += Vector3.down * moveSpeed * 3 / maxSpeed;
			}
			if (GetKey (KeyCode.D, KeyCode.RightArrow)) {
				transform.eulerAngles += Vector3.up * moveSpeed * 3 / maxSpeed;
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
		audioController.Collide (collision);
	}
	
	// Signals that the player has arrived at the target position.
	void OnTriggerEnter (Collider collider) {
		if (collider.name == "Free Spot") {
			audioController.foundSpot ();
		}
	}
}