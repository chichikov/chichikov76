using UnityEngine;
using System.Collections;



/*
  northwest    north   northeast
  noWe         nort         noEa
          +7    +8    +9
              \  |  /
  west    -1 <-  0 -> +1    east
              /  |  \
          -9    -8    -7
  soWe         sout         soEa
  southwest    south   southeast
*/

public class ChessBitBoard {	
	
	public const ulong ulUniverseBB = 0xffffffffffffffff;
	
	public const ulong notAFile = 0xfefefefefefefefe;
	public const ulong notHFile = 0x7f7f7f7f7f7f7f7f;
	public const ulong notABFile = 0xfcfcfcfcfcfcfcfc;
	public const ulong notGHFile = 0x3f3f3f3f3f3f3f3f;
	
	public const ulong firstRank = 0x00000000000000ff;
	public const ulong lastRank = 0xff00000000000000;
	
	// check for castling
	public const ulong ulWKBlankCastlingMask = 0x0000000000000006;
	public const ulong ulWQBlankCastlingMask = 0x0000000000000070;
	public const ulong ulBKBlankCastlingMask = 0x6000000000000000;
	public const ulong ulBQBlankCastlingMask = 0x0700000000000000;	
	
	public const ulong ulWKCastlingKingSqBB = 0x0000000000000040;
	public const ulong ulWQCastlingKingSqBB = 0x0000000000000004;
	public const ulong ulBKCastlingKingSqBB = 0x0400000000000000;
	public const ulong ulBQCastlingKingSqBB = 0x4000000000000000;	
	
	public const ulong ulWKCastlingRookSqBB = 0x0000000000000080;
	public const ulong ulWQCastlingRookSqBB = 0x0000000000000001;
	public const ulong ulBKCastlingRookSqBB = 0x0100000000000000;
	public const ulong ulBQCastlingRookSqBB = 0x8000000000000000;	
	
	
	public const int NorthDir = 0;
	public const int NorthEastDir = 1;
	public const int EastDir = 2;
	public const int SouthEastDir = 3;
	public const int SouthDir = 4;
	public const int SouthWestDir = 5;
	public const int WestDir = 6;
	public const int NorthWestDir = 7;
	public const int NumRayDir = 8;			
	
	
	public const int WhitePBB = 0;
	public const int BlackPBB = 1;
	public const int WhiteKingPBB = 2;
	public const int BlackKingPBB = 3;
	public const int WhiteQueenPBB = 4;
	public const int BlackQueenPBB = 5;
	public const int WhiteRookPBB = 6;
	public const int BlackRookPBB = 7;
	public const int WhiteBishopPBB = 8;
	public const int BlackBishopPBB = 9;
	public const int WhiteKnightPBB = 10;
	public const int BlackKnightPBB = 11;
	public const int WhitePawnPBB = 12;
	public const int BlackPawnPBB = 13;	
	public const int NumPieceBB = 14;
	
	
	ulong [] pieceBB;
	ulong emptyBB;
	ulong occupiedBB;	
	
	// attack map
	ulong [] arrKingAttacksBB;		
	ulong [] arrKnightAttacksBB;	
	ulong [,] arrPawnAttacksBB;
	
	ulong [,] rayAttacks;
	
	
	// castling
	public ChessCastling currCastlingState;
	
	// en passant target square
	public ChessEnPassant currEnPassantTrgSq;


	
	
	public ChessBitBoard()
	{
		pieceBB = new ulong[NumPieceBB];
		emptyBB = 0;
		occupiedBB = 0;
		
		currEnPassantTrgSq.enpassantCapturSqBB = 0;
		
		InitAttackPattern();		
	}
	
	// initialize game start state
	public void Init() {
		
		// bit board state init	
		pieceBB[WhitePBB] = 0x00000000000000FF;
		pieceBB[BlackPBB] = 0xFF00000000000000;
		pieceBB[WhiteKingPBB] = 0x0000000000000010;
		pieceBB[BlackKingPBB] = 0x1000000000000000;
		pieceBB[WhiteQueenPBB] = 0x0000000000000008;
		pieceBB[BlackQueenPBB] = 0x0800000000000000;
		pieceBB[WhiteRookPBB] = 0x0000000000000081;
		pieceBB[BlackRookPBB] = 0x8100000000000000;
		pieceBB[WhiteBishopPBB] = 0x0000000000000024;
		pieceBB[BlackBishopPBB] = 0x2400000000000000;
		pieceBB[WhiteKnightPBB] = 0x0000000000000042;
		pieceBB[BlackKnightPBB] = 0x4200000000000000;
		pieceBB[WhitePawnPBB] = 0x00000000000000F0;
		pieceBB[BlackPawnPBB] = 0x0F00000000000000;
		
		emptyBB = 0x0000FFFFFFFF0000;
		occupiedBB = 0xFFFF00000000FFFF;
		
		currEnPassantTrgSq.enpassantCapturSqBB = 0;
		
		currCastlingState = new ChessCastling() {
			
			CastlingWKSide = CastlingState.eCastling_Temporary_Disable_State,
			CastlingWQSide = CastlingState.eCastling_Temporary_Disable_State,
			CastlingBKSide = CastlingState.eCastling_Temporary_Disable_State,
			CastlingBQSide = CastlingState.eCastling_Temporary_Disable_State
		};	
	}
	
	public void InitAttackPattern() {
		
		// King attack
		InitKingAttackPattern();	
		
		// Knight attack
		InitKnightAttackPattern();
		
		// pawn attack
		InitPawnAttackPattern();
		
		// ray attack = queen/bishop/rook
		InitRayAttackPattern();
	}
	
	public void InitKingAttackPattern() {	
		
		arrKingAttacksBB = new ulong[64];
		
		ulong sqBB = 1;
		for( int sq = 0; sq < 64; sq++, sqBB <<= 1 )
		   arrKingAttacksBB[sq] = KingAttacks(sqBB);
	}
	
	public void InitKnightAttackPattern() {	
		
		arrKnightAttacksBB = new ulong[64];
		
		ulong sqBB = 1;
		for( int sq = 0; sq < 64; sq++, sqBB <<= 1 )
		   arrKnightAttacksBB[sq] = KnightAttacks(sqBB);		
	}	
	
	public void InitPawnAttackPattern() {			
		
		arrPawnAttacksBB = new ulong[2,64];				
		
		for( int playerSide=0; playerSide<2; ++playerSide ) {
			
			ulong sqBB = 1;			
			for( int sq = 0; sq < 64; sq++, sqBB <<= 1 ) {				
				
				//arrPawnAttacksBB[playerSide,sq] = playerSide == WhitePBB? WPawnAnyAttacks( sqBB ) : BPawnAnyAttacks( sqBB );
				arrPawnAttacksBB[playerSide,sq] = playerSide == WhitePBB? BPawnAnyAttacks( sqBB ) : WPawnAnyAttacks( sqBB );				
			}
		}
	}
	
	public void InitRayAttackPattern() {	
		
		rayAttacks = new ulong[64, NumRayDir];	
		
		for( int sq = 0; sq < 64; sq++ ) {
		 
			for( int i=0; i<NumRayDir; ++i ) {
				
				rayAttacks[sq,i] = RayDirectionMaskEx(i, sq);
			}
		}
	}
	
	
	// general move/attack update
	public void MoveUpdatePieceBB( int nSrcPlayerSide, int nSrcPieceBBIndex, ulong ulSrcSqBB, ulong ulTrgSqBB ) {
		
		pieceBB[nSrcPieceBBIndex] = (pieceBB[nSrcPieceBBIndex] ^ ulSrcSqBB) | ulTrgSqBB;		
		pieceBB[nSrcPlayerSide] = (pieceBB[nSrcPlayerSide] ^ ulSrcSqBB) | ulTrgSqBB;
		
		emptyBB = (emptyBB | ulSrcSqBB) ^ ulTrgSqBB;
		occupiedBB = (occupiedBB ^ ulSrcSqBB) | ulTrgSqBB;
	}
	
	public void CaptureUpdatePieceBB( int nSrcPlayerSide, int nSrcPieceBBIndex, int nTrgPlayerSide, int nTrgPieceBBIndex,  ulong ulSrcSqBB, ulong ulTrgSqBB ) {
		
		MoveUpdatePieceBB( nSrcPlayerSide, nSrcPieceBBIndex, ulSrcSqBB, ulTrgSqBB );		
		
		pieceBB[nTrgPieceBBIndex] = pieceBB[nTrgPieceBBIndex] ^ ulTrgSqBB;		
		pieceBB[nTrgPlayerSide] = (pieceBB[nTrgPlayerSide] ^ ulTrgSqBB);	
	}
	
	public void EnpassantCaptureUpdatePieceBB( int nSrcPlayerSide, int nSrcPieceBBIndex, int nCapturePlayerSide, int nCapturePieceBBIndex, 
												ulong ulSrcSqBB, ulong ulTrgSqBB, ulong ulCaptureSqBB ) {
		
		MoveUpdatePieceBB( nSrcPlayerSide, nSrcPieceBBIndex, ulSrcSqBB, ulTrgSqBB );		
		
		pieceBB[nCapturePieceBBIndex] = pieceBB[nCapturePieceBBIndex] ^ ulTrgSqBB;		
		pieceBB[nCapturePlayerSide] = (pieceBB[nCapturePlayerSide] ^ ulTrgSqBB);	
	}	
		
	public void MoveUpdate( ChessMoveManager.sMove move ) {
		
		bool bWillEnpassantInit = true;
		
		int nSrcPlayerSide, nSrcPieceBBIndex, nSrcSq, nTrgSq;		
		nSrcPlayerSide = (int)move.srcSquare.piece.playerSide;		
		nSrcPieceBBIndex = (int)move.srcSquare.piece.pieceType * 2 + 2 + nSrcPlayerSide;		
		nSrcSq = (int)move.srcSquare.position.pos;
		nTrgSq = (int)move.trgSquare.position.pos;
		
		ulong ulSrcSqBB = (ulong)1 << nSrcSq;	
		ulong ulTrgSqBB = (ulong)1 << nTrgSq;			
		
		
		// normal move
		if( ChessMoveManager.IsNormalMove( move.moveType ) ) {			
			
			// pawn move
			if( ChessMoveManager.IsPawnMove( move.moveType ) ) {								
						
				// promote move
				if( ChessMoveManager.IsPromoteMove( move.moveType ) ) {
					
				}
				// one/two move
				else  {
					
					// normal one square move
					if( ChessMoveManager.IsPawnOneMove( move.moveType ) ) {
						
					}
					// two square move
					else if( ChessMoveManager.IsPawnTwoMove( move.moveType ) ) {					
						
						// en passant target square update
						// if enemy pawn exist in east/west square, set enpassant target square				
						if( ((EastOne(ulTrgSqBB) & pieceBB[WhitePawnPBB + 1 - nSrcPlayerSide]) > 0) || 
							((WestOne(ulTrgSqBB) & pieceBB[WhitePawnPBB + 1 - nSrcPlayerSide]) > 0) ) {
							
							currEnPassantTrgSq.enpassantCapturSqBB = ulTrgSqBB;
							bWillEnpassantInit = false;	
						}					
					}					
				}
			}
			
			// update piece bb for normal move
			MoveUpdatePieceBB( nSrcPlayerSide, nSrcPieceBBIndex, ulSrcSqBB, ulTrgSqBB );
		}		
		// capture move
		else if( ChessMoveManager.IsCaptureMove( move.moveType ) ) {
			
			int nTrgPlayerSide, nTrgPieceBBIndex;		
			nTrgPlayerSide = (int)move.trgSquare.piece.playerSide;		
			nTrgPieceBBIndex = (int)move.trgSquare.piece.pieceType * 2 + 2 + nTrgPlayerSide;
			
			// pawn move
			if( ChessMoveManager.IsPawnMove( move.moveType ) ) {
				
				// promote move
				if( ChessMoveManager.IsPromoteMove( move.moveType ) ) {
					
				}				
			}
			
			// update piece bb for Capture
			CaptureUpdatePieceBB( nSrcPlayerSide, nSrcPieceBBIndex, nTrgPlayerSide, nTrgPieceBBIndex, ulSrcSqBB, ulTrgSqBB );
		}			
		// enpassantmove
		else if( ChessMoveManager.IsEnpassantMove( move.moveType ) ) {
			
			
			ulong ulCaptureSqBB = 0;			
			int nCapturePlayerSide, nCapurePieceBBIndex;		
			nCapturePlayerSide = 1 - (int)move.srcSquare.piece.playerSide;		
			nCapurePieceBBIndex = (int)PieceType.e_Pawn * 2 + 2 + nCapturePlayerSide;		
			if( nSrcPlayerSide == WhitePBB )
				ulCaptureSqBB = SoutOne(ulSrcSqBB);
			else
				ulCaptureSqBB = NortOne(ulSrcSqBB);
			
			// update piece bb for en passant capture move
			EnpassantCaptureUpdatePieceBB( nSrcPlayerSide, nSrcPieceBBIndex, nCapturePlayerSide, nCapurePieceBBIndex, ulSrcSqBB, ulTrgSqBB, ulCaptureSqBB );
		}		
		// castling move
		else if( ChessMoveManager.IsCastlingMove( move.moveType ) ) {			
			
			// update piece bb for castling move
			// king move
			MoveUpdatePieceBB( nSrcPlayerSide, nSrcPieceBBIndex, ulSrcSqBB, ulTrgSqBB );
			
			// rook move
			ulong ulRookSrcSqBB = 0, ulRookTrgSqBB = 0;
			int nRookSrcPieceBBIndex, nRookSrcSq, nRookTrgSq;
			nRookSrcPieceBBIndex = (int)PieceType.e_Rook * 2 + 2 + nSrcPlayerSide;
			if( ChessMoveManager.IsWhiteKingSideCastlingMove( move.moveType ) ||
				ChessMoveManager.IsBlackKingSideCastlingMove( move.moveType ) ) {									
						
				nRookSrcSq = nSrcPlayerSide == WhitePBB ? 7 : 63;
				ulRookSrcSqBB = (ulong)1 << nRookSrcSq;
				nRookTrgSq = nRookSrcSq - 2;
				ulRookTrgSqBB = (ulong)1 << nRookTrgSq;
			}
			else if( ChessMoveManager.IsWhiteQueenSideCastlingMove( move.moveType ) ||
					 ChessMoveManager.IsBlackQueenSideCastlingMove( move.moveType ) ) {
				
				nRookSrcSq = nSrcPlayerSide == WhitePBB ? 0 : 56;
				ulRookSrcSqBB = (ulong)1 << nRookSrcSq;
				nRookTrgSq = nRookSrcSq + 3;
				ulRookTrgSqBB = (ulong)1 << nRookTrgSq;
			}		
				
			MoveUpdatePieceBB( nSrcPlayerSide, nRookSrcPieceBBIndex, ulRookSrcSqBB, ulRookTrgSqBB );
		}
		
		// en passant target square init
		if( bWillEnpassantInit )
			currEnPassantTrgSq.enpassantCapturSqBB = 0;
		
		
		UpdateCastlingState( move );
	}	
	
	
	// update castling state
	public void UpdateCastlingState( ChessMoveManager.sMove move ) {
		
		// possible castling condition
		//1.The king has not previously moved.
		//2.The chosen rook has not previously moved.
		//3.There are no pieces between the king and the chosen rook.
		//4.The king is not currently in check.
		//5.The king does not pass through a square that is under attack by enemy pieces.[2]
		//6.The king does not end up in check (true of any legal move).		
		
		// 1 and 2 case, disable castling state
		switch( move.srcSquare.piece.piecePlayerType ) 						
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
			
		
		int nWhiteKingSquare = 4;
		int nWhiteKSideRookSquare = 7;
		int nWhiteQSideRookSquare = 0;
		
		int nBlackKingSquare = 60;
		int nBlackKSideRookSquare = 63;
		int nBlackQSideRookSquare = 56;		
		
		// white king side check		
		if( currCastlingState.CastlingWKSide != CastlingState.eCastling_Disable_State )
		{	
			// 3 case			
			if( IsWKSideCastlingRangeBlank() == false )			
				currCastlingState.CastlingWKSide = CastlingState.eCastling_Temporary_Disable_State;			
			
			// 4case
			if( IsWhiteKingInCheck() )			
				currCastlingState.CastlingWKSide = CastlingState.eCastling_Temporary_Disable_State;			
			
			// 5,6 case
			if( IsRangeNoneAttackBySquare( nWhiteKingSquare, nWhiteKSideRookSquare, BlackPBB ) == false )
				currCastlingState.CastlingWKSide = CastlingState.eCastling_Temporary_Disable_State;
		}
		// white queen side check
		if( currCastlingState.CastlingWQSide != CastlingState.eCastling_Disable_State )
		{	
			// 3 case			
			if( IsWQSideCastlingRangeBlank() == false )			
				currCastlingState.CastlingWQSide = CastlingState.eCastling_Temporary_Disable_State;
			
			// 4case	
			if( IsWhiteKingInCheck() )		
				currCastlingState.CastlingWQSide = CastlingState.eCastling_Temporary_Disable_State;	
			
			// 5,6 case				
			if( IsRangeNoneAttackBySquare( nWhiteKingSquare, nWhiteQSideRookSquare, BlackPBB ) == false )
				currCastlingState.CastlingWQSide = CastlingState.eCastling_Temporary_Disable_State;			
		}
		// black king side check
		if( currCastlingState.CastlingBKSide != CastlingState.eCastling_Disable_State )
		{	
			// 3 case			
			if( IsBKSideCastlingRangeBlank() == false )			
				currCastlingState.CastlingBKSide = CastlingState.eCastling_Temporary_Disable_State;
			
			// 4case			
			if( IsBlackKingInCheck() )		
				currCastlingState.CastlingBKSide = CastlingState.eCastling_Temporary_Disable_State;
			
			// 5,6 case		
			if( IsRangeNoneAttackBySquare( nBlackKingSquare, nBlackKSideRookSquare, WhitePBB ) == false )
				currCastlingState.CastlingBKSide = CastlingState.eCastling_Temporary_Disable_State;
		}
		// black queen side	check	
		if( currCastlingState.CastlingBQSide != CastlingState.eCastling_Disable_State )
		{	
			// 3 case			
			if( IsBQSideCastlingRangeBlank() == false )			
				currCastlingState.CastlingBQSide = CastlingState.eCastling_Temporary_Disable_State;			
			
			// 4case			
			if( IsBlackKingInCheck() )			
				currCastlingState.CastlingBQSide = CastlingState.eCastling_Temporary_Disable_State;	
			
			// 5,6 case		
			if( IsRangeNoneAttackBySquare( nBlackKingSquare, nBlackQSideRookSquare, WhitePBB ) == false )
				currCastlingState.CastlingBQSide = CastlingState.eCastling_Temporary_Disable_State;
		}					
	}
	
	
	
	
	
	
	
	
	
	
	// helper method
	public bool IsWhiteKingInCheck() {
		
		if( AttacksToKing( BitScanForward( pieceBB[WhiteKingPBB] ) ,WhitePBB ) > 0 )
			return true;
		
		return false;
	}
	
	public bool IsBlackKingInCheck() {
		
		if( AttacksToKing( BitScanForward( pieceBB[BlackKingPBB] ) ,BlackPBB ) > 0 )
			return true;
		
		return false;
	}
	
	public bool IsRangeBlankSquare( int nFromSq, int nToSq ) {
		
		ulong ulFromToBB = ~((ulUniverseBB << nFromSq) & (ulUniverseBB << nToSq));		
		if( (ulFromToBB & occupiedBB) > 0 )
			return false;
		
		return true;
	}
	
	public bool IsRangeNoneAttackBySquare( int nFromSq, int nToSq, int nSide ) {
		
		int nEndSq = System.Math.Max( nFromSq, nToSq );
		int nStartSq = System.Math.Min( nFromSq, nToSq );
		for( int i=nStartSq; i<nEndSq; ++i ) {
			
			if( Attacked( occupiedBB, i, nSide ) )
				return false;
		}
		
		return true;
	}
	
	public bool IsWKSideCastlingRangeBlank() {		
		
		if( (ulWKBlankCastlingMask & occupiedBB) > 0 )
			return false;
		
		return true;
	}
	
	public bool IsWQSideCastlingRangeBlank() {		
		
		if( (ulWQBlankCastlingMask & occupiedBB) > 0 )
			return false;
		
		return true;
	}
	
	public bool IsBKSideCastlingRangeBlank() {		
		
		if( (ulBKBlankCastlingMask & occupiedBB) > 0 )
			return false;
		
		return true;
	}
	
	public bool IsBQSideCastlingRangeBlank() {		
		
		if( (ulBQBlankCastlingMask & occupiedBB) > 0 )
			return false;
		
		return true;
	}
	
	
	
	
	
	// square attack by any other square
	ulong AttacksTo(ulong occupied, int sq) {
		
		ulong knights, kings, bishopsQueens, rooksQueens;
		knights        = pieceBB[WhiteKnightPBB] | pieceBB[BlackKnightPBB];
		kings          = pieceBB[WhiteKingPBB]   | pieceBB[BlackKingPBB];
		rooksQueens    =
		bishopsQueens  = pieceBB[WhiteQueenPBB]  | pieceBB[BlackQueenPBB];
		rooksQueens   |= pieceBB[WhiteRookPBB]   | pieceBB[BlackRookPBB];
		bishopsQueens |= pieceBB[WhiteBishopPBB] | pieceBB[BlackBishopPBB];
		
		return (arrPawnAttacksBB[WhitePBB,sq]	& pieceBB[BlackPawnPBB])
		    | (arrPawnAttacksBB[BlackPBB,sq] 	& pieceBB[WhitePawnPBB])
		    | (arrKnightAttacksBB[sq] 	 	& knights)
		    | (arrKingAttacksBB[sq] 			& kings)
		    | (BishopAttacks(occupied,sq) 	& bishopsQueens)
		    | (RookAttacks(occupied,sq)		& rooksQueens);
	}
	
	// 
	bool Attacked(ulong occupied, int square, int bySide) {
		
		ulong pawns   = pieceBB[WhitePawnPBB + bySide];
		ulong knights = pieceBB[WhiteKnightPBB + bySide];
		ulong king    = pieceBB[WhiteKingPBB   + bySide];
		
		if( (arrPawnAttacksBB[1-bySide,square] & pawns) > 0 )         return true;
		if( (arrKnightAttacksBB[square] & knights) > 0 )       return true;
		if( (arrKingAttacksBB[square] & king) > 0 )          return true;
		
		ulong bishopsQueens = pieceBB[WhiteQueenPBB + bySide]
		                 | pieceBB[WhiteBishopPBB + bySide];
		
		if( (BishopAttacks(occupied, square) & bishopsQueens) > 0 ) return true;
		
		ulong rooksQueens = pieceBB[WhiteQueenPBB  + bySide]
		                 | pieceBB[WhiteRookPBB + bySide];
		
		if( (RookAttacks(occupied, square) & rooksQueens) > 0 )   return true;
		return false;
	}
	
	ulong AttacksToKing(int squareOfKing, int colorOfKing) {
		
		ulong opPawns, opKnights, opRQ, opBQ;
			
		opPawns     = pieceBB[BlackPawnPBB - colorOfKing];
		opKnights   = pieceBB[BlackKnightPBB - colorOfKing];
		opRQ = opBQ = pieceBB[BlackQueenPBB - colorOfKing];
		opRQ       |= pieceBB[BlackRookPBB - colorOfKing];
		opBQ       |= pieceBB[BlackBishopPBB - colorOfKing];
			
		return (arrPawnAttacksBB[colorOfKing,squareOfKing] & opPawns)
		    | (arrKnightAttacksBB[squareOfKing]            & opKnights)
		    | (BishopAttacks(occupiedBB, squareOfKing)  & opBQ)
		    | (RookAttacks(occupiedBB, squareOfKing)  & opRQ);
	}
	
	
	// king move/attack
	public ulong KingMovesBB( int playerSide, int nSrcKingSq ) {				
		
		return arrKingAttacksBB[nSrcKingSq] & emptyBB;
	}
	
	public ulong KingAttacksBB( int playerSide, int nSrcKingSq ) {		
		
		return arrKingAttacksBB[nSrcKingSq] & pieceBB[BlackPBB - playerSide];				
	}
	
	public ulong KingCastlingBB( int playerSide ) {
		
		ulong ulCastlingBB = 0;
		if( playerSide == WhitePBB ) {
			
			if( currCastlingState.IsWhiteKingSideAvailable() )
				ulCastlingBB |= ulWKCastlingKingSqBB;
			
			if( currCastlingState.IsWhiteQueenSideAvailable() )
				ulCastlingBB |= ulWQCastlingKingSqBB;
		}
		else {
			
			if( currCastlingState.IsBlackKingSideAvailable() )
				ulCastlingBB |= ulBKCastlingKingSqBB;
			
			if( currCastlingState.IsBlackQueenSideAvailable() )
				ulCastlingBB |= ulBQCastlingKingSqBB;
		}
		
		return ulCastlingBB;
	}
	
	ulong KingAttacks(ulong kingSet) {
		
		ulong attacks = EastOne(kingSet) | WestOne(kingSet);
		kingSet    |= attacks;
		attacks    |= NortOne(kingSet) | SoutOne(kingSet);
		return attacks;
	}
	
	// pawn move/attack
	ulong SoutOne (ulong b) {return  b >> 8;}
	ulong NortOne (ulong b) {return  b << 8;}
	ulong EastOne (ulong b) {return (b & notHFile) << 1;}
	ulong NoEaOne (ulong b) {return (b & notHFile) << 9;}
	ulong SoEaOne (ulong b) {return (b & notHFile) >> 7;}
	ulong WestOne (ulong b) {return (b & notAFile) >> 1;}
	ulong SoWeOne (ulong b) {return (b & notAFile) >> 9;}
	ulong NoWeOne (ulong b) {return (b & notAFile) << 7;}
	
	ulong WSinglePushTargets(ulong wpawns, ulong empty) {
	   return NortOne(wpawns) & empty;
	}
	 
	ulong WDblPushTargets(ulong wpawns, ulong empty) {
	   const ulong rank4 = (ulong)0x00000000FF000000;
	   ulong singlePushs = WSinglePushTargets(wpawns, empty);
	   return NortOne(singlePushs) & empty & rank4;
	}
	 
	ulong BSinglePushTargets(ulong bpawns, ulong empty) {
	   return SoutOne(bpawns) & empty;
	}
	 
	ulong BDoublePushTargets(ulong bpawns, ulong empty) {
	   const ulong rank5 = (ulong)0x000000FF00000000;
	   ulong singlePushs = BSinglePushTargets(bpawns, empty);
	   return SoutOne(singlePushs) & empty & rank5;
	}
	
	ulong WPawnsAble2Push(ulong wpawns, ulong empty) {
	   return SoutOne(empty) & wpawns;
	}
	 
	ulong WPawnsAble2DblPush(ulong wpawns, ulong empty) {
	   const ulong rank4 = (ulong)0x00000000FF000000;
	   ulong emptyRank3 = SoutOne(empty & rank4) & empty;
	   return WPawnsAble2Push(wpawns, emptyRank3);
	}
	
	ulong WPawnEastAttacks(ulong wpawns) {return NoEaOne(wpawns);}
	ulong WPawnWestAttacks(ulong wpawns) {return NoWeOne(wpawns);}	 
	ulong BPawnEastAttacks(ulong bpawns) {return SoEaOne(bpawns);}
	ulong BPawnWestAttacks(ulong bpawns) {return SoWeOne(bpawns);}
	
	ulong WPawnAnyAttacks(ulong wpawns) {
	   return WPawnEastAttacks(wpawns) | WPawnWestAttacks(wpawns);
	}
	 
	ulong WPawnDblAttacks(ulong wpawns) {
	   return WPawnEastAttacks(wpawns) & WPawnWestAttacks(wpawns);
	}
	 
	ulong WPawnSingleAttacks(ulong wpawns) {
	   return WPawnEastAttacks(wpawns) ^ WPawnWestAttacks(wpawns);
	}
	
	ulong BPawnAnyAttacks(ulong bpawns) {
	   return BPawnEastAttacks(bpawns) | BPawnWestAttacks(bpawns);
	}
	 
	ulong BPawnDblAttacks(ulong bpawns) {
	   return BPawnEastAttacks(bpawns) & BPawnWestAttacks(bpawns);
	}
	 
	ulong BPawnSingleAttacks(ulong bpawns) {
	   return BPawnEastAttacks(bpawns) ^ BPawnWestAttacks(bpawns);
	}
	
	public ulong PawnOneMovesBB( int playerSide, int nSrcPawnSq ) {
		
		ulong ulSrcPawn = (ulong)1 << nSrcPawnSq;
		if( playerSide == WhitePBB )
			return WSinglePushTargets( ulSrcPawn, emptyBB );
		else
			return BSinglePushTargets( ulSrcPawn, emptyBB );
	}
	
	public ulong PawnTwoMovesBB( int playerSide, int nSrcPawnSq ) {
		
		ulong ulSrcPawn = (ulong)1 << nSrcPawnSq;
		if( playerSide == WhitePBB )
			return WDblPushTargets( ulSrcPawn, emptyBB );
		else
			return BDoublePushTargets( ulSrcPawn, emptyBB );
	}
	
	public ulong PawnAttackMovesBB( int playerSide, int nSrcPawnSq ) {	
		
		return arrPawnAttacksBB[playerSide, nSrcPawnSq] & pieceBB[WhitePBB + 1 - playerSide];		
	}
	
	public ulong PawnEnpassantMovesBB( int playerSide, int nSrcPawnSq ) {		
		
		ulong ulAnyDiagMove = arrPawnAttacksBB[playerSide, nSrcPawnSq] & emptyBB;		
		
		if( playerSide == WhitePBB ) {
			
			ulong ulTemp = SoutOne( ulAnyDiagMove );
			ulTemp = ulTemp & currEnPassantTrgSq.enpassantCapturSqBB;
			if( ulTemp > 0 )
				return NortOne(ulTemp);
		}
		else {
			
			ulong ulTemp = NortOne( ulAnyDiagMove );
			ulTemp = ulTemp & currEnPassantTrgSq.enpassantCapturSqBB;
			if( ulTemp > 0 )
				return SoutOne(ulTemp);
		}
		
		return 0;
	}
	
	
	// knight move/attack 
	ulong noNoEa(ulong b) {return (b & notHFile ) << 17;}
	ulong noEaEa(ulong b) {return (b & notGHFile) << 10;}
	ulong soEaEa(ulong b) {return (b & notGHFile) >>  6;}
	ulong soSoEa(ulong b) {return (b & notHFile ) >> 15;}
	ulong noNoWe(ulong b) {return (b & notAFile ) << 15;}
	ulong noWeWe(ulong b) {return (b & notABFile) <<  6;}
	ulong soWeWe(ulong b) {return (b & notABFile) >> 10;}
	ulong soSoWe(ulong b) {return (b & notAFile ) >> 17;}
	
	/*
	ulong KnightAttacks(ulong knights) {
		
		ulong west, east, attacks;
		east     = EastOne(knights);
		west     = WestOne(knights);
		attacks  = (east|west) << 16;
		attacks |= (east|west) >> 16;
		east     = EastOne (east);
		west     = WestOne (west);
		attacks |= (east|west) <<  8;
		attacks |= (east|west) >>  8;
		return attacks;
	}
	*/
	
	ulong KnightAttacks(ulong knights) {
		
		ulong l1 = (knights >> 1) & (ulong)0x7f7f7f7f7f7f7f7f;
		ulong l2 = (knights >> 2) & (ulong)0x3f3f3f3f3f3f3f3f;
		ulong r1 = (knights << 1) & (ulong)0xfefefefefefefefe;
		ulong r2 = (knights << 2) & (ulong)0xfcfcfcfcfcfcfcfc;
		ulong h1 = l1 | r1;
		ulong h2 = l2 | r2;
		return (h1<<16) | (h1>>16) | (h2<<8) | (h2>>8);
	}
	
	public ulong KnightAttacksBB(int nPlayerSide, int nSq) {		
		
		return (pieceBB[WhitePBB + 1 - nPlayerSide] & arrKnightAttacksBB[nSq]);
	}
	
	public ulong KnightMovesBB(int nSq) {		
		
		return (emptyBB & arrKnightAttacksBB[nSq]);
	}
	
		
	
	
	
	// ray move/attack
	// ex postfix = exclude self sq
	ulong EastMaskEx(int sq) {
		
	   const ulong one = 1;
	   return 2*( (one << (sq|7)) - (one << sq) );
	}
	 
	ulong NortMaskEx(int sq) {
		
	   return (ulong)0x0101010101010100 << sq;
	}
	
	ulong WestMaskEx(int sq) {
		
	   const ulong one = 1;
	   return (one << sq) - (one << (sq&56));
	}
	 
	ulong SoutMaskEx(int sq) {
		
	   return (ulong)0x0080808080808080 >> (sq ^ 63);
	}
	
	ulong NortEastMaskEx(int sq) {
		
		int nRank = sq % ChessData.nNumRank;
		int nFile = sq / ChessData.nNumRank;		
		
		ulong noea = (ulong)0x8040201008040200;		
		
		// rank east first
		for( int i=0; i<nRank; i++ ) {			
			noea = EastOne(noea);	
		}
		
		for( int j=0; j<nFile; j++ ) {			
			noea <<= ChessData.nNumRank;
		}
		
		return noea;
	}
	 
	ulong NortWestMaskEx(int sq) {
		
		int nRank = ChessData.nNumRank - sq % ChessData.nNumRank - 1;
		int nFile = sq / ChessData.nNumRank;		
		
		ulong nowe = (ulong)0x0102040810204000;		
		
		// rank west first
		for( int i=0; i<nRank; i++ ) {			
			nowe = WestOne(nowe);	
		}
		
		for( int j=0; j<nFile; j++ ) {			
			nowe <<= ChessData.nNumRank;
		}
		
		return nowe;	   
	}
	
	ulong SoutEastMaskEx(int sq) {
		
		int nRank = sq % ChessData.nNumRank;
		int nFile = ChessData.nNumPile - sq / ChessData.nNumRank - 1;		
		
		ulong soea = (ulong)0x0002040810204080;		
		
		// rank east first
		for( int i=0; i<nRank; i++ ) {			
			soea = EastOne(soea);	
		}
		
		for( int j=0; j<nFile; j++ ) {			
			soea >>= ChessData.nNumRank;
		}
		
		return soea;	  
	}
	 
	ulong SoutWestMaskEx(int sq) {
		
		int nRank = ChessData.nNumRank - sq % ChessData.nNumRank - 1;
		int nFile = ChessData.nNumPile - sq / ChessData.nNumRank - 1;		
		
		ulong sowe = (ulong)0x0040201008040201;		
		
		// rank west first
		for( int i=0; i<nRank; i++ ) {			
			sowe = WestOne(sowe);	
		}
		
		for( int j=0; j<nFile; j++ ) {			
			sowe >>= ChessData.nNumRank;
		}
		
		return sowe;	   
	}	
	
	ulong RayDirectionMaskEx(int nDir, int sq) {
		
		ulong ulRet = 0;
		switch( nDir ) {			
		
			case NorthDir:
				ulRet = NortMaskEx(sq);
			break;
			
			case NorthEastDir:
				ulRet = NortEastMaskEx(sq);
			break;
			
			case EastDir:
				ulRet = EastMaskEx(sq);
			break;
			
			case SouthEastDir:
				ulRet = SoutEastMaskEx(sq);
			break;
			
			case SouthDir:
				ulRet = SoutMaskEx(sq);
			break;
			
			case SouthWestDir:
				ulRet = SoutWestMaskEx(sq);
			break;
			
			case WestDir:
				ulRet = WestMaskEx(sq);
			break;
			
			case NorthWestDir:
				ulRet = NortWestMaskEx(sq);
			break;			
		}
		
		return ulRet;
	}	
	
	
	
	ulong RankMask(int sq) {return  (ulong)0xff << (sq & 56);}
 
	ulong FileMask(int sq) {return (ulong)0x0101010101010101 << (sq & 7);}
	 
	ulong DiagonalMask(int sq) {
	   const ulong maindia = (ulong)0x8040201008040201;
	   int diag =8*(sq & 7) - (sq & 56);
	   int nort = -diag & ( diag >> 31);
	   int sout =  diag & (-diag >> 31);
	   return (maindia >> sout) << nort;
	}
	 
	ulong AntiDiagMask(int sq) {
	   const ulong maindia = (ulong)0x0102040810204080;
	   int diag =56- 8*(sq&7) - (sq&56);
	   int nort = -diag & ( diag >> 31);
	   int sout =  diag & (-diag >> 31);
	   return (maindia >> sout) << nort;
	}
	
	ulong RankMaskEx    (int sq) {return ((ulong)(1) << sq) ^ RankMask(sq);}
	ulong FileMaskEx    (int sq) {return ((ulong)(1) << sq) ^ FileMask(sq);}
	ulong DiagonalMaskEx(int sq) {return ((ulong)(1) << sq) ^ DiagonalMask(sq);}
	ulong AntiDiagMaskEx(int sq) {return ((ulong)(1) << sq) ^ AntiDiagMask(sq);}
	
	ulong RookMask    (int sq) {return RankMask(sq)     | FileMask(sq);}
	ulong BishopMask  (int sq) {return DiagonalMask(sq) | AntiDiagMask(sq);}
	 
	 
	ulong RookMaskEx  (int sq) {return RankMask(sq)     ^ FileMask(sq);}
	ulong BishopMaskEx(int sq) {return DiagonalMask(sq) ^ AntiDiagMask(sq);}
	 
	ulong QueenMask   (int sq) {return RookMask(sq)     | BishopMask(sq);}
	ulong QueenMaskEx (int sq) {return RookMask(sq)     ^ BishopMask(sq);}		
	
	
			
	ulong GetPositiveRayAttacks(ulong occupied, int nDir, int square) {
		
		ulong attacks = rayAttacks[square, nDir];
		ulong blocker = attacks & occupied;
		
		if( blocker > 0 ) {
			
		  square = BitScanForward(blocker);
		  attacks ^= rayAttacks[square, nDir];
		}
		
		return attacks;
	}	
	
	ulong GetNegativeRayAttacks(ulong occupied, int nDir, int square) {
		
		ulong attacks = rayAttacks[square, nDir];
		ulong blocker = attacks & occupied;
		if ( blocker > 0 ) {
		  square = BitScanReverse(blocker);
		  attacks ^= rayAttacks[square, nDir];
		}
		return attacks;
	}	
	
	ulong DiagonalAttacks(ulong occ, int sq) {
		
		return GetPositiveRayAttacks(occ, NorthEastDir, sq) | GetNegativeRayAttacks(occ, SouthWestDir, sq); // ^ +
	}
	 
	ulong AntiDiagAttacks(ulong occ, int sq) {
		
		return GetPositiveRayAttacks(occ, NorthWestDir , sq) | GetNegativeRayAttacks(occ, SouthEastDir, sq); // ^ +
	}
	 
	ulong FileAttacks(ulong occ, int sq) {
		
		return GetPositiveRayAttacks(occ, NorthDir, sq) | GetNegativeRayAttacks(occ, SouthDir, sq); // ^ +
	}
	 
	ulong RankAttacks(ulong occ, int sq) {
		
		return GetPositiveRayAttacks(occ, EastDir, sq) | GetNegativeRayAttacks(occ, WestDir, sq); // ^ +
	}
	
	ulong RookAttacks(ulong occ, int sq) {
		
		return FileAttacks(occ, sq) | RankAttacks(occ, sq); // ^ +
	}
	 
	ulong BishopAttacks(ulong occ, int sq) {
		
		return DiagonalAttacks(occ, sq) | AntiDiagAttacks(occ, sq); // ^ +
	}
	 
	ulong QueenAttacks(ulong occ, int sq) {
		
		return RookAttacks(occ, sq) | BishopAttacks(occ, sq); // ^ +
	}
	
	public ulong RookAttacksBB(int nPlayerSide, int sq) {		
		
		return ((FileAttacks(occupiedBB, sq) | RankAttacks(occupiedBB, sq)) & pieceBB[BlackPBB - nPlayerSide] ); // ^ +
	}
	 
	public ulong BishopAttacksBB(int nPlayerSide, int sq) {
		
		return ((DiagonalAttacks(occupiedBB, sq) | AntiDiagAttacks(occupiedBB, sq)) & pieceBB[BlackPBB - nPlayerSide] ); // ^ +
	}
	 
	public ulong QueenAttacksBB(int nPlayerSide, int sq) {
		
		return RookAttacksBB(nPlayerSide, sq) | BishopAttacksBB(nPlayerSide, sq); // ^ +
	}
	
	public ulong RookMovesBB(int sq) {
		
		return ((FileAttacks(occupiedBB, sq) | RankAttacks(occupiedBB, sq)) & emptyBB); // ^ +
	}
	 
	public ulong BishopMovesBB(int sq) {
		
		return ((DiagonalAttacks(occupiedBB, sq) | AntiDiagAttacks(occupiedBB, sq)) & emptyBB); // ^ +
	}
	 
	public ulong QueenMovesBB(int sq) {
		
		return (RookMovesBB(sq) | BishopMovesBB(sq)); // ^ +
	}
	
	
	
	
	
	
	static int [] index64ForBSF;
	/**
	* bitScanForward
	* @author Kim Walisch (2012)
	* @param bb bitboard to scan
	* @precondition bb != 0
	* @return index (0..63) of least significant one bit
	*/
	public static int BitScanForward(ulong bb) {
		const ulong debruijn64 = (ulong)0x03f79d71b4cb0a89;		
		return index64ForBSF[((bb ^ (bb-1)) * debruijn64) >> 58];
	}	
	
	
	static int [] index64ForBSR;
	/**
	* bitScanReverse
	* @authors Kim Walisch, Mark Dickinson
	* @param bb bitboard to scan
	* @precondition bb != 0
	* @return index (0..63) of most significant one bit
	*/
	public static int BitScanReverse( ulong bb ) {
		
		const ulong debruijn64 = (ulong)0x03f79d71b4cb0a89;		
		bb |= bb >> 1; 
		bb |= bb >> 2;
		bb |= bb >> 4;
		bb |= bb >> 8;
		bb |= bb >> 16;
		bb |= bb >> 32;
		return index64ForBSR[(bb * debruijn64) >> 58];
	}
	
	static ChessBitBoard() {
		
		index64ForBSF = new int[] {
		
			0, 47,  1, 56, 48, 27,  2, 60,
			57, 49, 41, 37, 28, 16,  3, 61,
			54, 58, 35, 52, 50, 42, 21, 44,
			38, 32, 29, 23, 17, 11,  4, 62,
			46, 55, 26, 59, 40, 36, 15, 53,
			34, 51, 20, 43, 31, 22, 10, 45,
			25, 39, 14, 33, 19, 30,  9, 24,
			13, 18,  8, 12,  7,  6,  5, 63
		};
		
		
		index64ForBSR = new int[] {
		
			0, 47,  1, 56, 48, 27,  2, 60,
			57, 49, 41, 37, 28, 16,  3, 61,
			54, 58, 35, 52, 50, 42, 21, 44,
			38, 32, 29, 23, 17, 11,  4, 62,
			46, 55, 26, 59, 40, 36, 15, 53,
			34, 51, 20, 43, 31, 22, 10, 45,
			25, 39, 14, 33, 19, 30,  9, 24,
			13, 18,  8, 12,  7,  6,  5, 63
		};
	}
}



























