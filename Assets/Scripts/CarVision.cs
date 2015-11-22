using UnityEngine;
using System.Collections;

// Handles stopping the car when the player is in the way.
public class CarVision : MonoBehaviour {

	// The car component of the car.
	CarMover car;

	// Use this for initialization.
	void Start () {
		car = GetComponent<CarMover> ();
	}
	
	// Stops the car and plays a sound when the player is spotted.
	void OnTriggerEnter (Collider collider) {
		if (collider.tag == "Player" && !car.stopped) {
			car.stopped = true;
		}
	}
	
	// Resumes car motion when the player gets out of the way.
	void OnTriggerExit (Collider collider) {
		if (collider.tag == "Player") {
			car.stopped = false;
		}
	}
}
