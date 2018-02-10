using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RayCast : MonoBehaviour {

	RaycastHit[] raycastHit = new RaycastHit[1];
	LayerMask hotspotLayerMask, objectLayerMask; 
	GameObject hotspot;
	int hotspotID;

	public Image FadeImage;
	//touch
	Touch touch;
	int touchID;
	string objectID;
	float timer, timeLimit = .125f;
	bool clicking;

	void Awake(){
		hotspotLayerMask = LayerMask.NameToLayer("Hotspot");
		objectLayerMask = LayerMask.NameToLayer("Object");
		hotspot = null;
		timer = 0;
		clicking = false;
	}


	void Update () {
		#if UNITY_EDITOR

			if (Input.GetMouseButtonDown (0)) {
				hotspotID = -99;
				DetectObject(false); 

			}else if (Input.GetMouseButtonUp (0)){
				DetectObject(true);  
			}
						

		#elif UNITY_ANDROID || UNITY_IOS

			if (Input.touchCount == 1){
				
				touch = Input.GetTouch(0);

				switch (touch.phase){

					case TouchPhase.Began:
						hotspotID = -99;
						DetectObject(false);  
						touchID = touch.fingerId;

					break;

					case TouchPhase.Moved:
						if(touchID != touch.fingerId){
							touchID = -99;
						}
							
					break;

					case TouchPhase.Ended:
						if(touchID == touch.fingerId) {
							DetectObject(true); 
						}
					break;
				}

			}else{
				touchID = -99;
			}

		#endif

		if (clicking) {
			timer += Time.deltaTime;
		} else {
			timer = 0;
		}
	}

	void DetectObject(bool pressUp){

		if (pressUp) {
			clicking = false;
		} else {
			clicking = true;
		}

		if (timer < timeLimit) {
			
			if (Physics.RaycastNonAlloc (Camera.main.ScreenPointToRay (Input.mousePosition), raycastHit, 10f) > 0) {
				
				for (int i = 0; i < raycastHit.Length; i++) {
					
					if (raycastHit [i].transform.gameObject.layer == hotspotLayerMask.value) {
					
						hotspot = raycastHit [i].transform.gameObject;

						if (pressUp) {
						
							if (hotspot.GetInstanceID () == hotspotID) {
								ClickHotspot (hotspot);
							}

						} else {
							hotspotID = hotspot.GetInstanceID ();
						}
					// if hotspot found stop
						return;
					}
				}

			// else if hotspot not found 
			  if (raycastHit [0].transform.gameObject.layer == objectLayerMask.value) {
					
					if (pressUp) {
						
						if (objectID == raycastHit [0].transform.parent.GetComponent<SelectedObject> ().id) {
							ClickObject (objectID);

						}
					}
					else {
						objectID = raycastHit [0].transform.parent.GetComponent<SelectedObject> ().id;
					}

				}
			}
		}
	
	}

	void ClickHotspot(GameObject hotspot){
		hotspot.transform.parent.GetComponent<HotspotBehaviour>().ChangeView();
		FadeToWhite();
	}

	void FadeToWhite(){
		FadeImage.color = new Color (0, 0, 0, 1);
		FadeImage.canvasRenderer.SetAlpha (1);
		FadeImage.CrossFadeAlpha (0f, 1f, false);
	}

	void ClickObject(string objectID){
		
		AnimateSpriteManager.instance.AnimateSequence (objectID);

		string speechText = raycastHit [0].transform.parent.GetComponent<SelectedObject> ().arabicText;
		ArabicText.instance.PopupObject (speechText);

	}
}
