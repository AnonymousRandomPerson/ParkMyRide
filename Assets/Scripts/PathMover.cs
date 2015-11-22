using UnityEngine;
using System.Collections;

// Travels between points in 3D space.
public class PathMover : MonoBehaviour {

	[Tooltip("The speed of the object.")]
	public float speed = 1;
	[Tooltip("Whether the object faces the way it is moving.")]
	public bool rotates = true;
	// The point the object is currently traveling towards.
	int nextPointIndex = 0;
	// All points in the object's path.
	Vector2[] points;
	// The initial rotation of the object at the start of the scene.
	Quaternion initRotation;

	// Use this for initialization.
	protected void Start () {
		points = new Vector2[transform.childCount];
		for (int i = 0; i < transform.childCount; i++) {
			points[i] = GetXZ (transform.GetChild (i).position);
		}
		initRotation = transform.rotation;
	}
	
	// Update is called once per frame.
	protected void Update () {
		Vector2 xz = GetXZ (transform.position);
		xz = Vector2.MoveTowards (xz, points [nextPointIndex], speed);
		transform.position = SetXZ (transform.position, xz);
		if (rotates) {
			Vector3 look = SetXZ (Vector3.zero, points [nextPointIndex] - xz);
			transform.rotation = Quaternion.LookRotation (look);
			transform.eulerAngles += Vector3.right * 90;
		} else {
			transform.rotation = initRotation;
		}
		if (Vector2.Distance (xz, points [nextPointIndex]) < speed) {
			nextPointIndex++;
			if (nextPointIndex >= points.Length) {
				nextPointIndex = 0;
			}
		}
	}

	// Returns a 2D vector with the x and z components of the given 3D vector.
	private Vector2 GetXZ (Vector3 vector) {
		return new Vector2 (vector.x, vector.z);
	}

	// Sets the x and z components of a 3D vector.
	private Vector3 SetXZ (Vector3 vector3, Vector2 vector2) {
		return new Vector3 (vector2.x, vector3.y, vector2.y);
	}
}
