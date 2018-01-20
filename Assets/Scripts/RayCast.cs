using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RayCast : MonoBehaviour {

	RaycastHit2D[] raycastHit = new RaycastHit2D[1];
	LayerMask layerMask; 
	GameObject hotspot;
	int hotspotID;

	public Image FadeImage;
	//touch
	Touch touch;
	int touchID;

	void Awake(){
		layerMask = LayerMask.NameToLayer("Hotspot");
		hotspot = null;
	}

	void Update () {
		#if UNITY_EDITOR

			if (Input.GetMouseButtonDown (0)) {
				hotspotID = -99;
				
				if( DetectHotspot(ref hotspot)) {
					hotspotID = hotspot.GetInstanceID();
				}

			}else if (Input.GetMouseButtonUp (0)){
			
				if( DetectHotspot(ref hotspot) && (hotspot.GetInstanceID() == hotspotID)) {
					ClickHotspot(hotspot);
				}
			}
						
			
		#elif UNITY_ANDROID || UNITY_IOS
			if (Input.touchCount == 1){
				
				touch = Input.GetTouch(0);

				switch (touch.phase){

					case TouchPhase.Began:
						hotspotID = -99;
						if( DetectHotspot(ref hotspot)) {
							touchID = touch.fingerId;
							hotspotID = hotspot.GetInstanceID();
						}
					break;

					case TouchPhase.Moved:
						if(touchID != touch.fingerId){
							touchID = -99;
						}
							
					break;

					case TouchPhase.Ended:
						if( DetectHotspot(ref hotspot) && touchID == touch.fingerId && hotspotID == hotspot.GetInstanceID()) {
							ClickHotspot(hotspot);
						}
					break;

				}
			}else{
				touchID = -99;
			}
		#endif

	}

	bool DetectHotspot(ref GameObject gameObject){
		
		if (Physics2D.GetRayIntersectionNonAlloc(Camera.main.ScreenPointToRay(Input.mousePosition), raycastHit, 10f) > 0){

			if (raycastHit [0].transform.gameObject.layer == layerMask.value) {
				gameObject = raycastHit [0].transform.gameObject;
				return true;
			}
		}

		gameObject = null;
		return false;
	}

	void ClickHotspot(GameObject hotspot){
		hotspot.GetComponent<HotspotBehaviour>().ChangeView();
		FadeToWhite();
	}

	void FadeToWhite(){
		FadeImage.enabled = true;
		FadeImage.color = new Color (0, 0, 0, 1);
		FadeImage.CrossFadeAlpha (0f, 1f, false);
	}
}
