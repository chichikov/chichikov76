using UnityEngine;
using System.Collections;

using System;
using System.Collections.Generic;


//namespace BattleChess {	

public class ChessMoveManager {	
	
	
	public enum MoveDirectionType : int {
	
		eDirection_Move_Up = 0,
		eDirection_Move_Down,
		eDirection_Move_Left,
		eDirection_Move_Right,
		eDirection_Move_LeftUp_Diagonal,
		eDirection_Move_LeftDown_Diagonal,
		eDirection_Move_RightUp_Diagonal,
		eDirection_Move_RightDown_Diagonal,
		eDirection_Move_Steep_LeftUp_Leap,
		eDirection_Move_Steep_LeftDown_Leap,
		eDirection_Move_Steep_RightUp_Leap,
		eDirection_Move_Steep_RightDown_Leap,
		eDirection_Move_NonSteep_LeftUp_Leap,
		eDirection_Move_NonSteep_LeftDown_Leap,
		eDirection_Move_NonSteep_RightUp_Leap,
		eDirection_Move_NonSteep_RightDown_Leap,
	}
	
	[Flags]
	public enum MoveType {
		
		eNone_Move = 0x0,
		eNormal_Move = 0x1,
		eCapture_Move = 0x2,
		eEnPassan_Move = 0x04,
		ePromote_Move = 0x08,		
		ePawn_Move = 0x10,
		ePawn_One_Move = 0x20,
		ePawn_Two_Move = 0x40,
		eCastling_Move = 0x80,
		eCastling_White_KingSide_Move = 0x100,
		eCastling_White_QueenSide_Move = 0x200,
		eCastling_Black_KingSide_Move = 0x400,
		eCastling_Black_QueenSide_Move = 0x800,
	}	
	
	public static bool IsNormalMove( MoveType moveType ) {
		
		if( (moveType & MoveType.eNormal_Move) > 0 )
			return true;
	
		return false;		
	}
	
	public static bool IsCaptureMove( MoveType moveType ) {
		
		if( (moveType & MoveType.eCapture_Move) > 0 )
			return true;
	
		return false;		
	}
	
	public static bool IsEnpassantMove( MoveType moveType ) {
		
		if( (moveType & MoveType.eEnPassan_Move) > 0 )
			return true;
	
		return false;		
	}
	
	public static bool IsPawnMove( MoveType moveType ) {
		
		if( (moveType & MoveType.ePawn_Move) > 0 )
			return true;
	
		return false;		
	}
	
	public static bool IsPawnOneMove( MoveType moveType ) {
		
		if( (moveType & MoveType.ePawn_One_Move) > 0 )
			return true;
	
		return false;		
	}
	
	public static bool IsPawnTwoMove( MoveType moveType ) {
		
		if( (moveType & MoveType.ePawn_Two_Move) > 0 )
			return true;
	
		return false;		
	}
	
	public static bool IsPromoteMove( MoveType moveType ) {
		
		if( (moveType & MoveType.ePromote_Move) > 0 )
			return true;
	
		return false;		
	}
	
	public static bool IsCastlingMove( MoveType moveType ) {
		
		if( (moveType & MoveType.eCastling_Move) > 0 )
			return true;
	
		return false;		
	}
	
	public static bool IsWhiteKingSideCastlingMove( MoveType moveType ) {
		
		if( (moveType & MoveType.eCastling_White_KingSide_Move) > 0 )
			return true;
	
		return false;		
	}
	
	public static bool IsWhiteQueenSideCastlingMove( MoveType moveType ) {
		
		if( (moveType & MoveType.eCastling_White_QueenSide_Move) > 0 )
			return true;
	
		return false;		
	}	
	
	public static bool IsBlackKingSideCastlingMove( MoveType moveType ) {
		
		if( (moveType & MoveType.eCastling_Black_KingSide_Move) > 0 )
			return true;
	
		return false;		
	}	
	
	public static bool IsBlackQueenSideCastlingMove( MoveType moveType ) {
		
		if( (moveType & MoveType.eCastling_Black_QueenSide_Move) > 0 )
			return true;
	
		return false;		
	}	
	
	
	public class sMove {
		
		public MoveType moveType;			
		
		public ChessBoardSquare trgSquare;
		public ChessBoardSquare srcSquare;		
		public ChessBoardSquare capturedSquare;		
		
		public sMove() {
			
			this.moveType = MoveType.eNone_Move;		
			
			this.trgSquare = null;
			this.srcSquare = null;
			this.capturedSquare = null;			
		}	
		
		public sMove( ChessBoardSquare srcSquare, ChessBoardSquare trgSquare, MoveType moveType, ChessBoardSquare capturedSquare = null ) {
			
			this.moveType = moveType;			
			
			this.trgSquare = trgSquare;
			this.srcSquare = srcSquare;
			this.capturedSquare = capturedSquare;			
		}			
		
		public void Set( ChessBoardSquare srcSquare, ChessBoardSquare trgSquare, MoveType moveType, ChessBoardSquare capturedSquare = null ) {
			
			this.moveType = moveType;			
			
			this.trgSquare = trgSquare;
			this.srcSquare = srcSquare;
			this.capturedSquare = capturedSquare;			
		}		
		
		public void Set( sMove move ) {
			
			this.moveType = move.moveType;			
			
			this.trgSquare = move.trgSquare;
			this.srcSquare = move.srcSquare;
			this.capturedSquare = move.capturedSquare;			
		}
		
		public void Clear() {
			
			this.moveType = MoveType.eNone_Move;		
			
			this.trgSquare = null;
			this.srcSquare = null;
			this.capturedSquare = null;			
		}
		
		// is Increse half move
		public bool IsResetHalfMove() {
			
			if( (moveType & MoveType.ePawn_Move) > 0 || 
				(moveType & MoveType.eCapture_Move) > 0 || 
				(moveType & MoveType.eCastling_Move) > 0 )
				return true;
			return false;
		}
	}		
	
	// validate move position
	public static bool GetValidateMoveList( ChessBoard board, ChessBoardSquare selSquare, List<sMove> listRetBoardPos ) {		
		
		bool bRet = false;
		
		if( selSquare == null )
		{
			UnityEngine.Debug.LogError( "GetValidateMoveList - selSquare - selSquare == null" );
			return bRet;	
		}
		
		if( selSquare.IsBlank() )
		{
			UnityEngine.Debug.LogError( "GetValidateMoveList - selSquare - IsBlank()" );
			return bRet;	
		}
		
		if( selSquare.IsInvalidPos() )
		{
			UnityEngine.Debug.LogError( "GetValidateMoveList - selSquare - IsInvalidPos()" );
			return bRet;	
		}
			
		switch( selSquare.piece.pieceType ) {
			
			case PieceType.e_King:
			{
				bRet = GetKingMoveList( board, selSquare, listRetBoardPos );				
			}
			break;
			
			case PieceType.e_Queen:
			{
				bRet = GetQueenMoveList( board, selSquare, listRetBoardPos );			
			}	
			break;
			
			case PieceType.e_Rook:
			{
				bRet = GetRookMoveList( board, selSquare, listRetBoardPos );			
			}
			break;
			
			case PieceType.e_Bishop:
			{
				bRet = GetBishopMoveList( board, selSquare, listRetBoardPos );
			}
			break;
			
			case PieceType.e_Knight:
			{
				bRet = GetKnightMoveList( board, selSquare, listRetBoardPos );		
			}
			break;
			
			case PieceType.e_Pawn:
			{
				bRet = GetPawnMoveList( board, selSquare, listRetBoardPos );						
			}				
			break;
			
			default:	
			{
			}
			break;
		}		
		
		return bRet;
	}
	
	public static void BitBoardToMoveList( ulong ulBitboard, MoveType moveType,
		ChessBoard board, ChessBoardSquare srcSquare, List<sMove> listRetBoardPos ) {
		
		for( int i=0; i<ChessData.nNumBoardSquare; i++ ) {
			
			ulong ulCurrMask = (ulong)1 << i;
			ulong ulCurrPosition = ulBitboard & ulCurrMask;
			if( ulCurrPosition > 0 ) {
				
				int nCurrRank, nCurrFile;
				nCurrRank = i % ChessData.nNumRank;
				nCurrFile = i / ChessData.nNumPile;
				
				ChessBoardSquare trgSquare = board.aBoardSquare[nCurrFile, nCurrRank];
				
				ChessBoardSquare capturedSquare = null;
				// check capture move
				if( moveType == MoveType.eCapture_Move ) {
					
					if( moveType == MoveType.eEnPassan_Move ) {
						
						int nCapturedPawnRank, nCapturedPawnFile;	
						nCapturedPawnRank = nCurrRank;
						nCapturedPawnFile = srcSquare.piece.playerSide == PlayerSide.e_White ? nCurrFile - ChessData.nNumPile : nCurrFile + ChessData.nNumPile;
						capturedSquare = board.aBoardSquare[nCapturedPawnFile, nCapturedPawnRank];
					}
					else
						capturedSquare = trgSquare;
				}
				
				// check pawn promote move
				if( moveType == MoveType.ePawn_Move ) {
					// promote move	check
					if( srcSquare.piece.playerSide == PlayerSide.e_White ) {
						if( ulCurrMask & ChessBitBoard.firstRank )
							move.moveType |= MoveType.ePromote_Move;								
					}
					else {
						
						if( ulCurrMask & ChessBitBoard.lastRank )
							move.moveType |= MoveType.ePromote_Move;
					}	
				}
				
				sMove move = new sMove();
				// normal move
				move.moveType = moveType;				
			
				move.srcSquare = srcSquare;
				move.trgSquare = trgSquare;
				move.capturedSquare = capturedSquare;
				
				listRetBoardPos.Add( move );				
			}
		}
	}
	
	public static bool GetKingMoveList( ChessBoard board, ChessBoardSquare selSquare, List<sMove> listRetBoardPos ) {
		
		PlayerSide srcPlayerSide = selSquare.piece.playerSide;	
		
		// attack/move!!!!
		
		// calc viable king move
		ulong viableKingMove = board.bitBoard.KingMovesBB( srcPlayerSide );
		if( viableKingMove > 0 ) {
			
			MoveType moveType = MoveType.eNormal_Move;
			// convert move list
			BitBoardToMoveList( viableKingMove, moveType, board, selSquare, listRetBoardPos );
		}
		
		ulong viableKingAttack = board.bitBoard.KingAttacksBB( srcPlayerSide );
		if( viableKingAttack > 0 ) {
			
			MoveType moveType = MoveType.eCapture_Move;			
			// convert move list
			BitBoardToMoveList( viableKingAttack, moveType, board, selSquare, listRetBoardPos );
		}		
		
		
		// castling!!!!		
		// king side castling	
		ulong viableKingCastling = board.bitBoard.KingAttacksBB( srcPlayerSide );
		if( viableKingCastling > 0 ) {
			
			MoveType moveType = MoveType.eCastling_Move;
			
			if( srcPlayerSide == PlayerSide.e_White ) {
				if( board.bitBoard.currCastlingState.IsWhiteKingSideAvailable() )
					moveType |= MoveType.eCastling_White_KingSide_Move;
				if( board.bitBoard.currCastlingState.IsWhiteQueenSideAvailable() )
					moveType |= MoveType.eCastling_White_QueenSide_Move;
			}
			else{
				if( board.bitBoard.currCastlingState.IsBlackKingSideAvailable() )
					moveType |= MoveType.eCastling_Black_KingSide_Move;
				if( board.bitBoard.currCastlingState.IsWhiteQueenSideAvailable() )
					moveType |= MoveType.eCastling_Black_QueenSide_Move;
			}
			
			// convert move list
			BitBoardToMoveList( viableKingCastling, moveType, board, selSquare, listRetBoardPos );
		}			
		
		return listRetBoardPos.Count > 0;
	}
	
	
	
	public static bool GetPawnMoveList( ChessBoard board, ChessBoardSquare selSquare, List<sMove> listRetBoardPos ) {				
		
		//UnityEngine.Debug.LogError( "GetPawnMoveList - start" + " " + piece.position + " " + piece.playerSide );
		
		PlayerSide srcPlayerSide = selSquare.piece.playerSide;	
		int nSrcPawnSq = (int)selSquare.position.pos;
		
		// attack/move!!!!		
		// calc viable Pawn one move
		ulong viablePawnOneMove = board.bitBoard.PawnOneMovesBB( srcPlayerSide, nSrcPawnSq );
		if( viablePawnOneMove > 0 ) {
			
			MoveType moveType = MoveType.eNormal_Move | MoveType.ePawn_Move;			
			// convert move list
			BitBoardToMoveList( viablePawnOneMove, moveType, board, selSquare, listRetBoardPos );
		}
		
		// calc viable Pawn two move
		ulong viablePawnTwoMove = board.bitBoard.PawnTwoMovesBB( srcPlayerSide, nSrcPawnSq );
		if( viablePawnTwoMove > 0 ) {
			
			MoveType moveType = MoveType.eNormal_Move | MoveType.ePawn_Move | MoveType.ePawn_Two_Move;		
			// convert move list
			BitBoardToMoveList( viablePawnTwoMove, moveType, board, selSquare, listRetBoardPos );
		}		
		
		// calc viable Pawn attack move
		ulong viablePawnAttackMove = board.bitBoard.PawnAttackMovesBB( srcPlayerSide, nSrcPawnSq );
		if( viablePawnAttackMove > 0 ) {
			
			MoveType moveType = MoveType.eCapture_Move | MoveType.ePawn_Move;	
			// convert move list
			BitBoardToMoveList( viablePawnAttackMove, moveType, board, selSquare, listRetBoardPos );
		}
		
		// calc viable Pawn attack move
		ulong viablePawnEnpassantMove = board.bitBoard.PawnEnpassantMovesBB( srcPlayerSide, nSrcPawnSq );
		if( viablePawnEnpassantMove > 0 ) {
			
			MoveType moveType = MoveType.eEnPassan_Move | MoveType.ePawn_Move;	
			// convert move list
			BitBoardToMoveList( viablePawnEnpassantMove, moveType, board, selSquare, listRetBoardPos );
		}
		
		return listRetBoardPos.Count > 0;	
	}	
	
	
	/*
	public static bool GetKingMoveList( ChessBoard board, ChessBoardSquare selSquare, List<sMove> listRetBoardPos ) {	
		
		ChessPosition srcPos = selSquare.position;		
		PlayerSide srcPlayerSide = selSquare.piece.playerSide;					
		
		ChessPosition movePos = new ChessPosition(srcPos.pos);					
		
		// all(radial) direction one move		
		int nTempRank, nTempPile;
		
		for( int nMovePile=-1; nMovePile<=1; nMovePile++ ) {
			for( int nMoveRank=-1; nMoveRank<=1; nMoveRank++ ) {
			
				nTempRank = nMoveRank;		
				nTempPile = nMovePile;				
				
				movePos.SetPosition( srcPos );				
				bool bValidMove = movePos.MovePosition( nTempRank, nTempPile );
				if( bValidMove ) {					
					
					ChessBoardSquare trgSquare = board.aBoardSquare[movePos.nPile, movePos.nRank];
					// normal move				
					if( trgSquare.IsBlank() ) {																																											
						
						sMove move = new sMove();
						// normal move
						move.moveType = MoveType.eNormal_Move;						
					
						move.srcSquare = selSquare;
						move.trgSquare = trgSquare;
						
						listRetBoardPos.Add( move );
					}
					// capture move
					else if( trgSquare.IsEnemy( srcPlayerSide ) ) {
						
						sMove move = new sMove();
						// normal move
						move.moveType = MoveType.eCapture_Move;					
					
						move.srcSquare = selSquare;
						move.trgSquare = trgSquare;
						
						listRetBoardPos.Add( move );
					}
				}
			}
		}
		
		// castling move
		// king side castling			
		movePos.SetPosition( srcPos );
		
		nTempRank = 2;		
		nTempPile = 0;
		
		bool bValidCastlingMove = movePos.MovePosition( nTempRank, nTempPile );
		if( bValidCastlingMove ) {
		
			ChessBoardSquare trgSquare = board.aBoardSquare[movePos.nPile, movePos.nRank];
			if(	trgSquare.IsBlank() ) {
				
				// position check castling
				bool bCalstling = false;
				if( srcPlayerSide == PlayerSide.e_White ) {
					
					if( board.currCastlingState.CastlingWKSide == CastlingState.eCastling_Enable_State ) {											
						bCalstling = true;
					}
				}
				else {
					if( board.currCastlingState.CastlingBKSide == CastlingState.eCastling_Enable_State ) {											
						bCalstling = true;
					}
				}
				
				if( bCalstling ) {
					
					// check rook square blank
					nTempRank = -1;		
					nTempPile = 0;				
						
					ChessPosition moveRookPos = new ChessPosition(movePos.pos);						
					
					bValidCastlingMove = moveRookPos.MovePosition( nTempRank, nTempPile );
					if( bValidCastlingMove ) {
						
						ChessBoardSquare rookTrgSquare = board.aBoardSquare[moveRookPos.nPile, moveRookPos.nRank];
						if(	rookTrgSquare.IsBlank() ) {
							
							sMove move = new sMove();
							
							MoveType castlingSideType = srcPlayerSide == 
								PlayerSide.e_White ? MoveType.eCastling_White_KingSide_Move : MoveType.eCastling_Black_KingSide_Move;
							move.moveType = MoveType.eCastling_Move | castlingSideType;						
						
							move.srcSquare = selSquare;
							move.trgSquare = trgSquare;
							
							listRetBoardPos.Add( move );
						}
					}					
				}																
			}
		}
		
		// queen side castling		
		movePos.SetPosition( srcPos );
		
		nTempRank = -2;		
		nTempPile = 0;
		
		bValidCastlingMove = movePos.MovePosition( nTempRank, nTempPile );
		if( bValidCastlingMove ) {
		
			ChessBoardSquare trgSquare = board.aBoardSquare[movePos.nPile, movePos.nRank];
			if(	trgSquare.IsBlank() ) {
				
				// position check castling
				bool bCalstling = false;
				if( srcPlayerSide == PlayerSide.e_White ) {
					
					if( board.currCastlingState.CastlingWQSide == CastlingState.eCastling_Enable_State ) {											
						bCalstling = true;
					}
				}
				else {
					if( board.currCastlingState.CastlingBQSide == CastlingState.eCastling_Enable_State ) {											
						bCalstling = true;
					}
				}
				
				if( bCalstling ) {
					
					// check rook square blank
					nTempRank = 1;		
					nTempPile = 0;				
						
					ChessPosition moveRookPos = new ChessPosition(movePos.pos);						
					bValidCastlingMove = moveRookPos.MovePosition( nTempRank, nTempPile );
					if( bValidCastlingMove ) {
						
						ChessBoardSquare rookTrgSquare = board.aBoardSquare[moveRookPos.nPile, moveRookPos.nRank];
						if(	rookTrgSquare.IsBlank() ) {
							
							sMove move = new sMove();
							
							MoveType castlingSideType = srcPlayerSide == PlayerSide.e_White ? 
								MoveType.eCastling_White_QueenSide_Move : MoveType.eCastling_Black_QueenSide_Move;
							move.moveType = MoveType.eCastling_Move | castlingSideType;							
						
							move.srcSquare = selSquare;
							move.trgSquare = trgSquare;
							
							listRetBoardPos.Add( move );
						}
					}					
				}																
			}
		}		
		
		return true;
	}
	
	public static bool GetQueenMoveList( ChessBoard board, ChessBoardSquare selSquare, List<sMove> listRetBoardPos ) {		
		
		// up
		GetStraightMoveList( board, selSquare, listRetBoardPos, MoveDirectionType.eDirection_Move_Up );
		// down
		GetStraightMoveList( board, selSquare, listRetBoardPos, MoveDirectionType.eDirection_Move_Down );
		// left
		GetStraightMoveList( board, selSquare, listRetBoardPos, MoveDirectionType.eDirection_Move_Left );
		// right
		GetStraightMoveList( board, selSquare, listRetBoardPos, MoveDirectionType.eDirection_Move_Right );
		// left-up - diagonal
		GetStraightMoveList( board, selSquare, listRetBoardPos, MoveDirectionType.eDirection_Move_LeftUp_Diagonal );
		// left-down - diagonal
		GetStraightMoveList( board, selSquare, listRetBoardPos, MoveDirectionType.eDirection_Move_LeftDown_Diagonal );
		// right-up - diagonal
		GetStraightMoveList( board, selSquare, listRetBoardPos, MoveDirectionType.eDirection_Move_RightUp_Diagonal );
		// right-down - diagonal
		GetStraightMoveList( board, selSquare, listRetBoardPos, MoveDirectionType.eDirection_Move_RightDown_Diagonal );
		
		if( listRetBoardPos.Count > 0 )
			return true;
		
		return false;
	}
	
	public static bool GetRookMoveList( ChessBoard board, ChessBoardSquare selSquare, List<sMove> listRetBoardPos ) {		
		
		// up
		GetStraightMoveList( board, selSquare, listRetBoardPos, MoveDirectionType.eDirection_Move_Up );
		// down
		GetStraightMoveList( board, selSquare, listRetBoardPos, MoveDirectionType.eDirection_Move_Down );
		// left
		GetStraightMoveList( board, selSquare, listRetBoardPos, MoveDirectionType.eDirection_Move_Left );
		// right
		GetStraightMoveList( board, selSquare, listRetBoardPos, MoveDirectionType.eDirection_Move_Right );
		
		if( listRetBoardPos.Count > 0 )
			return true;
		
		return false;
	}
	
	public static bool GetBishopMoveList( ChessBoard board, ChessBoardSquare selSquare, List<sMove> listRetBoardPos ) {		
		
		// left-up - diagonal
		GetStraightMoveList( board, selSquare, listRetBoardPos, MoveDirectionType.eDirection_Move_LeftUp_Diagonal );
		// left-down - diagonal
		GetStraightMoveList( board, selSquare, listRetBoardPos, MoveDirectionType.eDirection_Move_LeftDown_Diagonal );
		// right-up - diagonal
		GetStraightMoveList( board, selSquare, listRetBoardPos, MoveDirectionType.eDirection_Move_RightUp_Diagonal );
		// right-down - diagonal
		GetStraightMoveList( board, selSquare, listRetBoardPos, MoveDirectionType.eDirection_Move_RightDown_Diagonal );
		
		if( listRetBoardPos.Count > 0 )
			return true;
		
		return false;
	}
	
	public static bool GetKnightMoveList( ChessBoard board, ChessBoardSquare selSquare, List<sMove> listRetBoardPos ) {	
		
		// left-up - steep diagonal
		GetLeapMoveList( board, selSquare, listRetBoardPos, MoveDirectionType.eDirection_Move_Steep_LeftUp_Leap );
		// left-down - steep diagonal
		GetLeapMoveList( board, selSquare, listRetBoardPos, MoveDirectionType.eDirection_Move_Steep_LeftDown_Leap );
		// right-up - steep diagonal
		GetLeapMoveList( board, selSquare, listRetBoardPos, MoveDirectionType.eDirection_Move_Steep_RightUp_Leap );
		// right-down - steep diagonal
		GetLeapMoveList( board, selSquare, listRetBoardPos, MoveDirectionType.eDirection_Move_Steep_RightDown_Leap );
		
		// left-up - non-steep diagonal
		GetLeapMoveList( board, selSquare, listRetBoardPos, MoveDirectionType.eDirection_Move_NonSteep_LeftUp_Leap );
		// left-down - on-steep diagonal
		GetLeapMoveList( board, selSquare, listRetBoardPos, MoveDirectionType.eDirection_Move_NonSteep_LeftDown_Leap );
		// right-up - on-steep diagonal
		GetLeapMoveList( board, selSquare, listRetBoardPos, MoveDirectionType.eDirection_Move_NonSteep_RightUp_Leap );
		// right-down - on-steep diagonal
		GetLeapMoveList( board, selSquare, listRetBoardPos, MoveDirectionType.eDirection_Move_NonSteep_RightDown_Leap );
		
		if( listRetBoardPos.Count > 0 )
			return true;
		
		return false;	
	}
	
	// sub move method
	// helper method
	public static int GetNumDirectionIterCount( int nCurrRank, int nCurrPile, MoveDirectionType moveDirection ) {
		
		int nNumRamnatSqure = 0, nNumRamnatRank, nNumRamnatPile;		
		switch( moveDirection ) {
			
			case MoveDirectionType.eDirection_Move_Left:
			{
				nNumRamnatSqure = nCurrRank;
			}
			break;
				
			case MoveDirectionType.eDirection_Move_Right:
			{
				nNumRamnatSqure = ChessData.nNumRank - (nCurrRank + 1);
			}
			break;
				
			case MoveDirectionType.eDirection_Move_Up:
			{
				nNumRamnatSqure = ChessData.nNumPile - (nCurrPile + 1);
			}
			break;
				
			case MoveDirectionType.eDirection_Move_Down:
			{				
				nNumRamnatSqure = nCurrPile;
			}
			break;	
			
			case MoveDirectionType.eDirection_Move_LeftUp_Diagonal:
			{
				nNumRamnatRank = nCurrRank;
				nNumRamnatPile = ChessData.nNumPile - (nCurrPile + 1);
				nNumRamnatSqure = Math.Min( nNumRamnatRank, nNumRamnatPile );
			}
			break;
				
			case MoveDirectionType.eDirection_Move_LeftDown_Diagonal:
			{
				nNumRamnatRank = nCurrRank;
				nNumRamnatPile = nCurrPile;
				nNumRamnatSqure = Math.Min( nNumRamnatRank, nNumRamnatPile );
			}
			break;
				
			case MoveDirectionType.eDirection_Move_RightUp_Diagonal:
			{
				nNumRamnatRank = ChessData.nNumRank - (nCurrRank + 1);
				nNumRamnatPile = ChessData.nNumPile - (nCurrPile + 1);
				nNumRamnatSqure = Math.Min( nNumRamnatRank, nNumRamnatPile );
			}
			break;
				
			case MoveDirectionType.eDirection_Move_RightDown_Diagonal:
			{
				nNumRamnatRank = ChessData.nNumRank - (nCurrRank + 1);
				nNumRamnatPile = nCurrPile;
				nNumRamnatSqure = Math.Min( nNumRamnatRank, nNumRamnatPile );
			}
			break;		
		}
		
		return nNumRamnatSqure;
	}
	
	public static void GetNextDirectionRankPile( ref int nNextRank, ref int nNextPile, MoveDirectionType moveDirection, int nCurrIter ) {
			
		switch( moveDirection ) {
			
			case MoveDirectionType.eDirection_Move_Left:
			{
				nNextRank = -nCurrIter;
			}
			break;
				
			case MoveDirectionType.eDirection_Move_Right:
			{
				nNextRank = nCurrIter;
			}
			break;
				
			case MoveDirectionType.eDirection_Move_Up:
			{
				nNextPile = nCurrIter;
			}
			break;
				
			case MoveDirectionType.eDirection_Move_Down:
			{				
				nNextPile = -nCurrIter;
			}
			break;	
			
			case MoveDirectionType.eDirection_Move_LeftUp_Diagonal:
			{
				nNextRank = -nCurrIter;
				nNextPile = nCurrIter;
			}
			break;
				
			case MoveDirectionType.eDirection_Move_LeftDown_Diagonal:
			{
				nNextRank = -nCurrIter;
				nNextPile = -nCurrIter;
			}
			break;
				
			case MoveDirectionType.eDirection_Move_RightUp_Diagonal:
			{
				nNextRank = nCurrIter;
				nNextPile = nCurrIter;
			}
			break;
				
			case MoveDirectionType.eDirection_Move_RightDown_Diagonal:
			{
				nNextRank = nCurrIter;
				nNextPile = -nCurrIter;
			}
			break;
			
			case MoveDirectionType.eDirection_Move_Steep_LeftUp_Leap:
			{
				nNextRank = -1;
				nNextPile = 2;
			}
			break;
				
			case MoveDirectionType.eDirection_Move_Steep_LeftDown_Leap:
			{
				nNextRank = -1;
				nNextPile = -2;
			}
			break;
				
			case MoveDirectionType.eDirection_Move_Steep_RightUp_Leap:
			{
				nNextRank = 1;
				nNextPile = 2;
			}
			break;
				
			case MoveDirectionType.eDirection_Move_Steep_RightDown_Leap:
			{
				nNextRank = 1;
				nNextPile = -2;
			}
			break;
			
			case MoveDirectionType.eDirection_Move_NonSteep_LeftUp_Leap:
			{
				nNextRank = -2;
				nNextPile = 1;
			}
			break;
				
			case MoveDirectionType.eDirection_Move_NonSteep_LeftDown_Leap:
			{
				nNextRank = -2;
				nNextPile = -1;
			}
			break;
				
			case MoveDirectionType.eDirection_Move_NonSteep_RightUp_Leap:
			{
				nNextRank = 2;
				nNextPile = 1;
			}
			break;
				
			case MoveDirectionType.eDirection_Move_NonSteep_RightDown_Leap:
			{
				nNextRank = 2;
				nNextPile = -1;
			}
			break;
		}	
	}
	
	// stright line move
	public static bool GetStraightMoveList( ChessBoard board, ChessBoardSquare selSquare, List<sMove> listRetBoardPos, MoveDirectionType moveDirection ) {	
		
		ChessPosition srcPos = selSquare.position;		
		PlayerSide srcPlayerSide = selSquare.piece.playerSide;				
		
		ChessPosition movePos = new ChessPosition(srcPos.pos);	
		
		// all(radial) direction one move		
		int nTempRank, nTempPile;
		
		int nIterCount;		
		nIterCount = GetNumDirectionIterCount( movePos.nRank, movePos.nPile, moveDirection );
		//UnityEngine.Debug.LogError( "GetStraightMoveList() - nIterCount = " + nIterCount + " movePos.nRank, movePos.nPile " + movePos.nRank + " " + movePos.nPile );
		
		for( int nCurrIter=1; nCurrIter<=nIterCount; nCurrIter++ ) {
			
			nTempRank = 0;		
			nTempPile = 0;
			
			GetNextDirectionRankPile( ref nTempRank, ref nTempPile, moveDirection, nCurrIter );						
			//UnityEngine.Debug.LogError( "GetStraightMoveList() - nTempRank, nTempPile " + nTempRank + " " + nTempPile );
			
			movePos.SetPosition( srcPos );				
			bool bValidMove = movePos.MovePosition( nTempRank, nTempPile );
			if( bValidMove ) {					
				
				//UnityEngine.Debug.LogError( "GetStraightMoveList() - bValidMove - nTempRank, nTempPile " + nTempRank + " " + nTempPile );
				
				ChessBoardSquare trgSquare = board.aBoardSquare[movePos.nPile, movePos.nRank];
				// normal move				
				if( trgSquare.IsBlank() ) {																																											
					
					sMove move = new sMove();
					
					// normal move
					move.moveType = MoveType.eNormal_Move;				
				
					move.srcSquare = selSquare;
					move.trgSquare = trgSquare;					
					
					listRetBoardPos.Add( move );					
				}				
				// capture move
				else if( trgSquare.IsEnemy( srcPlayerSide ) ) {
					
					sMove move = new sMove();
					
					// normal move
					move.moveType = MoveType.eCapture_Move;											
				
					move.srcSquare = selSquare;
					move.trgSquare = trgSquare;		
					
					listRetBoardPos.Add( move );
					
					return true;
				}				
				// our piece
				else {
					
					if( nCurrIter > 1 )
						return true;
					return false;
				}
			}			
		}
		
		return false;
	}	
	
	
	// leap move
	public static bool GetLeapMoveList( ChessBoard board, ChessBoardSquare selSquare, List<sMove> listRetBoardPos, MoveDirectionType moveDirection ) {	
		
		ChessPosition srcPos = selSquare.position;		
		PlayerSide srcPlayerSide = selSquare.piece.playerSide;							
		
		ChessPosition movePos = new ChessPosition(srcPos.pos);		
		
		// all(radial) direction one move		
		int nTempRank = 0, nTempPile = 0;			
		
		GetNextDirectionRankPile( ref nTempRank, ref nTempPile, moveDirection, 0 );						
		
		movePos.SetPosition( srcPos );				
		bool bValidMove = movePos.MovePosition( nTempRank, nTempPile );
		if( bValidMove ) {					
			
			ChessBoardSquare trgSquare = board.aBoardSquare[movePos.nPile, movePos.nRank];
			// normal move				
			if( trgSquare.IsBlank() ) {																																											
				
				sMove move = new sMove();
				
				// normal move
				move.moveType = MoveType.eNormal_Move;			
			
				move.srcSquare = selSquare;
				move.trgSquare = trgSquare;	
				
				listRetBoardPos.Add( move );
				
				return true;
			}				
			// capture move
			else if( trgSquare.IsEnemy( srcPlayerSide ) ) {
				
				sMove move = new sMove();
				
				// normal move
				move.moveType = MoveType.eCapture_Move;				
			
				move.srcSquare = selSquare;
				move.trgSquare = trgSquare;	
				
				listRetBoardPos.Add( move );
				
				return true;
			}				
			// our piece
			else {				
				
				return false;
			}
		}			
		
		return false;
	}	
	*/
	
	
	
	
	static ChessMoveManager() {
	}
}

//}
