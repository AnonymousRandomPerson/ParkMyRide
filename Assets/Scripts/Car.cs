using UnityEngine;
using System.Collections;

// Randomizes the car's color.
public class Car : MonoBehaviour {

	// Use this for initialization.
	void Start () {
		// Randomly color the car.
		GetComponent<Renderer> ().material.color = new Color (Random.value, Random.value, Random.value, 1);
	}
}
