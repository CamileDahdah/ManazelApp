using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using ArabicSupport;

public class ObjectPanel : MonoBehaviour {

	public Button nextObject, previousObject, xButton;

	public List<Sprite> spriteSheet = new List<Sprite>();
	public Image spriteSheetImage, background;
	float frameRate = 1/45f;
	string spriteResourceLocation = "Sequences";
	public static ObjectPanel instance;
	public GameObject speakGameObject, loadGameObject, youSaidGameobject;
	public GameObject textHolderGameobject;
	public RectTransform textHolderPosition1, initialTextHolderPosition, textHolderPosition2; 
	public float animationSpeed;
	public Sprite failPanel, failPlaceHolder, correctPanel, correctPlaceHolder, normalPanel, normalPlaceHolder;
	public Text wrongWord, actualText;
	public GameObject shadyBackground;
	public Color correctTextColor;

	void Awake(){
		
		if(instance == null){
			instance = this;
		
		}else{
			Destroy(this);
		}
	}

	void OnEnable(){
		ResetPanelState ();
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
		ResetPanelState ();
		ArabicText.instance.PopupNextObject ();

	}

	void PreviousObject(){
		ResetPanelState ();
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
		StopAllCoroutines ();

		Sprite[] spriteSheetArray = Resources.LoadAll <Sprite> (spriteResourceLocation + "/" + location);
		if (spriteSheetArray != null && spriteSheetArray.Length > 0) {

			spriteSheet.Clear ();
			spriteSheet.AddRange (spriteSheetArray);
			StartCoroutine ("Animate");
		} else {
			Debug.Log ("Resource " + spriteResourceLocation + "/" + location + "Not Found!");
		}
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
			yield return null;

		}
	}

	public void Loading(){
		nextObject.gameObject.SetActive (false);
		previousObject.gameObject.SetActive (false);
		xButton.gameObject.SetActive (false);
		speakGameObject.SetActive (false);
		loadGameObject.SetActive (true);
	}

	public void Speak(){
		speakGameObject.SetActive (true);
		loadGameObject.SetActive (false);
	}

	public void OnTextResult(bool correct){
		StartCoroutine ("AnimateTextHolder", correct);

	}

	IEnumerator AnimateTextHolder(bool correct){
		
		textHolderGameobject.SetActive (true);
		textHolderGameobject.GetComponent<RectTransform>().anchoredPosition = initialTextHolderPosition.anchoredPosition;
		loadGameObject.SetActive (false);

		while (Vector2.Distance (textHolderGameobject.GetComponent<RectTransform>().anchoredPosition, 
			textHolderPosition1.anchoredPosition) > 0f) {

			textHolderGameobject.GetComponent<RectTransform>().anchoredPosition = 
				Vector2.MoveTowards (textHolderGameobject.GetComponent<RectTransform>().anchoredPosition,
					textHolderPosition1.anchoredPosition, 
					Time.deltaTime * animationSpeed);
			
			yield return null;
		}

		yield return new WaitForSeconds (1.5f);

		shadyBackground.SetActive (true);

		if (correct) {
			background.sprite = correctPanel;
			textHolderGameobject.GetComponent<Image> ().sprite = correctPlaceHolder;
			youSaidGameobject.GetComponent<Text> ().color = Color.white;
			youSaidGameobject.GetComponent<Text> ().text = "correct";
			ScoreManager.instance.IncrementScore ();
			ReadJSON.instance.HandleCurrentCorrectObject ();
			actualText.text = actualText.text.Replace("02dfa5", "FFDD68FF");

		} else {
			if (ArabicText.instance.speechTextUI.text != "") {
				ArabicText.instance.speechTextUI.text = ArabicText.instance.theWord;
			} else {
				ArabicText.instance.speechTextUI.text = ArabicFixer.Fix (ArabicText.instance.theWord, true, true);
			}

			background.sprite = failPanel;
			textHolderGameobject.GetComponent<Image> ().sprite = failPlaceHolder;
			youSaidGameobject.GetComponent<Text> ().color = Color.white;
			youSaidGameobject.GetComponent<Text> ().text = "false";
		}

		while (Vector2.Distance (textHolderGameobject.GetComponent<RectTransform>().anchoredPosition, 
			textHolderPosition2.anchoredPosition) > 0f) {

			textHolderGameobject.GetComponent<RectTransform>().anchoredPosition = 
				Vector2.MoveTowards (textHolderGameobject.GetComponent<RectTransform>().anchoredPosition,
					textHolderPosition2.anchoredPosition, 
					Time.deltaTime * animationSpeed);

			yield return null;
		}

		yield return new WaitForSeconds(2f);
		UIManager.instance.EnableCurrentPanel (GameState.State.HUDPanel);
			
	}

	void ResetPanelState (){
		StopAllCoroutines ();
		background.sprite = normalPanel;
		textHolderGameobject.GetComponent<Image>().sprite = normalPlaceHolder;
		shadyBackground.SetActive (false);
		textHolderGameobject.SetActive (false);
		loadGameObject.SetActive (false);
		speakGameObject.SetActive (false);
		nextObject.gameObject.SetActive (true);
		previousObject.gameObject.SetActive (true);
		xButton.gameObject.SetActive (true);
		youSaidGameobject.GetComponent<Text> ().color = new Color(214/255f, 216/255f, 203/255f);
		youSaidGameobject.GetComponent<Text> ().text = "You said";
	}

}
