using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PanoControls : MonoBehaviour 
{
	#region Public Variables
	public GameObject panoCameraPivot ;
	public Camera panoCamera ;

	public bool useKeyboardControls ;
	public bool useMouseControls ;
	public bool useTouchControls ;

	public float minZoomFov ;
	public float maxZoomFov ;

	public float keyboardZoomSpeed = 20f;
	public float touchZoomSpeed = 10f ;
	public float mouseZoomSpeed = 10f ;

	public bool limitXRotation ;
	public bool limitYRotation ;

	public float maxUpRotation ;
	public float maxDownRotation ;
	public float maxLeftRotation ;
	public float maxRightRotation ;

	public float keyboardPanSpeed = 20f;
	public float touchPanSpeed = 10f ;
	public float mousePanSpeed = 50f ;

	public bool invertMouseZoom ;

	public bool invertMouseVertical ;
	public bool invertMouseHorizontal ;
	public bool invertKeyboardVertical ;
	public bool invertKeyboardHorizontal ;
	public bool invertTouchVertical ;
	public bool invertTouchHorizontal ;
	
	public bool hideHotspotsAtStart ;
	
	[HideInInspector]
	public List<Rect> guiButtonRects = new List<Rect>();
	
	[HideInInspector]
	public bool useUnityGuiBasedControls ;
	
	[HideInInspector]
	public bool overGuiButton ;
	#endregion
	
	#region Private Variables
	private Quaternion rotation ;
	private float y ;
	private float x ;
	private bool allowClickDrag ;
	private Touch finger1 ;
	private Touch finger2 ;
	private float fingersDistance ;
	private float previousFingersDistance ;
	private Vector2 finger1Position ;
	private Vector2 finger2Position ;
	
	private float startingCameraAngle ;
	
	private float initialMaxLeftRotation ;
	private float initialMaxRightRotation ;
	private float initialMaxUpRotation ;
	private float initialMaxDownRotation ;
	private float panZoomOffset ;
	
	private bool canTouchHotspot ;
	#endregion
	
	#region Constants
	private const float touchReactLow = -2f ;
	private const float touchReactHigh = 2f ;
	#endregion
	
	#region Enums
	public enum Originator
	{
		Keyboard,
		Mouse,
		TouchDevice,
		GuibuttonLeft,
		GuibuttonRight,
		GuibuttonUp,
		GuibuttonDown,
		GuibuttonZoomIn,
		GuibuttonZoomOut
	}
	#endregion
	
	// Initialize various variables and set rotational limits at start
	void Start()
	{
		DoInitialChecks() ;
		
		startingCameraAngle = panoCameraPivot.transform.eulerAngles.y ;
		panZoomOffset = 0 ;
		initialMaxLeftRotation = maxLeftRotation ;
		initialMaxRightRotation = maxRightRotation ;
		initialMaxUpRotation = maxUpRotation ;
		initialMaxDownRotation = maxDownRotation ;
		canTouchHotspot = true ;
		
		if((panoCamera.fieldOfView != maxZoomFov) && limitXRotation)
		{
			AdjustRotationalLimitZoomIn() ;
		}
		
		// hides all the hotspots at start if the option was selected
		if(hideHotspotsAtStart)
		{
			HotSpotMethods.HideHotspots("all") ;	
		}
	}
	
	// Responsible for checking that all settings are correct, and if not correct, to display an error message and counteract the error
	void DoInitialChecks()
	{
		if(useMouseControls && useTouchControls)
		{
			useMouseControls = false ;
			Debug.LogWarning("Mouse controls overrided by touch controls, if you want to use the mouse controls, turn off the touch controls first") ;
		}
		
		if(keyboardPanSpeed < 0)
		{
			Debug.LogWarning("Keyboard panspeed was less than , resetting keyboard panspeed to 20") ;
			keyboardPanSpeed = 20 ;	
		}
		
		if(keyboardZoomSpeed < 0)
		{
			Debug.LogWarning("Keyboard zoom speed was less than 0, resetting keyboard zoom speed to 0, for no zoom") ;
			keyboardZoomSpeed = 0 ;
		}
		
		if(touchZoomSpeed < 0)
		{
			Debug.LogWarning("Touch zoom speed value was less than 0, resetting touch zoom speed value to 0, for no zoom") ;
			touchZoomSpeed = 0 ;	
		}
		
		if(mouseZoomSpeed < 0)
		{
			Debug.LogWarning("Mouse zoom speed value was less than 0, resetting mouse zoom speed value to 0, for no zoom") ;
			mouseZoomSpeed = 0 ;	
		}
		
		if(minZoomFov < 1)
		{
			Debug.LogWarning("Minimum zoom level was less than 1, resetting minimum zoom level to 1") ;
			minZoomFov = 1 ;
		}
		
		if(maxZoomFov > 100)
		{
			Debug.LogWarning("Maximum zoom level was more than 100, resetting maximum zoom level to 100") ;
			maxZoomFov = 100 ;				
		}
		
		if(minZoomFov > maxZoomFov)
		{
			Debug.LogWarning("Minimum zoom level was greater than maximum zoom level, resetting zoom levels to minimum zoom level") ;
			maxZoomFov = minZoomFov;
		}
		
		if(minZoomFov == maxZoomFov && panoCamera.fieldOfView != maxZoomFov)
		{
			Debug.LogWarning("Starting zoom level was out of range of the minimum and maximum zoom levels, resetting starting zoom level to maximum zoom level") ;
			panoCamera.fieldOfView = maxZoomFov ;
			AdjustRotationalLimitZoomIn() ;
		}
		
		if(panoCamera.fieldOfView < minZoomFov || panoCamera.fieldOfView > maxZoomFov)
		{
			Debug.LogWarning("Starting zoom level was less than the minimum zoom level or greater than the maximum zoom level, resetting the starting zoom level to maximum zoom value") ;	
			panoCamera.fieldOfView = maxZoomFov ;
		}
		
		if(limitXRotation)
		{
			if(maxLeftRotation > 0)
			{
				Debug.LogWarning("Maximum left limit was greater than 0, resetting maximum left limit to 0") ;
				maxLeftRotation = 0 ;	
			}
			
			if(maxRightRotation < 0)
			{
				Debug.LogWarning("Maximum right limit was less than 0, resetting maximum right limit to 0") ;
				maxRightRotation = 0 ;
			}
		}
		
		if(limitYRotation)
		{
			if(maxUpRotation < 0)
			{
				Debug.LogWarning("Maximum up limit was less than 0, resetting maximum up limit to 0") ;
				maxUpRotation = 0 ;	
			}
			
			if(maxDownRotation > 0)
			{
				Debug.LogWarning("Maximum down limit was greater than 0, resetting maximum down limit to 0") ;
				maxDownRotation = 0 ;
			}
		}
	}
	
	void Update () 
	{	
		#region Keyboard Controls
		if(useKeyboardControls)
		{
			// Keyboard panning
			if(Input.GetKey(KeyCode.LeftArrow) || Input.GetKey(KeyCode.RightArrow) || Input.GetKey(KeyCode.UpArrow) || Input.GetKey(KeyCode.DownArrow))
			{
				Pan(Originator.Keyboard) ;
			}
			
			// Keyboard zooming in
			if(Input.GetKey(KeyCode.KeypadPlus))
			{
				ZoomIn(Originator.Keyboard) ;
			}
			
			// Keyboard zooming out
			if(Input.GetKey(KeyCode.KeypadMinus))
			{
				ZoomOut(Originator.Keyboard) ;
			}
		}
		#endregion
		
		#region Additional Mouse controls
		// Check to see if the mouse has clicked on a hotspot when the mouse button is released
		if(Input.GetMouseButtonUp(0) && useMouseControls)
		{
			if(useUnityGuiBasedControls)
			{
				CheckIfOverGuiElement() ;
			}
			
			HasClickedHotspot() ;
			
			if(useUnityGuiBasedControls)
			{
				overGuiButton = false ;
			}
		}
		#endregion
		
		#region Touch Controls
		// Touch control panning, and checks to see if you have touched a hotspot
		if(useTouchControls)
		{
			if(Input.touchCount == 1)
			{
				finger1 = Input.GetTouch(0) ;
				
				if(useUnityGuiBasedControls)
				{
					CheckIfOverGuiElement() ;
				}
			
				HasTouchedHotspot() ;
			
				if(useUnityGuiBasedControls)
				{
					overGuiButton = false ;
				}
				
				if (finger1.phase == TouchPhase.Began) 
				{
					canTouchHotspot = true ;
				}
				
				if (finger1.phase == TouchPhase.Moved) 
				{
					canTouchHotspot = false ;
					
					Pan (Originator.TouchDevice);
				}
			}
			
			// Touch control zooming
			if(Input.touchCount >= 2)
			{
				finger1 = Input.GetTouch(0) ;
				finger2 = Input.GetTouch(1) ;
					
				if (finger1.phase == TouchPhase.Moved || finger2.phase == TouchPhase.Moved)
				{
					if((finger1.deltaPosition.y < touchReactLow || finger1.deltaPosition.y > touchReactHigh) || (finger2.deltaPosition.y < touchReactLow || finger2.deltaPosition.y > touchReactHigh))
					{
						finger1Position = finger1.position ;
						finger2Position = finger2.position ;
							
						previousFingersDistance = fingersDistance ;
						fingersDistance = Vector2.Distance(finger1Position,finger2Position) ;
							
						if((fingersDistance - previousFingersDistance) > 0)
						{
							ZoomIn(Originator.TouchDevice) ;
						}
						else if((fingersDistance - previousFingersDistance) < 0)
						{
							ZoomOut(Originator.TouchDevice) ;
						}
					}
				}
			}
		}
		#endregion
	}
	
	void OnGUI()
	{
		#region Mouse Controls
		if(useMouseControls)
		{
			// Registers the start of a mouse click
			if(Event.current.type == EventType.mouseDown && Event.current.button == 1)
			{
				allowClickDrag = true ;
				Event.current.Use() ;
			}
			
			// Mouse panning
			if(Event.current.type == EventType.mouseDrag && allowClickDrag)
			{
				Pan(Originator.Mouse) ;
			}	
			
			// Registers the end of a mouse drag or click
			if(Event.current.type == EventType.mouseUp && Event.current.button == 1)
			{
				allowClickDrag = false ;
				Event.current.Use() ;
			}
			
			// Mouse zooming
			if(Event.current.type == EventType.scrollWheel)
			{
				if(Input.GetAxis("Mouse ScrollWheel") < 0)
				{
					if(invertMouseZoom == false)
					{
						ZoomIn(Originator.Mouse) ;
					}
					else
					{
						ZoomOut(Originator.Mouse) ;
					}
				}
				
				else if(Input.GetAxis("Mouse ScrollWheel") > 0)
				{
					if(invertMouseZoom == false)
					{
						ZoomOut(Originator.Mouse) ;
					}
					else
					{
						ZoomIn(Originator.Mouse) ;
					}
				}
				
				Event.current.Use() ;
			}
		}
		#endregion
	}
	
	#region Panning
	// Responsible for panning for all control schemes
	public void Pan(Originator originator)
	{
		// panning if camera has horizontal rotational limit but not a vertical rotational limit
		if(limitXRotation == true && limitYRotation == false)
		{
			// mouse panning
			if(originator == Originator.Mouse)
			{
				if(!invertMouseHorizontal)
				{
					x = Mathf.Clamp(x + (Input.GetAxis("Mouse X") * Time.deltaTime * mousePanSpeed), maxLeftRotation, maxRightRotation) ;
				}
				else
				{
					x = Mathf.Clamp(x + (-Input.GetAxis("Mouse X") * Time.deltaTime * mousePanSpeed), maxLeftRotation, maxRightRotation) ;
				}

				if(!invertMouseVertical)
				{
					panoCamera.transform.Rotate((-Input.GetAxis("Mouse Y")  * mousePanSpeed * Time.deltaTime), 0, 0);
				}
				else
				{
					panoCamera.transform.Rotate((Input.GetAxis("Mouse Y")  * mousePanSpeed * Time.deltaTime), 0, 0);
				}

				panoCameraPivot.transform.rotation = Quaternion.Euler( 0, x + startingCameraAngle, 0 );
			}
			
			// touch panning
			else if (originator == Originator.TouchDevice)
			{
				if(!invertTouchHorizontal)
				{
					x = Mathf.Clamp(x + (finger1.deltaPosition.x * Time.deltaTime * touchPanSpeed), maxLeftRotation, maxRightRotation) ;
				}
				else
				{
					x = Mathf.Clamp(x + (-finger1.deltaPosition.x * Time.deltaTime * touchPanSpeed), maxLeftRotation, maxRightRotation) ;
				}

				if(!invertTouchVertical)
				{
					panoCamera.transform.Rotate((-finger1.deltaPosition.y  * touchPanSpeed * Time.deltaTime), 0, 0);
				}
				else
				{
					panoCamera.transform.Rotate((finger1.deltaPosition.y  * touchPanSpeed * Time.deltaTime), 0, 0);
				}

				panoCameraPivot.transform.rotation = Quaternion.Euler( 0, x + startingCameraAngle, 0 );
			}
			
			// keyboard panning
			else if (originator == Originator.Keyboard)
			{
				if(!invertKeyboardHorizontal)
				{
					x = Mathf.Clamp(x + (Input.GetAxis("Horizontal") * Time.deltaTime * keyboardPanSpeed), maxLeftRotation, maxRightRotation) ;
				}
				else
				{
					x = Mathf.Clamp(x + ((-1 * Input.GetAxis("Horizontal")) * Time.deltaTime * keyboardPanSpeed), maxLeftRotation, maxRightRotation) ;
				}

				if(!invertKeyboardVertical)
				{
					panoCamera.transform.Rotate(-Input.GetAxis("Vertical") * keyboardPanSpeed * Time.deltaTime, 0, 0) ;
				}
				else
				{
					panoCamera.transform.Rotate(Input.GetAxis("Vertical") * keyboardPanSpeed * Time.deltaTime, 0, 0) ;
				}

				panoCameraPivot.transform.rotation = Quaternion.Euler( 0, x + startingCameraAngle , 0 );
			}

			// Guibutton panning
			else if (originator == Originator.GuibuttonLeft)
			{
				x = Mathf.Clamp(x + (-1 * Time.deltaTime * keyboardPanSpeed), maxLeftRotation, maxRightRotation) ;
				panoCameraPivot.transform.rotation = Quaternion.Euler( 0, x + startingCameraAngle , 0 );
			}
			
			else if (originator == Originator.GuibuttonRight)
			{
				x = Mathf.Clamp(x + (1 * Time.deltaTime * keyboardPanSpeed), maxLeftRotation, maxRightRotation) ;
				panoCameraPivot.transform.rotation = Quaternion.Euler( 0, x + startingCameraAngle , 0 );
			}
			
			else if (originator == Originator.GuibuttonUp)
			{
				panoCamera.transform.Rotate(1 * keyboardPanSpeed * Time.deltaTime, 0, 0) ;
			}
			
			else if (originator == Originator.GuibuttonDown)
			{
				panoCamera.transform.Rotate(-1 * keyboardPanSpeed * Time.deltaTime, 0, 0) ;
			}
		}
		
		// panning if camera has vertical rotational limit but not a horizontal rotational limit
		else if(limitYRotation == true && limitXRotation == false)
		{
			// mouse panning
			if(originator == Originator.Mouse)
			{
				if(!invertMouseVertical)
				{
					y = Mathf.Clamp(y + (Input.GetAxis("Mouse Y") * Time.deltaTime * mousePanSpeed), maxDownRotation, maxUpRotation) ;
				}
				else
				{
					y = Mathf.Clamp(y + (-Input.GetAxis("Mouse Y") * Time.deltaTime * mousePanSpeed), maxDownRotation, maxUpRotation) ;
				}
				
				panoCamera.transform.localRotation = Quaternion.Euler(-y, 0, 0 );

				if(!invertMouseHorizontal)
				{
					panoCameraPivot.transform.Rotate(0, (Input.GetAxis("Mouse X")  * mousePanSpeed * Time.deltaTime), 0);
				}
				else
				{
					panoCameraPivot.transform.Rotate(0, (-Input.GetAxis("Mouse X")  * mousePanSpeed * Time.deltaTime), 0);
				}
			}
			
			// touch panning
			else if (originator == Originator.TouchDevice)
			{
				if(!invertTouchVertical)
				{
					y = Mathf.Clamp(y + (finger1.deltaPosition.y * Time.deltaTime * touchPanSpeed), maxDownRotation, maxUpRotation) ;
				}
				else
				{
					y = Mathf.Clamp(y + (-finger1.deltaPosition.y * Time.deltaTime * touchPanSpeed), maxDownRotation, maxUpRotation) ;
				}
				
				panoCamera.transform.localRotation = Quaternion.Euler(-y, 0, 0 );

				if(!invertTouchHorizontal)
				{
					panoCameraPivot.transform.Rotate(0, (finger1.deltaPosition.x * touchPanSpeed * Time.deltaTime),0) ;
				}
				else
				{
					panoCameraPivot.transform.Rotate(0, (-finger1.deltaPosition.x * touchPanSpeed * Time.deltaTime),0) ;
				}
			}
			
			// keyboard panning
			else if (originator == Originator.Keyboard)
			{
				if(!invertKeyboardVertical)
				{
					y = Mathf.Clamp(y + (Input.GetAxis("Vertical") * Time.deltaTime * keyboardPanSpeed), maxDownRotation, maxUpRotation) ;
				}
				else
				{
					y = Mathf.Clamp(y + (-Input.GetAxis("Vertical") * Time.deltaTime * keyboardPanSpeed), maxDownRotation, maxUpRotation) ;
				}
				
				panoCamera.transform.localRotation = Quaternion.Euler(-y, 0, 0 );

				if(!invertKeyboardHorizontal)
				{
					panoCameraPivot.transform.Rotate(0, Input.GetAxis("Horizontal") * keyboardPanSpeed * Time.deltaTime, 0) ;
				}
				else
				{
					panoCameraPivot.transform.Rotate(0, -Input.GetAxis("Horizontal") * keyboardPanSpeed * Time.deltaTime, 0) ;
				}
			}
			
			// Guibutton panning
			else if (originator == Originator.GuibuttonUp)
			{
				y = Mathf.Clamp(y + (1 * Time.deltaTime * keyboardPanSpeed), maxDownRotation, maxUpRotation) ;
				
				panoCamera.transform.localRotation = Quaternion.Euler(-y, 0, 0 );
			}
			else if (originator == Originator.GuibuttonDown)
			{
				y = Mathf.Clamp(y + (-1 * Time.deltaTime * keyboardPanSpeed), maxDownRotation, maxUpRotation) ;
				
				panoCamera.transform.localRotation = Quaternion.Euler(-y, 0, 0 );
			}
			else if (originator == Originator.GuibuttonLeft)
			{
				panoCameraPivot.transform.Rotate(0, -1 * keyboardPanSpeed * Time.deltaTime, 0) ;
			}
			else if (originator == Originator.GuibuttonRight)
			{
				panoCameraPivot.transform.Rotate(0, 1 * keyboardPanSpeed * Time.deltaTime, 0) ;
			}
		}
		
		// panning if camera has both horizontal rotational limit and a vertical rotational limit
		else if(limitYRotation == true && limitXRotation == true)
		{
			// mouse panning
			if(originator == Originator.Mouse)
			{
				if(!invertMouseHorizontal)
				{
					x = Mathf.Clamp(x + (Input.GetAxis("Mouse X") * Time.deltaTime * mousePanSpeed), maxLeftRotation, maxRightRotation) ;
				}
				else
				{
					x = Mathf.Clamp(x + (-Input.GetAxis("Mouse X") * Time.deltaTime * mousePanSpeed), maxLeftRotation, maxRightRotation) ;
				}

				if(!invertMouseVertical)
				{
					y = Mathf.Clamp(y + (Input.GetAxis("Mouse Y") * Time.deltaTime * mousePanSpeed), maxDownRotation, maxUpRotation) ;
				}
				else
				{
					y = Mathf.Clamp(y + (-Input.GetAxis("Mouse Y") * Time.deltaTime * mousePanSpeed), maxDownRotation, maxUpRotation) ;
				}
				
				panoCamera.transform.localRotation = Quaternion.Euler(-y, 0, 0 );
				panoCameraPivot.transform.rotation = Quaternion.Euler( 0, x  + startingCameraAngle, 0 );
			}
			
			// touch panning
			else if (originator == Originator.TouchDevice)
			{
				if(!invertTouchHorizontal)
				{
					x = Mathf.Clamp(x + (finger1.deltaPosition.x * Time.deltaTime * touchPanSpeed), maxLeftRotation, maxRightRotation) ;
				}
				else
				{
					x = Mathf.Clamp(x + (-finger1.deltaPosition.x * Time.deltaTime * touchPanSpeed), maxLeftRotation, maxRightRotation) ;
				}

				if(!invertTouchVertical)
				{
					y = Mathf.Clamp(y + (finger1.deltaPosition.y * Time.deltaTime * touchPanSpeed), maxDownRotation, maxUpRotation) ;
				}
				else
				{
					y = Mathf.Clamp(y + (-finger1.deltaPosition.y * Time.deltaTime * touchPanSpeed), maxDownRotation, maxUpRotation) ;
				}
				
				panoCamera.transform.localRotation = Quaternion.Euler(-y, 0, 0 );
				panoCameraPivot.transform.rotation = Quaternion.Euler( 0, x  + startingCameraAngle, 0 );
			}
			
			// keyboard panning
			else if (originator == Originator.Keyboard)
			{
				if(!invertKeyboardHorizontal)
				{
					x = Mathf.Clamp(x + (Input.GetAxis("Horizontal") * Time.deltaTime * keyboardPanSpeed), maxLeftRotation, maxRightRotation) ;
				}
				else
				{
					x = Mathf.Clamp(x + (-Input.GetAxis("Horizontal") * Time.deltaTime * keyboardPanSpeed), maxLeftRotation, maxRightRotation) ;
				}

				if(!invertKeyboardVertical)
				{
					y = Mathf.Clamp(y + (Input.GetAxis("Vertical") * Time.deltaTime * keyboardPanSpeed), maxDownRotation, maxUpRotation) ;
				}
				else
				{
					y = Mathf.Clamp(y + (-Input.GetAxis("Vertical") * Time.deltaTime * keyboardPanSpeed), maxDownRotation, maxUpRotation) ;
				}
				
				panoCamera.transform.localRotation = Quaternion.Euler(-y, 0, 0 );
				panoCameraPivot.transform.rotation = Quaternion.Euler( 0, x + startingCameraAngle, 0 );
			}
			
			// Guibutton panning
			else if (originator == Originator.GuibuttonUp)
			{
				y = Mathf.Clamp(y + (1 * Time.deltaTime * keyboardPanSpeed), maxDownRotation, maxUpRotation) ;
				
				panoCamera.transform.localRotation = Quaternion.Euler(-y, 0, 0 );

			}
			else if (originator == Originator.GuibuttonDown)
			{
				y = Mathf.Clamp(y + (-1 * Time.deltaTime * keyboardPanSpeed), maxDownRotation, maxUpRotation) ;
				
				panoCamera.transform.localRotation = Quaternion.Euler(-y, 0, 0 );
			}
			else if (originator == Originator.GuibuttonLeft)
			{
				x = Mathf.Clamp(x + (-1 * Time.deltaTime * keyboardPanSpeed), maxLeftRotation, maxRightRotation) ;
				
				panoCameraPivot.transform.rotation = Quaternion.Euler( 0, x + startingCameraAngle, 0 );
			}
			else if (originator == Originator.GuibuttonRight)
			{
				x = Mathf.Clamp(x + (1 * Time.deltaTime * keyboardPanSpeed), maxLeftRotation, maxRightRotation) ;

				panoCameraPivot.transform.rotation = Quaternion.Euler( 0, x + startingCameraAngle, 0 );
			}
		}
		
		// if theres no rotational limits, pan freely
		else
		{
			// mouse panning
			if(originator == Originator.Mouse)
			{
				if(!invertMouseVertical)
				{
					panoCamera.transform.Rotate((-Input.GetAxis("Mouse Y")  * mousePanSpeed * Time.deltaTime), 0, 0);
				}
				else
				{
					panoCamera.transform.Rotate((Input.GetAxis("Mouse Y")  * mousePanSpeed * Time.deltaTime), 0, 0);
				}

				if(!invertMouseHorizontal)
				{
					panoCameraPivot.transform.Rotate(0, (Input.GetAxis("Mouse X")  * mousePanSpeed * Time.deltaTime), 0);
				}
				else
				{
					panoCameraPivot.transform.Rotate(0, (-Input.GetAxis("Mouse X")  * mousePanSpeed * Time.deltaTime), 0);
				}
			}
			
			// touch panning
			else if(originator == Originator.TouchDevice)
			{
				if(!invertTouchVertical)
				{
					panoCamera.transform.Rotate((-finger1.deltaPosition.y  * touchPanSpeed * Time.deltaTime), 0, 0);
				}
				else
				{
					panoCamera.transform.Rotate((finger1.deltaPosition.y  * touchPanSpeed * Time.deltaTime), 0, 0);
				}

				if(!invertTouchHorizontal)
				{
					panoCameraPivot.transform.Rotate(0, (finger1.deltaPosition.x * touchPanSpeed * Time.deltaTime),0) ;
				}
				else
				{
					panoCameraPivot.transform.Rotate(0, (-finger1.deltaPosition.x * touchPanSpeed * Time.deltaTime),0) ;
				}
			}
			
			// keyboard panning
			else if(originator == Originator.Keyboard)
			{
				if(!invertKeyboardVertical)
				{
					panoCamera.transform.Rotate(-Input.GetAxis("Vertical") * keyboardPanSpeed * Time.deltaTime, 0, 0) ;
				}
				else
				{
					panoCamera.transform.Rotate(Input.GetAxis("Vertical") * keyboardPanSpeed * Time.deltaTime, 0, 0) ;
				}

				if(!invertKeyboardHorizontal)
				{
					panoCameraPivot.transform.Rotate(0, Input.GetAxis("Horizontal") * keyboardPanSpeed * Time.deltaTime, 0) ;
				}
				else
				{
					panoCameraPivot.transform.Rotate(0, -Input.GetAxis("Horizontal") * keyboardPanSpeed * Time.deltaTime, 0) ;
				}
			}
			
			// Guibutton panning
			else if(originator == Originator.GuibuttonUp)
			{
				panoCamera.transform.Rotate(-1 * keyboardPanSpeed * Time.deltaTime, 0, 0) ;
			}
			else if(originator == Originator.GuibuttonDown)
			{
				panoCamera.transform.Rotate(1 * keyboardPanSpeed * Time.deltaTime, 0, 0) ;
			}
			else if(originator == Originator.GuibuttonLeft)
			{
				panoCameraPivot.transform.Rotate(0, -1 * keyboardPanSpeed * Time.deltaTime, 0) ;
			}
			else if(originator == Originator.GuibuttonRight)
			{
				panoCameraPivot.transform.Rotate(0, 1 * keyboardPanSpeed * Time.deltaTime, 0) ;
			}
		}
	}
	
	#endregion
	
	#region Zooming
	// Responsible for zooming in for all control schemes
	public void ZoomIn(Originator originator)
	{
		// First checks to see if we camera can zoom in
		if(panoCamera.fieldOfView > minZoomFov)
		{
			// Touch device zoom in
			if(originator == Originator.TouchDevice)
			{
				if(panoCamera.fieldOfView - (touchZoomSpeed * Time.deltaTime) >= minZoomFov)
				{
					panoCamera.fieldOfView = panoCamera.fieldOfView - (touchZoomSpeed * Time.deltaTime) ;
				}
				else
				{
					panoCamera.fieldOfView = minZoomFov ;	
				}
				
				if(limitXRotation || limitYRotation)
				{
					AdjustRotationalLimitZoomIn() ;
				}
			}
			
			// Mouse zoom in
			else if(originator == Originator.Mouse)
			{
				if(panoCamera.fieldOfView - (mouseZoomSpeed * Time.deltaTime) >= minZoomFov)
				{
					panoCamera.fieldOfView = panoCamera.fieldOfView - (mouseZoomSpeed * Time.deltaTime) ;
				}
				else
				{
					panoCamera.fieldOfView = minZoomFov ;	
				}
				
				if(limitXRotation || limitYRotation)
				{
					AdjustRotationalLimitZoomIn() ;
				}
			}
			
			else if(originator == Originator.Keyboard)
			{
				// Keyboard zoom in
				if(panoCamera.fieldOfView - ((keyboardZoomSpeed * Time.deltaTime)) >= minZoomFov)
				{
					panoCamera.fieldOfView = panoCamera.fieldOfView - ((keyboardZoomSpeed * Time.deltaTime)) ;
				}
				else
				{
					panoCamera.fieldOfView = minZoomFov ;	
				}
				
				if(limitXRotation || limitYRotation)
				{
					AdjustRotationalLimitZoomIn() ;
				}
			}
			else if(originator == Originator.GuibuttonZoomIn)
			{
				// guibutton zoom in
				if(panoCamera.fieldOfView - ((keyboardZoomSpeed * Time.deltaTime)) >= minZoomFov)
				{
					panoCamera.fieldOfView = panoCamera.fieldOfView - ((keyboardZoomSpeed * Time.deltaTime)) ;
				}
				else
				{
					panoCamera.fieldOfView = minZoomFov ;	
				}
				
				if(limitXRotation || limitYRotation)
				{
					AdjustRotationalLimitZoomIn() ;
				}
			}
		}
	}
	
	// Responsible for zooming out for all control schemes
	public void ZoomOut(Originator originator)
	{
		// First checks to see if we camera can zoom out
		if(panoCamera.fieldOfView < maxZoomFov)
		{
			// Touch zoom out
			if(originator == Originator.TouchDevice)
			{
				if(panoCamera.fieldOfView + (touchZoomSpeed * Time.deltaTime) <= maxZoomFov)
				{
					panoCamera.fieldOfView = panoCamera.fieldOfView + (touchZoomSpeed * Time.deltaTime) ;
				}
				else
				{
					panoCamera.fieldOfView = maxZoomFov ;	
				}
				
				if(limitXRotation || limitYRotation)
				{
					AdjustRotationalLimitZoomOut(2.5f) ;
				}
			}
			
			// Mouse zoom out
			else if(originator == Originator.Mouse)
			{
				if(panoCamera.fieldOfView + (mouseZoomSpeed * Time.deltaTime) <= maxZoomFov)
				{
					panoCamera.fieldOfView = panoCamera.fieldOfView + (mouseZoomSpeed * Time.deltaTime) ;
				}
				else
				{
					panoCamera.fieldOfView = maxZoomFov ;	
				}
				
				if(limitXRotation || limitYRotation)
				{
					AdjustRotationalLimitZoomOut(0.59f) ;
				}
			}
			
			else if(originator == Originator.Keyboard)
			{
				// Keyboard zoom out
				if(panoCamera.fieldOfView + ((keyboardZoomSpeed * Time.deltaTime)) <= maxZoomFov)
				{
					panoCamera.fieldOfView = panoCamera.fieldOfView + ((keyboardZoomSpeed * Time.deltaTime)) ;
				}
				else
				{
					panoCamera.fieldOfView = maxZoomFov ;	
				}
				
				if(limitXRotation || limitYRotation)
				{
					AdjustRotationalLimitZoomOut(0.25f) ;
				}
			}
			
			else if(originator == Originator.GuibuttonZoomOut)
			{
				// guibutton zoom out
				if(panoCamera.fieldOfView + ((keyboardZoomSpeed * Time.deltaTime)) <= maxZoomFov)
				{
					panoCamera.fieldOfView = panoCamera.fieldOfView + ((keyboardZoomSpeed * Time.deltaTime)) ;
				}
				else
				{
					panoCamera.fieldOfView = maxZoomFov ;	
				}
				
				if(limitXRotation || limitYRotation)
				{
					AdjustRotationalLimitZoomOut(0.25f) ;
				}
			}
		}
	}
	
	// Sets the rotational limits according to the zoom level, when zooming in
	void AdjustRotationalLimitZoomIn()
	{
		panZoomOffset = (maxZoomFov - panoCamera.fieldOfView) ;
		
		if(limitXRotation)
		{
			maxLeftRotation =  initialMaxLeftRotation - panZoomOffset / 1.475f ;
			maxRightRotation =  initialMaxRightRotation + panZoomOffset / 1.475f ;
		}
		
		if(limitYRotation)
		{
			maxDownRotation = initialMaxDownRotation - panZoomOffset / 2f ;
			maxUpRotation = initialMaxUpRotation + panZoomOffset / 2f ;
		}
	}
	
	// Sets the rotational limits according to the zoom level, when zooming out
	void AdjustRotationalLimitZoomOut(float amount)
	{
		if(maxLeftRotation < initialMaxLeftRotation && maxLeftRotation != 0)
		{
			maxLeftRotation =  maxLeftRotation +  amount ;
		}
		else
		{
			maxLeftRotation = initialMaxLeftRotation ;
		}
				
		if(maxRightRotation > initialMaxRightRotation && maxRightRotation != 0)
		{
			maxRightRotation =  maxRightRotation - amount;
		}
		else
		{
			maxRightRotation = initialMaxRightRotation ;
		}
		
		if(maxDownRotation < initialMaxDownRotation && maxDownRotation != 0)
		{
			maxDownRotation =  maxDownRotation +  amount ;
		}
		else
		{
			maxDownRotation = initialMaxDownRotation ;
		}
				
		if(maxUpRotation > initialMaxUpRotation && maxUpRotation != 0)
		{
			maxUpRotation =  maxUpRotation - amount;
		}
		else
		{
			maxUpRotation = initialMaxUpRotation ;
		}
		
		if(limitYRotation)
		{
			y = Mathf.Clamp(y , maxDownRotation, maxUpRotation) ;
			panoCamera.transform.localRotation = Quaternion.Euler(-y, 0, 0 );
		}
		
		if(limitXRotation)
		{
			x = Mathf.Clamp(x , maxLeftRotation, maxRightRotation) ;
			panoCameraPivot.transform.rotation = Quaternion.Euler( 0,  x + startingCameraAngle , 0 );
		}
		
		if(panoCamera.fieldOfView == maxZoomFov)
		{
			maxLeftRotation = initialMaxLeftRotation ;
			maxRightRotation = initialMaxRightRotation ;
			maxUpRotation = initialMaxUpRotation ;
			maxDownRotation = initialMaxDownRotation ;
		}
	}
	#endregion	
	
	#region Hotspot Handling
	// Responsible for checking if the user has touched a hotspot, and if so, triggering the hotspot
	void HasTouchedHotspot()
	{
		if(Input.touchCount == 1 && Input.GetTouch(0).phase == TouchPhase.Ended)
		{
			if(canTouchHotspot && overGuiButton == false)
			{
				RaycastHit hit ;
				
				Ray ray = panoCamera.ScreenPointToRay(Input.GetTouch(0).position) ;
				
				if(Physics.Raycast(ray, out hit, 100))
				{
					if(hit.collider.gameObject.GetComponent<HotSpot>() != null)
					{
						HotSpot hotSpot = hit.collider.gameObject.GetComponent<HotSpot>() ;
						hotSpot.Execute(" triggered by touch device") ;
					}
				}
			}
		}
	}
	
	// Responsible for checking if the user has clicked a hotspot, and if so, triggering the hotspot
	void HasClickedHotspot()
	{
		if(overGuiButton == false)
		{
			RaycastHit hit ;
			
			Ray ray = panoCamera.ScreenPointToRay(Input.mousePosition) ;
			
			if(Physics.Raycast(ray, out hit, 100))
			{
				if(hit.collider.gameObject.GetComponent<HotSpot>() != null)
				{
					HotSpot hotSpot = hit.collider.gameObject.GetComponent<HotSpot>() ;
					hotSpot.Execute(" triggered by mouse") ;
				}
			}
		}
	}
	#endregion
	
	#region Gui Button Checks
	void CheckIfOverGuiElement()
	{
		Vector2 pos ;
		
		if(useMouseControls)
		{
			pos = new Vector2(Input.mousePosition.x,Screen.height - Input.mousePosition.y); 
		}
		else
		{
			pos = new Vector2(Input.GetTouch(0).position.x,Screen.height - Input.GetTouch(0).position.y); 
		}
			
		if(guiButtonRects != null)
		{
			foreach(Rect rectCheck in guiButtonRects)
			{
				if(rectCheck.Contains(pos))
				{
					overGuiButton = true ;
				}
			}
		}
	}
	#endregion
}
