using UnityEngine;
using System.Collections;

public class HotSpotMethods : MonoBehaviour
{
	// Finds all the hotspots in the scene, and destroys a hotspot according to its id. use "all" to destroy all the hotspots
	public static void DestroyHotspots(string hotSpotID)
	{	
		HotSpot[] hotSpots = FindObjectsOfType(typeof(HotSpot)) as HotSpot[];
		
		foreach(HotSpot hotSpot in hotSpots)
		{
			if(hotSpot.hotSpotID == hotSpotID)
			{
				Destroy(hotSpot.gameObject) ;
			}
			else if(hotSpotID == "all")
			{
				Destroy(hotSpot.gameObject) ;	
			}
		}
	}
	
	// Finds all the hotspots in the scene, and hides a hotspot according to it's id. use "all" to hide all the hotspots
	// What it does is turn off the hotspots mesh render, so you can still find the object since its active
	public static void HideHotspots(string hotSpotID)
	{
		HotSpot[] hotSpots = FindObjectsOfType(typeof(HotSpot)) as HotSpot[];
		
		foreach(HotSpot hotSpot in hotSpots)
		{
			if(hotSpot.hotSpotID == hotSpotID)
			{
				hotSpot.gameObject.GetComponent<Renderer>().enabled = false ;
			}
			else if(hotSpotID == "all")
			{
				hotSpot.gameObject.GetComponent<Renderer>().enabled = false ;	
			}
		}
	}
	
	// Finds all the hotspots in the scene, and unhides a hotspot according to it's id. use "all" to unhide all the hotspots
	public static void ShowHotspots(string hotSpotID)
	{
		HotSpot[] hotSpots = FindObjectsOfType(typeof(HotSpot)) as HotSpot[];
		
		foreach(HotSpot hotSpot in hotSpots)
		{
			if(hotSpot.hotSpotID == hotSpotID)
			{
				hotSpot.gameObject.GetComponent<Renderer>().enabled = true ;
			}
			else if(hotSpotID == "all")
			{
				hotSpot.gameObject.GetComponent<Renderer>().enabled = true ;	
			}
		}
	}
}
