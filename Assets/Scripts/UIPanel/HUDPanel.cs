﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HUDPanel : MonoBehaviour {

	public Button homeButton;
	public GameObject scoreText;

	void OnEnable(){
		//update score text
		scoreText.GetComponent<Text> ().text = ScoreManager.instance.GetScore () + "/" + ScoreManager.instance.GetMaxLevelScore ();

	}


	void Start () {
		homeButton.onClick.AddListener (delegate {
			EnableMainPanel();
		});
	}
	
	void EnableMainPanel(){
		UIManager.instance.EnableCurrentPanel( (GameState.State) PlayerPrefs.GetInt("activeMainPanel"));

	}
}
