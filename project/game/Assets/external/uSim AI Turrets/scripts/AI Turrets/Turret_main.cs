/*** Turret main class. V1.0 ***/
 

using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace uSimFramework {

public class Turret_main : MonoBehaviour {

	//Hardpoint sub-class. Used to create and manage mounted weapons and rigging configurations.
	[System.Serializable]
	public class TurretHardpoint {

		//The reference transform used as pivot point for rigging.
		public Transform pivotPoint;
		//NOTE: For firing turret will call weaponScript.HandleFireEvent(); in the hardpoint. 
		//So users can just add the method in their own weapons scripts
		//to handle the fire action.
		public GenericWeapon weaponScript;

		//*** CONFIGURATIONS ***\\\
		public float movementSpeed;
		public float maxElevationAngle;
		public float minElevationAngle;

	}

	//Turret states
	public enum TurretState {Searching, Tracking, Firing, Standby, Disabled, Destroyed};
	//Current target
	public GameObject Target;
	//target distance
	public float rangeToTarget;
	//Tracked objects: All objects with enemy tag inside turret range;
	public List<GameObject> trackedObjects;
	//Current state
	public TurretState state;

	//Hardpoints
	public TurretHardpoint[] Hardpoints;
	
	public Transform attachedTo;
	public Transform viewPoint;
	//*** CONFIGURATIONS ***\\\

	//Enemies tags
	public string[] enemiesTags;
	//Enemies layers
	public string[] enemiesLayers;
	//Turret range
	public float range;
	// used for flak style ammo
	public bool calculateDetonationTime;
	//should the turret look for targets automatically while on stand by
	public bool autoEngage;
	//Lead amount: how much ahead the target direction the turret should aim. (based on target velocity vector).
	public float leadCoefficient;
	//Horizontal movement coefficient.
	public float horizontalMovement;
	//isthe turret upside down?
	public bool invertVerticalTracking;
	//Absolute clamping angles. from and to. for example from 270 (west) to 90 (east).
	// if parent is null and absolute angles are set to other than 0, this takes effect.
	// set to 0 for free rotation.
	[HideInInspector]
	public float absoluteAngleFrom;
	[HideInInspector]
	public float absoluteAngleTo;
	//Relatve clamping angles. from and to. in this case we use signed values. for example from -45 to 45 
	//relative to parent forward direction.
	//if turret has parent and relative angles not set to 0, this takes effect.
	//set to 0 for free rotation.
	[HideInInspector]
	public float relativeAngleFrom;
	[HideInInspector]
	public float relativeAngleTo;
		public	float detonationTime;
	//determine when to start shooting based on angle from target.
	public float maximumShootAngle;
	public float leadFactor;
	//this is based on ammo setup.  muzzle speed.
	public float targetbulletspeed = 5;

	//initial rotation angle
		public float initalAngle = 0f;
		public float deltaAngle = 0f;
		public float maxDeltaAngle = 180f;
	//Console
	[HideInInspector]
	public List<string> consoleMsg;


	//states flag used to wait on coroutines
	internal bool searching;
	internal bool targeting;

	//internal
	Vector3 dirToTarget;

	Vector3 initialEulers;


	// Use this for initialization
	void Start () {
		initialEulers = transform.localEulerAngles;
			initalAngle = initialEulers.y;
	}
		public bool calculateBalistic;
	public float balisticAngleAdd;
		float timeToUpdate = 5f;
	// Update is called once per frame
	void FixedUpdate () {

		
	
			if (autoEngage) {

				timeToUpdate -= Time.deltaTime;
				if (timeToUpdate <= 0f) {
					timeToUpdate = 5f;
					state = TurretState.Standby;
				}
			}

		switch (state){

			//Searching: We update the enemy entities inside turrent range based on enemy tags;
			case TurretState.Searching :

				if(!searching){
					consoleMsg.Add ("Searching...");
					StartCoroutine (UpdateEnemiesInRange());
					state = TurretState.Standby;
				}
				
				break;

			case TurretState.Tracking:
			
				if (Target == null || !Target.gameObject.activeSelf) {
					consoleMsg.Add ("No Target designated... standby...");
					state = TurretState.Searching;
				}
				
				rangeToTarget = Vector3.Distance (Target.transform.position, transform.position);

				balisticAngleAdd = ((rangeToTarget / targetbulletspeed)) / 100f;
				if (autoEngage) {
					if (rangeToTarget > range) {
						Target = null;
						state = TurretState.Searching;
						break;
					}
				}

				//time it takes for bullet to hit target.
				float btime = rangeToTarget / targetbulletspeed;
				detonationTime = btime;
				//Pseudo AI calculation of leading coefficient.
				leadCoefficient = Mathf.Lerp (leadCoefficient, Random.Range (0.5f, 1.5f) * leadFactor, Time.deltaTime * 5);
				//TODO: Need to add a distance coefficient for leading. right now it only consider target velocity.

				//Horizontal tracking\\
				
				Vector3 aimingSolution = new Vector3 (Target.transform.position.x, Target.transform.position.y, Target.transform.position.z); 

				//If the target is a rigidbody we can calculate leading aim and add to target solution.
				if (Target.GetComponent<Rigidbody> () != null) {
					Rigidbody targetRb = Target.GetComponent<Rigidbody> ();
					//aimingSolution += transform.InverseTransformDirection (targetRb.velocity.x * btime * leadCoefficient, targetRb.velocity.y * btime * leadCoefficient, targetRb.velocity.z * btime * leadCoefficient);
					aimingSolution += new Vector3 (targetRb.velocity.x * btime * leadCoefficient, targetRb.velocity.y * btime * leadCoefficient, targetRb.velocity.z * btime * leadCoefficient);
				}

				Vector3 invertAimingSolution = transform.InverseTransformPoint (aimingSolution);
				invertAimingSolution.y = 0f;

				Vector3 backaimingSolution = transform.TransformPoint (invertAimingSolution);

				float yOffset = 0f;
				Vector3 up = Vector3.up;
				if (attachedTo != null) 
					up = attachedTo.up;



				var targetRotation = Quaternion.LookRotation (backaimingSolution - transform.position, up);
				if (attachedTo == null)
					deltaAngle = initalAngle - transform.InverseTransformDirection (targetRotation.eulerAngles).y;
				else
					deltaAngle = transform.localEulerAngles.y - initalAngle;
						
				if (deltaAngle < -maxDeltaAngle)
					deltaAngle = -maxDeltaAngle;
				if (deltaAngle > maxDeltaAngle)
					deltaAngle = maxDeltaAngle;
				
				Vector3 targetRotationEulers = targetRotation.eulerAngles;
				//targetRotationEulers.x = 0f;
				//targetRotationEulers.z = 0f;
				//Vector3 turretAngles = transform.localEulerAngles;
				//turretAngles = targetRotationEulers;
				//transform.localEulerAngles = turretAngles;
			//	targetRotation = Quaternion.Euler (targetRotationEulers);
				transform.rotation = Quaternion.Slerp (transform.rotation, targetRotation, Time.deltaTime * horizontalMovement); 
				//transform.localRotation = targetRotation;
				Vector3 turretAngles = transform.localEulerAngles;
				turretAngles.x = turretAngles.z = 0f;
				//turretAngles.y = Mathf.Lerp (turretAngles.y, turretAngles.y - yOffset, Time.deltaTime * horizontalMovement);
				transform.localEulerAngles = turretAngles;

				if (maxDeltaAngle > 0) {
					Vector3 rotEulers = transform.localRotation.eulerAngles;
					rotEulers.y = Mathf.Clamp (rotEulers.y, initalAngle - maxDeltaAngle, initalAngle + maxDeltaAngle);
					transform.localRotation = Quaternion.Euler (rotEulers);
				}

			//Vector3 resulteulers = transform.localEulerAngles;
			//resulteulers.x = initialEulers.x;
			//resulteulers.z = initialEulers.z;
			//transform.localEulerAngles = resulteulers;



				//Hardpoints vertical tracking\\

				Vector3 verticalSolution = new Vector3 (0, 0, 0);
				//Again, if target is a rigidbody we add some leading.
				if (Target.GetComponent<Rigidbody> () != null) {
					verticalSolution = new Vector3 (0, Target.GetComponent<Rigidbody> ().velocity.y * leadCoefficient, 0);
				}
				var inverseTarget = transform.InverseTransformPoint (Target.transform.position + (verticalSolution));
				dirToTarget = inverseTarget - transform.position;
				float targetAngle = Mathf.Atan2 (inverseTarget.y, inverseTarget.z) * Mathf.Rad2Deg;
			//float targetAngle = Quaternion.LookRotation(dirToTarget,transform.up).eulerAngles.x;
				//Iteract trough hardpoints and move them vertically
				
				foreach (TurretHardpoint hardpoint in Hardpoints) {
					//limit targetAngle to hardpoint settings. each hardpoint could have different settings.
					float limitAngle = Mathf.Clamp (targetAngle, hardpoint.minElevationAngle, hardpoint.maxElevationAngle);	
					if (invertVerticalTracking)
						limitAngle *= -1;
					if (!calculateBalistic)
						balisticAngleAdd = 0f;
					Quaternion rotationVertical = Quaternion.Euler (-limitAngle - balisticAngleAdd, 0, 0);
					hardpoint.pivotPoint.localRotation = Quaternion.Slerp (hardpoint.pivotPoint.localRotation, rotationVertical, Time.deltaTime * hardpoint.movementSpeed);
				}				

				if (autoEngage)
					state = TurretState.Firing;
			
				//check Line Of Sight
			if (!checkingLineOfSight)
				StartCoroutine (CheckLineOfSight ());

			if (!lineOfSight)
				state = TurretState.Standby;

				break;

		case TurretState.Firing:

			FireCommand ();	

				if(Target)
					state = TurretState.Tracking;
				else 
					state = TurretState.Standby;
				
				break;

		case TurretState.Standby:

			if (autoEngage) {
				//if enemy in range start firing.
				if (trackedObjects.Count > 0) {
					if (!targeting)
						StartCoroutine (FindClosestTarget ());
					else {
						if (Target == null)
							state = TurretState.Searching;
						else
							state = TurretState.Tracking;
					}
				} else
					state = TurretState.Searching;
			}

				break;

			case TurretState.Disabled :
				
				break;

			case TurretState.Destroyed:

				if (gameObject.activeSelf)
					gameObject.SetActive (false);

				break;

		}

	}

	public void FireCommand(){

		foreach (TurretHardpoint hardpoint in Hardpoints){

				if (calculateDetonationTime) {
					hardpoint.weaponScript.setTimer = true;
					hardpoint.weaponScript.timer = UnityEngine.Random.Range (detonationTime * 0.9f, detonationTime * 1.1f);

				}

						
		}
			StartCoroutine (FireEvent ());		
	}

		private IEnumerator FireEvent (){

			yield return new WaitForEndOfFrame ();
			foreach (TurretHardpoint hardpoint in Hardpoints) {
				hardpoint.weaponScript.HandleFireEvent ();
			}

		}

	private IEnumerator UpdateEnemiesInRange (){

		//we set searching to true to avoid calling the coroutine again from State Machine.
		searching = true;
		
		foreach (string tag in enemiesTags){
			
			var foundObjects = GameObject.FindGameObjectsWithTag (tag);
			
			foreach (GameObject obj in foundObjects){
				
				if(Vector3.Distance (obj.transform.position, transform.position) < range) {

						AddTarget (obj);
				}
			}
		}

			yield return new WaitForEndOfFrame ();

			GameObject[] foundObjects2 = GameObject.FindObjectsOfType<GameObject> ();

			foreach (GameObject obj in foundObjects2){

					if(Vector3.Distance (obj.transform.position, transform.position) < range ) {

					foreach (string layer in enemiesLayers){
						if(obj.layer == LayerMask.NameToLayer(layer))
						AddTarget (obj);
						}
					}
				}
		
	

		//we wait some time not to overload with search routine...
		yield return new WaitForSeconds (3);
		searching = false;
	}

		public void AddTarget (GameObject target){

			if(!trackedObjects.Contains (target)){

				trackedObjects.Add (target);
				consoleMsg.Add ("Added enemy : " + target.name);

			}

		}

	private IEnumerator FindClosestTarget()
	{
		targeting = true;
		float cdistance = range + 10;
		GameObject ftarget = trackedObjects[0];
		foreach(GameObject ctarget in trackedObjects)
		{
			float distance = Mathf.Abs(Vector3.Distance(ctarget.transform.position, transform.position));
			if(distance < cdistance)
			{
				cdistance = distance;
				ftarget = ctarget;
			}
		}
		rangeToTarget = Vector3.Distance(ftarget.transform.position, transform.position);
		if(rangeToTarget > range)
			ftarget = null;
		else
			consoleMsg.Add ("Locking in Target : " + ftarget.name);
		Target = ftarget;
		//we wait some time not to overload with target routine...
		yield return new WaitForSeconds (3);
		targeting = false;
	}

	bool lineOfSight;
	bool checkingLineOfSight;
	IEnumerator CheckLineOfSight (){

		checkingLineOfSight = true;

		lineOfSight = true;

		RaycastHit hit = new RaycastHit ();
			if (Physics.Raycast (Hardpoints [0].weaponScript.FirePoint.position, dirToTarget, out hit, range)) {
			if (hit.distance < rangeToTarget)
				lineOfSight = false;
		}

		yield return new WaitForEndOfFrame ();
		checkingLineOfSight = false;
	}
	/*
	void OnGUI (){

		if(consoleMsg.Count > 0)
		GUI.Label (new Rect (10,10,200,50), "Turret msg : " + consoleMsg[consoleMsg.Count -1]);

	}
*/
}
}
