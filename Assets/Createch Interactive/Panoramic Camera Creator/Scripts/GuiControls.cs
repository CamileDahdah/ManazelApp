using UnityEngine;
using System.Collections;

public class GuiControls : MonoBehaviour 
{
	private PanoControls panoControls ;
	
	public GUISkin guiSkinToUse ;
	
	public string panLeftLabel = "Pan Left";
	public Rect panLeftRect = new Rect(30, Screen.height -60,100,60) ;
	public string panRightLabel = "Pan Right";
	public Rect panRightRect = new Rect(130, Screen.height -60,100,60) ;
	public string panUpLabel = "Pan Up";
	public Rect panUpRect = new Rect(230, Screen.height -60,100,60);
	public string panDownLabel = "Pan Down";
	public Rect panDownRect = new Rect(330, Screen.height -60,100,60);
	public string zoomInLabel = "Zoom In";
	public Rect zoomInRect = new Rect(430, Screen.height -60,100,60);
	public string zoomOutLabel = "Zoom Out";
	public Rect zoomOutRect = new Rect(530, Screen.height -60,100,60);
	
	void Start()
	{
		panoControls = GetComponent<PanoControls>() ;
		
		if(panoControls == null)
		{
			Debug.LogError("Couldn't find PanoControls, Please place the GuiControls script on the same object the PanoControls script is attached to") ;	
		}
		
		if(guiSkinToUse == null)
		{
			Debug.LogWarning("A guiskin hasnt been assigned to the GuiControls script, defaulting to Unity's default skin") ;
		}
		
		// Prevents the user from clicking through the gui element
		panoControls.useUnityGuiBasedControls = true ;
		
		// Adds all the Gui Element Rects to a list, for checking if a touch or click is over a GUI element
		panoControls.guiButtonRects.Add(panLeftRect) ;
		panoControls.guiButtonRects.Add(panRightRect) ;
		panoControls.guiButtonRects.Add(panUpRect) ;
		panoControls.guiButtonRects.Add(panDownRect) ;
		panoControls.guiButtonRects.Add(zoomInRect) ;
		panoControls.guiButtonRects.Add(zoomOutRect) ;
	}
	
	void OnGUI()
	{
		// A GUI skin must be assigned first
		if(guiSkinToUse != null)
		{
			if(GUI.RepeatButton(panLeftRect, panLeftLabel, guiSkinToUse.GetStyle("PanLeft")))
			{
				panoControls.Pan(PanoControls.Originator.GuibuttonLeft) ;
			}
			
			if(GUI.RepeatButton(panRightRect, panRightLabel, guiSkinToUse.GetStyle("PanRight")))
			{
				panoControls.Pan(PanoControls.Originator.GuibuttonRight) ;
			}
			
			if(GUI.RepeatButton(panUpRect, panUpLabel, guiSkinToUse.GetStyle("PanUp")))
			{
				panoControls.Pan(PanoControls.Originator.GuibuttonUp) ;
			}
			
			if(GUI.RepeatButton(panDownRect, panDownLabel, guiSkinToUse.GetStyle("PanDown")))
			{
				panoControls.Pan(PanoControls.Originator.GuibuttonDown) ;
			}
			
			if(GUI.RepeatButton(zoomInRect, zoomInLabel, guiSkinToUse.GetStyle("ZoomIn")))
			{
				panoControls.ZoomIn(PanoControls.Originator.GuibuttonZoomIn) ;
			}
			
			if(GUI.RepeatButton(zoomOutRect, zoomOutLabel, guiSkinToUse.GetStyle("ZoomOut")))
			{
				panoControls.ZoomOut(PanoControls.Originator.GuibuttonZoomOut) ;
			}
		}
		else
		{
			if(GUI.RepeatButton(panLeftRect, panLeftLabel))
			{
				panoControls.Pan(PanoControls.Originator.GuibuttonLeft) ;
			}
			
			if(GUI.RepeatButton(panRightRect, panRightLabel))
			{
				panoControls.Pan(PanoControls.Originator.GuibuttonRight) ;
			}
			
			if(GUI.RepeatButton(panUpRect, panUpLabel))
			{
				panoControls.Pan(PanoControls.Originator.GuibuttonUp) ;
			}
			
			if(GUI.RepeatButton(panDownRect, panDownLabel))
			{
				panoControls.Pan(PanoControls.Originator.GuibuttonDown) ;
			}
			
			if(GUI.RepeatButton(zoomInRect, zoomInLabel))
			{
				panoControls.ZoomIn(PanoControls.Originator.GuibuttonZoomIn) ;
			}
			
			if(GUI.RepeatButton(zoomOutRect, zoomOutLabel))
			{
				panoControls.ZoomOut(PanoControls.Originator.GuibuttonZoomOut) ;
			}
		}
	}
}
