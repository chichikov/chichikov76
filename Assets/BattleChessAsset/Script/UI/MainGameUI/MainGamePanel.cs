using UnityEngine;
using System.Collections;

public class MainGamePanel : MonoBehaviour {	
	
	
	public UIButton restartBtn;
	public UIButton backBtn;
	
	
	// Use this for initialization	
	void Awake() {	
							
	}
	
	void Start() {	
		
	}		
	
	// Update is called once per frame
	void Update() {	
			
	}
	
	void OnDestroy() {		
	}	
	
	
	
	
	// ui event handler	
	public void OnRestartClick() {				
		
		GameState.Instance.Call( GameState.eGameState.Restart );
	}
	
	public void OnBackClick() {		
		
		ChessEngineManager.Instance.SendStop();
		GameState.Instance.currentState = GameState.eGameState.Init;				
	}
}
