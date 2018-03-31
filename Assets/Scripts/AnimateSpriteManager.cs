using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

//Animate object spritesheet located in the resources folder based on object's ID

public class AnimateSpriteManager : MonoBehaviour {
	
	public List<Sprite> spriteSheet = new List<Sprite>();
	public Image spriteSheetImage;
	float frameRate = 1/45f;
	string spriteResourceLocation = "Sequences";
	public Button xButton;

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

		UIManager.instance.EnableCurrentPanel(GameState.State.HUDPanel);

		StopAllCoroutines ();
		GoogleVoiceSpeech.instance.enabled = false;

		spriteSheet.Clear ();
		Resources.UnloadUnusedAssets ();

	}
}
