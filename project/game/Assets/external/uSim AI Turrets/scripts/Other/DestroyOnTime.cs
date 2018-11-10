using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace uSimFramework {

public class DestroyOnTime : MonoBehaviour {

		public float timeToDestroy;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void FixedUpdate () {
		
			timeToDestroy -= Time.deltaTime;
			if (timeToDestroy <= 0f)
				Destroy (gameObject);
	}
}
}
