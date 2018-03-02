﻿using System.Collections;
using System.Collections.Generic;


[System.Serializable]
public class ObjectData {

	public string id;
	public string lbArabicWord;
	public string saudiArabicWord;
	public bool visited;
}

[System.Serializable]
public class ObjectsData {
	public List<ObjectData> objects;
}