using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HotspotBehaviour : MonoBehaviour {

	public int viewNumber;

	public void ChangeView(){
		
		transform.parent.gameObject.SetActive (false);
		Main.instance.ChangeView (viewNumber);

	}
}
