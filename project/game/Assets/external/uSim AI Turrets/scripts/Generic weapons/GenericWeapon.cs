/*** GenericCannon class. V1.1 ***/

//class used to handle weapons fire. HandleFireEvent can be used to customize the firing event.

using UnityEngine;
using System.Collections;

namespace uSimFramework {

public class GenericWeapon : MonoBehaviour {

	//Fire point, where the bullet will be instantiated.
	public Transform FirePoint;
		public AmmoClip ammoClipPrefab;
		AmmoClip ammoClip;
		public int clipCount;
	//The ammo/bullet to use.
	public GameObject AmmoPrefab;
	//The Firing FX
	public GameObject fireFx;
	//is the weapon ready to fire?
	public bool readyToFire;
	//How fast the next ammo is back in the chamber.
	public float reloadTime;
		//How fast the weapon clip can be reloaded.
		public float clipReloadTime;
	//auto timer used for detonation
		[HideInInspector]
	public bool setTimer;
		[HideInInspector]
	public float timer;
	//Internal reload clock. for ammo
	private float reloadTimer;
		//Internal reload clock. for ammo
		private float clipReloadTimer;
		public bool consumeAmmo = false;

	void Start () {

		readyToFire = true;
			ammoClip = new AmmoClip();
			ammoClip.initialAmmoCount = ammoClipPrefab.initialAmmoCount;
			ammoClip.ammoObject = ammoClipPrefab.ammoObject;
		AmmoPrefab = ammoClip.ammoObject.gameObject;

			ammoClip.ammoCount = ammoClip.initialAmmoCount;
	}
	

	public void HandleFireEvent(){

			if(readyToFire && ammoClip.ammoCount > 0 && clipCount > 0) StartCoroutine(Action_Fire());
		// or: yourWeaponScript.FireMyWeapon() as example.

	}

		void FixedUpdate(){

			reloadTimer += Time.deltaTime;

		}

	private IEnumerator Action_Fire (){

		readyToFire = false;
		

			GameObject ammo = (GameObject) Instantiate (AmmoPrefab, FirePoint.position, FirePoint.rotation);
			Instantiate (fireFx, FirePoint.position, FirePoint.rotation, FirePoint.transform);
			reloadTimer = 0;
			if (setTimer) {
			
			
				GennericAmmo ammoScript = ammo.GetComponent<GennericAmmo> ();

				if (ammoScript != null) {

					ammoScript.timed = true;
					ammoScript.lifetime = timer;
				}

			}

			yield return new WaitForEndOfFrame ();

			if (consumeAmmo) {
				ammoClip.ammoCount--;
				if (ammoClip.ammoCount == 0)
					yield return ReloadClip ();
			}
		do{
			yield return new WaitForEndOfFrame ();
		}
		while (reloadTimer < reloadTime);
		
		readyToFire = true;
	}

		IEnumerator ReloadClip () {

			clipCount--;
			if (clipCount <= 0)
				clipCount = 0;
			else
				ammoClip.ammoCount = ammoClip.initialAmmoCount;

			yield return new WaitForSeconds (clipReloadTime);
		}
}
}
