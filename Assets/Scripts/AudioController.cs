using UnityEngine;
using System.Collections;

// Controls the audio interface on the player.
public class AudioController : MonoBehaviour {

	// The driver on the player.
	Driver driver;
	// The audio source on the player used for the interface.
	AudioSource uiSource;
	// The audio source on the player used for the engine sound.
	AudioSource engineSource;
	[Tooltip("The default sound used if the sound to be played is not implemented yet.")]
	public AudioClip defaultBeep;
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

	// Minimum time delay between collision sounds being played.
	const int COLLIDETIMERLIMIT = 5;
	// Timer for playing collision sounds.
	int collideTimer = 0;

	// Time delay between sounds that guide the player to a free parking spot.
	int rightFloorTimerLimit = 10;
	// Timer for playing sounds that guide the player to a free parking spot.
	int rightFloorTimer = 0;

	// Use this for initialization.
	void Start () {
		driver = transform.parent.GetComponent<Driver> ();
		uiSource = GetComponent<AudioSource> ();
		engineSource = transform.parent.GetComponent<AudioSource> ();
	}
	
	// Update is called once per frame.
	void Update () {
		collideTimer++;

		if (driver.floor != driver.targetFloor) {
			rightFloorTimer = 0;
		}
		if (++rightFloorTimer > rightFloorTimerLimit) {
			rightFloorTimer = 0;
			float distance = driver.GetTargetDistance ();
			uiSource.PlayOneShot (rightFloor);
		}

		engineSource.volume = driver.moveSpeed / driver.maxSpeed / 10;
		engineSource.pitch = Mathf.Min (1, (driver.moveSpeed * driver.moveSpeed) / (driver.maxSpeed * driver.maxSpeed));

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

	// Plays a sound when the player or the target changes floors.
	public void ChangeFloor () {
		if (driver.floor < driver.targetFloor) {
			uiSource.PlayOneShot (goUp);
		} else if (driver.floor > driver.targetFloor) {
			uiSource.PlayOneShot (goDown);
		} else {
			uiSource.PlayOneShot (rightFloor);
		}
	}

	// Plays a sound when a person gets in the way of the player.
	void OnTriggerEnter (Collider collider) {
		if (collider.name == "Person") {
			uiSource.PlayOneShot (personWarning);
		}
	}
}
