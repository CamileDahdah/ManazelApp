using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DragCamera : MonoBehaviour {
	//mouse
	public float dragSpeed = 8f;
	Vector2 previousPosition;
	float maxDragSpeed;
	Vector3 direction ;
	float desiredSpeed;

	//touch
	Touch touch;
	int touchID;

	//camera 
	public float perspectiveZoomSpeed = 0.5f;
	public float orthoZoomSpeed = 0.5f;
	float maxFieldOfView;
	Camera camera;


	void Start () {
		maxDragSpeed = dragSpeed;
		desiredSpeed = maxDragSpeed / 1.5f;
		camera = GetComponent<Camera> ();
		maxFieldOfView = camera.fieldOfView;
	}


	void Update () {



		#if UNITY_EDITOR
			if (Input.GetMouseButtonDown (0)) { //on press
				dragSpeed = maxDragSpeed;
				previousPosition = new Vector2 (Input.mousePosition.x, Input.mousePosition.y);

			} else if (Input.GetMouseButton (0)) { //while pressing 

				 direction = Camera.main.ScreenToViewportPoint(Input.mousePosition - new Vector3(previousPosition.x,previousPosition.y, 0));
				 transform.RotateAround(transform.position, transform.right, direction.y * dragSpeed);
				 transform.RotateAround(transform.position, Vector3.up, - direction.x * dragSpeed);
				dragSpeed = Mathf.Lerp(dragSpeed, 0, Time.deltaTime * (maxDragSpeed / desiredSpeed));
			} 
			else{
	
			dragSpeed = Mathf.Lerp(dragSpeed, 0, Time.deltaTime * (maxDragSpeed / desiredSpeed));
				transform.RotateAround(transform.position, transform.right, direction.y * dragSpeed);
				transform.RotateAround(transform.position, Vector3.up,  - direction.x * dragSpeed);
			
			}

		#elif UNITY_ANDROID || UNITY_IOS


			if (Input.touchCount >= 2){
				
				// Store both touches.
				Touch touchZero = Input.GetTouch(0);
				Touch touchOne = Input.GetTouch(1);

				previousPosition = new Vector2 (touchZero.position.x, touchZero.position.y);
				dragSpeed = 0;

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
			if(Input.touchCount == 1){
			touch = Input.GetTouch(0);

			switch (touch.phase){

				case TouchPhase.Began:
					touchID = touch.fingerId;
					previousPosition = new Vector2 (touch.position.x, touch.position.y);
					dragSpeed = maxDragSpeed;
					break;
				
				case TouchPhase.Moved :
				case TouchPhase.Stationary:
					if(touchID != touch.fingerId){
						previousPosition = new Vector2 (touch.position.x, touch.position.y);
						touchID = touch.fingerId;
					}

					direction = Camera.main.ScreenToViewportPoint(new Vector3 (touch.position.x, touch.position.y, 0f) - 
						new Vector3(previousPosition.x,previousPosition.y, 0));
					transform.RotateAround(transform.position, transform.right, direction.y * dragSpeed);
					transform.RotateAround(transform.position, Vector3.up, - direction.x * dragSpeed);
					dragSpeed = Mathf.Lerp(dragSpeed, 0, Time.deltaTime * (maxDragSpeed / desiredSpeed));

					break;

				case TouchPhase.Ended:
					dragSpeed = Mathf.Lerp(dragSpeed, 0, Time.deltaTime * (maxDragSpeed / desiredSpeed));
					break;

			}
		}
			else{

			dragSpeed = Mathf.Lerp(dragSpeed, 0, Time.deltaTime * (maxDragSpeed / desiredSpeed));
				transform.RotateAround(transform.position, transform.right, direction.y * dragSpeed);
				transform.RotateAround(transform.position, Vector3.up,  - direction.x * dragSpeed);

			}
		}
		#endif

	}
}
