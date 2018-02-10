using UnityEngine;
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

	public Text textButton;

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

	}

	void Start (){

		arabicText = ArabicText.instance;

		apiURL = "https://speech.googleapis.com/v1/speech:recognize?key=" + apiKey;


		if (Microphone.devices.Length <= 0) {
			//Throw a warning message at the console if there isn't
			textButton.text = "Microphone not connected!";
		} else {

			micConnected = true;

			//Get the default microphone recording capabilities
			Microphone.GetDeviceCaps (null, out minFreq, out maxFreq);

			maxFreq = 16000;

			goAudioSource = this.GetComponent<AudioSource> ();
		}
	}

	//onClick
	public void RecordClick(){
		
		if (micConnected) {
			
			if (!Microphone.IsRecording (null)) {
				if(!recording && !uploading){
					textButton.text = "Recording...";
					recording = true;
					goAudioSource.clip = Microphone.Start (null, false, clipSeconds, maxFreq);

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


	void Update(){
		
		if (micConnected) {

			if (!Microphone.IsRecording (null) && recording) {
				if (!uploading) {
					string filePath = Record ();
					StartCoroutine ("HttpUploadFile", filePath);
				}
			}
		}


	}

	public IEnumerator HttpUploadFile (string filePath){
				
		textButton.text = "Uploading ...";

		yield return null;

		JSONClass rootNode = new JSONClass ();
		JSONClass configNode = new JSONClass ();
		JSONClass audioNode = new JSONClass ();

		rootNode.Add ("config", configNode);

		configNode.Add ("encoding", new JSONData ("LINEAR16"));
		configNode.Add ("sampleRateHertz", new JSONData (16000));
		configNode.Add ("language_code", new JSONData ("ar-LB"));

		string byteString = Convert.ToBase64String(File.ReadAllBytes (filePath));
		//Debug.Log (byteString);



		audioNode.Add ("content", new JSONData(byteString));


		rootNode.Add ("audio", audioNode);

		string jsson = "{"+"\"config\""+":" +
			"{"+"\"encoding\":\"LINEAR16\""+"," + "\"sampleRateHertz\""+":"+"\"16000\""+","+ "\"language_code\""+":"+"\"ar-LB\""+"}," +
			"\"audio\""+":{"+"\"content\""+":\""+byteString+"\"}"
			+"}";

		Debug.Log (jsson);
		//Debug.Log (rootNode.ToString ());

		UnityWebRequest request = UnityWebRequest.Post (apiURL, "");

		request.uploadHandler = new UploadHandlerRaw (System.Text.Encoding.UTF8.GetBytes (jsson));

		request.SetRequestHeader ("Content-Type", "application/json");

		yield return request.SendWebRequest ();

		if (request.isNetworkError || request.isHttpError) {
			Debug.Log ("Error: " + request.responseCode);

		} else {

			String response = request.downloadHandler.text;
			//Debug.Log (response);
			if (response != "") {
				var jsonresponse = JSON.Parse (response);

				var resultString = jsonresponse ["results"] [0];

				Debug.Log ("transcript string: " + resultString.ToString ());

				string transcripts = resultString ["alternatives"] [0] ["transcript"].Value;

				Debug.Log ("transcript string: " + transcripts);

				arabicText.DetectArabicWords (transcripts, arabicText.wordsTest);
			}
		}
		yield return null;

		File.Delete (filePath); //Delete the Temporary Wav file
		textButton.text = "Start Recording";
		uploading = false;

		yield return null;
	}


	string Record (){
		
		uploading = true;

		//textButton.text = "Uploading...";

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

	int counter = 0;
	int counterThreshold = 10;

	float soundThreshold = .05f;

	IEnumerator IsSpeaking(){
		
		counter = 0;
		int lastTime = 0;
		//Wait till Microphone records
		while (lastTime <= 0) {

			lastTime = Microphone.GetPosition(null); 

			yield return null;

		}

		goAudioSource.Play();

		while (recording) {

			clipSampleData = new float[256];
			goAudioSource.GetSpectrumData (clipSampleData, 0, FFTWindow.Rectangular);

			float sum = 0f;

			for (var i = 0; i < clipSampleData.Length; i++) {

				sum += clipSampleData [i];

			}
			arabicText.scoreText.text = "" + sum;
			if (sum < soundThreshold) {
				counter++;

			} else {
				counter = 0;
			}

			if (counter > counterThreshold) {
				if (!uploading) {
					string filePath = Record ();
					StartCoroutine ("HttpUploadFile", filePath);
				}
			}

			yield return new WaitForSeconds(.1f);

		}
	}

}
		