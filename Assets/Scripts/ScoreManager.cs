using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScoreManager : MonoBehaviour {


	int score;
	public static ScoreManager instance;
	int maxScore = 21;

	void Awake(){
		
		if (instance == null) {
			instance = this;

		} else {
			Destroy (this);

		}

		score = GetScore ();

	}


	public void SetScore(int newScore){
		score = newScore;
		PlayerPrefs.SetInt ("scoreLevel1", newScore);
		PlayerPrefs.Save ();
	}


	public int GetScore(){

		return PlayerPrefs.GetInt ("scoreLevel1");

	}


	public void IncrementScore(){
		
		if(score + 1 <= maxScore){
			SetScore (score + 1);

		}else{
			Debug.Log("Something is wrong in the score");
		}

	}

}
