using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ArabicSupport;

public class SelectedObject : MonoBehaviour {

	public string id;
	public string arabicText;


	void Awake () {
		//arabicText = ArabicFixer.Fix (ReadJSON.instance.GetLebaneseWord (id));
	}
	

}
