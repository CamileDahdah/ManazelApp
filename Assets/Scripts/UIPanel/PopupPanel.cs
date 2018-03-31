using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PopupPanel : MonoBehaviour {


	public Button greenOkButton, redOkButton;


	void Start () {
		
		greenOkButton.onClick.AddListener (delegate {
			ClickOkGreenButton();

		});

		redOkButton.onClick.AddListener (delegate {
			ClickOkErrorButton();

		});

	}

	void ClickOkGreenButton(){
		UIManager.instance.EnableCurrentPanel (GameState.State.HUDPanel);

	}

	void ClickOkErrorButton(){
		UIManager.instance.EnableCurrentPanel (GameState.State.HUDPanel);

	}

}
