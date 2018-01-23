using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

public class Main : MonoBehaviour {

	Material mainMaterial;
	public static Main instance;

	public GameObject hotspotParent;

	string resourcehotspots = "Hotspots", roomPath = "LivingRoom" , viewPath = "View", texturePath = "Textures";
	string roomsPath = "Rooms";
	List<GameObject> hotspotList = new List<GameObject>();
	List<Texture> textureList = new List<Texture>();

	void Awake () {
		
		if (instance == null) {
			instance = this;
		} else {
			Destroy (this);
		}

		mainMaterial = Instantiate( (Material) Resources.Load ("Materials/360View") );
		GetComponent<MeshRenderer> ().material = mainMaterial;
		string livingRoom = roomsPath + "/" + roomPath + "/"; //+ resourcehotspots + "/"; //+ view + "/";
		LoadHotspots(livingRoom);

	}


	public Material GetMainMaterial(){
		return mainMaterial;
	}
		
	void LoadHotspots(string resourceLocation){

		//Load All
		Object[] hotspotsObj = Resources.LoadAll (resourceLocation + resourcehotspots);

		foreach (GameObject hotspot in hotspotsObj) {
			hotspotList.Add(Instantiate (hotspot, hotspotParent.transform));
		}

		//Load
		GameObject originalHotspot = Resources.Load<GameObject>("OriginalHotspot");

		SpriteRenderer originalSprite = originalHotspot.GetComponent<SpriteRenderer> ();
		BoxCollider2D originalBoxCollider2D = originalHotspot.GetComponent<BoxCollider2D> ();

		GameObject childOriginalHotspot = null;

		if (originalHotspot.transform.childCount > 0) {
			childOriginalHotspot = originalHotspot.transform.GetChild (0).gameObject;
		}

		foreach (GameObject hotspotParent in hotspotList) {
			foreach (Transform hotspotChild in hotspotParent.transform) {
				
				hotspotChild.transform.localScale = originalHotspot.transform.localScale;
				hotspotChild.gameObject.AddComponent (originalSprite);
				hotspotChild.gameObject.AddComponent (originalBoxCollider2D);

				hotspotChild.gameObject.GetComponent<BoxCollider2D>().offset = originalBoxCollider2D.offset;

				if (childOriginalHotspot) {
					Instantiate (childOriginalHotspot, hotspotChild);
				}
			}
		}

		for (int i = 1; i < hotspotList.Count; i++) {
			hotspotList [i].SetActive (false);
		}

		//Load All
		Object[] textures = Resources.LoadAll (resourceLocation + texturePath);

		foreach (Texture texture in textures) {
			textureList.Add(texture);
		}

	}

	public void ChangeView (int viewNumber){
		viewNumber--;
		hotspotList [viewNumber].SetActive(true);

		ChangeMaterial (textureList [viewNumber]);
	}

	void ChangeMaterial(Texture texture){
		mainMaterial.mainTexture = texture;
	}

}





