using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HotspotBehaviour : MonoBehaviour {

	public Texture textureView;

	public void ChangeView(){
		
		transform.parent.gameObject.SetActive (false);
		Main.instance.GetMainMaterial ().mainTexture = textureView;

	}
}
