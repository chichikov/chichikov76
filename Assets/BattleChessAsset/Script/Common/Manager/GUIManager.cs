using UnityEngine;
using System.Collections;


using System.Collections.Generic;



public class GUIManager : MonoBehaviour {
	
	// field
	Dictionary<string, GameObject> mapGUI;
	
		
	
	// property
		
	
	
	void Awake() {		
		
		mapGUI = new Dictionary<string, GameObject>();
		// enroll game object singleton
		SingletonGameObject<GUIManager>.AddSingleton( this ); 
	}
	
	void Start() {				
		
	}	
	
	void Update() {			
		
	}	
	
	
	
	
	public GameObject GetGUI( string strName ) {
		
		if( mapGUI.ContainsKey(strName) ) {
			
			return mapGUI[strName];
		}
		
		return null;
	}	
	
	public bool AddGUI( string strName, GameObject gameObj ) {
		
		if( mapGUI.ContainsKey(strName) )			
			return false;		
		
		// do not unload object at new scene loading	
		Object.DontDestroyOnLoad( gameObj );
		
		
		mapGUI.Add(strName, gameObj);			
		return true;
	}
	
	public bool RemoveGUI( string strName ) {
		
		if( mapGUI.ContainsKey(strName) ) {					
			
			Object.Destroy( mapGUI[strName] );
			mapGUI.Remove(strName);			
			return true;
		}	
			
		return false;
	}	
	
	
	
	
	
	
	public void ShowPanel( string strPanel, bool bShow ) {		
		
		if( mapGUI.ContainsKey( strPanel ) ) {
			
			 NGUITools.SetActive( mapGUI[strPanel], bShow ); 
		}
	}
	
	public void ShowAllPanel( bool bShow ) {
		
		foreach( KeyValuePair<string, GameObject> pair in mapGUI ) {
			
			 NGUITools.SetActive( pair.Value, bShow ); 
		}
	}	
	
	
	// static
	public static GUIManager Instance {
		
		get { 
			return SingletonGameObject<GUIManager>.Instance; 
		} 
	}
}
