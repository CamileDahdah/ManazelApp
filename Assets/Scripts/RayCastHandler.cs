using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

//Handle onClickListener on objects, hotpots, etc. using rayCastHit

public class RayCastHandler : MonoBehaviour {

	RaycastHit[] raycastHit = new RaycastHit[1];
	LayerMask hotspotLayerMask, objectLayerMask; 
	GameObject hotspot;
	int hotspotID;

	//touch properties
	Touch touch;
	int touchID;

	string objectID;
	float clickTimer, timeLimit = .15f;
	bool clicking;

	void Awake(){
		
		hotspotLayerMask = LayerMask.NameToLayer("Hotspot");
		objectLayerMask = LayerMask.NameToLayer("Object");
		hotspot = null;
		clickTimer = 0;
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
			clickTimer += Time.deltaTime;
		} else {
			clickTimer = 0;
		}
	}

	void DetectObject(bool pressUp){

		if (pressUp) {
			clicking = false;
		} else {
			clicking = true;
		}

		if (clickTimer < timeLimit) {

			// if raycast Hit
			if ( (raycastHit = Physics.RaycastAll (Camera.main.ScreenPointToRay (Input.mousePosition), 30f)).Length > 0) {
		
				for (int i = 0; i < raycastHit.Length; i++) {
					
					//if hotspot detected (hotspot has highest priority)
					if (raycastHit [i].transform.gameObject.layer == hotspotLayerMask.value) {
					
						hotspot = raycastHit [i].transform.gameObject;

						if (pressUp && hotspot.GetInstanceID () == hotspotID) {

							ClickHotspot (hotspot);

						} 
						//else if press down
						else {
							hotspotID = hotspot.GetInstanceID ();
						}

						// if hotspot found exit method
						return;
					}

				}

			  // else if hotspot not found 
			  if (raycastHit [0].transform.gameObject.layer == objectLayerMask.value) {

					//if object found
					if (pressUp && objectID == raycastHit [0].transform.parent.GetComponent<SelectedObject> ().id) {
						ClickObject (objectID);
					}
					// else if press down
					else {
						objectID = raycastHit [0].transform.parent.GetComponent<SelectedObject> ().id;
					}

				}
			}
		}
	
	}

	void ClickHotspot(GameObject hotspot){
		hotspot.transform.parent.GetComponent<HotspotBehaviour>().ChangeView();
	}
		
	void ClickObject(string objectID){

		ArabicTextHandler.instance.PopupObject (objectID);

	}
		
}
