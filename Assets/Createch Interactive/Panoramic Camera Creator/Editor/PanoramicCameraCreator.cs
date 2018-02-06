using UnityEngine;
using System.Collections;
using UnityEditor;

public class PanoramicCameraCreator : ScriptableWizard 
{
	#region public variables
	public Camera cameraToUse ;
	public string layerToAssignTo = "Default";
	public bool applyControls = true ;
	public bool useKeyboardControls = true ;
	public bool useMouseControls = true ;
	public bool useTouchControls ;
	public bool useGuiControls ;
	public bool limitXAxis ;
	public bool limitYAxis ;
	public float xAxisLeftLimit ;
	public float xAxisRightLimit ;
	public float yAxisDownLimit ;
	public float yAxisUpLimit ;
	public float minZoom = 20 ;
	public float maxZoom = 70 ;
	public float startingZoom = 70 ;
	public float keyboardZoomSpeed = 20 ;
	public float mouseZoomSpeed = 50 ;
	public float touchZoomSpeed = 30 ;
	public bool invertMouseZoom = true ;
	public float keyboardPanSpeed = 20 ;
	public float mousePanSpeed = 50 ;
	public float touchPanSpeed = 30 ;

	public bool invertMouseVertical ;
	public bool invertMouseHorizontal ;
	public bool invertKeyboardVertical ;
	public bool invertKeyboardHorizontal ;
	public bool invertTouchVertical ;
	public bool invertTouchHorizontal ;

	public bool hideHotspotsAtStart ;
	#endregion
	
	// Opens the panoramic camera creator window when the menu item is selected
	[MenuItem ("Window/Createch Interactive/Panoramic Framework/Panoramic Camera Creator")]
    static void CreateWizard () 
	{
        ScriptableWizard.DisplayWizard<PanoramicCameraCreator>("Panoramic Camera Creator", "Create Panoramic Camera");
    }
	
	// Will run when something in the window changes. Set the error string and help string here
	void OnWizardUpdate()
	{
		helpString = "Please select a camera";

		if(cameraToUse == null) 
		{
			errorString = "Please choose a camera first";
			isValid = false ;
		}
		else if(LayerMask.NameToLayer(layerToAssignTo) == -1)
		{
			errorString = "That layer does not exist";
			isValid = false ;	
		}
		else 
		{
			errorString = "";
			isValid = true ;
		}
	}
	
	// Executed when the button "Create panoramic rig" is clicked. Creates the panoramic rig, with the option to undo.
	void OnWizardCreate () 
	{
		Vector3 camPosition = cameraToUse.transform.position ;
		GameObject camPivot = new GameObject("Panoramic Cam Pivot") ;
		Undo.RegisterCreatedObjectUndo(camPivot, "Create panoramic rig") ;

		camPivot.transform.position = camPosition ;
		Undo.SetTransformParent(cameraToUse.transform, camPivot.transform, "Parent Camera") ;

		if(LayerMask.NameToLayer(layerToAssignTo) != -1 && layerToAssignTo != "")
		{
			Transform[] transforms = camPivot.GetComponentsInChildren<Transform>() ;
				
			foreach(Transform tf in transforms)
			{
				tf.gameObject.layer = LayerMask.NameToLayer(layerToAssignTo) ;	
			}
		}
		
		// Applies all the settings specified in the window to the actual script on the rig
		if(applyControls)
		{
			PanoControls panoControl = camPivot.gameObject.AddComponent<PanoControls>() ;
			
			if(useGuiControls)
			{
				camPivot.gameObject.AddComponent<GuiControls>() ;
			}
			
			panoControl.panoCamera = cameraToUse ;
			panoControl.panoCameraPivot = camPivot ;
			
			panoControl.minZoomFov = minZoom ;
			panoControl.maxZoomFov = maxZoom ;
			cameraToUse.fieldOfView = startingZoom ;
			
			panoControl.useKeyboardControls = useKeyboardControls ;
			panoControl.useMouseControls = useMouseControls ;
			panoControl.useTouchControls = useTouchControls ;
			
			panoControl.keyboardZoomSpeed = keyboardZoomSpeed ;
			panoControl.mouseZoomSpeed = mouseZoomSpeed ;
			panoControl.touchZoomSpeed = touchZoomSpeed;
			panoControl.invertMouseZoom = invertMouseZoom ;
			
			panoControl.keyboardPanSpeed = keyboardPanSpeed ;
			panoControl.mousePanSpeed = mousePanSpeed ;
			panoControl.touchPanSpeed = touchPanSpeed ;

			panoControl.invertMouseVertical = invertMouseVertical ;
			panoControl.invertMouseHorizontal = invertMouseHorizontal ;
			panoControl.invertKeyboardVertical = invertKeyboardVertical ;
			panoControl.invertKeyboardHorizontal = invertKeyboardHorizontal ;
			panoControl.invertTouchVertical = invertTouchVertical ;
			panoControl.invertTouchHorizontal = invertTouchHorizontal ;

			panoControl.hideHotspotsAtStart = hideHotspotsAtStart ;
				
			if(limitXAxis == true)
			{
				panoControl.limitXRotation = true ;
				panoControl.maxLeftRotation = xAxisLeftLimit ;
				panoControl.maxRightRotation = xAxisRightLimit ;
			}
				
			else if(limitYAxis == true)
			{
				panoControl.limitYRotation = true ;
				panoControl.maxUpRotation = yAxisUpLimit ;
				panoControl.maxDownRotation = yAxisDownLimit ;
			}
		}
	} 
}
