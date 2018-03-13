﻿using UnityEngine;
using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using UnityEngine.Networking;
using SimpleJSON;
using UnityEngine.UI;
using ArabicSupport;

[RequireComponent (typeof(AudioSource))]

public class GoogleVoiceSpeech : MonoBehaviour{

	public GameObject recordingButton;

	float minimumLevel = 10f;
	private ArabicText arabicText;

	struct ClipData
	{
		public int samples;
	}

	private int minFreq;
	private int maxFreq;

	int clipSeconds = 20;

	private bool micConnected = false;

	//A handle to the attached AudioSource
	private AudioSource goAudioSource;

	public string apiKey;
	bool recording = false, uploading = false;
	string apiURL; 

	public static GoogleVoiceSpeech instance;

	void Awake(){
		
		if (instance == null) {
			instance = this;
		} else {
			Destroy (this);
		}

		apiURL = "https://speech.googleapis.com/v1/speech:recognize?key=" + apiKey;


		if (Microphone.devices.Length <= 0) {
			//Throw a warning message at the console if there isn't
			//"Microphone not connected!";
		} else {

			micConnected = true;

			//Get the default microphone recording capabilities
			Microphone.GetDeviceCaps (null, out minFreq, out maxFreq);

			maxFreq = 16000;

			goAudioSource = this.GetComponent<AudioSource> ();
		}
			
		recordingButton.GetComponent<Button> ().onClick.AddListener (delegate() {

			StopRecording();

		});

	}


	void OnEnable(){
		RecordClick ();
	}

	void Start (){
		arabicText = ArabicText.instance;
	}

	//onClick
	public void RecordClick(){
		
		if (micConnected) {
			
			if (!Microphone.IsRecording (null)) {
				if(!recording && !uploading){

					recording = true;
					goAudioSource.clip = Microphone.Start (null, false, clipSeconds, maxFreq);

					recordingButton.GetComponent<Button> ().interactable = true;

					StopCoroutine ("IsSpeaking");
					StartCoroutine ("IsSpeaking");
				}

			} else {
				if (!uploading) {
					string filePath = Record ();
					StartCoroutine ("HttpUploadFile", filePath);
				
				}
			}
		}
	}


	public void StopRecording(){
		
		if (!uploading) {
			string filePath = Record ();
			StartCoroutine ("HttpUploadFile", filePath);

		}
	}


	public IEnumerator HttpUploadFile (string filePath){
				
		recordingButton.GetComponent<Button> ().interactable = false;

		ObjectPanel.instance.Loading ();

		yield return null;

		JSONClass rootNode = new JSONClass ();
		JSONClass configNode = new JSONClass ();
		JSONClass audioNode = new JSONClass ();

		rootNode.Add ("config", configNode);

		configNode.Add ("encoding", new JSONData ("LINEAR16"));
		configNode.Add ("sampleRateHertz", new JSONData (16000));
		configNode.Add ("language_code", new JSONData ("ar-LB"));

		string byteString = Convert.ToBase64String(File.ReadAllBytes (filePath));


		audioNode.Add ("content", new JSONData(byteString));


		rootNode.Add ("audio", audioNode);

		string JSONObject = "{"+"\"config\""+":" +
			"{"+"\"encoding\":\"LINEAR16\""+"," + "\"sampleRateHertz\""+":"+"\"16000\""+","+ "\"language_code\""+":"+"\"ar-LB\""+"}," +
			"\"audio\""+":{"+"\"content\""+":\""+byteString+"\"}"
			+"}";


		UnityWebRequest request = UnityWebRequest.Post (apiURL, "");

		request.uploadHandler = new UploadHandlerRaw (System.Text.Encoding.UTF8.GetBytes (JSONObject));

		request.SetRequestHeader ("Content-Type", "application/json");

		yield return request.SendWebRequest ();

		if (request.isNetworkError || request.isHttpError) {
			Debug.Log ("Error: " + request.responseCode);

		} else {

			String response = request.downloadHandler.text;
			//Debug.Log (response);
			if (response != "") {
				var jsonresponse = JSON.Parse (response);

				Debug.Log ("response string: " + response);

				var resultString = jsonresponse ["results"] [0];

				Debug.Log ("result string: " + resultString.ToString ());

				string transcripts = resultString ["alternatives"] [0] ["transcript"].Value;

				Debug.Log ("transcript string: " + transcripts);

				arabicText.DetectArabicWords (transcripts);
			}
		}
		yield return null;

		File.Delete (filePath); //Delete the Temporary Wav file

		uploading = false;

		yield return null;
	}


	string Record (){
		
		uploading = true;

		float filenameRand = UnityEngine.Random.Range (0.0f, 10.0f);

		string filename = "testing" + filenameRand;

		recording = false;

		/// Trim Audio
		int lastTime = Microphone.GetPosition(null); 

		//Debug.Log("lastTime =" + lastTime);

		Microphone.End(null);

		if (lastTime > 0) {
			float[] samples = new float[goAudioSource.clip.samples]; 

			goAudioSource.clip.GetData (samples, 0);

			float[] ClipSamples = new float[lastTime];

			Array.Copy (samples, ClipSamples, ClipSamples.Length - 1);

			goAudioSource.clip = AudioClip.Create ("playRecordClip", ClipSamples.Length, 1, maxFreq, false, false);
			goAudioSource.clip.SetData (ClipSamples, 0);
		}
		/// New code

		Microphone.End (null); //Stop the audio recording

		Debug.Log ("Recording Stopped");

		if (!filename.ToLower ().EndsWith (".wav")) {
			filename += ".wav";
		}

		var filePath = "testing/" + filename;
		filePath = Application.persistentDataPath + filePath;
		Debug.Log ("Created filepath string: " + filePath);

		// Make sure directory exists if user is saving to sub dir.
		Directory.CreateDirectory (Path.GetDirectoryName (filePath));
		SavWav.Save (filePath, goAudioSource.clip); //Save a temporary Wav File

		Debug.Log ("Uploading " + filePath);

		return filePath;


	}


	float[] clipSampleData = new float[256];

	double speakingCounter;
	int counterThreshold = 10;

	float soundThreshold, soundAverage;
	bool spokeOnce;
	float speakTimer;
	float maxTimerThreshold = 20f, refreshRate = .1f, soundSum;
	int iteration, lastTime;

	IEnumerator IsSpeaking(){

		spokeOnce = false;
		 
		speakingCounter = speakTimer = soundAverage = iteration = lastTime = 0;
		soundThreshold = .048f;

		//Wait till Microphone records
		while (lastTime <= 0) {

			lastTime = Microphone.GetPosition(null); 

			yield return null;

		}

		goAudioSource.Play();

		while (recording) {
			//wait for 20 seconds if no one speaks
			if (speakTimer < maxTimerThreshold) {
				
				speakTimer += refreshRate;

			} else {

				spokeOnce = true;
			}
			//compute final threshhold
			if (iteration == 3) {
				soundThreshold += (soundAverage / iteration) / 2f;
			}

			//compute sound Sum and sound Average
			clipSampleData = new float[256];
			goAudioSource.GetSpectrumData (clipSampleData, 0, FFTWindow.Rectangular);

			soundSum = 0f;

			for (var i = 0; i < clipSampleData.Length; i++) {

				soundSum += clipSampleData [i];

			}

			soundAverage += soundSum;
//			Debug.Log (soundThreshold);
			arabicText.scoreText.text = "" + soundSum;
			arabicText.limitText.text = "" + soundThreshold;

			//if avg sound is computed
			if (iteration >= 3) {
				
				if (soundSum < soundThreshold) {
					if (spokeOnce) {
						speakingCounter--;

					} else {
						speakingCounter -= .5;

						if (speakingCounter < 0) {
							speakingCounter = 0;
						}
					}

				} else {
					if (spokeOnce) {
						speakingCounter = 0;
					} else {
						speakingCounter++;
					}

				}

				if (!spokeOnce) {
					if (speakingCounter >= (counterThreshold / 3f)) {
						spokeOnce = true;
						speakingCounter = 0;
					}
				} else if (speakingCounter < - counterThreshold && !uploading) {
				
					string filePath = Record ();
					StartCoroutine ("HttpUploadFile", filePath);

				}
			}
			iteration++;
			yield return new WaitForSeconds(refreshRate);
		}
			
	
	}

	void OnDisable(){
		StopAllCoroutines ();
		recording = false;
		uploading = false;
		if (recordingButton) {
			recordingButton.SetActive (false);
		}
		Microphone.End(null);
	}
}


		