using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScriptManager : MonoBehaviour {

	public DragCamera dragCamera;
	public RayCast raycast;

	public static ScriptManager instance;

	void Awake(){
		if (instance == null) {
			instance = this;
		} else {
			Destroy (this);
		}
	}

	public void DisableInput(){
		raycast.enabled = false;
		dragCamera.enabled = false;
	}

	public void EnableInput(){
		raycast.enabled = true;
		dragCamera.enabled = true;
	}
}
