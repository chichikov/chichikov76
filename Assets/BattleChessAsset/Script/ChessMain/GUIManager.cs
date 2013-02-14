using UnityEngine;
using System.Collections;

using System.Collections.Generic;

public class GUIManager {
	
	// field
	Dictionary<string, GameObject> mapGUI;
	
		
	
	// property
	
	// singleton
	public static GUIManager Instance { 
		get { 
			return Singleton<GUIManager>.Instance; 
		} 
	}
	
	
	private GUIManager() {
		
		mapGUI = new Dictionary<string, GameObject>();
	}
	
	
	public GameObject GetPanel( string strPanelName ) {
		
		if( mapGUI.ContainsKey(strPanelName) ) {
			
			return mapGUI[strPanelName];
		}
		
		return null;
	}
	
	public bool AddPanel( string strPanel, GameObject panel ) {
		
		if( mapGUI.ContainsKey(strPanel) )			
			return false;
		
		mapGUI.Add(strPanel, panel);			
		return true;
	}
	
	public bool RemovePanel( string strPanel ) {
		
		if( mapGUI.ContainsKey(strPanel) ) {					
			
			mapGUI.Remove(strPanel);			
			return true;
		}	
			
		return false;
	}
	
	
	
	public void ShowPanel( string strPanel, bool bShow ) {
		
		if( mapGUI.ContainsKey(strPanel) ) {
			
			 NGUITools.SetActive( mapGUI[strPanel], bShow ); 
		}
	}
	
	public void ShowAllPanel( bool bShow ) {
		
		foreach( KeyValuePair<string, GameObject> pair in mapGUI ) {
			
			 NGUITools.SetActive( pair.Value, bShow ); 
		}
	}
}
