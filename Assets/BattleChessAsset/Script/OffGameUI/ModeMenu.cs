using UnityEngine;
using System.Collections;

public class ModeMenu : MonoBehaviour {

	public void OnAloneActivate() {			
		
		
	}
	
	public void OnTogetherActivate() {		
		
		
	}
	
	public void OnBackClick() {
		
		
		GUIManager.Instance.ShowPanel( gameObject.name, false );
		GUIManager.Instance.ShowPanel( "InitPanel", true );
	}	
}
