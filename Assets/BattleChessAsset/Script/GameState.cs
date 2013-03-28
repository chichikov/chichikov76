using UnityEngine;
using System.Collections;


using System.Collections.Generic;



public class GameState : StateMachineBehaviourEx {
	
	// field
	public enum eGameState {
		
		Intro,
		Init,
		GamePlay,
		Restart
	}
	
		
		
	
	// property
		
	
	
	protected override void OnAwake() {	
		
		// enroll game object singleton
		SingletonGameObject<GameState>.AddSingleton( this );		
	}	
	
	
	// game state
	
	// Intro state
	IEnumerator Intro_EnterState() {		
		
		GUIManager.Instance.ShowAllPanel( "IntroUI", false );
		GUIManager.Instance.ShowPanel( "IntroUI", "IntroPanel", true );		
		
		yield return ScreenFader.Instance.StartFadeAndWait( true, 2.0f, null, null, true );	
		yield return ScreenFader.Instance.StartFadeAndWait( false, 2.0f, null, null, true );		
		
		currentState = GameState.eGameState.Init;
	}
	
	void Intro_Update() {
		
	}
	
	void Intro_ExitState() {				
			
	}
	
	
	// Init state
	IEnumerator Init_EnterState() {			
		
		// remove previouse ui house
		if( (eGameState)lastState == eGameState.Intro )
			GUIManager.Instance.RemoveGUI( "IntroUI" );	
		else if( (eGameState)lastState == eGameState.GamePlay ) {			
			
			GUIManager.Instance.RemoveGUI( "MainGameUI" );	
			yield return ScreenFader.Instance.StartFadeAndWait( false, 2.0f, null, null, true );	
		}
		
		// async Scene load		
		yield return SceneManager.Instance.AsyncLoadLevel( "Init" );	
		
		// show ui
		GUIManager.Instance.ShowAllPanel( "OffGameUI", false );
		GUIManager.Instance.ShowPanel( "OffGameUI", "InitPanel", true );								
		
		// chess engine start	
		if( ChessEngineManager.Instance.IsEngineRunning == false )
			ChessEngineManager.Instance.Start();
		
		ScreenFader.Instance.StartFade( true, 2.0f );		
	}
	
	void Init_Update() {
		
	}
	
	void Init_ExitState() {	
			
	}
	
	
	// GamePlay state
	IEnumerator GamePlay_EnterState() {		
		
		yield return StartCoroutine( ScreenFader.Instance.WaitingForFadeEnd() );
		yield return ScreenFader.Instance.StartFadeAndWait( false, 2.0f, null, null, true );
		
		GUIManager.Instance.RemoveGUI( "OffGameUI" );	
		
		// async Scene load
		yield return SceneManager.Instance.AsyncLoadLevel( "BattleChessInfinity" );		
		
		// first init menu select
		GUIManager.Instance.ShowAllPanel( "MainGameUI", false );
		GUIManager.Instance.ShowPanel( "MainGameUI", "MainGamePanel", true );	
		
		// send engine option
		ChessEngineManager.Instance.SendCurrentOption();
		ChessEngineManager.Instance.SendNewGame();		
		
		if( ChessEngineManager.Instance.IsWaitforNewGame )
			yield return null;
		
		ScreenFader.Instance.StartFade( true, 2.0f );		
	}
	
	void GamePlay_Update() {
		
	}
	
	void GamePlay_ExitState() {		
			
	}
	
	
	// Restart state
	IEnumerator Restart_EnterState() {		
		
		yield return StartCoroutine( ScreenFader.Instance.WaitingForFadeEnd() );
		yield return ScreenFader.Instance.StartFadeAndWait( false, 2.0f, null, null, true );		
		
		// send engine command - stop and uci new game		
		ChessEngineManager.Instance.SendStop();
		ChessEngineManager.Instance.SendNewGame();
		
		if( ChessEngineManager.Instance.IsWaitforNewGame )
			yield return null;			
		
		ScreenFader.Instance.StartFade( true, 2.0f );
		
		Return();
	}
	
	void Restart_Update() {
		
	}
	
	void Restart_ExitState() {		
		
	}
	
	
	
	
	// static
	public static GameState Instance {
		
		get { 
			return SingletonGameObject<GameState>.Instance; 
		} 
	}
}
