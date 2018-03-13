using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.UI;
using ArabicSupport;

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


	//return percentage
	public void DetectArabicWords (string speechText){

		bool correct = false;

		if (speechText != "") {


			//speechText = ArabicFixer.Fix(speechText, true, true);
			//theWord = ArabicFixer.Fix (theWord, true, true);

			string[] speechwords = speechText.Split (' ');

			Debug.Log ("theWord: " + theWord + "  speechText: " + speechText);

			int wordLength = 0;
			float letterCounter = 0;
			float theLetterCounter = 0;

			string finalText = "";

			List<int> highlightIndexes = new List<int>();

			foreach (string speechword in speechwords) {

				wordLength = speechword.Length;

				int firstIndex = 0, lastIndex = 0;

				int colorLength = 0;

				//foreach (KeyValuePair<string, float> correctWord in correctWords) {
					
					letterCounter = 0;

					bool isFirstIndex;

					int I = theWord.Length - 1;
					int J = speechword.Length - 1;

					int totalSteps = I + J + 1;
				
					int i1, i2, j1, j2;

					for (int i = 1; i <= totalSteps; i++) {

						i1 = Math.Max ( (i - J) - 1, 0 );

						i2 = Math.Min (i - 1, I);

						j1 = Math.Min (J, (I + J) - (i - 1));

						j2 = Math.Max (J - (i - 1), 0);

						isFirstIndex = true;

						List<int> tempIndexes = new List<int>();

						for( ; i1 <= i2; i1++){

							if (theWord [i1] == speechword [j2]) { 

								letterCounter += Mathf.Min (100f / speechword.Length, 100f / theWord.Length);
								tempIndexes.Add (j2);
								Debug.Log ("!!! " + theWord[i1]);
								Debug.Log ("!!! " + speechword[j2]);
							}
							
							j2++;
						}

						if (letterCounter > theLetterCounter) { 
							
							theLetterCounter = letterCounter;

							Debug.Log ("!! highlightIndexes " + highlightIndexes.Count);
							Debug.Log ("!! tempIndexes "+ tempIndexes.Count);

							highlightIndexes = tempIndexes;

							Debug.Log ("highlightIndexes " + highlightIndexes.Count);
							Debug.Log ("tempIndexes "+ tempIndexes.Count);

						}
						

					//word by word
					/*if (speechword == correctWord.Key) {
					
						if (wordCounter < correctWord.Value) {
							wordCounter = correctWord.Value;
						}

					}*/
				}

				Debug.Log (speechText);

				finalText = ArabicFixer.Fix(speechText, true, true);

				int size = speechword.Length;
	
				//change Color
				for ( int i = 0; i < highlightIndexes.Count; i++ ) {
					Debug.Log (highlightIndexes[i]);
					finalText = finalText.Insert ( (size - (highlightIndexes[i])) , "</color>");
					finalText = finalText.Insert ( (size - (highlightIndexes[i])) - 1, "<color=#02dfa5>");
				}

				Debug.Log (finalText);

				speechText = finalText;

			}
			Debug.Log (speechText);

			scoreText.text = "Score: " + theLetterCounter;

			speechTextUI.text =  speechText;

		
			Debug.Log ("Current Accuracy: " + GameState.instance.GetAccuracy ());
			Debug.Log ("Score (LetterCounter): " + theLetterCounter);

			if ( theLetterCounter >= GameState.instance.GetAccuracy() ) {
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

	public void PopupObject(string id){

		//already visited object
		if (ReadJSON.instance.GetObjectData(id) != null) {
			
			theWord = ReadJSON.instance.GetObjectData(id).saudiArabicWord;

			UIManager.instance.MovePanelUp (GameState.State.objectPanel);
			//Debug.Log (id);

			GoogleVoiceSpeech.instance.enabled = false;
			StopAllCoroutines ();
			ResetState ();

			ObjectPanel.instance.AnimateSequence (id);

			audioSource.clip = whatIsThisClip1;
			audioSource.Play ();
			StartCoroutine ("ActivateGoogleVoice");

			

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
		GoogleVoiceSpeech.instance.enabled = false;
		ObjectData objectData = ReadJSON.instance.GetNextObjectData ();
		StopAllCoroutines ();
		ResetState ();
		ObjectPanel.instance.AnimateSequence (objectData.id);
		theWord = objectData.saudiArabicWord;
		audioSource.clip = whatIsThisClip1;
		audioSource.Play ();
		StartCoroutine ("ActivateGoogleVoice");
	}

	public void PopupPreviousObject(){
		GoogleVoiceSpeech.instance.enabled = false;
		ObjectData objectData = ReadJSON.instance.GetPreviousObjectData ();
		StopAllCoroutines ();
		ResetState ();
		ObjectPanel.instance.AnimateSequence (objectData.id);
		theWord = objectData.saudiArabicWord;
		audioSource.clip = whatIsThisClip1;
		audioSource.Play ();
		StartCoroutine ("ActivateGoogleVoice");
	}
}
