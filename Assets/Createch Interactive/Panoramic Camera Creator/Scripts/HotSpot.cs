using UnityEngine;
using System.Collections;

public class HotSpot : MonoBehaviour 
{
	#region public variables
	public string hotSpotID ;
	
	public bool runExternalFunction ;
	public bool sendHotSpotIdExternal ;
	public string externalFunctionName ;
	public object[] variableToSendWith ;
	
	public bool sendHotSpotIdInternal ;
	public GameObject objectToSendIdTo ;
	public string nameOfRecievingMethod ;
	public bool requiresReciever ;
	
	public bool debugHotSpotIdInConsole ;
	
	public bool loadsNewScene ;
	public string sceneName ;
	
	public bool loadsNewPano ;
	public Material panoMaterialToLoad ;
	
	public bool playsSound ;
	public AudioClip soundToPlay ;
	#endregion
	
	// The main method that will be called when the hotspot is clicked upon
	public void Execute(string message)
	{
		if(runExternalFunction)
		{
			RunExternalFunction() ;
		}
		
		if(sendHotSpotIdExternal)
		{
			SendHotSpotIdExternal() ;
		}
		
		if(sendHotSpotIdInternal)
		{
			SendHotSpotIdInternal() ;
		}
		
		if(debugHotSpotIdInConsole)
		{
			DebugHotSpotIdInConsole(message) ;
		}
		
		if(loadsNewScene)
		{
			if(sceneName != "")
			{
				Application.LoadLevel(sceneName) ;
			}
			else
			{
				Debug.LogWarning("Scene to load is missing a value or has invalid name specified") ;	
			}
		}
		
		if(loadsNewPano)
		{
			if(panoMaterialToLoad != null)
			{
				RenderSettings.skybox = panoMaterialToLoad ;
			}
			else
			{
				Debug.LogWarning("Missing a valid pano material to load, originated from hotspot with id: " + hotSpotID) ;	
			}
		}
		
		if(playsSound && soundToPlay != null)
		{
			if(GetComponent<AudioSource>() == null)
			{
				gameObject.AddComponent<AudioSource>() ;
			}
			
			GetComponent<AudioSource>().GetComponent<AudioSource>().clip = soundToPlay ;
			GetComponent<AudioSource>().Play() ;
		}
	}
	
	#region External Methods
	// Runs an external function on the webpage if the unity object is embedded on a webpage (Webplayer platform)
	public void RunExternalFunction()
	{
		Application.ExternalCall(externalFunctionName) ;
	}
	
	// Send the hotspot ID to an external function on the webpage if the unity object is embedded on a webpage (Webplayer platform)
	public void SendHotSpotIdExternal()
	{
		Application.ExternalCall(externalFunctionName, hotSpotID) ;
	}
	#endregion
	
	#region Internal Methods
	// Sends the hotspot ID to an internal gameObject that contains a specific method in a script.
	// also sets if a receiver is required or not
	public void SendHotSpotIdInternal()
	{
		if(requiresReciever == true)
		{
			objectToSendIdTo.SendMessage(nameOfRecievingMethod, hotSpotID, SendMessageOptions.RequireReceiver) ;
		}
		else
		{
			objectToSendIdTo.SendMessage(nameOfRecievingMethod, hotSpotID, SendMessageOptions.DontRequireReceiver) ;
		}
	}
	#endregion
	
	#region Testing
	// For testing purposes. Prints the hotspot ID to the console, of the hotspot that was clicked.
	public void DebugHotSpotIdInConsole(string msg)
	{
		Debug.Log(hotSpotID + msg) ;
	}
	#endregion
}
