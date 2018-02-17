using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameState : MonoBehaviour {

	public enum State {mainGame = 0, objectUI = 1}

	public static State currentState;

	public static GameState instance;

	void Awake () {
		
		if (instance == null) {
			instance = this;
		} else {
			Destroy (this);
		}
		 
		currentState = State.mainGame;
	}
	

}
