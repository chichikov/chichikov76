using UnityEngine;
using System.Collections;

using System.Collections.Generic;

using UnityExtention;


[ExecuteInEditMode]
[AddComponentMenu("NGUI/UI/House")]
public class UIHouse : MonoBehaviour {
	
	// field
	public string nameID;	
	public bool is3DUI;
	
	public UIPanel mainPanel;	
	
	Dictionary<string, UIPanel> mapPanel;	
	
			
	
	// MonoBehaviour
	// Use this for first initialization	
	void Awake() {	
		
		mapPanel = new Dictionary<string, UIPanel>();
		
		// populate Gui Panel!!!
		UIPanel [] aPanel = gameObject.GetChildrenComponent<UIPanel>();		
		if( aPanel != null ) {			
			
			foreach( UIPanel currPanel in aPanel ) {
				
				NGUITools.SetActive( currPanel.gameObject, false ); 				
				mapPanel.Add( currPanel.name, currPanel );	
			}
		}
	}
	
	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}		
	
	
	
	
	
	// panel management
	public UIPanel GetPanel( string strName ) {
		
		if( mapPanel.ContainsKey(strName) ) {
			
			return mapPanel[strName];
		}
		
		return null;
	}	
	
	public bool AddPanel( string strName, UIPanel uiPanel ) {
		
		if( mapPanel.ContainsKey(strName) )			
			return false;		
		
		mapPanel.Add(strName, uiPanel);			
		return true;
	}
	
	public bool RemovePanel( string strName ) {
		
		if( mapPanel.ContainsKey(strName) ) {					
			
			Object.Destroy( mapPanel[strName].gameObject );
			mapPanel.Remove(strName);			
			return true;
		}	
			
		return false;
	}	
	
	public void RemoveAllPanel() {
		
		foreach( KeyValuePair<string, UIPanel> pair in mapPanel )			
			Object.Destroy( pair.Value.gameObject );								
		
		mapPanel.Clear();
	}	
	
	
	
	
	public void ShowPanel( string strPanel, bool bShow ) {		
		
		if( mapPanel.ContainsKey( strPanel ) ) {			
			
			NGUITools.SetActive( mapPanel[strPanel].gameObject, bShow );	
			
			// child panel show - for unity 4.0's new active scheme
			UIPanel [] aPanel = mapPanel[strPanel].gameObject.GetChildrenComponent<UIPanel>();			
			if( aPanel != null ) {
				foreach( UIPanel childPanel in aPanel )			
					NGUITools.SetActive( childPanel.gameObject, bShow ); 							
			}
		}
	}
	
	public void ShowMainPanel( bool bShow ) {		
		
		if( mainPanel != null ) {			
			
			NGUITools.SetActive( mainPanel.gameObject, bShow );	
			
			// child panel show - for unity 4.0's new active scheme
			UIPanel [] aPanel = mainPanel.gameObject.GetChildrenComponent<UIPanel>();			
			if( aPanel != null ) {
				foreach( UIPanel childPanel in aPanel )			
					NGUITools.SetActive( childPanel.gameObject, bShow ); 							
			}
		}
	}
	
	public void ShowAllPanel( bool bShow ) {
		
		foreach( KeyValuePair<string, UIPanel> pair in mapPanel )			
			NGUITools.SetActive( pair.Value.gameObject, bShow ); 					
	}	
	
}