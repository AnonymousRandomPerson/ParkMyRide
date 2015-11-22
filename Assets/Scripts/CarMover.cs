using UnityEngine;
using System.Collections;

// Stops if the player gets in the way.
public class CarMover : PathMover {

	// Whether the car has stopped due to the player being in the way.
	[HideInInspector]
	public bool stopped = false;
	// The moving speed of the car.
	float startSpeed;
	// The amount that speed is changed every tick when changing states.
	float speedIncrement;

	// Use this for initialization.
	new void Start () {
		startSpeed = speed;
		speedIncrement = startSpeed / 40;
		base.Start ();
	}
	
	// Update is called once per frame.
	new void Update () {
		if (stopped) {
			if (speed > 0) {
				speed -= speedIncrement;
			} else if (speed < 0) {
				speed = 0;
			}
		} else if (speed < startSpeed) {
			speed += speedIncrement;
		}
		base.Update ();
	}
}
