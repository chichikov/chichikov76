using UnityEngine;
using System.Collections;

public class OptionPanel : MonoBehaviour {
	
	
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
		
		GUIManager.Instance.ShowPanel( "OffGameUI", gameObject.name, false );
		GUIManager.Instance.ShowPanel( "OffGameUI", "InitPanel", true );
	}	
}
