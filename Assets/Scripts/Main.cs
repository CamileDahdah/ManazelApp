using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using UnityEngine.Video;

public class Main : MonoBehaviour {

	Material mainMaterial;
	public static Main instance;

	public GameObject hotspotParent, objectParent, videoParent;

	string resourcehotspots = "Hotspots", roomPath = "LivingRoom" , viewPath = "View", texturePath = "Textures", objectPath = "Objects";
	string roomsPath = "Rooms", videoPath = "Videos", videoViewsPath = "VideoViews";
	List<GameObject> hotspotList = new List<GameObject>();
	List<GameObject> objectList = new List<GameObject>();
	List<Texture> textureList = new List<Texture>();
	List<GameObject> videoList = new List<GameObject>();
	public int currentViewNumber = 0;

	public GameObject cubeTransition;

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
		BoxCollider originalBoxCollider = originalHotspot.GetComponent<BoxCollider> ();

		foreach (GameObject hotspotParent in hotspotList) {
			foreach (Transform hotspotChild in hotspotParent.transform) {

				if(hotspotChild.childCount > 0){
					Destroy(hotspotChild.GetChild(0).gameObject);
				}

				Instantiate (originalHotspot, hotspotChild);

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

		ChangeMaterial (textureList [currentViewNumber]);

		//Load All
		Object[] objects = Resources.LoadAll (resourceLocation + objectPath);

		foreach (GameObject selectedObject in objects) {
			objectList.Add(Instantiate (selectedObject, objectParent.transform));
		}

		for (int i = 1; i < objectList.Count; i++) {
			objectList [i].SetActive (false);
		}
			
		//TODO: Change this shit
		//Load All video views
		GameObject[] videoViews = Resources.LoadAll<GameObject> (resourceLocation + videoViewsPath);

		foreach (GameObject videoView in videoViews) {
			videoList.Add(Instantiate (videoView, videoParent.transform));
		}

		for (int i = 2; i < videoList.Count; i++) {
			videoList [i].SetActive (false);
		}

		GameObject[] videoObjects = GameObject.FindGameObjectsWithTag ("Video");

		//load all videos
		VideoClip[] videoClip = Resources.LoadAll<VideoClip>(resourceLocation + videoPath);

		VideoPlayer videoPlayer = videoList[0].GetComponentInChildren<VideoPlayer>();

		videoPlayer.clip = videoClip[0];
		videoPlayer.SetDirectAudioMute (0, true);

	}

	public void ChangeView (int viewNumber){
		
		viewNumber--;

		objectList [currentViewNumber].SetActive (false);
		videoList [currentViewNumber + 1].SetActive (false);

		currentViewNumber = viewNumber;


		StartCoroutine("TransitionAnimation");
	}

	void ChangeMaterial(Texture texture){
		mainMaterial.mainTexture = texture;
	}

	float transitionSpeed = 2.5f;

	IEnumerator TransitionAnimation(){

		Vector3 direction = new Vector3(Camera.main.transform.forward.x, 0 ,Camera.main.transform.forward.z).normalized;
		cubeTransition.transform.position = Camera.main.transform.position + direction;

		Material cubeTransitionMaterial = cubeTransition.GetComponent<MeshRenderer> ().material;

		cubeTransitionMaterial.mainTexture = textureList [currentViewNumber];

		cubeTransitionMaterial.SetColor("_Color", new Color(1,1,1, 0));

		Color cubeTransitionColor = cubeTransitionMaterial.GetColor("_Color");

		while (Vector3.Distance(cubeTransition.transform.position, gameObject.transform.position) > 0) {
			
			cubeTransition.transform.position = Vector3.MoveTowards 
				(cubeTransition.transform.position, gameObject.transform.position, Time.deltaTime * transitionSpeed);
			
			cubeTransitionMaterial.SetColor("_Color", new Color (cubeTransitionColor.r, cubeTransitionColor.g, cubeTransitionColor.b, 
				Mathf.MoveTowards(cubeTransitionMaterial.GetColor("_Color").a, 1, Time.deltaTime * transitionSpeed)));
			
			yield return null;
		}

		gameObject.GetComponent<MeshRenderer> ().material.mainTexture = textureList [currentViewNumber];

		cubeTransitionMaterial.SetColor("_Color", new Color(1,1,1, 0));


		hotspotList [currentViewNumber].SetActive(true);
		objectList [currentViewNumber].SetActive (true);
		videoList [currentViewNumber + 1].SetActive (true);
	}


}





