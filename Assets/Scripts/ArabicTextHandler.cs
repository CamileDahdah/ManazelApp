using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.UI;
using ArabicSupport;

public class ArabicTextHandler : MonoBehaviour {

	public string correctText = "";
	public Text scoreText, limitText;
	public static ArabicTextHandler instance;
	private AudioSource audioSource;
	public AudioClip correctAnswerClip, wrongAnswerClip, whatIsThisClip1, whatIsThisClip2;

	public Text speechTextUI;

	void Awake(){
		
		if (instance == null) {
			instance = this;
		} else {
			Destroy (this);
		}

		audioSource = GetComponent<AudioSource> ();

		speechTextUI.text = "";
	}

	//compares speech text with correct word and returns highest possible score
	//TODO compare multiple alternative correct textx within the sentence (example of multiple correct words: television and TV)
	public float CompareArabicWords (string speechText){

		//check if speech text is correct
		bool correct = false;
		//score based on how many letters match with the correct word
		float highestScore = 0;

		if (speechText != "") {

			string[] speechwordArray = speechText.Split (' ');

			Debug.Log ("Correct word: " + correctText + "\n Speech text: " + speechText);

			int wordLength = 0;
			float matchingLettersCounter = 0;
			float highestMatchingLettersCounter = 0, wordHighScore = 0;
			string finalText = "";
		

			int correctWordIndex = 0;
			speechText = "";

			//data structure to highlight matching letters
			List<int> highlightIndexes = new List<int>();
			string speechword = "";

			//if the correct word is composed of 2 words (example: Floor Lamp)
			string[] correctWordArray = correctText.Split (' ');

			float[] highScore = new float[correctWordArray.Length];

			//loop through all spoken words
			for (int l = speechwordArray.Length -1; l >= 0; l--) {

				highlightIndexes.Clear ();
				wordHighScore = 0;

				//loop through all correct words
				for (int c = 0; c < correctWordArray.Length; c++) {

					string correctWord = correctWordArray[c];

					correctWordIndex = c;

					speechword = speechwordArray [l];

					wordLength = speechword.Length;

					highestMatchingLettersCounter = 0;

					//foreach (KeyValuePair<string, float> correctWord in correctWords) {
						
					matchingLettersCounter = 0;			

					int I = correctWord.Length - 1;
					int J = speechword.Length - 1;

					int totalSteps = I + J + 1;
					
					int i1, i2, j1, j2;

					//algorithm to compare matching sequences when comparing two words
					for (int i = 1; i <= totalSteps; i++) {

						i1 = Math.Max ((i - J) - 1, 0);

						i2 = Math.Min (i - 1, I);

						j1 = Math.Min (J, (I + J) - (i - 1));

						j2 = Math.Max (J - (i - 1), 0);

						List<int> tempIndexes = new List<int> ();
						    
						matchingLettersCounter = 0;

						for (; i1 <= i2; i1++) {

							if (correctWord [i1] == speechword [j2]) { 

								matchingLettersCounter += Mathf.Min (100f / speechword.Length, 
									100f / (correctWord.Length - (correctWordArray.Length - 1) ) );
								
								//Debug.Log (correctWord [i1]);
								//Debug.Log (speechword [j2]);
								tempIndexes.Add (j2);
		
							}
								
							j2++;
						}

						if (matchingLettersCounter > highestMatchingLettersCounter) { 
							Debug.Log ( matchingLettersCounter);
							highestMatchingLettersCounter = matchingLettersCounter;

							if (matchingLettersCounter > wordHighScore) {
								wordHighScore = matchingLettersCounter;
								highlightIndexes = tempIndexes;
							}

						}
							
					}

					if (highestMatchingLettersCounter > highScore[correctWordIndex]) {
						highScore[correctWordIndex] = highestMatchingLettersCounter;
					}

				}

				//attach and reverse order of arabic letters (unity does not support arabic text)
				finalText = ArabicFixer.Fix(speechword, true, true);

				int size = speechword.Length;

				//change mathcing letters' color
				for ( int i = 0; i < highlightIndexes.Count; i++ ) {
					finalText = finalText.Insert ( (size - (highlightIndexes[i])) , "</color>");
					finalText = finalText.Insert ( (size - (highlightIndexes[i])) - 1, "<color=#02dfa5>");
				}


				Debug.Log ("finalText: " + finalText);

				speechText += " " + finalText;

				Debug.Log (speechText);
					
			}
				
			foreach (float score in highScore) {
				highestScore += score;
			}

			Debug.Log (speechText);
		

			scoreText.text = "Score: " + highestScore;

			speechTextUI.text =  speechText;

		
			Debug.Log ("Current Accuracy: " + GameState.instance.GetAccuracy ());
			Debug.Log ("Score: " + highestScore);

			if ( highestScore >= GameState.instance.GetAccuracy() ) {
				correct = true;
				CorrectAnswer ();

			} else {
				WrongAnswer ();
			}



		} else {
			
			speechTextUI.text =  "";

		}

		ObjectPanel.instance.OnTextResult (correct);
		return highestScore;

	}


	private void ResetState(){
		speechTextUI.text = "";
		//score text is only used for testing purposes for now
		scoreText.text = "Score: 0";
	}

	public void PopupObject(string objectId){

		ObjectData objectData = HandleObjectData.instance.GetObjectData (objectId);

		//if object is already visited 
		if (HandleObjectData.instance.GetObjectData(objectId) != null) {

			UIManager.instance.MovePanelUp (GameState.State.objectPanel);
			HandleNewObject (objectData);


		} else {
			UIManager.instance.MovePanelUp (GameState.State.greenPopupPanel);
		}

	}

	void CorrectAnswer(){
		audioSource.clip = correctAnswerClip;
		audioSource.Play ();
	}

	void WrongAnswer(){
		audioSource.clip = wrongAnswerClip;
		audioSource.Play ();
	}

	IEnumerator ActivateGoogleVoice(){
		//wait while mahaza voice is being played before start recording
		while (audioSource.isPlaying) {
			yield return null;
		}

		ObjectPanel.instance.Speak ();

		if (GameState.CurrentState == GameState.State.objectPanel) {
			GoogleVoiceSpeech.instance.enabled = true;
		}
	}

	public void PopupNextObject(){
		
		ObjectData objectData = HandleObjectData.instance.GetNextObjectData ();
		HandleNewObject (objectData);
	}

	public void PopupPreviousObject(){
		
		ObjectData objectData = HandleObjectData.instance.GetPreviousObjectData ();
		HandleNewObject (objectData);
	
	}
		
	void HandleNewObject(ObjectData objectData){
		
		GoogleVoiceSpeech.instance.enabled = false;
		StopAllCoroutines ();
		ResetState ();
		ObjectPanel.instance.AnimateSequence (objectData.id);
		correctText = objectData.lbArabicWord;
		audioSource.clip = whatIsThisClip1;
		audioSource.Play ();
		StartCoroutine ("ActivateGoogleVoice");

	}

}
