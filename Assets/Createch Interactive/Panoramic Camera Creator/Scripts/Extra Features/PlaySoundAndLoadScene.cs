using UnityEngine;
using System.Collections;

// This scripts enables you to play a sound, wait for it to finish playing, and then load another scene
// To use it, please drop it on an object then refer a hotspot object to it via SendHotSpotIdInternal
// Specify PlaySoundThenLoadScene as the recieving method

[AddComponentMenu("Createch Interactive/Play sound then load scene")]
public class PlaySoundAndLoadScene : MonoBehaviour 
{
	public AudioClip soundToPlay ;
	public string sceneToLoadName ;

	private AudioClip _soundToPlay ;
	private string _sceneToLoad ;

	public void PlaySoundThenLoadScene()
	{
		if(soundToPlay != null && sceneToLoadName != "")
		{
			_soundToPlay = soundToPlay ;
			_sceneToLoad = sceneToLoadName ;
			
			if(GetComponent<AudioSource>() == null)
			{
				gameObject.AddComponent<AudioSource>() ;
			}
			
			if(GetComponent<AudioSource>().isPlaying == false)
			{
				StartCoroutine("PlayAudioThenChangeScene") ;
			}
			else
			{
				Debug.LogWarning("Already playing sound and loading level") ;
			}
		}
		else
		{
			Debug.LogError("There was no sound to play, or couldn't determine scene to load") ;
		}
	}

	// This is for users who want to programatically play a sound then load a scene
	public void PlaySoundThenLoadSceneAdvanced(AudioClip soundToPlayBefore, string sceneToLoadAfter)
	{
		if(soundToPlayBefore != null && sceneToLoadAfter != "")
		{
			_soundToPlay = soundToPlayBefore ;
			_sceneToLoad = sceneToLoadAfter ;

			if(GetComponent<AudioSource>() == null)
			{
				gameObject.AddComponent<AudioSource>() ;
			}

			if(GetComponent<AudioSource>().isPlaying == false)
			{
				StartCoroutine("PlayAudioThenChangeScene") ;
			}
			else
			{
				Debug.LogWarning("Already playing sound and loading level") ;
			}
		}
		else
		{
			Debug.LogError("There was no sound to play, or couldn't determine scene to load") ;
		}
	}

	private IEnumerator PlayAudioThenChangeScene()
	{
		AudioSource audioSource = GetComponent<AudioSource>() ;
		audioSource.GetComponent<AudioSource>().clip = _soundToPlay ;
		audioSource.Play() ;

		while(audioSource.isPlaying)
		{
			yield return null ;
		}

		Application.LoadLevel(_sceneToLoad) ;
	}

}
