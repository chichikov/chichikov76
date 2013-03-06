using UnityEngine;
using System.Collections;

using System;
using System.Collections.Generic;


//namespace BattleChess {	

public class ChessMover {	
	
	
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
		
		// for ponder
		public string GetFenString() {
			
			if( srcSquare == null || trgSquare == null ) {
				
				UnityEngine.Debug.LogError( "sMove::GetFenString() - src or trg square is null!!" );
			}				
			
			return srcSquare.GetFenString() + " " + trgSquare.GetFenString();
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
				
				sMove move = new sMove();
				
				// check capture move
				if( ChessMover.IsCaptureMove( moveType ) ) {					
					
					if( ChessMover.IsEnpassantMove( moveType ) ) {
						
						int nCapturedPawnRank, nCapturedPawnFile;	
						nCapturedPawnRank = nCurrRank;
						nCapturedPawnFile = srcSquare.piece.playerSide == PlayerSide.e_White ? nCurrFile - ChessData.nNumPile : nCurrFile + ChessData.nNumPile;
						capturedSquare = board.aBoardSquare[nCapturedPawnFile, nCapturedPawnRank];
					}
					else
						capturedSquare = trgSquare;
				}
				
				// check pawn promote move
				if( ChessMover.IsPawnMove( moveType ) ) {
					// promote move	check
					if( srcSquare.piece.playerSide == PlayerSide.e_White ) {
						if( (ulCurrMask & ChessBitBoard.firstRank) > 0 )
							move.moveType |= MoveType.ePromote_Move;								
					}
					else {
						
						if( (ulCurrMask & ChessBitBoard.lastRank) > 0 )
							move.moveType |= MoveType.ePromote_Move;
					}	
				}
				
				// normal move
				move.moveType = moveType;				
			
				move.srcSquare = srcSquare;
				move.trgSquare = trgSquare;
				move.capturedSquare = capturedSquare;
				
				// check mate state	
				// virtually move
				board.bitBoardVirtual.CopyFrom( board.bitBoard );
				board.bitBoardVirtual.MoveUpdate( move );
				
				bool bWillCheckMateState = false;
				if( srcSquare.piece.playerSide == PlayerSide.e_White ) {
					
					if( board.bitBoardVirtual.IsWhiteKingInCheck() ) {
						bWillCheckMateState = true;						 
					}
				}
				else if( srcSquare.piece.playerSide == PlayerSide.e_Black ) {
					
					if( board.bitBoardVirtual.IsBlackKingInCheck() ) {
						bWillCheckMateState = true;						 
					}
				}
				
				// unmove virtually				
				
				// skip this move
				if( bWillCheckMateState )
					continue;
				
				
				// for debug				
				foreach( sMove aMove in listRetBoardPos ) {
					
					if( aMove.trgSquare == move.trgSquare ) {
						UnityEngine.Debug.Log( "!!!!!!!!!!!!!!!!!!!!!!!!ChessMover::BitBoardToMoveList() - move collision Aleady exist!!!!   " +
						 	"file : " + move.trgSquare.position.nPile + "   Rank : " + move.trgSquare.position.nRank );
						
						string strOccupied = string.Format( "occupied : {0:X}", board.bitBoard.occupiedBB );
						UnityEngine.Debug.LogError( "!!!!!!!!!!!!!!!!!!!!!!!!ChessMover::BitBoardToMoveList() - " + strOccupied );
						
						string strEmpty = string.Format( "empty : {0:X}", board.bitBoard.emptyBB );
						UnityEngine.Debug.LogError( "!!!!!!!!!!!!!!!!!!!!!!!!ChessMover::BitBoardToMoveList() - " + strEmpty );
					}
				}
				
				listRetBoardPos.Add( move );				
			}
		}
	}
	
	public static bool GetKingMoveList( ChessBoard board, ChessBoardSquare selSquare, List<sMove> listRetBoardPos ) {
		
		PlayerSide srcPlayerSide = selSquare.piece.playerSide;
		int nSrcKingSq = (int)selSquare.position.pos;
		
		// attack/move!!!!
		
		// calc viable king move
		ulong viableKingMove = board.bitBoard.KingMovesBB( (int)srcPlayerSide, nSrcKingSq );
		if( viableKingMove > 0 ) {
			
			MoveType moveType = MoveType.eNormal_Move;
			// convert move list
			BitBoardToMoveList( viableKingMove, moveType, board, selSquare, listRetBoardPos );
		}
		
		ulong viableKingAttack = board.bitBoard.KingAttacksBB( (int)srcPlayerSide, nSrcKingSq );
		if( viableKingAttack > 0 ) {
			
			MoveType moveType = MoveType.eCapture_Move;			
			// convert move list
			BitBoardToMoveList( viableKingAttack, moveType, board, selSquare, listRetBoardPos );
		}		
		
		
		// castling!!!!		
		// king side castling	
		ulong viableKingCastling = board.bitBoard.KingCastlingBB( (int)srcPlayerSide );
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
		ulong viablePawnOneMove = board.bitBoard.PawnOneMovesBB( (int)srcPlayerSide, nSrcPawnSq );
		if( viablePawnOneMove > 0 ) {
			
			MoveType moveType = MoveType.eNormal_Move | MoveType.ePawn_Move;			
			// convert move list
			BitBoardToMoveList( viablePawnOneMove, moveType, board, selSquare, listRetBoardPos );
		}
		
		// calc viable Pawn two move
		ulong viablePawnTwoMove = board.bitBoard.PawnTwoMovesBB( (int)srcPlayerSide, nSrcPawnSq );
		if( viablePawnTwoMove > 0 ) {
			
			MoveType moveType = MoveType.eNormal_Move | MoveType.ePawn_Move | MoveType.ePawn_Two_Move;		
			// convert move list
			BitBoardToMoveList( viablePawnTwoMove, moveType, board, selSquare, listRetBoardPos );
		}		
		
		// calc viable Pawn attack move
		ulong viablePawnAttackMove = board.bitBoard.PawnAttackMovesBB( (int)srcPlayerSide, nSrcPawnSq );
		if( viablePawnAttackMove > 0 ) {
			
			MoveType moveType = MoveType.eCapture_Move | MoveType.ePawn_Move;	
			// convert move list
			BitBoardToMoveList( viablePawnAttackMove, moveType, board, selSquare, listRetBoardPos );
		}
		
		// calc viable Pawn attack move
		ulong viablePawnEnpassantMove = board.bitBoard.PawnEnpassantMovesBB( (int)srcPlayerSide, nSrcPawnSq );
		if( viablePawnEnpassantMove > 0 ) {
			
			MoveType moveType = MoveType.eEnPassan_Move | MoveType.ePawn_Move;	
			// convert move list
			BitBoardToMoveList( viablePawnEnpassantMove, moveType, board, selSquare, listRetBoardPos );
		}
		
		return listRetBoardPos.Count > 0;	
	}		
	
	public static bool GetQueenMoveList( ChessBoard board, ChessBoardSquare selSquare, List<sMove> listRetBoardPos ) {		
		
		PlayerSide srcPlayerSide = selSquare.piece.playerSide;	
		int nSrcQeenSq = (int)selSquare.position.pos;
		
		// attack/move!!!!		
		// calc viable queen move
		ulong viableQueenMove = board.bitBoard.QueenMovesBB( nSrcQeenSq );
		if( viableQueenMove > 0 ) {
			
			MoveType moveType = MoveType.eNormal_Move;
			// convert move list
			BitBoardToMoveList( viableQueenMove, moveType, board, selSquare, listRetBoardPos );
		}
		
		// calc viable queen attack
		ulong viableQueenAttack = board.bitBoard.QueenAttacksBB( (int)srcPlayerSide, nSrcQeenSq );
		if( viableQueenAttack > 0 ) {
			
			MoveType moveType = MoveType.eCapture_Move;
			// convert move list
			BitBoardToMoveList( viableQueenAttack, moveType, board, selSquare, listRetBoardPos );
		}
		
		return listRetBoardPos.Count > 0;	
	}
	
	public static bool GetRookMoveList( ChessBoard board, ChessBoardSquare selSquare, List<sMove> listRetBoardPos ) {		
		
		PlayerSide srcPlayerSide = selSquare.piece.playerSide;	
		int nSrcRookSq = (int)selSquare.position.pos;
		
		// attack/move!!!!		
		// calc viable queen move
		ulong viableRookMove = board.bitBoard.RookMovesBB( nSrcRookSq );
		if( viableRookMove > 0 ) {
			
			MoveType moveType = MoveType.eNormal_Move;
			// convert move list
			BitBoardToMoveList( viableRookMove, moveType, board, selSquare, listRetBoardPos );
		}
		
		// calc viable queen attack
		ulong viableRookAttack = board.bitBoard.RookAttacksBB( (int)srcPlayerSide, nSrcRookSq );
		if( viableRookAttack > 0 ) {
			
			MoveType moveType = MoveType.eCapture_Move;
			// convert move list
			BitBoardToMoveList( viableRookAttack, moveType, board, selSquare, listRetBoardPos );
		}
		
		return listRetBoardPos.Count > 0;			
	}
	
	public static bool GetBishopMoveList( ChessBoard board, ChessBoardSquare selSquare, List<sMove> listRetBoardPos ) {		
		
		PlayerSide srcPlayerSide = selSquare.piece.playerSide;	
		int nSrcBishopSq = (int)selSquare.position.pos;
		
		// attack/move!!!!		
		// calc viable queen move
		ulong viableBishopMove = board.bitBoard.BishopMovesBB( nSrcBishopSq );
		if( viableBishopMove > 0 ) {
			
			MoveType moveType = MoveType.eNormal_Move;
			// convert move list
			BitBoardToMoveList( viableBishopMove, moveType, board, selSquare, listRetBoardPos );
		}
		
		// calc viable queen attack
		ulong viableBishopAttack = board.bitBoard.BishopAttacksBB( (int)srcPlayerSide, nSrcBishopSq );
		if( viableBishopAttack > 0 ) {
			
			MoveType moveType = MoveType.eCapture_Move;
			// convert move list
			BitBoardToMoveList( viableBishopAttack, moveType, board, selSquare, listRetBoardPos );
		}
		
		return listRetBoardPos.Count > 0;	
	}
	
	public static bool GetKnightMoveList( ChessBoard board, ChessBoardSquare selSquare, List<sMove> listRetBoardPos ) {	
		
		PlayerSide srcPlayerSide = selSquare.piece.playerSide;	
		int nSrcKnightSq = (int)selSquare.position.pos;
		
		// attack/move!!!!		
		// calc viable queen move
		ulong viableKnightMove = board.bitBoard.KnightMovesBB( nSrcKnightSq );
		if( viableKnightMove > 0 ) {
			
			MoveType moveType = MoveType.eNormal_Move;
			// convert move list
			BitBoardToMoveList( viableKnightMove, moveType, board, selSquare, listRetBoardPos );
		}
		
		// calc viable queen attack
		ulong viableKnightAttack = board.bitBoard.KnightAttacksBB( (int)srcPlayerSide, nSrcKnightSq );
		if( viableKnightAttack > 0 ) {
			
			MoveType moveType = MoveType.eCapture_Move;
			// convert move list
			BitBoardToMoveList( viableKnightAttack, moveType, board, selSquare, listRetBoardPos );
		}
		
		return listRetBoardPos.Count > 0;
	}	
	
	static ChessMover() {
	}
}

//}
