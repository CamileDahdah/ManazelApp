using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScoreManager : MonoBehaviour {


	int score;
	public static ScoreManager instance;
	int[] maxLevelScore = { 21, 0 , 0 , 0 , 0 };
	int currentLevel;

	//TODO Handle score of different score levels (not only level 1)

	void Awake(){
		
		if (instance == null) {
			instance = this;

		} else {
			Destroy (this);

		}

		score = GetScore ();
		currentLevel = GameState.instance.GetCurrentLevel ();

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
		
		if( score + 1 <= GetMaxLevelScore(currentLevel) ){
			SetScore (score + 1);

		}else{
			Debug.LogError("Something is wrong in the score");
		}

	}

	public int GetMaxLevelScore(int currentLevel){

		return maxLevelScore [currentLevel - 1];

	}

	public int GetMaxLevelScore(){

		return maxLevelScore [currentLevel - 1];

	}

}
