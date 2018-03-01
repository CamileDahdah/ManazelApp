using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
using UnityEngine.Events;

public class LanguagePanel : MonoBehaviour {

	public List<GameObject> languageButtons;
	int selectedButton;
	public Sprite selectedButtonSprite, unselectedButtonSprite;
	public GameObject nextButton;


	void OnEnable(){
		GameState.CurrentState = GameState.State.languagePanel;
	}

	void Start () {

		if(languageButtons != null && languageButtons.Count > 0){

			int buttonID = 0;

			SelectLanguage (languageButtons[0].transform.GetComponentInChildren<Text>().text.ToLower(), buttonID);

			nextButton.GetComponent<Button> ().onClick.AddListener (delegate {
				NextLanguage ();
			});

			foreach (GameObject languageButton in languageButtons) {
				
				int localID = buttonID;

				languageButton.GetComponent<Button> ().onClick.AddListener ( 
					
					delegate {
						SelectLanguage(languageButton.transform.GetComponentInChildren<Text>().text.ToLower(), localID);
					}
				);

				buttonID++;
			}
		}
	}
	
	void SelectLanguage(string language, int buttonID){

		SelectButton (buttonID);
		GameState.instance.SetLanguage (language);
	}

	void SelectButton(int buttonID){
		
		languageButtons [selectedButton].GetComponent<Image> ().sprite = unselectedButtonSprite;
		languageButtons [buttonID].GetComponent<Image> ().sprite = selectedButtonSprite;

		selectedButton = buttonID;
	}

	void NextLanguage(){
		UIManager.instance.MovePanelRight(GameState.State.settingsPanel);
	}

}
