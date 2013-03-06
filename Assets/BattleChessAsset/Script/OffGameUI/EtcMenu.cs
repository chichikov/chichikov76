using UnityEngine;
using System.Collections;

public class EtcMenu : MonoBehaviour {
	
	
	// Use this for first initialization	
	void Awake() {	
		
		// Add to GUIManager
		GUIManager.Instance.AddGUI( this.gameObject.name, this.gameObject );
	}
	
	// Use this for initialization
	void Start () {		
			
	}
	
	// Update is called once per frame
	void Update () {
	
	}
	
	void OnDestroy () {
		
		GUIManager.Instance.RemoveGUI( this.gameObject.name );
	}
	
	

	public void OnBackClick() {		
		
		GUIManager.Instance.ShowPanel( gameObject.name, false );
		GUIManager.Instance.ShowPanel( "InitPanel", true );
	}	
}
