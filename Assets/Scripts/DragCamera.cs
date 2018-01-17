using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DragCamera : MonoBehaviour {
	//mouse
	public float dragSpeed = 2f;
	Vector2 previousPosition;
	Vector2 direction;
	Transform cameraTransform;

	//touch
	Touch touch;
	int touchID;

	//camera 
	public float perspectiveZoomSpeed = 0.5f;
	public float orthoZoomSpeed = 0.5f;
	float maxFieldOfView;
	Camera camera;

	void Start () {
		cameraTransform = GetComponent<Transform> ();
		camera = GetComponent<Camera> ();
		maxFieldOfView = camera.fieldOfView;
	}
	

	void Update () {

		#if UNITY_EDITOR
			if (Input.GetMouseButtonDown (0)) { //on press
				previousPosition = new Vector2 (Input.mousePosition.x, Input.mousePosition.y);

			} else if (Input.GetMouseButton (0)) { //while pressing 
				direction = new Vector2 (Input.mousePosition.x, Input.mousePosition.y) - previousPosition;

//			transform.rotation = Quaternion.RotateTowards (camera.transform.rotation, Quaternion.(direction.y, - direction.x, 0) );// ,Time.deltaTime * dragSpeed);
				transform.Rotate ( new Vector3 (0, - direction.x * Time.deltaTime, 0) * dragSpeed, Space.World) ;
				transform.Rotate ( new Vector3 (direction.y * Time.deltaTime, 0, 0) * dragSpeed);

				previousPosition = new Vector2 (Input.mousePosition.x, Input.mousePosition.y);

			} else if (Input.GetMouseButtonUp (0)) { //on press up
				
			}

		#elif UNITY_ANDROID || Unity_IOS


			if (Input.touchCount >= 2){
				
				// Store both touches.
				Touch touchZero = Input.GetTouch(0);
				Touch touchOne = Input.GetTouch(1);

				previousPosition = new Vector2 (touchZero.position.x, touchZero.position.y);

				// Find the position in the previous frame of each touch.
				Vector2 touchZeroPrevPos = touchZero.position - touchZero.deltaPosition;
				Vector2 touchOnePrevPos = touchOne.position - touchOne.deltaPosition;

				// Find the magnitude of the vector (the distance) between the touches in each frame.
				float prevTouchDeltaMag = (touchZeroPrevPos - touchOnePrevPos).magnitude;
				float touchDeltaMag = (touchZero.position - touchOne.position).magnitude;

				// Find the difference in the distances between each frame.
				float deltaMagnitudeDiff = prevTouchDeltaMag - touchDeltaMag;

				// If the camera is orthographic...
				if (camera.orthographic){
					
					// ... change the orthographic size based on the change in distance between the touches.
					camera.orthographicSize += deltaMagnitudeDiff * orthoZoomSpeed;

					// Make sure the orthographic size never drops below zero.
					camera.orthographicSize = Mathf.Max(camera.orthographicSize, 0.1f);
				}
				else{
					
					// Otherwise change the field of view based on the change in distance between the touches.
					camera.fieldOfView += deltaMagnitudeDiff * perspectiveZoomSpeed;

					// Clamp the field of view to make sure it's between 0 and 180.
					camera.fieldOfView = Mathf.Clamp(camera.fieldOfView, 30f, maxFieldOfView);
				}
			}else{
			
				if(!camera.orthographic){
					if(camera.fieldOfView < maxFieldOfView){
						camera.fieldOfView = Mathf.MoveTowards(camera.fieldOfView, maxFieldOfView, Time.deltaTime * 150);
					}
				}

				touch = Input.GetTouch(0);

				switch (touch.phase){

					case TouchPhase.Began:
						touchID = touch.fingerId;
						previousPosition = new Vector2 (touch.position.x, touch.position.y);
						break;
					
					case TouchPhase.Moved:
						if(touchID != touch.fingerId){
							previousPosition = new Vector2 (touch.position.x, touch.position.y);
							touchID = touch.fingerId;
						}

						direction = new Vector2 (touch.position.x, touch.position.y) - previousPosition;

						transform.Rotate ( new Vector3 (0, - direction.x * Time.deltaTime, 0) * dragSpeed, Space.World) ;
						transform.Rotate ( new Vector3 (direction.y * Time.deltaTime, 0, 0) * dragSpeed);

						previousPosition = new Vector2 (touch.position.x, touch.position.y);
						break;

					case TouchPhase.Ended:

						break;

				}
			}
		#endif

	}
}
