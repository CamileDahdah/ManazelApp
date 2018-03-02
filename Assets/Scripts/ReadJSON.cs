using System.Collections;
using System.Collections.Generic;
using System.IO;     
using UnityEngine;

public class ReadJSON : MonoBehaviour {

	string fileName = "Objects.json";
	List<ObjectData> loadedData;
	ObjectsData objectsData;
	public static ReadJSON instance;
	public string currentID = "";
	public int currentOrder = 0;
	int length = 0;
	string filePath;

	void Awake () {
		
		if (instance == null) {
			instance = this;
		} else {
			Destroy (this);
		}

		filePath = Path.Combine (Application.streamingAssetsPath + "/JSON", fileName);
		LoadObjectsData ();
	}

	private void LoadObjectsData(){

		if (File.Exists (filePath)) {

			string dataAsJson; 

			if (Application.platform == RuntimePlatform.Android) {
				
				WWW reader = new WWW (filePath);
				while (!reader.isDone) {
				}

				dataAsJson = reader.text;

			} else {
				
				dataAsJson = File.ReadAllText (filePath); 
	
			}

			objectsData = JsonUtility.FromJson<ObjectsData> (dataAsJson);
			loadedData = objectsData.objects;
			length = loadedData.Count;

		}
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

		return loadedData[currentOrder];

	}

	public void HandleCurrentCorrectObject(){

		if (File.Exists (filePath)) {

			loadedData[currentOrder].visited = true;
			string JSONResult = JsonUtility.ToJson (objectsData, true);

			File.WriteAllText (filePath, JSONResult);

		}
			

	}

	public bool CheckCurrentObject(){
		return loadedData [currentOrder].visited;
	}

	public void ResetAll(){

		foreach(ObjectData objectData in loadedData){
			
			objectData.visited = false;

		}

		if (File.Exists (filePath)) {

		
			string JSONResult = JsonUtility.ToJson (objectsData, true);

			File.WriteAllText (filePath, JSONResult);

		}
	}

}
