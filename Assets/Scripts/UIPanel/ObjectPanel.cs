using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ObjectPanel : MonoBehaviour {

	public Button nextObject, previousObject, xButton;

	public List<Sprite> spriteSheet = new List<Sprite>();
	public Image spriteSheetImage;
	float frameRate = 1/45f;
	string spriteResourceLocation = "Sequences";
	public static ObjectPanel instance;
	public GameObject speakGameObject, loadGameObject;

	void Awake(){
		
		if(instance == null){
			instance = this;
		
		}else{
			Destroy(this);

		}

	}

	void OnEnable(){
		//Speak ();
	}

	void Start () {
		
		nextObject.onClick.AddListener (delegate {
			NextObject();
		});

		previousObject.onClick.AddListener (delegate {
			PreviousObject();
		});

		xButton.onClick.AddListener (delegate {
			ExitButton();
		});


	}

	void NextObject(){
		ArabicText.instance.PopupNextObject ();

	}

	void PreviousObject(){
		ArabicText.instance.PopupPreviousObject ();

	}

	void ExitButton(){
		
		UIManager.instance.EnableCurrentPanel (GameState.State.HUDPanel);

		StopAllCoroutines ();
		GoogleVoiceSpeech.instance.enabled = false;

		spriteSheet.Clear ();
		Resources.UnloadUnusedAssets ();

	}


	public void AnimateSequence(string location){

		//Load All sprites
		Sprite[] spriteSheetArray = Resources.LoadAll <Sprite> (spriteResourceLocation + "/" + location);
		spriteSheet.Clear ();
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

	public void Loading(){
		speakGameObject.SetActive (false);
		loadGameObject.SetActive (true);
	}

	void Speak(){
		speakGameObject.SetActive (true);
		loadGameObject.SetActive (false);
	}

}
