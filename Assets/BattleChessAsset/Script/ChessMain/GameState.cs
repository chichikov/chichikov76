using UnityEngine;
using System.Collections;


using System.Collections.Generic;



public class GameState : StateMachineBehaviourEx {
	
	// field
	public enum eGameState {
		
		Intro,
		Init,
		GamePlay		
	}
	
		
		
	
	// property
		
	
	
	void Awake() {	
		
		// enroll game object singleton
		SingletonGameObject<GameState>.AddSingleton( this ); 
	}
	
	void Start() {				
		
	}	
	
	void Update() {			
		
	}	
	
	
	
	// game state
	
	// Intro state
	IEnumerator Intro_EnterState() {		
		
		// temporary
		currentState = eGameState.Init;
		yield return null;
	}
	
	void Intro_Update() {
		
	}
	
	void Intro_ExitState() {
		
	}
	
	
	// Init state
	IEnumerator Init_EnterState() {		
		
		yield return null;
	}
	
	void Init_Update() {
		
	}
	
	void Init_ExitState() {
		
	}
	
	
	// Init state
	IEnumerator GamePlay_EnterState() {		
		
		yield return null;
	}
	
	void GamePlay_Update() {
		
	}
	
	void GamePlay_ExitState() {
		
	}
	
	
	
	
	// static
	public static GameState Instance {
		
		get { 
			return SingletonGameObject<GameState>.Instance; 
		} 
	}
}
