using UnityEngine;
using UnityEngine.UI;
using System.Collections;

// Keeps track of the floor the player is on.
public class FloorTracker : MonoBehaviour {
	
	// The audio controller for the UI.
	AudioController audioController;
	// The current floor the car is on.
	[HideInInspector]
	public int floor = 1;
	// The floor the car needs to get to.
	[HideInInspector]
	public int targetFloor = 0;
	[Tooltip("The index of the location the car wants to get to.")]
	public int targetIndex = 0;
	[Tooltip("The locations the car wants to get to.")]
	public GameObject[] target;
	[Tooltip("The UI text element that displays the current and target floors.")]
	public Text floorText;
	
	// The index of the location the car wanted to get to on the previous frame.
	int prevIndex = -1;

	// Use this for initialization.
	void Start () {
		audioController = GetComponentInChildren<AudioController> ();
	}
	
	// Update is called once per frame.
	void Update () {
		int prevFloor = floor;
		floor = GetFloor (gameObject);
		if (floor != prevFloor) {
			audioController.ChangeFloor ();
		}
		CheckTargetFloor ();
		floorText.text = floor + " -> " + targetFloor;
	}
	
	// Checks if the target has changed, and changes the target floor if it has.
	void CheckTargetFloor () {
		if (prevIndex != targetIndex) {
			prevIndex = targetIndex;
			targetFloor = GetFloor (target [targetIndex]);
			audioController.ChangeTarget (targetIndex);
		}
	}

	// Calculates the vector from the player to the target.
	public Vector3 GetTargetVector () {
		return target [targetIndex].transform.position - transform.position;
	}
	
	// Calculates the distance between the player and the target.
	public float GetTargetDistance () {
		return Vector3.Distance (transform.position, target [targetIndex].transform.position);
	}
	
	// Gets the current floor of an object.
	public int GetFloor (GameObject gameObject) {
		return (int)(gameObject.transform.position.y - 0.5f) / 8 + 1;
	}
}
