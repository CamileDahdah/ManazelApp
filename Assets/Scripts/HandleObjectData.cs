using System.Collections;
using System.Collections.Generic;
using System.IO;     
using UnityEngine;
using UnityEngine.Networking;

public class HandleObjectData : MonoBehaviour {

	string fileName = "Objects.json";
	List<ObjectData> loadedData;
	ObjectsData objectsData;
	public static HandleObjectData instance;
	public string currenObjecttID = "";
	public int currentOrder = 0;
	int length = 0;
	string readableFilePath, writableFilePath;


	void Awake () {
		
		if (instance == null) {
			instance = this;
		} else {
			Destroy (this);
		}

		readableFilePath = Path.Combine (Application.streamingAssetsPath , fileName);
		writableFilePath = Path.Combine (Application.persistentDataPath , fileName);

		StartCoroutine("LoadObjectsData");
	}

	//Initialize JSON Objects file
	IEnumerator LoadObjectsData(){

		string dataAsJson = "";

		//If file not found in persistentDataPath then create new file (Currently supports android and standalone only)
		//TODO Also include IOS support (It might work like this but I haven't tested it on IOS yet)
		if (!File.Exists (writableFilePath)) {
			
			if (Application.platform == RuntimePlatform.Android) {
				
				UnityWebRequest www = UnityWebRequest.Get (readableFilePath);
				yield return www.SendWebRequest ();
				dataAsJson = www.downloadHandler.text;

			} else {
				dataAsJson = File.ReadAllText (readableFilePath);
			}

			FileStream file = File.Create (writableFilePath);
			file.Close ();
			File.WriteAllText (writableFilePath, dataAsJson);
		
		} else {
			Debug.Log (writableFilePath);
			dataAsJson = File.ReadAllText (writableFilePath); 
	
		}
	
		objectsData = JsonUtility.FromJson<ObjectsData> (dataAsJson);
		loadedData = objectsData.objects;
		length = loadedData.Count;



	}

	public ObjectData GetObjectData(string id){
		
		currenObjecttID = id;

		int iteration = 0;

		foreach(ObjectData objectData in loadedData){
			
			if (objectData.id == id) {
				
				if (!objectData.visited) {
					
					currentOrder = iteration;
					return objectData;

				} else {
					return null;
				}
			}
			iteration++;
		}
		return null;
	}

	public ObjectData GetNextObjectData(){
		
		do{
			
			if (currentOrder + 1 >= length) {
				currentOrder = 0; 
			} else {
				currentOrder++;
			}

		}while(CheckCurrentObject());

		currenObjecttID = loadedData [currentOrder].id;
		return loadedData[currentOrder];

	}

	public ObjectData GetPreviousObjectData(){

		do{
			if (currentOrder - 1 < 0) {
				currentOrder = length - 1; 
			} else {
				currentOrder--;
			}
		}while(CheckCurrentObject());

		currenObjecttID = loadedData [currentOrder].id;
		return loadedData[currentOrder];

	}

	public void SaveCurrentCorrectObject(){


		loadedData[currentOrder].visited = true;
		string JSONResult = JsonUtility.ToJson (objectsData, true);

		File.WriteAllText (writableFilePath, JSONResult);


	}

	public bool CheckCurrentObject(){
		return loadedData [currentOrder].visited;
	}

	//For testing purposes only, this method resets the state of the game (score is back to 0)
	//TODO For testing purposes I have resetted the app when I click on the help toggle and exit the app, this should be changed later
	public void ResetAll(){

		foreach(ObjectData objectData in loadedData){
			
			objectData.visited = false;

		}
			
		if (File.Exists (writableFilePath)) {
			File.Delete (writableFilePath);
		}
			
	}

}
