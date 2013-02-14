using UnityEngine;
using System.Collections;

using System.Collections.Generic;

// for moblie : do not support name space, android
//namespace BattleChess {	
	
	
public class ChessBoard {	
	
	// bitboard
	public ChessBitBoard bitBoard;
	public ChessBitBoard bitBoardVirtual;
	
	// board square, 8 x 8, pile x rank
	public ChessBoardSquare[,] aBoardSquare;	
	
	// current live piece list	
	List<ChessPiece> listLivePiece;
	// current captured piece list
	List<ChessPiece> listCapturedPiece;		
	// current movable board position move list
	List<ChessMoveManager.sMove> listCurrMovable;	
	
	// current selected square
	ChessBoardSquare currSelectedSquare;	
	
	// current move, en passant target move
	ChessMoveManager.sMove currWhiteMove;
	ChessMoveManager.sMove currBlackMove;	
	
	List<ChessMoveManager.sMove> listWhiteMoveHistory;	
	List<ChessMoveManager.sMove> listBlackMoveHistory;	
	
	
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
	
	public bool WhiteCallCheck { get; set; }
	public bool BlackCallCheck { get; set; }
	
	public bool WhiteInCheckMate { get; set; }
	public bool BlackInCheckMate { get; set; }
	
	
	
	
	// unity object
	// board material
	Material matBoard1;	
	Material matBoard2;	
	
	ParticleSystem selectPiecePSystem;	
	
	
	// constructor
	public ChessBoard() {			
		
		bitBoard = new ChessBitBoard();
		bitBoardVirtual = new ChessBitBoard();
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
		
		WhiteCallCheck = false;
		BlackCallCheck = false;
		
		WhiteInCheckMate = false;
		BlackInCheckMate = false;
		
		// move
		currWhiteMove = new ChessMoveManager.sMove();
		currBlackMove = new ChessMoveManager.sMove();
		
		listWhiteMoveHistory = new List<ChessMoveManager.sMove>();
		listBlackMoveHistory = new List<ChessMoveManager.sMove>();
		
		listCurrMovable = new List<ChessMoveManager.sMove>();
		
		listCapturedPiece = new List<ChessPiece>();
		
		// init board
		bitBoard.Init();
		bitBoardVirtual.Init();
		
		// piece list
		listLivePiece = new List<ChessPiece>();		
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
						listLivePiece.Add( currPiece );
						
						aBoardSquare[i,j] = new ChessBoardSquare( currPiece, movablePiecePSystem, i, j );
					}
					else if( i == 6 || i == 7 ) {						
						
						currPiece = new ChessPiece( currPieceObject.gameObject, PlayerSide.e_Black,	ChessData.aStartPiecePos[i,j] );
						listLivePiece.Add( currPiece );
						
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
		currSelectedSquare = null;
		selectPiecePSystem = MonoBehaviour.Instantiate( selectPSystemRef, Vector3.zero, Quaternion.identity ) as ParticleSystem;				
		selectPiecePSystem.Stop();
	}
	
	public void SelectSquare( ChessBoardSquare selSquare ) {
		
		if( selSquare != null && selSquare.IsBlank() == false ) {									
						
			if( selSquare.piece.playerSide == UserPlayerSide ) {
				
				PlaySelectEffect( selSquare.piece.gameObject.transform.position, selSquare.piece.gameObject.transform.rotation );				
				currSelectedSquare = selSquare;		
				return;
			}
						
		}		
			
		// movable pos	
		StopSelectEffect();
		currSelectedSquare = null;			
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
	
	public void CaptureSquare( ChessBoardSquare capturedSquare ) {
		
		//if( capturedSquare.IsBlank() )
		//	UnityEngine.Debug.LogError( "ChessBoard::CaptureSquare() - File : " + capturedSquare.position.nPile + "   Rank : " + capturedSquare.position.nRank );
		
		listLivePiece.Remove( capturedSquare.piece );
		listCapturedPiece.Add( capturedSquare.piece );
		
		capturedSquare.ClearPiece(true);		
	}
	
	public void UpdateMoveCount() {
		
		// increase half move and total move
		if( CurrTurn == PlayerSide.e_Black )
			nCurrTotalMove++;					
		
		if( currWhiteMove.IsResetHalfMove() )						
			nCurrHalfMove = 0;	
		else						
			nCurrHalfMove++;		
	}	
	
	public void MoveUpdate( ChessMoveManager.sMove move ) {
		
		if( move.srcSquare.piece.playerSide == PlayerSide.e_White )
			listWhiteMoveHistory.Add( move );
		else if( move.srcSquare.piece.playerSide == PlayerSide.e_Black )
			listBlackMoveHistory.Add( move );
		
		// bit board update
		bitBoard.MoveUpdate( move );		
		
		// normal move
		if( ChessMoveManager.IsNormalMove( move.moveType ) ) {			
			
			// promote move
			if( ChessMoveManager.IsPromoteMove( move.moveType ) ) {						
				
				//UnityEngine.Debug.LogError( "ChessBoard::MoveUpdate() - Normal Move(promote)" );				
			}
			else {
			
				//UnityEngine.Debug.LogError( "ChessBoard::MoveUpdate() - Normal Move" );
				move.trgSquare.SetPiece( move.srcSquare.piece );			
				move.srcSquare.ClearPiece();	
			}
		}		
		// capture move
		else if( ChessMoveManager.IsCaptureMove( move.moveType ) ) {			
			
			// promote move
			if( ChessMoveManager.IsPromoteMove( move.moveType ) ) {
				
				//UnityEngine.Debug.LogError( "ChessBoard::MoveUpdate() - Capture Move(promote)" );
			}
			else {			
			
				//UnityEngine.Debug.LogError( "ChessBoard::MoveUpdate() - Capture Move" );
				
				CaptureSquare( move.trgSquare );
				
				move.trgSquare.SetPiece( move.srcSquare.piece );			
				move.srcSquare.ClearPiece();			
			}
		}	
		
		
		// enpassantmove
		if( ChessMoveManager.IsEnpassantMove( move.moveType ) ) {
			
			//UnityEngine.Debug.LogError( "ChessBoard::MoveUpdate() - en passant move" );
			
			move.trgSquare.SetPiece( move.srcSquare.piece );			
			move.srcSquare.ClearPiece();
			
			// remove captured pawn
			CaptureSquare( move.capturedSquare );
		}
		
		// castling move
		if( ChessMoveManager.IsCastlingMove( move.moveType ) ) {
			
			//UnityEngine.Debug.LogError( "ChessBoard::MoveUpdate() - castling move" );
			
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
			
			move.trgSquare.SetPiece( move.srcSquare.piece );			
			move.srcSquare.ClearPiece();
		}			
		
		UpdateMoveCount();
		
		// for debugging
		if( bitBoard.CheckPositionSync( this ) == false ) {
			
			UnityEngine.Debug.LogError( "ChessBoard::MoveUpdate() - Position Sync Broken!!!!!!!!" );
		}	
		
		UpdateMoveFinalize();
	}
	
	public void UpdateMoveFinalize() {
		
		// turn
		CurrTurn = ChessData.GetOppositeSide( CurrTurn );
		
		// invalidate ready state
		Ready = CurrTurn == UserPlayerSide;
		
		if( CurrTurn == PlayerSide.e_White ) {				
			if( bitBoard.IsWhiteKingInCheck() ) {					
				BlackCallCheck = true;
			}
			
			if( bitBoard.IsWhiteKingInCheckMate() ) {					
				WhiteInCheckMate = true;
			}
		}
		else if( CurrTurn == PlayerSide.e_Black ) {
			if( bitBoard.IsBlackKingInCheck() ) {					
				WhiteCallCheck = true;
			}
		
			if( bitBoard.IsBlackKingInCheckMate() ) {					
				BlackInCheckMate = true;
			}
		}
	}
	
	public bool MoveTo( Vector3 vPos ) {		
		
		if( CurrTurn == UserPlayerSide ) {
			
			//UnityEngine.Debug.LogError( "ChessBoard::MoveTo() - MoveTo" );
			int nTrgRank = 0, nTrgPile = 0;
			bool bValidPos = ChessPosition.CalcPositionIndex( vPos, ref nTrgRank, ref nTrgPile );				
			if( bValidPos ) {					
				
				ChessBoardSquare trgSquare = aBoardSquare[nTrgPile, nTrgRank];
				if( IsValidMove( currSelectedSquare, trgSquare, currWhiteMove ) ) {									
					
					//UnityEngine.Debug.LogError( "ChessBoard::MoveTo() - IsValidMove" );
					MoveUpdate( currWhiteMove );
					
					return true;
				}					
			}	
		}
		
		UnityEngine.Debug.LogError( "ChessBoard::MoveTo() - MoveTo" );
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
					if( IsValidAIMove( srcSquare, trgSquare, listAiMovable, currBlackMove ) ) {
						
						//UnityEngine.Debug.LogError( "AIMoveTo() - IsValidAIMove()" );					
						
						MoveUpdate( currBlackMove );
							
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
		
		if( currSelectedSquare != null )
			ChessMoveManager.GetValidateMoveList( this, currSelectedSquare, listCurrMovable );
		
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
		
		// player turn - represent engine turn		
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
		string strCastling = bitBoard.currCastlingState.GetFenString();		
		
		// en passant target square
		string strEnPassant = bitBoard.currEnPassantTrgSq.GetFenString();
		
		strResFen = strResFen + strCastling + strEnPassant;
		
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
	
	bool IsValidMove( ChessBoardSquare srcSquare, ChessBoardSquare trgSquare, ChessMoveManager.sMove userMove ) {
		
		foreach( ChessMoveManager.sMove move in listCurrMovable ) {
			
			if( move.srcSquare == srcSquare && move.trgSquare == trgSquare ) {
				
				userMove.Set( move );
				return true;
			}
		}
		
		userMove.Clear();
		return false;
	}
		
	bool IsValidAIMove( ChessBoardSquare srcSquare, ChessBoardSquare trgSquare, List<ChessMoveManager.sMove> listMove, ChessMoveManager.sMove aiMove ) {
		
		foreach( ChessMoveManager.sMove move in listMove ) {			
			
			if( move.srcSquare == srcSquare && move.trgSquare == trgSquare ) {
				
				aiMove.Set( move );
				return true;
			}
		}
		
		aiMove.Clear();
		return false;
	}	
}
	
//}