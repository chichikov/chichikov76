using UnityEngine;
using System.Collections;

public class InitPanel : MonoBehaviour {
	
	public UIButton startBtn;
	public UIButton modeBtn;
	public UIButton optionBtn;
	public UIButton etcBtn;
	public UIButton friendListBtn;
	
	
	// Use this for first initialization	
	void Awake() {		
		
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
		
	}
	
	
	
	

	public void OnStartClick() {		
		
		GameState.Instance.currentState = GameState.eGameState.GamePlay;	
	}
	
	public void OnModeClick() {
		
		GUIManager.Instance.ShowPanel( "OffGameUI", gameObject.name, false );
		GUIManager.Instance.ShowPanel( "OffGameUI", "ModePanel", true );
	}	
	
	public void OnOptionClick() {		
		
		GUIManager.Instance.ShowPanel( "OffGameUI", gameObject.name, false );
		GUIManager.Instance.ShowPanel( "OffGameUI", "OptionPanel", true );
	}
	
	public void OnEtcClick() {		
		
		GUIManager.Instance.ShowPanel( "OffGameUI", gameObject.name, false );
		GUIManager.Instance.ShowPanel( "OffGameUI", "EtcPanel", true );
	}
	
	public void OnFriendListClick() {		
		
		GUIManager.Instance.ShowPanel( "OffGameUI", gameObject.name, false );
		GUIManager.Instance.ShowPanel( "OffGameUI", "FriendListPanel", true );
	}
}
