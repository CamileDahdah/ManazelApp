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

	void Awake () {
		
		if (instance == null) {
			instance = this;
		} else {
			Destroy (this);
		}

		LoadObjectsData ();
	}

	private void LoadObjectsData(){

		string filePath = Path.Combine (Application.streamingAssetsPath + "/JSON", fileName);

		if (File.Exists (filePath)) {

			string dataAsJson = File.ReadAllText (filePath); 
	
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
				currentOrder = iteration;
				return objectData;
			}
			iteration++;
		}
		return null;
	}

	public ObjectData GetNextObjectData(){

		if (currentOrder + 1 >= length) {
			currentOrder = 0; 
		} else {
			currentOrder++;
		}

		return loadedData[currentOrder];

	}

	public ObjectData GetPreviousObjectData(){

		if (currentOrder - 1 < 0) {
			currentOrder = length - 1; 
		} else {
			currentOrder--;
		}

		return loadedData[currentOrder];

	}


}
