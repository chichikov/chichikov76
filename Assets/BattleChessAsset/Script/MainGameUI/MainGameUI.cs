using UnityEngine;
using System.Collections;

public class MainGameUI : MonoBehaviour {
	
	public MainGamePanel mainGamePanelScript;
	
	// Use this for first initialization	
	void Awake() {		
		
		// first init menu select
		GUIManager.Instance.ShowAllPanel(false);
		GUIManager.Instance.ShowPanel( "MainGamePanel", true );			
	}
	
	// Use this for initialization
	void Start() {			
					
	}	
	
	void OnDisable() {
	}
	
	// Update is called once per frame
	void Update() {
	
	}
	
	void OnDestroy() {		
		
	}
}
