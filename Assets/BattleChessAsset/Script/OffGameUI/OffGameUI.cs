using UnityEngine;
using System.Collections;

public class OffGameUI : MonoBehaviour {

	// Use this for initialization
	void Start () {
		
		// retrieve tagged(panel) GameObject and Add GUIManager
		GameObject [] aPanel = GameObject.FindGameObjectsWithTag( "Panel" );
		foreach( GameObject panel in aPanel ) {
			
			GUIManager.Instance.AddPanel( panel.name, panel );
		}
		
		// first init menu select
		GUIManager.Instance.ShowAllPanel(false);
		GUIManager.Instance.ShowPanel( "InitPanel", true );
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
