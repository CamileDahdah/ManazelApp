using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameState : MonoBehaviour {

	public enum State { HUDPanel = 0, objectPanel = 1, languagePanel = 2, settingsPanel, levelPanel, greenPopupPanel, blurPanel, errorPopupPanel }

	private static State currentState;

	private float accuracy;

	public static State CurrentState{
		
		get{ 
			return currentState; 
		}

		set{
			currentState = value;
			
			if (UIManager.instance) {
				UIManager.instance.currentPanelString = value.ToString ().ToLower (); 
			}
			if(currentState == State.languagePanel || currentState == State.settingsPanel ||  currentState == State.levelPanel){
				PlayerPrefs.SetInt ( "activeMainPanel" , (int) value);
				PlayerPrefs.Save ();

			}

			if(currentState == State.HUDPanel){
				
				UIManager.instance.EnableBlur (false);

			}else{
				
				UIManager.instance.EnableBlur (true);

			}

		}
	}

	private bool firstTime;

	public static GameState instance;

	string language;

	void Awake () {
		
		if (instance == null) {
			instance = this;
		} else {
			Destroy (this);
		}

		accuracy = PlayerPrefs.GetFloat ("accuracySlider", 80);

		if (PlayerPrefs.GetInt ("firstTime", 1) > 0) {
			firstTime = true;
		} else {
			firstTime = false;
		}

	}

	void Start(){
		
		if (firstTime) {
			UIManager.instance.EnableCurrentPanel (State.languagePanel);
		} else {
			UIManager.instance.EnableCurrentPanel (State.HUDPanel);
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
