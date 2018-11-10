using UnityEngine;
using System.Collections;

namespace uSimFramework {

public class GennericAmmo : MonoBehaviour {

	public float muzzleVelocity;
	public GameObject hitFx;
	public bool timed;
	public float timeToDetonate;
	//[HideInInspector]
	public float lifetime;

	// Use this for initialization
	void Start () {
			
		lifetime = 8f;
		GetComponent<Rigidbody>().velocity = transform.TransformDirection (Vector3.forward * muzzleVelocity);
			if (timed)
				lifetime = timeToDetonate;
			
	}

	
	// Update is called once per frame
	void Update () {
		lifetime -= Time.deltaTime;
			if (lifetime <= 0f) {
				if (!timed)
					Destroy (gameObject);
				else
					Detonate ();

			}
	}

	void OnCollisionEnter (Collision collision){

		//Add impact handling here. Recomended use of SendMessage
		//collision.collider.gameObject.SendMessage("ApplyDamage", damage);
			print ("hit! " + collision.collider.gameObject.name);
			Detonate ();
	}

		void Detonate (){

			Instantiate (hitFx, transform.position, transform.rotation);
			Destroy (gameObject);
		}
}

}
