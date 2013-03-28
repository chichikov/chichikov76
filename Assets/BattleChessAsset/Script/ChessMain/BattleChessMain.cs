using UnityEngine;
using System.Collections;
using System.Collections.Generic;




public class BattleChessMain : MonoBehaviour, IProcessChessEngine {		
	
	// chess piece prefab reference
	public Transform[] aWholePiece;				
	
	// particle effect
	public ParticleSystem selectPiecePSystemRef;		
	// movable particle effect
	public ParticleSystem movablePiecePSystemRef;
	// check particle effect
	public ParticleSystem checkPSystemRef;
	// check mate particle effect
	public ParticleSystem checkMatePSystemRef;
	
	
	// chess board
	ChessBoard board;
	
	
	// Use this for first initialization	
	void Awake() {	
		
		// enroll command executor proxy
		GameMain.Instance.EngineCmdExecuterProxy = this;
	}
	
	// Use this for initialization
	void Start() {				
		
		board = new ChessBoard();
		board.Init( this, aWholePiece, selectPiecePSystemRef, movablePiecePSystemRef );				
	}
	
	// Update is called once per frame
	void Update() {	
		
		// input
		// piece selection
		if( Input.GetMouseButtonDown(0) && board.Ready ) {
			
			// collision check
			RaycastHit hitInfo;
			Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);        
			if( Physics.Raycast( ray, out hitInfo, 1000 ) ) {
				
				// collision to piece
				if( hitInfo.collider.gameObject.tag == "Piece" ) {				
					
					Vector3 vPos = hitInfo.collider.gameObject.transform.position;				
					
					// Capture enemy
					ChessBoardSquare collisionSqaure = board.GetSquare( vPos, true );					
					if( collisionSqaure != null ) {
						if( collisionSqaure.piece.IsEnemy( board.UserPlayerSide ) ) {	
							
							Move( vPos );
							board.SelectSquare( null );							
						}
						else {
							
							// my side								
							board.SelectSquare( collisionSqaure );					
						}
					}
					else {
						
						board.SelectSquare( null );
						
						UnityEngine.Debug.LogError( "Update() - Invalid Chess Piece" );						
					}
				}
				// collision to board
				else if( hitInfo.collider.gameObject.tag == "Board" ) {	
					
					// move to blank position					
					Vector3 vPos = hitInfo.point;					
					Move( vPos );					
					
					board.SelectSquare( null );															
				}
			}
			// extracollision
			else {				
				
				board.SelectSquare( null );								
			}
			
			board.UpdateCurrMoveable();
		}	
	}
	
	void OnDestroy () { 	
		
	}	
	
	// helper function
	void Move( Vector3 vMoveTo ) {
		
		if( board.MoveTo( vMoveTo ) )
		{
			bool bPonder = ChessEngineManager.Instance.IsPonderMode;
			if( bPonder ) {
				
				string strCurrMoveFen = board.GetCurrMoveFenString();	
				
				// ponder hit
				if( strCurrMoveFen == ChessEngineManager.Instance.CurrentPonder ) {
					
					ChessEngineManager.Instance.Send( "ponderhit" );
					ChessEngineManager.Instance.IsPonderFailed = false;					
				}
				// ponder fail
				else {
				
					ChessEngineManager.Instance.Send( "stop" );
					ChessEngineManager.Instance.IsPonderFailed = true;
					
					// move command
					string strMoveCmd = board.GetCurrMoveCommand();						
					UnityEngine.Debug.Log( strMoveCmd );						
					ChessEngineManager.Instance.Send( strMoveCmd );
					
					// go command
					string strGoCmd = board.GetCurrGoCommand();						
					UnityEngine.Debug.Log( strGoCmd );						
					ChessEngineManager.Instance.Send( strGoCmd );	
				}
			}
			else {			
				
				// move command
				string strMoveCmd = board.GetCurrMoveCommand();						
				UnityEngine.Debug.Log( strMoveCmd );						
				ChessEngineManager.Instance.Send( strMoveCmd );
				
				// go command
				string strGoCmd = board.GetCurrGoCommand();						
				UnityEngine.Debug.Log( strGoCmd );						
				ChessEngineManager.Instance.Send( strGoCmd );			
			}
		}		
	}
	
	
	
	
	
	
	// IProcessChessEngine	
	public bool OnInitStockfishCommand( CommandBase.CommandData cmdData )
	{			
		return true;				
	}
	
	public bool OnIdCommand( CommandBase.CommandData cmdData )
	{		
		return true;		
	}
	public bool OnUciOkCommand( CommandBase.CommandData cmdData )
	{		
		return true;
	}
	
	public bool OnReadyOkCommand( CommandBase.CommandData cmdData )
	{	
		// new game		
		if( ChessEngineManager.Instance.IsWaitforNewGame ) {
		
			// chess engine restart		
			board.Restart();
			
			ChessEngineManager.Instance.IsWaitforNewGame = false;
			ChessEngineManager.Instance.IsForceStop = false;
		}
		// normal wait chess engine ready
		else {
			
		}
		
		return true;		
	}
	
	public bool OnCopyProtectionCommand( CommandBase.CommandData cmdData )
	{		
		return true;
	}
	
	public bool OnRegistrationCommand( CommandBase.CommandData cmdData )
	{
		
		return true;
	}
	
	public bool OnOptionCommand( CommandBase.CommandData cmdData )
	{				
		return false;
	}
	
	public bool OnInfoCommand( CommandBase.CommandData cmdData )
	{
		
		return true;
	}		
	
	public bool OnBestMoveCommand( CommandBase.CommandData cmdData )
	{		
		// best move string to rank/pile
		string strBestMove = cmdData.QueueStrValue.Peek();	
		
		//UnityEngine.Debug.LogError( "ExcuteBestMoveCommand() - best move string = " + strBestMove );
		
		string strSrcPos = strBestMove.Substring(0, 2);
		string strTrgPos = strBestMove.Substring(2, 2);
		
		int nSrcRank, nSrcPile;
		ChessData.GetStringToRankPile( strSrcPos, out nSrcRank, out nSrcPile );				
		if( ChessPosition.IsInvalidPositionIndex(nSrcRank, nSrcPile) ) {
			
			UnityEngine.Debug.LogError( "ExcuteBestMoveCommand() - Invalid src rank, pile" );
			return false;
		}
				
		int nTrgRank, nTrgPile;
		ChessData.GetStringToRankPile( strTrgPos, out nTrgRank, out nTrgPile );		
		
		if( ChessPosition.IsInvalidPositionIndex(nTrgRank, nTrgPile) ) {
			
			UnityEngine.Debug.LogError( "ExcuteBestMoveCommand() - Invalid src rank, pile" );
			return false;
		}	
		
		// for ponder		
		string strPonderMoveCommand = null;		
		string strPonderGoCommand = null;
		string strPonderMove = ChessEngineManager.Instance.CurrentPonder;
		bool bPonder = ChessEngineManager.Instance.IsPonderMode;
		
		if( bPonder ) {
			
			strPonderMoveCommand = board.GetCurrPonderMoveCommand( strBestMove, strPonderMove );
			strPonderGoCommand = board.GetCurrPonderGoCommand();
		}
		
		ChessBoardSquare srcSquare, trgSquare;
		srcSquare = board.aBoardSquare[nSrcPile, nSrcRank];
		trgSquare = board.aBoardSquare[nTrgPile, nTrgRank];
		
		//UnityEngine.Debug.LogError( "ExcuteBestMoveCommand() - src rank, pile " + nSrcRank + " , " + nSrcPile );
		//UnityEngine.Debug.LogError( "ExcuteBestMoveCommand() - trg rank, pile " + nTrgRank + " , " + nTrgPile );
		
		if( board.AIMoveTo( srcSquare, trgSquare ) )
		{
			// send pondering command
			if( bPonder ) {	
				
				// ponder move command						
				//UnityEngine.Debug.Log( strMoveCmd );						
				ChessEngineManager.Instance.Send( strPonderMoveCommand );
				
				// ponder go command							
				//UnityEngine.Debug.Log( strGoCmd );						
				ChessEngineManager.Instance.Send( strPonderGoCommand );				
			}
			
			return true;
		}
		
		UnityEngine.Debug.LogError( "ExcuteBestMoveCommand() - Invalid src rank, pile" );
		return false;				
	}
}
