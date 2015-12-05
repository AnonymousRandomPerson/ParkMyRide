using UnityEngine;
using System.Collections;

// Controls the audio interface on the player.
public class AudioController : MonoBehaviour {

	// The driver on the player.
	Driver driver;
	// The floor tracker on the player.
	FloorTracker floorTracker;
	// The audio source on the player used for the interface.
	AudioSource uiSource;
	// The audio source on the player used for the engine sound.
	AudioSource engineSource;
	// The container for the target source.
	GameObject targetSound;
	// The moving audio source for the target.
	AudioSource targetSource;
	[Tooltip("Car collision sound.")]
	public AudioClip carCollide;
	[Tooltip("Warning when a person is in front of the car.")]
	public AudioClip personWarning;
	[Tooltip("Indicates that the player needs to go up a floor.")]
	public AudioClip goUp;
	[Tooltip("Indicates that the player needs to go down a floor.")]
	public AudioClip goDown;
	[Tooltip("Guides the player towards a free parking spot.")]
	public AudioClip rightFloor;
	[Tooltip("Indicates that the player is on the way to find an empty parking spot.")]
	public AudioClip lookForSpot;
	[Tooltip("Indicates that the player is on the way to find the exit.")]
	public AudioClip lookForExit;
	[Tooltip("Indicates that the player arrived at the target.")]
	public AudioClip arriving;
	[Tooltip("The reverse warning sound.")]
	public AudioClip reverseWarning;
	[Tooltip("The obstacle tracker disabling sound.")]
	public AudioClip disable;
	[Tooltip("The obstacle tracker enabling sound.")]
	public AudioClip enable;
	[Tooltip("Notifies the player about a change in parking spot availability.")]
	public AudioClip changeScene;
	[Tooltip("Notifies the player that there are no parking spots available.")]
	public AudioClip noSpot;

	// Minimum time delay between collision sounds being played.
	const int COLLIDETIMERLIMIT = 5;
	// Timer for playing collision sounds.
	int collideTimer = 0;

	// Time delay between sounds that guide the player to a free parking spot.
	int rightFloorTimerLimit = 100;
	// Timer for playing sounds that guide the player to a free parking spot.
	int rightFloorTimer = 0;
	// Whether the player has found its target.
	[HideInInspector]
	public bool found = false;
	[HideInInspector]
	// Whether obstacle sounds are enabled.
	public bool obstacleSounds = true;

	// Timer for scheduling the floor change sound after changing targets.
	int changeFloorTimer = -1;

	// Use this for initialization.
	void Start () {
		driver = transform.GetComponentInParent<Driver> ();
		floorTracker = transform.GetComponentInParent<FloorTracker> ();
		uiSource = GetComponent<AudioSource> ();
		engineSource = transform.parent.GetComponent<AudioSource> ();
		targetSound = transform.FindChild ("Target Sound").gameObject;
		targetSource = targetSound.GetComponent<AudioSource> ();
		ChangeTarget (0, false);
	}
	
	// Update is called once per frame.
	void Update () {
		collideTimer++;
		if (--changeFloorTimer == 0) {
			ChangeFloor ();
		}

		if (floorTracker.floor != floorTracker.targetFloor || found) {
			rightFloorTimer = 0;
		}
		if (++rightFloorTimer > rightFloorTimerLimit) {
			rightFloorTimer = 0;
			float distance = floorTracker.GetTargetDistance ();
			rightFloorTimerLimit = 10 + (int)(90 * distance / 100);
			targetSound.transform.position = transform.position + 2 * Vector3.Normalize (floorTracker.GetTargetVector ());
			targetSource.PlayOneShot (rightFloor);
		}

		float absoluteSpeed = Mathf.Abs (driver.moveSpeed);
		engineSource.volume = absoluteSpeed / driver.maxSpeed / 10;
		engineSource.pitch = Mathf.Min (1, (absoluteSpeed * absoluteSpeed) / (driver.maxSpeed * driver.maxSpeed));
	}
	
	// Plays sounds when the player collides with obstacles.
	public void Collide (Collision collision) {
		if (collideTimer > COLLIDETIMERLIMIT) {
			collideTimer = 0;
			float volume = Mathf.Abs (driver.moveSpeed) / driver.maxSpeed;
			if (collision.collider.tag == "Obstacle") {
				uiSource.PlayOneShot (carCollide, volume);
			}
		}
	}

	// Plays a sound if the player reaches the target.
	public void FoundSpot (Collider collider) {
		if (floorTracker.targetIndex > -1 && !found && collider.name == floorTracker.target[floorTracker.targetIndex].name) {
			uiSource.PlayOneShot (arriving);
			found = true;
		}
	}

	// Plays a sound when looking for a target.
	public void ChangeTarget(int targetIndex, bool sceneChanged) {
		if (targetIndex == 1) {
			uiSource.PlayOneShot (lookForExit);
		} else if (sceneChanged) {
			uiSource.PlayOneShot (changeScene);
		} else {
			uiSource.PlayOneShot (lookForSpot);
		}
		found = false;
		changeFloorTimer = 40;
	}

	// Plays a sound when the player or the target changes floors.
	public void ChangeFloor () {
		if (floorTracker.targetFloor == -1) {
			uiSource.PlayOneShot (noSpot);
		} else if (floorTracker.floor < floorTracker.targetFloor) {
			uiSource.PlayOneShot (goUp);
		} else if (floorTracker.floor > floorTracker.targetFloor) {
			uiSource.PlayOneShot (goDown);
		} else {
			uiSource.PlayOneShot (rightFloor);
		}
	}

	// Plays a sound when the player is reversing and an obstacle is in the way.
	public void ReverseWarning () {
		if (obstacleSounds) {
			uiSource.PlayOneShot (reverseWarning);
		}
	}

	// Toggles obstacle sounds on and off.
	public void ToggleObstacleSound () {
		obstacleSounds = !obstacleSounds;
		uiSource.PlayOneShot (obstacleSounds ? enable : disable);
	}
}
