using UnityEngine;
using System.Collections;

public class MainGamePanel : MonoBehaviour {	
	
	
	public UIButton restartBtn;
	public UIButton backBtn;
	
	
	// Use this for initialization	
	void Awake() {
		
		// Add to GUIManager
		GUIManager.Instance.AddGUI( this.gameObject.name, this.gameObject );					
	}
	
	void Start() {	
		
	}		
	
	// Update is called once per frame
	void Update() {	
			
	}
	
	void OnDestroy() {
		
		GUIManager.Instance.RemoveGUI( this.gameObject.name );
	}	
	
	
	
	
	// ui event handler	
	public void OnRestartClick() {		
		
		// send engine command - uci new game		
		ChessEngineManager.Instance.Send( "ucinewgame" );
		ChessEngineManager.Instance.Send( "isready" );
	}
	
	public void OnBackClick() {		
		
		Application.LoadLevel("Init");			
	}
}
