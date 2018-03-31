using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using UnityEngine.Video;


public class RoomManager : MonoBehaviour {

	Material mainMaterial;
	public static RoomManager instance;

	public GameObject hotspotParent, objectParent, videoParent;

	string resourcehotspots = "Hotspots", roomPath = "LivingRoom" , viewPath = "View", texturePath = "Textures", objectPath = "Objects";
	string roomsPath = "Rooms", videoPath = "Videos", videoViewsPath = "VideoViews";
	List<GameObject> hotspotList = new List<GameObject>();
	List<GameObject> objectList = new List<GameObject>();
	List<Texture> textureList = new List<Texture>();
	List<GameObject> videoList = new List<GameObject>();
	public int currentViewNumber = 0;

	public GameObject movingRoom;

	void Awake () {
		
		if (instance == null) {
			instance = this;
		} else {
			Destroy (this);
		}

		mainMaterial = Instantiate( (Material) Resources.Load ("Materials/360View") );
		GetComponent<MeshRenderer> ().material = mainMaterial;
		string livingRoom = roomsPath + "/" + roomPath + "/";
		LoadRoom(livingRoom);

	}

	public Material GetMainMaterial(){
		return mainMaterial;
	}
		
	void LoadRoom(string resourceLocation){

		//Load All hotspot locations
		Object[] hotspotsObj = Resources.LoadAll (resourceLocation + resourcehotspots);

		foreach (GameObject hotspot in hotspotsObj) {
			hotspotList.Add(Instantiate (hotspot, hotspotParent.transform));
		}

		//Load hotspot prefab
		GameObject originalHotspot = Resources.Load<GameObject>("OriginalHotspot");

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

		//Load All texture views
		Object[] textures = Resources.LoadAll (resourceLocation + texturePath);

		foreach (Texture texture in textures) {
			textureList.Add(texture);
		}

		ChangeMaterial (textureList [currentViewNumber]);

		//Load All objects
		Object[] objects = Resources.LoadAll (resourceLocation + objectPath);

		foreach (GameObject selectedObject in objects) {
			objectList.Add(Instantiate (selectedObject, objectParent.transform));
		}

		for (int i = 1; i < objectList.Count; i++) {
			objectList [i].SetActive (false);
		}
			
		//TODO: Refactor code below
		//Load All video views
		GameObject[] videoViews = Resources.LoadAll<GameObject> (resourceLocation + videoViewsPath);

		foreach (GameObject videoView in videoViews) {
			videoList.Add(Instantiate (videoView, videoParent.transform));
		}

		for (int i = 2; i < videoList.Count; i++) {
			videoList [i].SetActive (false);
		}

		//load all videos
		VideoClip[] videoClip = Resources.LoadAll<VideoClip>(resourceLocation + videoPath);

		VideoPlayer videoPlayer = videoList[0].GetComponentInChildren<VideoPlayer>();

		videoPlayer.clip = videoClip[0];
		videoPlayer.SetDirectAudioMute (0, true);

	}

	void ChangeMaterial(Texture texture){
		mainMaterial.mainTexture = texture;
	}
		
	public void ChangeView (int viewNumber){
		
		viewNumber--;

		EnableViewLists (false);

		currentViewNumber = viewNumber;

		StartCoroutine("TransitionAnimation");
	}
		

	float animationSpeed = 1 / 0.15f;

	//hotspot transition animation logic: 
	//assign new view to temporary cube and blend it with original cube view
	//by moving it towards the latter and fading in temp cube's alpha channel
	IEnumerator TransitionAnimation(){

		Vector3 direction = new Vector3(Camera.main.transform.forward.x, 0 ,Camera.main.transform.forward.z).normalized;
		movingRoom.transform.position = Camera.main.transform.position + direction;

		Material cubeTransitionMaterial = movingRoom.GetComponent<MeshRenderer> ().material;

		cubeTransitionMaterial.mainTexture = textureList [currentViewNumber];

		cubeTransitionMaterial.SetColor("_Color", new Color(1,1,1, 0));

		Color cubeTransitionColor = cubeTransitionMaterial.GetColor("_Color");

		while (Vector3.Distance(movingRoom.transform.position, gameObject.transform.position) > 0) {
			
			movingRoom.transform.position = Vector3.MoveTowards 
				(movingRoom.transform.position, gameObject.transform.position, Time.deltaTime * animationSpeed);
			
			cubeTransitionMaterial.SetColor("_Color", new Color (cubeTransitionColor.r, cubeTransitionColor.g, cubeTransitionColor.b, 
				Mathf.MoveTowards(cubeTransitionMaterial.GetColor("_Color").a, 1, Time.deltaTime * animationSpeed)));
			
			yield return null;
		}

		gameObject.GetComponent<MeshRenderer> ().material.mainTexture = textureList [currentViewNumber];

		cubeTransitionMaterial.SetColor("_Color", new Color(1,1,1, 0));

		EnableViewLists (true);

	}

	void EnableViewLists(bool enable){
		
		hotspotList [currentViewNumber].SetActive(enable);
		objectList [currentViewNumber].SetActive (enable);
		videoList [currentViewNumber + 1].SetActive (enable);

	}
}





