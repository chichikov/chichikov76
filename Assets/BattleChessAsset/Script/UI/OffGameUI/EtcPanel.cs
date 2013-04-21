using UnityEngine;
using System.Collections;

public class EtcPanel : MonoBehaviour {
	
	
	// Use this for first initialization	
	void Awake() {		
		
	}
	
	// Use this for initialization
	void Start () {		
			
	}
	
	// Update is called once per frame
	void Update () {
	
	}
	
	void OnDestroy () {	
		
	}
	
	

	public void OnBackClick() {				
		
		GUIManager.Instance.ShowAllPanel( false );
		GUIManager.Instance.ShowMainPanel( true );
	}	
}
