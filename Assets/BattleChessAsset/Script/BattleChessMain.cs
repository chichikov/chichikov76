using UnityEngine;
using System.Collections;
using System.Collections.Generic;




public class BattleChessMain : MonoBehaviour {		
	
	// chess piece prefab reference
	public Transform[] aWholePiece;				
	
	// particle effect
	public ParticleSystem selectPiecePSystemRef;		
	// movable particle effect
	public ParticleSystem movablePiecePSystemRef;
	
	// chess board
	ChessBoard board;
	
	// chess engine management
	ChessEngineManager chessEngineMgr;	
	
	
	
	// Use this for initialization
	void Start() {				
		
		board = new ChessBoard();
		board.Init( this, aWholePiece, selectPiecePSystemRef, movablePiecePSystemRef );		
		
		// chess engine init/start
		chessEngineMgr = new ChessEngineManager();	
		
		StartCoroutine( chessEngineMgr.Start() );		
	}
	
	// Update is called once per frame
	void Update() {		
		
		// process engine command respond
		ProcessEngineCommand();	
		
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
					
					//UnityEngine.Debug.LogError( "Update() - Board Clicked" );
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
		
		// chess engine end
		chessEngineMgr.End();
		
	}
	
	
	// helper function
	void Move( Vector3 vMoveTo ) {
		
		if( board.MoveTo( vMoveTo ) )
		{
			// move command
			string strMoveCmd = board.GetCurrMoveCommand();						
			UnityEngine.Debug.Log( strMoveCmd );						
			chessEngineMgr.Send( strMoveCmd );
			
			// go command
			string strGoCmd = board.GetCurrGoCommand();						
			UnityEngine.Debug.Log( strGoCmd );						
			chessEngineMgr.Send( strGoCmd );
			
			// turn
			board.CurrTurn = ChessData.GetOppositeSide( board.CurrTurn );
			
			// invalidate ready state
			board.Ready = false;
		}		
	}
	
	
	
	
	
	
	// process engine command respond
	void ProcessEngineCommand() {
		// read one line
		string strCurCommandLine = chessEngineMgr.PopReceivedQueue();
		while( strCurCommandLine != null ) {
			
			UnityEngine.Debug.Log(strCurCommandLine);
			
			// process one engine respond
			EngineToGuiCommand command = chessEngineMgr.ParseCommand( strCurCommandLine );
			if( command != null ) {				
				
				//command.PrintCommand();
				chessEngineMgr.SetConfigCommand( command.CmdData );
				
				ExcuteEngineCommand( command );								
			}
			
			strCurCommandLine = chessEngineMgr.PopReceivedQueue();
		}
		
	}
	
	
	
	// 
	bool ExcuteIdCommand( CommandBase.CommandData cmdData ) {
		
		return false;
	}
	
	bool ExcuteUciOkCommand( CommandBase.CommandData cmdData ) {
		
		// send setoption command!!!
		
		// send isready command	
		chessEngineMgr.Send( "isready" );		
		
		return true;
	}
	
	bool ExcuteReadyOkCommand( CommandBase.CommandData cmdData ) {
		
		// send isready command	
		chessEngineMgr.Send( "ucinewgame" );
		
		board.Ready = true;	
		
		return true;
	}
	
	bool ExcuteCopyProtectionCommand( CommandBase.CommandData cmdData ) {
		
		return false;
	}
	
	bool ExcuteRegistrationCommand( CommandBase.CommandData cmdData ) {
		
		return false;
	}
	
	bool ExcuteOptionCommand( CommandBase.CommandData cmdData ) {
		
		return false;
	}
	
	bool ExcuteInfoCommand( CommandBase.CommandData cmdData ) {
		
		return false;
	}
	
	bool ExcuteBestMoveCommand( CommandBase.CommandData cmdData ) {
		
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
		
		ChessBoardSquare srcSquare, trgSquare;
		srcSquare = board.aBoardSquare[nSrcPile, nSrcRank];
		trgSquare = board.aBoardSquare[nTrgPile, nTrgRank];
		
		//UnityEngine.Debug.LogError( "ExcuteBestMoveCommand() - src rank, pile " + nSrcRank + " , " + nSrcPile );
		//UnityEngine.Debug.LogError( "ExcuteBestMoveCommand() - trg rank, pile " + nTrgRank + " , " + nTrgPile );
		
		if( board.AIMoveTo( srcSquare, trgSquare ) )
		{	
			// turn
			board.CurrTurn = ChessData.GetOppositeSide( board.CurrTurn );
			// invalidate ready state
			board.Ready = true;			
			
			return true;
		}
		
		UnityEngine.Debug.LogError( "ExcuteBestMoveCommand() - Invalid src rank, pile" );
		
		return false;
	}	
	
	// delegate function for engine command
	public bool ExcuteEngineCommand( EngineToGuiCommand cmd ) {		
		
		if( cmd == null )
			return false;
		
		bool bValidCmd = !cmd.CmdData.InvalidCmd;		
		if( bValidCmd ) {
			
			string strCmd = cmd.CmdData.StrCmd;
			
			switch( strCmd ) {
					
				case "id":		
					return ExcuteIdCommand( cmd.CmdData );						
						
				case "uciok":	
					return ExcuteUciOkCommand( cmd.CmdData );				
					
				case "readyok":		
					return ExcuteReadyOkCommand( cmd.CmdData );							
				
				case "copyprotection":
					return ExcuteCopyProtectionCommand( cmd.CmdData );						
				
				case "registration":
					return ExcuteRegistrationCommand( cmd.CmdData );							
					
				case "option":
					return ExcuteOptionCommand( cmd.CmdData );							
				
				case "info":
					return ExcuteInfoCommand( cmd.CmdData );											
				
				case "bestmove":
					return ExcuteBestMoveCommand( cmd.CmdData );							
					
				default:								
					return false;					
			} // switch	
			
			//return true;
		}	
		
		return false;
	}	
	
}
