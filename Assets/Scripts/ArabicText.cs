﻿using System.Collections;
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


			speechText = ArabicFixer.Fix(speechText, true, true);
			theWord = ArabicFixer.Fix (theWord, true, true);

			string[] speechwords = speechText.Split (' ');

			Debug.Log ("theWord: " + theWord + "  speechText: " + speechText);

			int wordLength = 0;
			float letterCounter = 0;
			float theLetterCounter = 0;

			string finalText = "";

			foreach (string speechword in speechwords) {

				wordLength = speechword.Length;

				int firstIndex = 0, lastIndex = 0;
				int colorFirstIndex = 0, colorLastIndex = 0;
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

						for( ; i1 <= i2; i1++){


							if (theWord [i1] == speechword [j2]) { 

								letterCounter += 100f / speechword.Length;

								if (isFirstIndex) {
									firstIndex = j2;
									isFirstIndex = false;

								} else {
									lastIndex = j2;
								}
							}


							j2++;
						}

						if (lastIndex - firstIndex > colorLength) { //check best sequence
							colorLength = lastIndex - firstIndex;
							colorFirstIndex = firstIndex;
							colorLastIndex = lastIndex;
						}

						if (letterCounter > theLetterCounter) { 
							theLetterCounter = letterCounter;
						}

					//}

					//word by word
					/*if (speechword == correctWord.Key) {
					
						if (wordCounter < correctWord.Value) {
							wordCounter = correctWord.Value;
						}

					}*/
				}

				finalText = speechword;
				//finalText = ArabicFixer.Fix(finalText, true, true);
				//change Color
				if (colorLastIndex > colorFirstIndex) {
					finalText = finalText.Insert (colorLastIndex + 1, "</color>");
					finalText = finalText.Insert (colorFirstIndex, "<color=#02dfa5>");
				}

				speechText = speechText.Replace (speechword, finalText);

			}
			Debug.Log (speechText);

			scoreText.text = "Score: " + theLetterCounter;
			//return theLetterCounter;
			speechTextUI.text =  speechText;

		

			if (theLetterCounter >= 100) {
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
		GoogleVoiceSpeech.instance.enabled = false;
		StopAllCoroutines ();
		ResetState ();
		ObjectPanel.instance.AnimateSequence (id);
		theWord = ReadJSON.instance.GetObjectData(id).lbArabicWord;
		audioSource.clip = whatIsThisClip1;
		audioSource.Play ();
		StartCoroutine ("ActivateGoogleVoice");
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
		theWord = objectData.lbArabicWord;
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
		theWord = objectData.lbArabicWord;
		audioSource.clip = whatIsThisClip1;
		audioSource.Play ();
		StartCoroutine ("ActivateGoogleVoice");
	}
}
