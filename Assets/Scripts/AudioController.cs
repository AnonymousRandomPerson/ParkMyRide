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

	// Minimum time delay between collision sounds being played
	const int COLLIDETIMERLIMIT = 5;
	// Timer for playing collision sounds.
	int collideTimer;

	// Use this for initialization.
	void Start () {
		driver = transform.parent.GetComponent<Driver> ();
		uiSource = GetComponent<AudioSource> ();
		engineSource = transform.parent.GetComponent<AudioSource> ();
		collideTimer = COLLIDETIMERLIMIT;
	}
	
	// Update is called once per frame.
	void Update () {
		collideTimer--;
		engineSource.volume = driver.moveSpeed / driver.maxSpeed / 10;
		engineSource.pitch = Mathf.Min (1, (driver.moveSpeed * driver.moveSpeed) / (driver.maxSpeed * driver.maxSpeed));
	}

	// Plays an audio clip, or a beep if the clip hasn't been implemented yet.
	void PlaySound (AudioClip clip) {
		if (clip == null) {
			uiSource.PlayOneShot (defaultBeep);
		} else {
			uiSource.PlayOneShot (clip);
		}
	}
	
	// Plays sounds when the player collides with obstacles.
	public void Collide (Collision collision) {
		if (collideTimer < 0) {
			collideTimer = COLLIDETIMERLIMIT;
			float volume = driver.moveSpeed / driver.maxSpeed;
			if (collision.collider.name == "Car" || collision.collider.name == "Wall") {
				uiSource.PlayOneShot (carCollide, volume);
			}
		}
	}
}
