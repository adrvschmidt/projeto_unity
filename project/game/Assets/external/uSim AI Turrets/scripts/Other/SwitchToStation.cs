using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace uSimFramework {

public class SwitchToStation : MonoBehaviour {

	public TurretCommander turretStation;


	void Update (){

		if (Input.GetKeyUp (KeyCode.F1) && turretStation.commandMode == TurretCommander.CommandModes.FireAtWill)
			turretStation.commandMode = TurretCommander.CommandModes.Manual;
		else if(Input.GetKeyUp (KeyCode.F1) && turretStation.commandMode == TurretCommander.CommandModes.Manual)
			turretStation.commandMode = TurretCommander.CommandModes.FireAtWill;
	}

}
}
