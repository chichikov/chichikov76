using UnityEngine;
using System.Collections;

public class MainGamePanel : MonoBehaviour {	
	
	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}
	
	
	public void OnRestartClick() {		
		
		// send engine command - uci new game		
		ChessEngineManager.Instance.Send( "ucinewgame" );
		ChessEngineManager.Instance.Send( "isready" );
	}
	
	public void OnBackClick() {		
		
		Application.LoadLevel("Init");			
	}
}
