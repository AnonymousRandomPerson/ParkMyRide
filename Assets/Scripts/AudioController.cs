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

	// Minimum time delay between collision sounds being played.
	const int COLLIDETIMERLIMIT = 5;
	// Timer for playing collision sounds.
	int collideTimer = 0;

	// Time delay between sounds that guide the player to a free parking spot.
	int rightFloorTimerLimit = 100;
	// Timer for playing sounds that guide the player to a free parking spot.
	int rightFloorTimer = 0;
	// Whether the player has found its target.
	bool found = false;

	// Timer for scheduling the floor change sound after changing targets.
	int changeFloorTimer = -1;

	// Use this for initialization.
	void Start () {
		driver = transform.GetComponentInParent<Driver> ();
		floorTracker = transform.GetComponentInParent<FloorTracker> ();
		uiSource = GetComponent<AudioSource> ();
		engineSource = transform.parent.GetComponent<AudioSource> ();
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
			uiSource.PlayOneShot (rightFloor);
		}

		float absoluteSpeed = Mathf.Abs (driver.moveSpeed);
		engineSource.volume = absoluteSpeed / driver.maxSpeed / 10;
		engineSource.pitch = Mathf.Min (1, (absoluteSpeed * absoluteSpeed) / (driver.maxSpeed * driver.maxSpeed));
	}
	
	// Plays sounds when the player collides with obstacles.
	public void Collide (Collision collision) {
		if (collideTimer > COLLIDETIMERLIMIT) {
			collideTimer = 0;
			float volume = driver.moveSpeed / driver.maxSpeed;
			if (collision.collider.name == "Car" || collision.collider.name == "Wall") {
				uiSource.PlayOneShot (carCollide, volume);
			}
		}
	}

	public void foundSpot() {
		if (!found) {
			uiSource.PlayOneShot (arriving);
			found = true;
		}
	}

	public void ChangeTarget(int targetIndex) {
		if (targetIndex == 1) {
			uiSource.PlayOneShot (lookForExit);
		} else {
			uiSource.PlayOneShot (lookForSpot);
		}
		found = false;
		changeFloorTimer = 40;
	}

	// Plays a sound when the player or the target changes floors.
	public void ChangeFloor () {
		if (floorTracker.floor < floorTracker.targetFloor) {
			uiSource.PlayOneShot (goUp);
		} else if (floorTracker.floor > floorTracker.targetFloor) {
			uiSource.PlayOneShot (goDown);
		} else {
			uiSource.PlayOneShot (rightFloor);
		}
	}

	// Plays a sound when a person gets in the way of the player.
	void OnTriggerEnter (Collider collider) {
		if (collider.name == "Person") {
			uiSource.PlayOneShot (personWarning);
		} else if (collider.name == "Free Spot") {
			foundSpot();
		}
	}
}
