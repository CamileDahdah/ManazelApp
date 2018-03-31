using System.Collections;
using System.Collections.Generic;
using System.IO;     
using UnityEngine;
using UnityEngine.Networking;

public class ReadJSON : MonoBehaviour {

	string fileName = "Objects.json";
	List<ObjectData> loadedData;
	ObjectsData objectsData;
	public static ReadJSON instance;
	public string currentID = "";
	public int currentOrder = 0;
	int length = 0;
	string filePath, newFilePath;


	void Awake () {
		
		if (instance == null) {
			instance = this;
		} else {
			Destroy (this);
		}

		filePath = Path.Combine (Application.streamingAssetsPath , fileName);
		newFilePath = Path.Combine (Application.persistentDataPath , fileName);
		StartCoroutine("LoadObjectsData");
	}

	IEnumerator LoadObjectsData(){

		string dataAsJson = "";

		if (!File.Exists (newFilePath)) {
			
			if (Application.platform == RuntimePlatform.Android) {
				
				UnityWebRequest www = UnityWebRequest.Get (filePath);
				yield return www.SendWebRequest ();
				dataAsJson = www.downloadHandler.text;

			} else {
				dataAsJson = File.ReadAllText (filePath);
			}

			FileStream file = File.Create (newFilePath);
			file.Close ();
			File.WriteAllText (newFilePath, dataAsJson);
		
		} else {
			Debug.Log (newFilePath);
			dataAsJson = File.ReadAllText (newFilePath); 
	
		}
	
		objectsData = JsonUtility.FromJson<ObjectsData> (dataAsJson);
		loadedData = objectsData.objects;
		length = loadedData.Count;



	}

	public ObjectData GetObjectData(string id){
		
		currentID = id;
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

		currentID = loadedData [currentOrder].id;
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

		currentID = loadedData [currentOrder].id;
		return loadedData[currentOrder];

	}

	public void HandleCurrentCorrectObject(){


		loadedData[currentOrder].visited = true;
		string JSONResult = JsonUtility.ToJson (objectsData, true);

		File.WriteAllText (newFilePath, JSONResult);


	}

	public bool CheckCurrentObject(){
		return loadedData [currentOrder].visited;
	}

	public void ResetAll(){

		foreach(ObjectData objectData in loadedData){
			
			objectData.visited = false;

		}
			
		Debug.Log ("File Exists: " + File.Exists (newFilePath));
		if (File.Exists (newFilePath)) {
			File.Delete (newFilePath);
		}
			
	}

}
