using UnityEngine;
using System.Collections;

public class InitMenu : MonoBehaviour {
	
	public UIButton startBtn;
	public UIButton modeBtn;
	public UIButton optionBtn;
	public UIButton etcBtn;
	
	
	// Use this for first initialization	
	void Awake() {	
		
		// Add to GUIManager
		GUIManager.Instance.AddGUI( this.gameObject.name, this.gameObject );
		
		if( ChessEngineManager.Instance.IsEngineInit ) {
			
			startBtn.isEnabled = true;
			optionBtn.isEnabled = true;
		}
		else {
			
			startBtn.isEnabled = false;
			optionBtn.isEnabled = false;
		}
	}
	
	// Use this for initialization
	void Start() {		
		
	}
	
	// Update is called once per frame
	void Update() {
	
	}
	
	void OnDestroy() {
		
		GUIManager.Instance.RemoveGUI( this.gameObject.name );
	}
	
	
	
	

	public void OnStartClick() {		
		
		// send engine option
		ChessEngineManager.Instance.SendCurrentOption();
		ChessEngineManager.Instance.Send( "isready" );		
	}
	
	public void OnModeClick() {
		
		GUIManager.Instance.ShowPanel( gameObject.name, false );
		GUIManager.Instance.ShowPanel( "ModePanel", true );
	}	
	
	public void OnOptionClick() {		
		
		GUIManager.Instance.ShowPanel( gameObject.name, false );
		GUIManager.Instance.ShowPanel( "OptionPanel", true );
	}
	
	public void OnEtcClick() {		
		
		GUIManager.Instance.ShowPanel( gameObject.name, false );
		GUIManager.Instance.ShowPanel( "EtcPanel", true );
	}
}
