using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace uSimFramework {

public class TurretCommander_aimFire : MonoBehaviour {

	TurretCommander turretCommander;
	public Transform camTarget;
	float distance = 2.0f;

	float xSpeed = 250.0f;
	float ySpeed = 120.0f;

	float yMinLimit = -20f;
	float yMaxLimit = 80f;

	private float x = 0.0f;
	private float y = 0.0f;
	// Use this for initialization
	void Start () {

		turretCommander = GetComponent<TurretCommander> ();

	}
	
	// Update is called once per frame
	void Update () {
			
			if (turretCommander.commandMode == TurretCommander.CommandModes.FireAtWill)
			return;

			if (Input.GetKey (KeyCode.LeftControl))
				return;

			turretCommander.fireCommandOn = Input.GetMouseButton (0);

		if (camTarget) {
				x += Input.GetAxis("Mouse X") * xSpeed * 0.02f;
				y -= Input.GetAxis("Mouse Y") * ySpeed * 0.02f;

				y = ClampAngle(y, yMinLimit, yMaxLimit);

			Quaternion rotation = Quaternion.Euler(y, x, 0f);
			Vector3 position = rotation * new Vector3(0.0f, 0.0f, -distance) + camTarget.position;

			turretCommander.camera.transform.rotation = rotation;
			turretCommander.camera.transform.position = position;
			}
		




	}

	float ClampAngle (float angle,float min,float max) {
		if (angle < -360)
			angle += 360;
		if (angle > 360)
			angle -= 360;
		return Mathf.Clamp (angle, min, max);
	}
}
}
