using UnityEngine;
using System.Collections;

public class HazardSensor : MonoBehaviour {

	[Tooltip("A list of objects in the scene to consider as hazards.")]
	public GameObject[] hazards;

	// The audio source that plays hazard indicator sounds.
	AudioSource source;
	// Counters for playing sounds for each hazard.
	int[] hazardCounters;
	// The floor tracker on the player.
	FloorTracker tracker;

	// Use this for initialization.
	void Start () {
		if (hazards == null) {
			hazards = new GameObject[0];
		}
		hazardCounters = new int[hazards.Length];
		for (int i = 0; i < hazards.Length; i++) {
			float distance = Vector3.Distance (hazards[i].transform.position, transform.parent.position);
			hazardCounters[i] = 10 + (int)(90 * distance / 100);
		}
		source = GetComponent<AudioSource> ();
		tracker = GetComponentInParent<FloorTracker> ();
	}
	
	// Update is called once per frame.
	void Update () {
		for (int i = 0; i < hazards.Length; i++) {
			GameObject hazard = hazards[i];
			if (tracker.GetFloor (hazard) == tracker.floor) {
				float distance = Vector3.Distance (hazard.transform.position, transform.parent.position);
				int distanceTime = (int)Mathf.Max (10 + (int)(90 * distance / 100), 5);
				hazardCounters[i] = Mathf.Min (hazardCounters[i], distanceTime);
				if (--hazardCounters[i] < 0) {
					transform.position = transform.parent.position + 2 * Vector3.Normalize (hazard.transform.position - transform.parent.position);
					hazardCounters[i] = distanceTime;
					source.PlayOneShot(source.clip);
				}
			}
		}
	}
}
