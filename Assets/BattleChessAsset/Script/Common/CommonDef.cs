using UnityEngine;
using System.Collections;



public class CommonZDepth {
	
	static float fScrollPanelZDepth = -1.0f;	
	static float fModalDialogZDepth = -1.5f;
	static float fScreenFaderZDepth = -1000.0f;
	
	public static float ScrollPanelF { get { return fScrollPanelZDepth; } private set {} }
	public static int ScrollPanelI { get { return (int)fScrollPanelZDepth; } private set {} }	
	
	public static float ModalDialogF { get { return fModalDialogZDepth; } private set {} }
	public static int ModalDialogI { get { return (int)fModalDialogZDepth; } private set {} }
	
	public static float ScreenFaderF { get { return fScreenFaderZDepth; } private set {} }
	public static int ScreenFaderI { get { return (int)fScreenFaderZDepth; } private set {} }
}
