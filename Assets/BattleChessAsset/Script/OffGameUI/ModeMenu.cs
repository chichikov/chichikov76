using UnityEngine;
using System.Collections;

public class ModeMenu : MonoBehaviour {
	
	
	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}
	
	

	public void OnAloneActivate() {			
		
		
	}
	
	public void OnTogetherActivate() {		
		
		
	}
	
	public void OnBackClick() {
		
		
		GUIManager.Instance.ShowPanel( gameObject.name, false );
		GUIManager.Instance.ShowPanel( "InitPanel", true );
	}	
}
