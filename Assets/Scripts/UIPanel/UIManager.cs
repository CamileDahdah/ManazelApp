using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class UIManager : MonoBehaviour {

	public static UIManager instance;
	public List<GameObject> panels;
	private Dictionary<string, GameObject> panelsDictionary = new Dictionary<string, GameObject>();
	public string currentPanelString = "";
	public RectTransform referenceRight, referenceLeft, referenceDown;
	GameObject currentPanel = null;
	public float panelAnimationSpeed;
	public bool moveUIDone, blurEffect;
	Image blurImage;


	void Awake () {
		
		if (instance == null) {
			instance = this;
		} else {
			Destroy (this);
		}

		foreach (GameObject panel in panels) {
			panelsDictionary.Add (panel.name.ToLower(), panel);
		}

		blurImage = panelsDictionary ["blurpanel"].GetComponent<Image> ();
	
	}

	void Start(){

		moveUIDone = false;

		currentPanelString = GameState.CurrentState.ToString().ToLower();

	}

	private void ExitCurrentPanel(){

		if (panelsDictionary.ContainsKey (currentPanelString)) {
			panelsDictionary [currentPanelString].SetActive (false);
			ScriptManager.instance.EnableInput ();

		} else {
			Debug.Log ("Dictionary key not found");
		}

	}

	public void EnableCurrentPanel(GameState.State panelState){
		
		ExitCurrentPanel ();
		GameState.CurrentState = panelState;

		if(panelState == GameState.State.HUDPanel){
			ScriptManager.instance.EnableInput ();
		}else{
			ScriptManager.instance.DisableInput ();
		}

		panelsDictionary[panelState.ToString().ToLower()].SetActive (true);
		currentPanelString = panelState.ToString().ToLower();
		currentPanel = panelsDictionary [panelState.ToString ().ToLower ()];

	}

	public void MovePanelRight(GameState.State panelState){
		MovePanel (panelState, referenceRight);
	}

	public void MovePanelLeft(GameState.State panelState){
		MovePanel (panelState, referenceLeft);
	}

	public void MovePanelUp(GameState.State panelState){
		MovePanel (panelState, referenceDown);
	}

	private void MovePanel(GameState.State panelState, RectTransform direction){
		moveUIDone = false;
		EnableCurrentPanel (panelState);
		StartCoroutine ("AnimatePanel", direction);
	}

	IEnumerator AnimatePanel(RectTransform direction){

		currentPanel.transform.position = direction.position;
		EventSystem eventSystem = EventSystem.current;
		eventSystem.enabled = false;

		while (Vector2.Distance(currentPanel.GetComponent<RectTransform>().anchoredPosition,
			Vector2.zero) > 0f ){
			
			currentPanel.GetComponent<RectTransform> ().anchoredPosition =
				Vector2.MoveTowards (currentPanel.GetComponent<RectTransform> ().anchoredPosition, 
					Vector2.zero, Time.deltaTime * panelAnimationSpeed);
	
			yield return null;

		}
			
		eventSystem.enabled = true;
		moveUIDone = true;
	}

	public void EnableBlur(bool enable){
		
		blurEffect = false;

		StopCoroutine ("Blur");
		StartCoroutine ("Blur", enable);

	}

	IEnumerator Blur(bool enable){
		
		float alpha;

		if (enable) {
			alpha = 206 / 255f;
		} else {
			alpha = 0f;
		}

		while (blurImage.color.a != alpha) {
			blurImage.color = new Color( blurImage.color.r, 
				blurImage.color.g, blurImage.color.b, Mathf.MoveTowards (blurImage.color.a, alpha, Time.deltaTime * 1.5f) );
			yield return null;
		}

		blurEffect = true;

	}
}
