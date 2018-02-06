using UnityEngine;
using System.Collections;

public class DisplayControls : MonoBehaviour
{	
	void OnGUI()
	{
		GUI.Label(new Rect(10, 10, 500, 50), "Panoramic Framework for Unity3D by CreaTech Interactive http://www.createchinteractive.com");	
		GUI.Label(new Rect(10, 50, 500, 50), "Mouse controls : Right click and drag to pan, mouse wheel to zoom, left click to trigger hotspot");	
		GUI.Label(new Rect(10, 90, 500, 50), "Keyboard controls : Arrow keys to pan, + and - keys to zoom");
		GUI.Label(new Rect(10, 120, 500, 50), "Alternatively, use the Gui buttons at the bottom of the screen to navigate");
		GUI.Label(new Rect(10, 150, 500, 50), "In this example, there's a hotspot on the eiffel tower that will play a sound when clicked on");	
	}
}
