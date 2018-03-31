using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//attach class to hotspots
public class HotspotBehaviour : MonoBehaviour {

	//insert view number field in inspector
	public int viewNumber;

	public void ChangeView(){
		
		RoomManager.instance.ChangeView (viewNumber);

	}
}
