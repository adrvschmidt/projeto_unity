using UnityEngine;
using System.Collections;

namespace uSimFramework {
	
public class MoveRigidbody : MonoBehaviour {


	
	// Very simple script to move a rigidbody around in absolute directions.
	void FixedUpdate () {
		Vector3 speed = GetComponent<Rigidbody>().velocity;
		speed.x = 10 * Input.GetAxis ("Horizontal");
		speed.z = 10 * Input.GetAxis ("Vertical");
		GetComponent<Rigidbody>().velocity = speed;

	}
}
}
