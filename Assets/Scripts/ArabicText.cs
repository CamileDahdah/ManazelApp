using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.UI;
using ArabicSupport;
using System.Collections.Generic;

public class ArabicText : MonoBehaviour {

	public string theWord = "";
	public Text scoreText, limitText;
	public static ArabicText instance;
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

		theWord = ArabicFixer.Fix (theWord, true, true);

		speechTextUI.text = "";
	}



	public void DetectArabicWords (string speechText){

		bool correct = false;

		if (speechText != "") {

			string[] speechwords = speechText.Split (' ');

			Debug.Log ("theWord: " + theWord + "  speechText: " + speechText);

			int wordLength = 0;
			float letterCounter = 0;
			float theLetterCounter = 0, tempHighScore = 0;
			float highestScore = 0;
			string finalText = "";
		

			int correctWordIndex = 0;
			speechText = "";

			List<int> highlightIndexes = new List<int>();
			Stack<String> finalResult = new Stack<String> ();
			string speechword = "";

			string[] correctWords = theWord.Split (' ');

			float[] highScore = new float[correctWords.Length];

			for (int l = speechwords.Length -1; l >= 0; l--) {

				highlightIndexes.Clear ();
				tempHighScore = 0;

				for (int c = 0; c < correctWords.Length; c++) {

					string correctWord = correctWords[c];

					correctWordIndex = c;

					speechword = speechwords [l];

					wordLength = speechword.Length;

					theLetterCounter = 0;

					//foreach (KeyValuePair<string, float> correctWord in correctWords) {
						
					letterCounter = 0;			

					int I = correctWord.Length - 1;
					int J = speechword.Length - 1;

					int totalSteps = I + J + 1;
					
					int i1, i2, j1, j2;

					for (int i = 1; i <= totalSteps; i++) {

						i1 = Math.Max ((i - J) - 1, 0);

						i2 = Math.Min (i - 1, I);

						j1 = Math.Min (J, (I + J) - (i - 1));

						j2 = Math.Max (J - (i - 1), 0);

						List<int> tempIndexes = new List<int> ();
						    
						letterCounter = 0;

						for (; i1 <= i2; i1++) {

							if (correctWord [i1] == speechword [j2]) { 

								letterCounter += Mathf.Min (100f / speechword.Length, 100f / (theWord.Length - (correctWords.Length - 1) ) );
								Debug.Log (correctWord [i1]);
								Debug.Log ( speechword [j2]);
								tempIndexes.Add (j2);
		
							}
								
							j2++;
						}

						if (letterCounter > theLetterCounter) { 
							Debug.Log ( letterCounter);
							theLetterCounter = letterCounter;
							if (letterCounter > tempHighScore) {
								tempHighScore = letterCounter;
								highlightIndexes = tempIndexes;
							}

						}
							
					}

					if (highScore[correctWordIndex] < theLetterCounter) {
						highScore[correctWordIndex] = theLetterCounter;
					}

				}


				finalText = ArabicFixer.Fix(speechword, true, true);

				int size = speechword.Length;

				//change Color
				for ( int i = 0; i < highlightIndexes.Count; i++ ) {
					finalText = finalText.Insert ( (size - (highlightIndexes[i])) , "</color>");
					finalText = finalText.Insert ( (size - (highlightIndexes[i])) - 1, "<color=#02dfa5>");
				}


				Debug.Log ("finalText: " + finalText);

				speechText += " " + finalText;

				Debug.Log (speechText);
				//finalResult.Push (" " + finalText);

				
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
	}

	private void ResetState(){
		speechTextUI.text = "";
		scoreText.text = "Score: 0";
	}

	public void PopupObject(string objectId){

		ObjectData objectData = ReadJSON.instance.GetObjectData (objectId);

		//already visited object
		if (ReadJSON.instance.GetObjectData(objectId) != null) {

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
		
		while (audioSource.isPlaying) {
			yield return null;
		}

		ObjectPanel.instance.Speak ();

		if (GameState.CurrentState == GameState.State.objectPanel) {
			GoogleVoiceSpeech.instance.enabled = true;
		}
	}

	public void PopupNextObject(){
		
		ObjectData objectData = ReadJSON.instance.GetNextObjectData ();
		HandleNewObject (objectData);
	}

	public void PopupPreviousObject(){
		
		ObjectData objectData = ReadJSON.instance.GetPreviousObjectData ();
		HandleNewObject (objectData);
	
	}



	void HandleNewObject(ObjectData objectData){
		
		GoogleVoiceSpeech.instance.enabled = false;
		StopAllCoroutines ();
		ResetState ();
		ObjectPanel.instance.AnimateSequence (objectData.id);
		theWord = objectData.lbArabicWord;
		audioSource.clip = whatIsThisClip1;
		audioSource.Play ();
		StartCoroutine ("ActivateGoogleVoice");

	}

}
