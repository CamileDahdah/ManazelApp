using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AnimateSpriteManager : MonoBehaviour {
	
	public List<Sprite> spriteSheet = new List<Sprite>();
	public Image spriteSheetImage;
	float frameRate = 1/45f;
	string spriteResourceLocation = "Sequences";
	public Button xButton;
	public GameObject spritePanel;

	public static AnimateSpriteManager instance;

	void Awake(){
		
		if (instance == null) {
			instance = this;
		} else {
			Destroy (this);
		}

		xButton.onClick.AddListener ( () =>  XButtonClick() );
	}
		

	public void AnimateSequence(string location){

		//Load All sprites
		Sprite[] spriteSheetArray = Resources.LoadAll <Sprite> (spriteResourceLocation + "/" + location); 
		spriteSheet.AddRange (spriteSheetArray);
		ScriptManager.instance.DisableInput ();
		spritePanel.SetActive (true);
		StartCoroutine ("Animate");
	}

	IEnumerator Animate(){
		int size = spriteSheet.Count;
		int current = 0;

		while (true) {

			while(current < size){
				spriteSheetImage.sprite = spriteSheet [current++];
				yield return new WaitForSeconds (frameRate);
			}

			while(current > 0){
				spriteSheetImage.sprite = spriteSheet [--current];
				yield return new WaitForSeconds (frameRate);
			}

		}
	}
		

	public void XButtonClick(){
		
		spritePanel.SetActive (false);
		ScriptManager.instance.EnableInput ();

		StopAllCoroutines ();
		GoogleVoiceSpeech.instance.enabled = false;
		GameState.currentState = GameState.State.mainGame;
		spriteSheet.Clear ();
		Resources.UnloadUnusedAssets ();

	}
}
