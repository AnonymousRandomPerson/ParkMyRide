using UnityEngine;
using System.Collections;

// Warns the driver about obstacles when in reverse.
public class ReverseSensor : MonoBehaviour {

	// The audio source to play the warning from.
	AudioController audioController;
	// The player.
	Driver driver;
	// Timer used to repeatedly play the sound.
	int timer;
	// Whether the sound is playing.
	bool triggered = false;

	// Use this for initialization.
	void Start () {
		audioController = GetComponentInParent<AudioController> ();
		driver = GetComponentInParent<Driver> ();
	}
	
	// Plays the warning sound when the player is reversing and an obstacle is present.
	void Update () {
		if (triggered && --timer < 0 && driver.moveSpeed < 0) {
			timer = 20;
			audioController.ReverseWarning ();
		}
	}
	
	// Starts the warning sound.
	void OnTriggerEnter (Collider collider) {
		if (collider.tag != "Finish") {
			triggered = true;
		}
	}

	// Stops the warning sound.
	void OnTriggerExit (Collider collider) {
		if (collider.tag != "Finish") {
			triggered = false;
		}
	}
}
