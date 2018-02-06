using UnityEngine;
using System.Collections;
using UnityEditor ;

[CustomEditor(typeof(PanoControls))]
public class PanoControlsInspector : Editor
{
	private bool showCameraRigOptions ;
	private bool showControlOptions ;
	private bool showZoomOptions ;
	private bool showRotationOptions ;
	private bool showPanSpeedOptions ;
	private bool showControlInversionOptions ;
	private bool showHotspotOptions ;

	private GUIStyle heading ;

	public override void OnInspectorGUI() 
	{
		PanoControls panoScript = (PanoControls) target;

		heading = new GUIStyle(EditorStyles.foldout) ;
		heading.fontSize = 12 ;
		heading.normal.textColor = Color.white ;
		heading.stretchWidth = true ;
		heading.fontStyle = FontStyle.Bold ;

		GUILayout.Space(20) ;
		showCameraRigOptions = EditorGUILayout.Foldout(showCameraRigOptions,"Camera rig objects", heading) ;

		if(showCameraRigOptions)
		{
			GUILayout.Space(10) ;
			panoScript.panoCameraPivot = (GameObject) EditorGUILayout.ObjectField("Pano camera pivot", panoScript.panoCameraPivot, typeof(GameObject), true) ;
			panoScript.panoCamera = (Camera) EditorGUILayout.ObjectField("Pano camera", panoScript.panoCamera, typeof(Camera), true) ;
		}

		GUILayout.Space(10) ;
		showControlOptions = EditorGUILayout.Foldout(showControlOptions,"Control options", heading);

		if(showControlOptions)
		{
			GUILayout.Space(10) ;
			panoScript.useKeyboardControls = EditorGUILayout.Toggle("Use keyboard controls", panoScript.useKeyboardControls) ;
			panoScript.useMouseControls = EditorGUILayout.Toggle("Use mouse controls", panoScript.useMouseControls) ;
			panoScript.useTouchControls = EditorGUILayout.Toggle("Use touch controls", panoScript.useTouchControls) ;
		}

		GUILayout.Space(10) ;
		showZoomOptions = EditorGUILayout.Foldout(showZoomOptions,"Zoom options", heading);

		if(showZoomOptions)
		{
			GUILayout.Space(10) ;
			panoScript.minZoomFov = EditorGUILayout.FloatField("Minimum zoom FOV", panoScript.minZoomFov) ;
			panoScript.maxZoomFov = EditorGUILayout.FloatField("Maximum zoom FOV", panoScript.maxZoomFov) ;

			GUILayout.Space(10) ;
			panoScript.keyboardZoomSpeed = EditorGUILayout.FloatField("Keyboard zoom speed", panoScript.keyboardZoomSpeed) ;
			panoScript.touchZoomSpeed = EditorGUILayout.FloatField("Touch zoom speed", panoScript.touchZoomSpeed) ;
			panoScript.mouseZoomSpeed= EditorGUILayout.FloatField("Mouse zoom speed", panoScript.mouseZoomSpeed) ;
		}

		GUILayout.Space(10) ;
		showRotationOptions = EditorGUILayout.Foldout(showRotationOptions,"Rotation options", heading);

		if(showRotationOptions)
		{
			GUILayout.Space(10) ;
			panoScript.limitXRotation  = EditorGUILayout.Toggle("Limit horizontal rotation", panoScript.limitXRotation) ;

			if(panoScript.limitXRotation)
			{
				GUILayout.Space(10) ;
				panoScript.maxLeftRotation = EditorGUILayout.FloatField("Maximum left rotation", panoScript.maxLeftRotation) ;
				panoScript.maxRightRotation = EditorGUILayout.FloatField("Maximum right rotation", panoScript.maxRightRotation) ;
				GUILayout.Space(10) ;
			}

			panoScript.limitYRotation  = EditorGUILayout.Toggle("Limit vertical rotation", panoScript.limitYRotation) ;

			if(panoScript.limitYRotation)
			{
				GUILayout.Space(10) ;
				panoScript.maxUpRotation = EditorGUILayout.FloatField("Maximum up rotation", panoScript.maxUpRotation) ;
				panoScript.maxDownRotation = EditorGUILayout.FloatField("Maximum down rotation", panoScript.maxDownRotation) ;
				GUILayout.Space(10) ;
			}

			if(!panoScript.limitXRotation && !panoScript.limitYRotation)
			{
				GUILayout.Space(10) ;
			}
		}
		else
		{
			GUILayout.Space(10) ;
		}

		showPanSpeedOptions = EditorGUILayout.Foldout(showPanSpeedOptions,"Pan speed options", heading);

		if(showPanSpeedOptions)
		{
			GUILayout.Space(10) ;
			panoScript.keyboardPanSpeed = EditorGUILayout.FloatField("Keyboard pan speed", panoScript.keyboardPanSpeed) ;
			panoScript.touchPanSpeed = EditorGUILayout.FloatField("Touch pan speed", panoScript.touchPanSpeed) ;
			panoScript.mousePanSpeed = EditorGUILayout.FloatField("Mouse pan speed", panoScript.mousePanSpeed) ;
		}

		GUILayout.Space(10) ;
		showControlInversionOptions = EditorGUILayout.Foldout(showControlInversionOptions,"Control inversion options", heading);

		if(showControlInversionOptions)
		{
			GUILayout.Space(10) ;
			panoScript.invertMouseZoom = EditorGUILayout.Toggle("Invert mouse zoom", panoScript.invertMouseZoom) ;
			panoScript.invertMouseVertical = EditorGUILayout.Toggle("Invert mouse vertical", panoScript.invertMouseVertical) ;
			panoScript.invertMouseHorizontal = EditorGUILayout.Toggle("Invert mouse horizontal", panoScript.invertMouseHorizontal) ;
			GUILayout.Space(5) ;

			panoScript.invertKeyboardVertical = EditorGUILayout.Toggle("Invert keyboard vertical", panoScript.invertKeyboardVertical) ;
			panoScript.invertKeyboardHorizontal = EditorGUILayout.Toggle("Invert keyboard horizontal", panoScript.invertKeyboardHorizontal) ;
			GUILayout.Space(5) ;

			panoScript.invertTouchVertical = EditorGUILayout.Toggle("Invert touch vertical", panoScript.invertTouchVertical) ;
			panoScript.invertTouchHorizontal = EditorGUILayout.Toggle("Invert touch horizontal", panoScript.invertTouchHorizontal) ;
		}

		GUILayout.Space(10) ;
		showHotspotOptions = EditorGUILayout.Foldout(showHotspotOptions,"Hotspot options", heading);

		if(showHotspotOptions)
		{
			GUILayout.Space(10) ;
			panoScript.hideHotspotsAtStart = EditorGUILayout.Toggle("Hide hotspots at start", panoScript.hideHotspotsAtStart) ;
		}

		GUILayout.Space(20) ;
	}
}
