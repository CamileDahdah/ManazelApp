using UnityEngine;
using System.Collections;
using UnityEditor;

public class HotSpotCreator : ScriptableWizard 
{
	#region public variables
	public GameObject hotSpotObject ;
	public string hotSpotID ;
	
	public bool runExternalFunction ;
	public bool sendHotSpotIdExternal ;
	public string externalFunctionName ;
	
	public bool sendHotSpotIdInternal ;
	public GameObject objectToSendIdTo ;
	public string nameOfRecievingMethod ;
	public bool requiresReciever ;
	public bool debugIdToConsole ;
	
	public bool loadsNewScene ;
	public string sceneName ;
	
	public bool loadsNewPano ;
	public Material panoMaterialToLoad ;
	
	public bool playsSound ;
	public AudioClip soundToPlay ;
	#endregion
	
	#region private variables
	private HotSpot hotSpot ;
	#endregion
	
	// Opens the panoramic hotspot creator window when the menu item is selected
	[MenuItem ("Window/Createch Interactive/Panoramic Framework/Hotspot Creator")]
    static void CreateWizard () 
	{
        ScriptableWizard.DisplayWizard<HotSpotCreator>("Hotspot Creator", "Create a hotspot");
    }
	
	// Will run when something in the window changes. Set the error string and help string here
	void OnWizardUpdate()
	{
		helpString = "Please select the object in the scene to make a hotspot";

		if(hotSpotObject == null) 
		{
			errorString = "Please choose an object first";
			isValid = false ;
		}
		else if(hotSpotObject.GetComponent<Collider>() == null)
		{
			errorString = "The object doesnt have a collider, please add one first, then reselect the object";
			isValid = false ;
		}
		else 
		{
			errorString = "";
			isValid = true ;
		}
	}
	
	// Executed when the button "Create hotspot" is clicked. Creates the hotspot, with the option to undo.
	void OnWizardCreate () 
	{
		// Applies all the settings specified in the window to the actual script on the hotspot object
		hotSpot = hotSpotObject.AddComponent<HotSpot>() ;
		hotSpot.hotSpotID = hotSpotID ;
		hotSpot.runExternalFunction = runExternalFunction ;
		hotSpot.sendHotSpotIdExternal = sendHotSpotIdExternal ;
		hotSpot.externalFunctionName = externalFunctionName ;
		hotSpot.sendHotSpotIdInternal = sendHotSpotIdInternal ;
		hotSpot.objectToSendIdTo = objectToSendIdTo ;
		hotSpot.nameOfRecievingMethod = nameOfRecievingMethod ;
		hotSpot.requiresReciever = requiresReciever ;
		hotSpot.debugHotSpotIdInConsole = debugIdToConsole ;
		hotSpot.loadsNewScene = loadsNewScene ;
		hotSpot.sceneName = sceneName ;
		hotSpot.loadsNewPano = loadsNewPano ;
		hotSpot.panoMaterialToLoad = panoMaterialToLoad ;
		hotSpot.playsSound = playsSound ;
		hotSpot.soundToPlay = soundToPlay ;

		Undo.RegisterCreatedObjectUndo(hotSpot, "Create hotspot") ;
	} 
}
