using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraMovement : MonoBehaviour {

	public float dragSpeed = 8f, moveSpeed = .1f;
	Vector2 previousPosition;
	float maxDragSpeed;
	Vector3 direction, direction2;
	float desiredSpeed;

	//touch properties
	Touch touch;
	int touchID;
	bool zoom;

	//camera properties
	public float perspectiveZoomSpeed = 0.5f;
	public float orthoZoomSpeed = 0.5f;
	float maxFieldOfView;
	Camera camera;


	void Start () {
		maxDragSpeed = dragSpeed;
		desiredSpeed = maxDragSpeed / 1.5f;
		camera = GetComponent<Camera> ();
		maxFieldOfView = camera.fieldOfView;
		Input.gyro.enabled = true;
		zoom = false;
	}
		

	void Update () {


		#if UNITY_EDITOR
			if (Input.GetMouseButtonDown (0)) { //on press
				BeginInput(Input.mousePosition);
				

			} else if (Input.GetMouseButton (0)) { //while pressing 
				MoveInput(Input.mousePosition);

			} else{
				EndInput();
			}

		#elif UNITY_ANDROID || UNITY_IOS


			if (Input.touchCount >= 2){
				
				dragSpeed = 0;

				// Store both touches.
				Touch touchZero = Input.GetTouch(0);
				Touch touchOne = Input.GetTouch(1);

				zoom = true;

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
							BeginInput(touch.position);
							break;
						
						case TouchPhase.Moved :
						case TouchPhase.Stationary:

							if(zoom){
								touchID = touch.fingerId;
								previousPosition = new Vector2 (touch.position.x, touch.position.y) *  ScaleDPI();
								zoom = false;
							}
							MoveInput(touch.position);						
							
							break;

						case TouchPhase.Ended:
								dragSpeed = Mathf.Lerp(dragSpeed, 0, Time.deltaTime * (maxDragSpeed / desiredSpeed));
							break;

						}
				}
				else{
					EndInput();
				}
		}
		#endif

		transform.parent.Rotate (0, - Input.gyro.rotationRate.y, 0);
		transform.Rotate (- Input.gyro.rotationRate.x, 0, 0, Space.Self);

		ClampRotation(-30f, 75f, 0);
	}


	void BeginInput(Vector3 input){
		dragSpeed = maxDragSpeed;
		previousPosition = new Vector2 (input.x, input.y) *  ScaleDPI();
	}


	void MoveInput(Vector3 input){ 
		direction2 = Camera.main.ScreenToViewportPoint (input * ScaleDPI() - new Vector3 (previousPosition.x, previousPosition.y, 0) );
		direction = new Vector2 (input.x, input.y) *  ScaleDPI() - previousPosition; 
		 
		transform.RotateAround(transform.position, transform.right, direction.y * moveSpeed);
		transform.parent.RotateAround(transform.parent.position, transform.parent.up, - direction.x * moveSpeed);

		previousPosition = new Vector2 (input.x, input.y) *  ScaleDPI(); 

	}

	void EndInput(){
		dragSpeed = Mathf.Lerp(dragSpeed, 0, Time.deltaTime * (maxDragSpeed / desiredSpeed));

		transform.RotateAround(transform.position, transform.right, direction2.y * dragSpeed);
		transform.parent.RotateAround(transform.parent.position, transform.parent.up, - direction2.x * dragSpeed);

	}

	float ScaleDPI(){
		return 1 / Screen.dpi * 221f;
	}

	void ClampRotation(float minAngle, float maxAngle, float clampAroundAngle = 0){
		
		clampAroundAngle += 180;
		float x = transform.rotation.eulerAngles.x - clampAroundAngle;

		x = WrapAngle(x);

		x -= 180;

		x = Mathf.Clamp(x, minAngle, maxAngle);

		x += 180;

		transform.localRotation = Quaternion.Euler(x + clampAroundAngle, 0, 0);
	}


	//Make sure angle is within 0,360 range
	float WrapAngle(float angle){
		
		while (angle < 0)
			angle += 360;

		return Mathf.Repeat(angle, 360);
	}

	void OnDisable(){
		dragSpeed = 0;

		if (camera.orthographic){
			camera.orthographicSize = Mathf.Max(camera.orthographicSize);
		}

		else{
			camera.fieldOfView = maxFieldOfView;
		}
	}

}
