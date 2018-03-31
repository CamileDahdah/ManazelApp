using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputManager : MonoBehaviour {

	public CameraMovement cameraMovement;
	public RayCastHandler raycastHandler;

	public static InputManager instance;

	void Awake(){
		
		if (instance == null) {
			instance = this;
		} else {
			Destroy (this);
		}
	}

	public void DisableInput(){
		raycastHandler.enabled = false;
		cameraMovement.enabled = false;
	}

	public void EnableInput(){
		raycastHandler.enabled = true;
		cameraMovement.enabled = true;
	}

}
