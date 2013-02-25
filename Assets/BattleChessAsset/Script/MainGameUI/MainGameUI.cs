using UnityEngine;
using System.Collections;

public class MainGameUI : MonoBehaviour {

	// Use this for initialization
	void Start () {
		
		// retrieve tagged(panel) GameObject and Add GUIManager
		GameObject [] aPanel = GameObject.FindGameObjectsWithTag( "Panel" );
		foreach( GameObject panel in aPanel ) {
			
			GUIManager.Instance.AddPanel( panel.name, panel );
		}
		
		// first init menu select
		GUIManager.Instance.ShowAllPanel(false);
		GUIManager.Instance.ShowPanel( "MainGamePanel", true );	
	}
	
	// Update is called once per frame
	void Update () {
	
	}
	
	void OnDestroy () {
		
		// retrieve tagged(panel) GameObject and Remove GUIManager
		GameObject [] aPanel = GameObject.FindGameObjectsWithTag( "Panel" );
		foreach( GameObject panel in aPanel ) {
			
			GUIManager.Instance.RemovePanel( panel.name );
		}
	}
}
