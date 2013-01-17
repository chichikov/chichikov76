using UnityEngine;
using System.Collections;

using System.Collections.Generic;

// for moblie : do not support name space, android
//namespace BattleChess {	
	
	
public class ChessBoard {
	
	// board
	// bitboard
	public ChessBitBoard bitBoard;
	// 8 x 8, pile x rank
	public ChessBoardSquare[,] aBoardSquare;	
	
	public List<ChessPiece> listPiece;
	
	// board material
	Material matBoard1;	
	Material matBoard2;	
	
	// selected square
	ChessBoardSquare selectSquare;		
	ParticleSystem selectPiecePSystem;	
	
	// movable board pos
	List<ChessMoveManager.sMove> listCurrMovable;			
	
	// current move, en passant target move
	ChessMoveManager.sMove currMove;
	
	// half move
	int nCurrHalfMove;
	
	// total move
	int nCurrTotalMove;
	
	// user player side
	public PlayerSide UserPlayerSide { get; set; }
	
	// time
	public int ThinkingTime { get; set; }
	
	public bool Ready { get; set; }
	
	// player turn
	public PlayerSide CurrTurn { get; set; }
	
	// castling
	public ChessCastling currCastlingState;
	
	
	
	
	// constructor
	public ChessBoard() {			
		
		bitBoard = new ChessBitBoard();
	}
	
	
	// interface
	public void Init( BattleChessMain chessMain, Transform[] aPieceRef, ParticleSystem selectPSystemRef, ParticleSystem movablePSystemRef ) {
		
		// etc property		
		CurrTurn = PlayerSide.e_White;	
		UserPlayerSide = PlayerSide.e_White;
		ThinkingTime = 18000;
		nCurrHalfMove = 0;
		nCurrTotalMove = 0;
		
		Ready = false;
		
		// move
		currMove = new ChessMoveManager.sMove();
		currCastlingState = new ChessCastling() {
			
			CastlingWKSide = CastlingState.eCastling_Enable_State,
			CastlingWQSide = CastlingState.eCastling_Enable_State,
			CastlingBKSide = CastlingState.eCastling_Enable_State,
			CastlingBQSide = CastlingState.eCastling_Enable_State
		};	
		
		listCurrMovable = new List<ChessMoveManager.sMove>();
		
		// init board
		bitBoard.Init();
		
		// piece list
		listPiece = new List<ChessPiece>();		
		aBoardSquare = new ChessBoardSquare[ChessData.nNumPile,ChessData.nNumRank];
		
		ChessPiece currPiece = null;
		for( int i=0; i<ChessData.nNumPile; i++ ){
			for( int j=0; j<ChessData.nNumRank; j++ ){				
				
				// movable square effect Particle System
				ParticleSystem movablePiecePSystem = MonoBehaviour.Instantiate( movablePSystemRef, Vector3.zero, Quaternion.identity ) as ParticleSystem;
				
				if( ChessData.aStartPiecePos[i,j] == PiecePlayerType.eNone_Piece ) {					
					
					aBoardSquare[i,j] = new ChessBoardSquare( null, movablePiecePSystem, i, j );							
				}
				else
				{
					Vector3 currPos = new Vector3( j - 3.5f, 0.0f, i - 3.5f );					
					
					Transform currTransform = aPieceRef[(int)ChessData.aStartPiecePos[i,j]];
					Transform currPieceObject = MonoBehaviour.Instantiate( currTransform, currPos, currTransform.rotation ) as Transform;
					
					
					if( i == 0 || i == 1 ) {																						
						
						currPiece = new ChessPiece( currPieceObject.gameObject, PlayerSide.e_White,	ChessData.aStartPiecePos[i,j] );
						listPiece.Add( currPiece );
						
						aBoardSquare[i,j] = new ChessBoardSquare( currPiece, movablePiecePSystem, i, j );
					}
					else if( i == 6 || i == 7 ) {						
						
						currPiece = new ChessPiece( currPieceObject.gameObject, PlayerSide.e_Black,	ChessData.aStartPiecePos[i,j] );
						listPiece.Add( currPiece );
						
						aBoardSquare[i,j] = new ChessBoardSquare( currPiece, movablePiecePSystem, i, j );
					}									
				}				
			}		
		}
		
		// piece coloar
		SetWhiteSidePieceColor( Color.white );
		SetBlackSidePieceColor( Color.white );
		
		
		// board material
		if( chessMain.renderer.materials.Length == 2 ) {
			
			matBoard1 = chessMain.renderer.materials[0];
			matBoard2 = chessMain.renderer.materials[1];			
			
			Color rgbaWhiteBoard, rgbaBlackBoard;
			rgbaWhiteBoard = new Color( 1.0f, 1.0f, 1.0f, 1.0f );
			rgbaBlackBoard = new Color( 0.039f, 0.34f, 0.22f, 1.0f );
			
			SetWhiteSideBoardColor( rgbaWhiteBoard );
			SetBlackSideBoardColor( rgbaBlackBoard );
		}	
		
		// particle effect
		selectSquare = null;
		selectPiecePSystem = MonoBehaviour.Instantiate( selectPSystemRef, Vector3.zero, Quaternion.identity ) as ParticleSystem;				
		selectPiecePSystem.Stop();
	}
	
	public void SelectSquare( ChessBoardSquare selSquare ) {
		
		if( selSquare != null && selSquare.IsBlank() == false ) {									
						
			if( selSquare.piece.playerSide == UserPlayerSide ) {
				
				PlaySelectEffect( selSquare.piece.gameObject.transform.position, selSquare.piece.gameObject.transform.rotation );				
				selectSquare = selSquare;		
				return;
			}
						
		}		
			
		// movable pos	
		StopSelectEffect();
		selectSquare = null;			
	}
	
	public ChessPiece GetPiece( Vector3 vPos ) {	
		
		// movable pos	
		int nRank = 0, nPile = 0;
		bool bValidPos = ChessPosition.CalcPositionIndex( vPos, ref nRank, ref nPile );
		if( bValidPos ) {				
					
			if( aBoardSquare[nPile,nRank].IsBlank() == false ) {
			
				return aBoardSquare[nPile,nRank].piece;				
			}
		}
		
		return null;
	}
	
	public ChessBoardSquare GetSquare( Vector3 vPos, bool bExistPiece ) {	
		
		// movable pos	
		int nRank = 0, nPile = 0;
		bool bValidPos = ChessPosition.CalcPositionIndex( vPos, ref nRank, ref nPile );
		if( bValidPos ) {				
			
			if( bExistPiece ) {
				
				if( aBoardSquare[nPile,nRank].IsBlank() == false ) {
					
					return aBoardSquare[nPile,nRank];
				}
			}
			else
				return aBoardSquare[nPile,nRank];					
		}
		
		return null;
	}	
	
	public void UpdateMoveCount() {
		
		// increase half move and total move
		if( CurrTurn == PlayerSide.e_Black )
			nCurrTotalMove++;					
		
		if( currMove.IsResetHalfMove() )						
			nCurrHalfMove = 0;	
		else						
			nCurrHalfMove++;		
	}
	
	public void UpdateCastlingState( ChessBoardSquare srcSquare ) {
		
		// possible castling condition
		//1.The king has not previously moved.
		//2.The chosen rook has not previously moved.
		//3.There are no pieces between the king and the chosen rook.
		//4.The king is not currently in check.
		//5.The king does not pass through a square that is under attack by enemy pieces.[2]
		//6.The king does not end up in check (true of any legal move).
		//여기부터...
		
		// disable castling state
		switch( srcSquare.piece.piecePlayerType ) 						
		{
			case PiecePlayerType.eWhite_King:
			{
				currCastlingState.CastlingWKSide = CastlingState.eCastling_Disable_State;
				currCastlingState.CastlingWQSide = CastlingState.eCastling_Disable_State;
			}
			break;
			
			case PiecePlayerType.eWhite_RookLeft:
			{
				currCastlingState.CastlingWQSide = CastlingState.eCastling_Disable_State;
			}
			break;
				
			case PiecePlayerType.eWhite_RookRight:
			{
				currCastlingState.CastlingWKSide = CastlingState.eCastling_Disable_State;
			}
			break;
				
			case PiecePlayerType.eBlack_King:
			{
				currCastlingState.CastlingBKSide = CastlingState.eCastling_Disable_State;
				currCastlingState.CastlingBQSide = CastlingState.eCastling_Disable_State;
			}
			break;
				
			case PiecePlayerType.eBlack_RookLeft:
			{
				currCastlingState.CastlingBQSide = CastlingState.eCastling_Disable_State;
			}
			break;
				
			case PiecePlayerType.eBlack_RookRight:
			{
				currCastlingState.CastlingBKSide = CastlingState.eCastling_Disable_State;
			}
			break;									
		}		
	}
	
	public void UpdateMove( ChessMoveManager.sMove move ) {
		
		UpdateMoveCount();
		UpdateCastlingState( move.srcSquare );					
		
		// normal move
		if( ChessMoveManager.IsNormalMove( move.moveType ) ) {			
			
			// pawn move
			if( ChessMoveManager.IsPawnMove( move.moveType ) ) {
				
				// two square move
				if( ChessMoveManager.IsPawnTwoMove( move.moveType ) ) {					
					
					move.srcSquare.piece.bEnPassantCapture = true;					
				}				
				
				// promote move
				if( ChessMoveManager.IsPromoteMove( move.moveType ) ) {
					
				}				
			}
		}
		
		// capture move
		if( ChessMoveManager.IsCaptureMove( move.moveType ) ) {
			
			// pawn move
			if( ChessMoveManager.IsPawnMove( move.moveType ) ) {
				
				// promote move
				if( ChessMoveManager.IsPromoteMove( move.moveType ) ) {
					
				}				
			}
			
			move.trgSquare.ClearPiece(true);
		}	
		
		
		// enpassantmove
		if( ChessMoveManager.IsEnpassantMove( move.moveType ) ) {
			
			// pawn move
			if( ChessMoveManager.IsPawnMove( move.moveType ) ) {				
							
			}			
		}
		
		// castling move
		if( ChessMoveManager.IsCastlingMove( move.moveType ) ) {
			
			int nDestRookRank = 0, nDestRookPile = 0;
			nDestRookRank = move.trgSquare.position.nRank;
			nDestRookPile = move.trgSquare.position.nPile;
			// white king side castling
			if( ChessMoveManager.IsWhiteKingSideCastlingMove( move.moveType ) ) {
				
				ChessBoardSquare rookSquare = aBoardSquare[0,7];				
				nDestRookRank = nDestRookRank - 1;
				aBoardSquare[nDestRookPile, nDestRookRank].SetPiece( rookSquare.piece );			
				rookSquare.ClearPiece();
			}				
			
			// white Queen side castling
			if( ChessMoveManager.IsWhiteQueenSideCastlingMove( move.moveType ) ) {
				
				ChessBoardSquare rookSquare = aBoardSquare[0,0];				
				nDestRookRank = nDestRookRank + 1;
				aBoardSquare[nDestRookPile, nDestRookRank].SetPiece( rookSquare.piece );	
				rookSquare.ClearPiece();				
			}	
			
			// black king side castling
			if( ChessMoveManager.IsBlackKingSideCastlingMove( move.moveType ) ) {
				
				ChessBoardSquare rookSquare = aBoardSquare[7, 7];				
				nDestRookRank = nDestRookRank - 1;
				aBoardSquare[nDestRookPile, nDestRookRank].SetPiece( rookSquare.piece );	
				rookSquare.ClearPiece();
			}	
			
			// black queen side castling
			if( ChessMoveManager.IsBlackQueenSideCastlingMove( move.moveType ) ) {
				
				ChessBoardSquare rookSquare = aBoardSquare[7, 0];				
				nDestRookRank = nDestRookRank + 1;
				aBoardSquare[nDestRookPile, nDestRookRank].SetPiece( rookSquare.piece );	
				rookSquare.ClearPiece();
			}	
		}		
		
		
		// move real board piece
		move.trgSquare.SetPiece( move.srcSquare.piece );			
		move.srcSquare.ClearPiece();		
	}
	
	public bool MoveTo( Vector3 vPos ) {		
		
		if( CurrTurn == UserPlayerSide ) {
			
			//UnityEngine.Debug.LogError( "ChessBoard::MoveTo() - MoveTo" );
			int nTrgRank = 0, nTrgPile = 0;
			bool bValidPos = ChessPosition.CalcPositionIndex( vPos, ref nTrgRank, ref nTrgPile );				
			if( bValidPos ) {					
				
				ChessBoardSquare trgSquare = aBoardSquare[nTrgPile, nTrgRank];
				if( IsValidMove( trgSquare, currMove ) ) {									
					
					//UnityEngine.Debug.LogError( "ChessBoard::MoveTo() - IsValidMove" );
					UpdateMove( currMove );
					
					return true;
				}					
			}	
		}
		
		return false;
	}
	
	// AI Move
	public bool AIMoveTo( ChessBoardSquare srcSquare, ChessBoardSquare trgSquare ) {		
		
		if( CurrTurn != UserPlayerSide ) {
			
			//UnityEngine.Debug.LogError( "AIMoveTo() - current turn = " + CurrTurn );						
			if( srcSquare.IsBlank() == false ) {	
				
				//UnityEngine.Debug.LogError( "AIMoveTo() - no blank" );
				
				List<ChessMoveManager.sMove> listAiMovable = new List<ChessMoveManager.sMove>();
				bool bMoveList = ChessMoveManager.GetValidateMoveList( this, srcSquare, listAiMovable );
				if( bMoveList ) {
					//UnityEngine.Debug.LogError( "AIMoveTo() - no blank" + " " + bMoveList );
					ChessMoveManager.sMove aIMove = new ChessMoveManager.sMove();
					if( IsValidAIMove( trgSquare, listAiMovable, aIMove ) ) {
						
						//UnityEngine.Debug.LogError( "AIMoveTo() - IsValidAIMove()" );					
						
						UpdateMove( aIMove );
							
						return true;					
					}
				}
			}	
		}
		
		UnityEngine.Debug.LogError( "AIMoveTo() - !!!!" );
		
		return false;
	}
	
	public void UpdateCurrMoveable() {
		
		StopMovableEffect();
		
		listCurrMovable.Clear();
		
		if( selectSquare != null )
			ChessMoveManager.GetValidateMoveList( this, selectSquare, listCurrMovable );
		
		// movable effect start
		PlayMovableEffect();			
	}
	
	public string GetCurrMoveCommand() {
		
		// position fen string
		string strPosFen = "position fen ", strResFen;
		int nNumBlank = 0;
		
		for( int i=ChessData.nNumPile-1; i>=0; i-- ){			
			for( int j=0; j<ChessData.nNumRank; j++ ){
				
				if( aBoardSquare[i,j].IsBlank() ) {					
					
					nNumBlank++;
				}	
				else {
					
					if( nNumBlank > 0 ) {
						
						strPosFen += nNumBlank;
					}
						
					strPosFen += ChessData.GetPieceFenString( aBoardSquare[i,j].piece.piecePlayerType );
					nNumBlank = 0;
				}
			}
			
			if( nNumBlank > 0 )			
				strPosFen += nNumBlank;			
			
			if( i == 0 )
				strPosFen += " ";
			else
				strPosFen += "/";
			
			nNumBlank = 0;
		}
		
		// player turn
		string strTurn;
		if( CurrTurn == PlayerSide.e_White )
			strTurn = "w";
		else if( CurrTurn == PlayerSide.e_Black )
			strTurn = "b";
		else {
			
			strTurn = "w";
			UnityEngine.Debug.LogError( "Fen String Error - Player Turn" );
		}
		
		strResFen = strPosFen + strTurn;
		
		// cstling
		string strCastling = currCastlingState.GetFenString();		
		
		// en passant target square
		string strEnPassant = currMove.enPassantTargetSquare.GetFenString();
		
		strResFen = strPosFen + strCastling + strEnPassant;
		
		// curr half move count for 50 move rule
		strResFen += " " + nCurrHalfMove;
		
		// total move - if black piece move completed, increse move
		strResFen += " " + nCurrTotalMove;
		
		return strResFen;
	}
	
	public string GetCurrGoCommand() {
		
		string strSide = "wtime";
		if( UserPlayerSide == PlayerSide.e_White )
			strSide = "btime";
		
		string strRetGoCmd = "go " + strSide + " " + ThinkingTime;
		return strRetGoCmd;
	}
	
	
	
	
	
	
	// private method
	// SetBoardColor
	void SetWhiteSideBoardColor( Color rgbaWhite ) {
		
		if( matBoard1 != null ) {
			
			matBoard1.SetColor( "_Color", rgbaWhite );					
		}
	}
	
	void SetBlackSideBoardColor( Color rgbaBlack ) {
		
		if( matBoard2 != null ) {			
			
			matBoard2.SetColor( "_Color", rgbaBlack );			
		}
	}
	
	void SetWhiteSidePieceColor( Color rgbaWhite ) {
		
		for( int i=0; i<ChessData.nNumPile; i++ ) {
			for( int j=0; j<ChessData.nNumRank; j++ ) {
				
				if( aBoardSquare[i,j].IsBlank() == false ) {
					
					if( aBoardSquare[i,j].piece.playerSide == PlayerSide.e_White )
						aBoardSquare[i,j].piece.gameObject.renderer.material.SetColor( "_Color", rgbaWhite );											
				}
			}
		}		
	}
	
	void SetBlackSidePieceColor( Color rgbaBlack ) {
		
		for( int i=0; i<ChessData.nNumPile; i++ ) {
			for( int j=0; j<ChessData.nNumRank; j++ ) {
				
				if( aBoardSquare[i,j].IsBlank() == false ) {
					
					if( aBoardSquare[i,j].piece.playerSide == PlayerSide.e_Black )
						aBoardSquare[i,j].piece.gameObject.renderer.material.SetColor( "_Color", rgbaBlack );	
				}
			}
		}	
	}
	
	
	//
	void StopSelectEffect() {
		
		if( selectPiecePSystem != null )
			//UnityEngine.Debug.Log( "de select" );				
			selectPiecePSystem.Stop();	
			selectPiecePSystem.renderer.enabled = false;
	}
	
	void PlaySelectEffect( Vector3 vPos, Quaternion rot ) {
		
		if( selectPiecePSystem != null && selectPiecePSystem.gameObject != null ) {
			
			selectPiecePSystem.renderer.enabled = true;
			
			selectPiecePSystem.gameObject.transform.position = vPos;
			selectPiecePSystem.gameObject.transform.rotation = rot;
			selectPiecePSystem.Play();	
		}
	}	
	
	void StopMovableEffect() {
		
		// previous movable effect stop
		foreach( ChessBoardSquare square in aBoardSquare ) {					
			
			square.ShowMovableEffect(false);
		}		
	}
	
	void PlayMovableEffect() {
		
		foreach( ChessMoveManager.sMove move in listCurrMovable ) {					
			
			move.trgSquare.ShowMovableEffect(true);
		}	
	}
	
	bool IsValidMove( ChessBoardSquare trgSquare, ChessMoveManager.sMove targetMove ) {
		
		foreach( ChessMoveManager.sMove move in listCurrMovable ) {
			
			if( move.trgSquare.position == trgSquare.position ) {
				
				targetMove.Set( move );
				return true;
			}
		}
		
		targetMove.Clear();
		return false;
	}
		
	bool IsValidAIMove( ChessBoardSquare trgSquare, List<ChessMoveManager.sMove> listMove, ChessMoveManager.sMove targetMove ) {
		
		foreach( ChessMoveManager.sMove move in listMove ) {			
			
			if( move.trgSquare.position == trgSquare.position ) {
				
				targetMove.Set( move );
				return true;
			}
		}
		
		targetMove.Clear();
		return false;
	}	
}
	
//}
