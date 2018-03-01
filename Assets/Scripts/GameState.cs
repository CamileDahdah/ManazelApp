using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameState : MonoBehaviour {

	public enum State {HUDPanel = 0, objectPanel = 1, languagePanel = 2, settingsPanel, levelPanel}

	private static State currentState;

	private float accuracy;

	public static State CurrentState{
		
		get{return currentState; }

		set{currentState = value;
			if (UIManager.instance) {
				UIManager.instance.currentPanelString = value.ToString ().ToLower (); 
			}
			if(currentState == State.languagePanel || currentState == State.settingsPanel ||  currentState == State.levelPanel){
				PlayerPrefs.SetInt ( "activeMainPanel" , (int) value);
				PlayerPrefs.Save ();

			}
		}
	}

	public bool firstTimeDebug;

	private bool firstTime;

	public static GameState instance;

	string language;

	void Awake () {
		
		if (instance == null) {
			instance = this;
		} else {
			Destroy (this);
		}

		if (firstTimeDebug) {
			PlayerPrefs.SetInt ("firstTime", 1);
			PlayerPrefs.Save ();
		}

		accuracy = PlayerPrefs.GetFloat ("accuracySlider", 80);

		if (PlayerPrefs.GetInt ("firstTime") > 0) {
			firstTime = true;
		} else {
			firstTime = false;
		}
	}

	void Start(){
		if(firstTime){
			UIManager.instance.EnableCurrentPanel (State.languagePanel);
		}
	}

	public void SetLanguage(string newLanguage){

		language = newLanguage;

	}

	public void SetAccuracy(float newAccuracy){
		accuracy = newAccuracy;
		PlayerPrefs.Save ();
	}

	public float GetAccuracy(){
		
		return accuracy;

	}

	public void SetFirstTime(int newFirsTime){
		if (newFirsTime > 0) {
			firstTime = true;
		} else {
			firstTime = false;
		}
		PlayerPrefs.SetInt ("firstTime", newFirsTime);
		PlayerPrefs.Save ();
	}

	public bool GetFirstTime(){
		return firstTime;
	}

}
