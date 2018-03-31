using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LevelPanel : MonoBehaviour {


	public GameObject level1, play, previous;
	public Color highlightedColor;
	public GameObject XButton;
	public GameObject scoreText;


	void OnEnable(){
		
		if (GameState.instance.GetFirstTime()) {

			XButton.SetActive (false);
			play.GetComponentInChildren<Text>().text = "Play";

		} else {
			
			XButton.SetActive (true);
			play.GetComponentInChildren<Text>().text = "Resume";
		}

		scoreText.GetComponent<Text> ().text = ScoreManager.instance.GetScore () + "/" + ScoreManager.instance.GetMaxLevelScore ();

	}


	void Start () {
		
		level1.GetComponent<Button> ().onClick.AddListener (delegate {
			level1.GetComponent<Image>().color = highlightedColor;
		});

		play.GetComponent<Button> ().onClick.AddListener (delegate {
			UIManager.instance.EnableCurrentPanel (GameState.State.HUDPanel);
			GameState.instance.SetFirstTime(0);
		});

		previous.GetComponent<Button> ().onClick.AddListener (delegate {
			UIManager.instance.MovePanelLeft(GameState.State.settingsPanel);
		});

		XButton.GetComponent<Button> ().onClick.AddListener (delegate {
			UIManager.instance.EnableCurrentPanel (GameState.State.HUDPanel);
		});
	
	}

}
