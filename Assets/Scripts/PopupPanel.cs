using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PopupPanel : MonoBehaviour {


	public Button greenOkButton;


	void Start () {
		
		greenOkButton.onClick.AddListener (delegate {
			ClickGreenButton();

		});

	}

	void ClickGreenButton(){
		UIManager.instance.EnableCurrentPanel (GameState.State.HUDPanel);

	}

}
