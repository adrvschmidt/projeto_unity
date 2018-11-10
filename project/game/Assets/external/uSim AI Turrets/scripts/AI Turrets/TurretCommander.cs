using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace uSimFramework {

public class TurretCommander : MonoBehaviour {

	public enum CommandModes {Manual, FireAtWill};

	public CommandModes commandMode;
	CommandModes lastMode;
	public Transform camera;
	public GameObject hud;
	public Transform targetMarker;
	public bool fireCommandOn;
	public Turret_main[] artilleryHardpoints;
	
	void Start (){

		lastMode = commandMode;

	}

	void Update () {

		if (commandMode == CommandModes.Manual) {
		
				//targetMarker.position = SightRaycast ();		
			
		}

		if (commandMode != lastMode) {

			if (commandMode == CommandModes.Manual)
				SwitchToManual ();
			if (commandMode == CommandModes.FireAtWill)
				SwitchToAuto ();

			lastMode = commandMode;
		}
	}

	void FixedUpdate () {

		foreach (Turret_main hardpoint in artilleryHardpoints) {

			if (fireCommandOn)
				hardpoint.FireCommand ();

			if (commandMode == CommandModes.Manual) {
				hardpoint.Target = targetMarker.gameObject;
				if(hardpoint.state != Turret_main.TurretState.Firing)
				hardpoint.state = Turret_main.TurretState.Tracking;
			}
		}

	}

	void SwitchToManual () {

		foreach (Turret_main hardpoint in artilleryHardpoints) {

			hardpoint.autoEngage = false;
			hardpoint.state = Turret_main.TurretState.Tracking;
			hardpoint.Target = targetMarker.gameObject;
			hud.SetActive (true);
		}

	}

	void SwitchToAuto () {

		foreach (Turret_main hardpoint in artilleryHardpoints) {

			hardpoint.autoEngage = true;
			hardpoint.state = Turret_main.TurretState.Standby;
			hardpoint.Target = null;
			hud.SetActive(false);

		}

	}

	Vector3 SightRaycast () {

			Vector3 defaultValue = camera.transform.position + (camera.transform.forward * (artilleryHardpoints [0].range - 1f));
		RaycastHit hit = new RaycastHit();
			if (Physics.Raycast (camera.transform.position, camera.transform.forward, out hit)) {
				if (hit.collider.transform.root.tag != "Player")
					return hit.point;
				else
					return defaultValue;
		} else
				return defaultValue;
	}
}
}
