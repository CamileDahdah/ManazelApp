using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PopupPanel : MonoBehaviour {


	public Button greenOkButton, redOkButton;


	void Start () {
		
		greenOkButton.onClick.AddListener (delegate {
			ClickGreenButton();

		});

		redOkButton.onClick.AddListener (delegate {
			ClickErrorButton();

		});

	}

	void ClickGreenButton(){
		UIManager.instance.EnableCurrentPanel (GameState.State.HUDPanel);

	}

	void ClickErrorButton(){
		UIManager.instance.EnableCurrentPanel (GameState.State.HUDPanel);

	}

}
