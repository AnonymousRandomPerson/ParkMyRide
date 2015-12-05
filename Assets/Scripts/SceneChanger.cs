using UnityEngine;
using System.Collections;

// Handles inputs that change the scene.
public class SceneChanger : MonoBehaviour {

	// The player's floor tracker.
	FloorTracker tracker;
	// The player's audio controller.
	AudioController controller;

	[Tooltip("The car to appear on the third floor.")]
	public GameObject car1;
	[Tooltip("The car to appear on the fourth floor.")]
	public GameObject car2;

	// Delay between allowing the target to be changed.
	int targetChangeTimer;
	// Delay between allowing the scene to be changed.
	int sceneChangeTimer;
	// The index of the spot that will be the target when looking for a parking spot.
	int targetSpotIndex;

	// Use this for initialization.
	void Start () {
		tracker = GetComponent<FloorTracker> ();
		controller = GetComponentInChildren<AudioController> ();
	}
	
	// Update is called once per frame.
	void Update () {
		if (--targetChangeTimer < 0 && Input.GetKey (KeyCode.Space)) {
			tracker.targetIndex = tracker.targetIndex == 1 ? targetSpotIndex : 1;
			targetChangeTimer = 20;
			controller.ChangeTarget (tracker.targetIndex, false);
		}
		if (--sceneChangeTimer < 0 && Input.GetKey (KeyCode.LeftControl)) {
			sceneChangeTimer = 20;
			if (targetSpotIndex == 0) {
				targetSpotIndex = 2;
				car1.SetActive (true);
				tracker.target[0].SetActive (false);
				tracker.target[2].SetActive (true);
			} else if (targetSpotIndex == 2) {
				targetSpotIndex = -1;
				car2.SetActive (true);
				tracker.target[2].SetActive (false);
			} else {
				targetSpotIndex = 0;
				car1.SetActive (false);
				car2.SetActive (false);
				tracker.target[0].SetActive (true);
			}

			if (tracker.targetIndex != 1) {
				tracker.targetIndex = targetSpotIndex;
				controller.ChangeTarget (targetSpotIndex, true);
			}
		}
	}
}
