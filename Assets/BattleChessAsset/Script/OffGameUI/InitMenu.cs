using UnityEngine;
using System.Collections;

public class InitMenu : MonoBehaviour {
	
	public UIButton startBtn;
	public UIButton modeBtn;
	public UIButton optionBtn;
	public UIButton etcBtn;
	
	// Use this for initialization
	void Start () {
		
		startBtn.isEnabled = false;
		optionBtn.isEnabled = false;
	}
	
	// Update is called once per frame
	void Update () {
	
	}
	
	

	public void OnStartClick() {		
		
		// send engine option
		ChessEngineManager.Instance.SendCurrentOption();
		//Application.LoadLevel("BattleChessInfinity");
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
