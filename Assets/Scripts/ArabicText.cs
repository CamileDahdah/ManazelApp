using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.UI;
using ArabicSupport;

public class ArabicText : MonoBehaviour {

	public Dictionary<string, float> wordsTest = new Dictionary<string, float>()
	{
		
		{"كرسي", 100}
		,{"كرسة", 100},		
		{"طاولة", 75},		
				{"صحن",50}			
	};

	public string theWord = "صحن";
	public Text scoreText;
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
	public void DetectArabicWords (string speechText, Dictionary<string, float> correctWords){

		if (speechText != "") {


			speechText = ArabicFixer.Fix(speechText, true, true);
				
			string[] speechwords = speechText.Split (' ');

			float wordCounter = 0;
			int wordLength = 0;
			float letterCounter = 0;
			float theLetterCounter = 0;

			string finalText = "";

			foreach (string speechword in speechwords) {

				wordLength = speechword.Length;

				int firstIndex = 0, lastIndex = 0;
				int colorFirstIndex = 0, colorLastIndex = 0;
				int colorLength = 0;

				foreach (KeyValuePair<string, float> correctWord in correctWords) {
					
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

					}

					//word by word
					if (speechword == correctWord.Key) {
					
						if (wordCounter < correctWord.Value) {
							wordCounter = correctWord.Value;
						}

					}
				}

				finalText = speechword;
				//finalText = ArabicFixer.Fix(finalText, true, true);
				//change Color
				if (colorLastIndex > colorFirstIndex) {
					finalText = finalText.Insert (colorLastIndex + 1, "</color>");
					finalText = finalText.Insert (colorFirstIndex, "<color=#ffffff>");
				}

				speechText = speechText.Replace (speechword, finalText);

			}
			Debug.Log (speechText);

			scoreText.text = "Score: " + theLetterCounter;
			//return theLetterCounter;
			speechTextUI.text =  speechText;

			if (theLetterCounter >= 100) {
				CorrectAnswer ();
			} else {
				WrongAnswer ();
			}

		} else {
			speechTextUI.text =  "";
		}
	}

	private void ResetState(){
		speechTextUI.text = "";
		GoogleVoiceSpeech.instance.textButton.text = "Start Recording";
		scoreText.text = "Score: 0";
	}

	public void PopupObject(string speechText){
		ResetState ();
		theWord = speechText;
		audioSource.clip = whatIsThisClip1;
		audioSource.Play ();
	}

	void CorrectAnswer(){
		audioSource.clip = correctAnswerClip;
		audioSource.Play ();
		AnimateSpriteManager.instance.XButtonClick ();
	}

	void WrongAnswer(){
		audioSource.clip = wrongAnswerClip;
		audioSource.Play ();
	}

}
