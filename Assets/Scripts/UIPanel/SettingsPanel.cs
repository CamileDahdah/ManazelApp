using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

//TODO refactor code since this class was written in a hurry

public class SettingsPanel : MonoBehaviour {

	public GameObject audioToggle, tagsToggle, helpToggle, accuracySlider;
	public Button audioToggleButton, tagsToggleButton, helpToggleButton;
	int audioOn, tagsOn, helpOn;
	private Animator audioToggleAnim, tagsToggleAnim, helpToggleAnim;
	string disabled = "Disabled", enabled = "Enabled";
	public GameObject next, previous;
	private Slider slider;
	public Button xButton;

	void Start () {
		
		audioOn = PlayerPrefs.GetInt ("audioOn", 1);
		tagsOn = PlayerPrefs.GetInt ("tagsOn", 1);
		helpOn = PlayerPrefs.GetInt ("helpOn", 1);

		audioToggleAnim = audioToggle.GetComponent<Animator>();
		tagsToggleAnim = tagsToggle.GetComponent<Animator>();
		helpToggleAnim = helpToggle.GetComponent<Animator>();

		InitializeToggles ();

		if (PlayerPrefs.GetInt ("Mute", 0) > 0) {
			DisableSound (true);
		} else {
			DisableSound (false);
		}
			
		audioToggleButton.onClick.AddListener (delegate {
			AudioToggle();

			}
		);

		tagsToggleButton.onClick.AddListener (delegate {
			TagsToggle();

			}
		);

		helpToggleButton.onClick.AddListener (delegate {
			HelpToggle();

			}
		);

		slider = accuracySlider.GetComponent<Slider>();

		slider.value = GameState.instance.GetAccuracy();

		next.GetComponent<Button> ().onClick.AddListener (delegate {
			UIManager.instance.MovePanelRight(GameState.State.levelPanel);
		});

		previous.GetComponent<Button> ().onClick.AddListener (delegate {
			UIManager.instance.MovePanelLeft(GameState.State.languagePanel);
		});

		xButton.GetComponent<Button> ().onClick.AddListener (delegate {
			UIManager.instance.EnableCurrentPanel (GameState.State.HUDPanel);
		});

	}

	void OnEnable(){

		//dont enable X Button for the first time user so that he passes through all the necessary panels
		if (!GameState.instance.GetFirstTime()) {
			xButton.gameObject.SetActive (true);

		} else {
			xButton.gameObject.SetActive (false);
		}

	}


	public void DeselectSlider(){
		PlayerPrefs.SetFloat ("accuracySlider", slider.value);
		GameState.instance.SetAccuracy (slider.value);
		PlayerPrefs.Save ();
		Debug.Log ("Accuracy Saved");
	}

	void InitializeToggles(){

		if (GetAudioOn() > 0) {
			audioToggleAnim.Play (enabled);

		} else {
			audioToggleAnim.Play (disabled);
		}

		if (GetTagsOn() > 0) {
			tagsToggleAnim.Play (enabled);

		} else {
			tagsToggleAnim.Play (disabled);
		}


		if (GetHelpOn() > 0) {
			helpToggleAnim.Play (enabled);

		} else {
			helpToggleAnim.Play (disabled);
		}
	
	}


	void AudioToggle(){
		
		if (GetAudioOn() > 0) {
			audioToggleAnim.Play (disabled);
			DisableSound (true);
			SetAudioOn(0);

		} else {
			audioToggleAnim.Play (enabled);
			DisableSound (false);
			SetAudioOn(1);
		}


	}

	void TagsToggle(){
		if (GetTagsOn() > 0) {
			tagsToggleAnim.Play (disabled);
			SetTagsOn(0);

		} else {
			tagsToggleAnim.Play (enabled);
			SetTagsOn(1);
		}
		
	}

	void HelpToggle(){
		// We used this toggle to reset the state of the app (for testing purposes only)
		// TODO: This toggle needs to be changed later when the app is released

		PlayerPrefs.DeleteAll ();
		HandleObjectData.instance.ResetAll ();

		if (GetHelpOn() > 0) {
			helpToggleAnim.Play (disabled);
			SetHelpOn(0);

		} else {
			helpToggleAnim.Play (enabled);
			SetHelpOn(1);
		}
	}


	void SetHelpOn(int newHelpOn){
		
		helpOn = newHelpOn;
		PlayerPrefs.SetInt ("helpOn", newHelpOn);

	}

	int GetHelpOn(){

		return helpOn;
	}

	void SetTagsOn(int newTagsOn){

		tagsOn = newTagsOn;
		PlayerPrefs.SetInt ("tagsOn", newTagsOn);

	}

	int GetTagsOn(){

		return tagsOn;
	}

	void SetAudioOn(int newAudioOn){

		audioOn = newAudioOn;
		PlayerPrefs.SetInt ("audioOn", newAudioOn);

	}

	int GetAudioOn(){

		return audioOn;
	}

	void DisableSound(bool disable){
		
		if (disable) {
			PlayerPrefs.SetInt ("Mute", 1);
		} else {
			PlayerPrefs.SetInt ("Mute", 0);
		}

		PlayerPrefs.Save ();

		foreach (AudioSource audioSource in GameObject.FindObjectsOfType<AudioSource>()) {
			
			if (audioSource.tag != "Speech") {
				audioSource.mute = disable;
			}
		}
	}

}
