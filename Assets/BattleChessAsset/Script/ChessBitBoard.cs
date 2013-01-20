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
	
	const ulong notAFile = 0xfefefefefefefefe; // ~0x0101010101010101
	const ulong notHFile = 0x7f7f7f7f7f7f7f7f; // ~0x8080808080808080	
	
	// check for castling
	const ulong ulWKBlankCastlingMask = 0x0000000000000006;
	const ulong ulWQBlankCastlingMask = 0x0000000000000070;
	const ulong ulBKBlankCastlingMask = 0x6000000000000000;
	const ulong ulBQBlankCastlingMask = 0x0700000000000000;
	
	
	public const int NumPieceBB = 14;	
	
	public enum RayDirBBType : int {
		
		eNorth = 0,
		eNorthEast,
		eEast,
		eSouthEast,
		eSouth,	
		eSouthWest,
		eWest,
		eNorthWest,
		
		eNumRayDir
	}
	
	public enum PieceBBType : int {
		
		eWhite = 0,
		eBlack,
		eWhite_King,
		eBlack_King,
		eWhite_Queen,
		eBlack_Queen,
		eWhite_Rook,
		eBlack_Rook,
		eWhite_Bishop,
		eBlack_Bishop,
		eWhite_Knight,
		eBlack_Knight,
		eWhite_Pawn,
		eBlack_Pawn,			
	}
	
	
	ulong [] pieceBB;
	ulong emptyBB;
	ulong occupiedBB;
	
	// attack map
	ulong [] arrKingAttacksBB;		
	ulong [] arrKnightAttacksBB;	
	ulong [,] arrPawnAttacksBB;
	
	ulong [,] rayAttacks;


	
	
	public ChessBitBoard()
	{
		pieceBB = new ulong[NumPieceBB];
		emptyBB = 0;
		occupiedBB = 0;
		
		InitAttackPattern();		
	}
	
	// initialize game start state
	public void Init() {
		
		// bit board state init	
		pieceBB[(int)PieceBBType.eWhite] = 0x00000000000000FF;
		pieceBB[(int)PieceBBType.eBlack] = 0xFF00000000000000;
		pieceBB[(int)PieceBBType.eWhite_King] = 0x0000000000000010;
		pieceBB[(int)PieceBBType.eBlack_King] = 0x1000000000000000;
		pieceBB[(int)PieceBBType.eWhite_Queen] = 0x0000000000000008;
		pieceBB[(int)PieceBBType.eBlack_Queen] = 0x0800000000000000;
		pieceBB[(int)PieceBBType.eWhite_Rook] = 0x0000000000000081;
		pieceBB[(int)PieceBBType.eBlack_Rook] = 0x8100000000000000;
		pieceBB[(int)PieceBBType.eWhite_Bishop] = 0x0000000000000024;
		pieceBB[(int)PieceBBType.eBlack_Bishop] = 0x2400000000000000;
		pieceBB[(int)PieceBBType.eWhite_Knight] = 0x0000000000000042;
		pieceBB[(int)PieceBBType.eBlack_Knight] = 0x4200000000000000;
		pieceBB[(int)PieceBBType.eWhite_Pawn] = 0x00000000000000F0;
		pieceBB[(int)PieceBBType.eBlack_Pawn] = 0x0F00000000000000;
		
		emptyBB = 0x00FFFFFFFFFFFF00;
		occupiedBB = 0xFF000000000000FF;
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
		
		arrKingAttacksBB = new ulong[BoardPosition.NumOfBoardPosition];
		
		ulong sqBB = 1;
		for( BoardPosition sq = 0; sq < BoardPosition.NumOfBoardPosition; sq++, sqBB <<= 1 )
		   arrKingAttacksBB[sq] = KingAttacks(sqBB);
	}
	
	public void InitKnightAttackPattern() {	
		
	}	
	
	public void InitPawnAttackPattern() {			
		
		arrPawnAttacksBB = new ulong[PlayerSide.e_NumOfPlayerSide][BoardPosition.NumOfBoardPosition];		
		
		for( int playerSide=0; playerSide<PlayerSide.e_NumOfPlayerSide; ++playerSide ) {
			
			ulong sqBB = 1;			
			for( BoardPosition sq = 0; sq <= BoardPosition.NumOfBoardPosition; sq++, sqBB <<= 1 ) {
				
				arrPawnAttacksBB[playerSide][sq] = playerSide == eWhite? WPawnAnyAttacks( sqBB ) : BPawnAnyAttacks( sqBB );				
			}
		}
	}
	
	public void InitRayAttackPattern() {	
		
		rayAttacks = new ulong[RayDirBBType.eNumRayDir, BoardPosition.NumOfBoardPosition];		
		
		ulong sqBB = 1;		
		for( BoardPosition sq = 0; sq < BoardPosition.NumOfBoardPosition; sq++, sqBB <<= 1 ) {
		 
			for( int i=0; i<RayDirBBType.eNumRayDir; ++i ) {
				
				rayAttacks[i][sq] = KingAttacks(sqBB);
			}
		}
	}
	
	// square attack by any other square
	ulong AttacksTo(ulong occupied, BoardPosition sq) {
		
		ulong knights, kings, bishopsQueens, rooksQueens;
		knights        = pieceBB[PieceBBType.eWhite_Knight] | pieceBB[PieceBBType.eBlack_Knight];
		kings          = pieceBB[PieceBBType.eWhite_King]   | pieceBB[PieceBBType.eBlack_King];
		rooksQueens    =
		bishopsQueens  = pieceBB[PieceBBType.eWhite_Queen]  | pieceBB[PieceBBType.eBlack_Queen];
		rooksQueens   |= pieceBB[PieceBBType.eWhite_Rook]   | pieceBB[PieceBBType.eBlack_Rook];
		bishopsQueens |= pieceBB[PieceBBType.eWhite_Bishop] | pieceBB[PieceBBType.eBlack_Bishop];
		
		return (arrPawnAttacks[PlayerSide.e_White][sq]	& pieceBB[PieceBBType.eBlack_Pawn])
		    | (arrPawnAttacks[PlayerSide.e_Black][sq] 	& pieceBB[PieceBBType.eWhite_Pawn])
		    | (arrKnightAttacks[sq] 	 	& knights)
		    | (arrKingAttacks[sq] 			& kings)
		    | (bishopAttacks(occupied,sq) 	& bishopsQueens)
		    | (rookAttacks(occupied,sq)		& rooksQueens);
	}
	
	// 
	bool Attacked(ulong occupied, BoardPosition square, PlayerSide bySide) {
		
		ulong pawns   = pieceBB[PieceBBType.eWhite_Pawn + bySide];
		ulong knights = pieceBB[PieceBBType.eWhite_Knight + bySide];
		ulong king    = pieceBB[PieceBBType.eWhite_King   + bySide];
		
		if ( arrPawnAttacks[1-bySide][square]    & pawns )         return true;
		if ( arrKnightAttacks[square]            & knights )       return true;
		if ( arrKingAttacks[square]              & king )          return true;
		
		ulong bishopsQueens = pieceBB[PieceBBType.eWhite_Queen  + bySide]
		                 | pieceBB[PieceBBType.eWhite_Bishop + bySide];
		
		if ( bishopAttacks(occupied, square) & bishopsQueens ) return true;
		
		ulong rooksQueens = pieceBB[PieceBBType.eWhite_Queen  + bySide]
		                 | pieceBB[PieceBBType.eWhite_Rook + bySide];
		
		if ( rookAttacks(occupied, square) & rooksQueens )   return true;
		return false;
	}
	
	ulong AttacksToKing(BoardPosition squareOfKing, enumColor colorOfKing) {
		
		ulong opPawns, opKnights, opRQ, opBQ;
			
		opPawns     = pieceBB[nBlackPawn   - colorOfKing];
		opKnights   = pieceBB[nBlackKnight - colorOfKing];
		opRQ = opBQ = pieceBB[nBlackQueen  - colorOfKing];
		opRQ       |= pieceBB[nBlackRook   - colorOfKing];
		opBQ       |= pieceBB[nBlackBishop - colorOfKing];
			
		return (arrPawnAttacks[colorOfKing][squareOfKing] & opPawns)
		    | (arrKnightAttacks[squareOfKing]            & opKnights)
		    | (bishopAttacks (occupiedBB, squareOfKing)  & opBQ)
		    | (rookAttacks   (occupiedBB, squareOfKing)  & opRQ)
		    ;
	}
	
	
	// king move/attack
	public ulong KingMovesBB( PlayerSide playerSide, ulong kingSet ) {
		
		if( playerSide == PlayerSide.e_White ) {
			
			return KingAttacks( pieceBB[(int)PieceBBType.eWhite_King] ) & emptyBB;
		}
		else {
			
			return KingAttacks( pieceBB[(int)PieceBBType.eWhite_King] ) & emptyBB;
		}
		
		return (ulong)0;
	}
	
	public ulong KingAttacksBB( PlayerSide playerSide, ulong kingSet ) {
		
		if( playerSide == PlayerSide.e_White ) {
			
			return KingAttacks( pieceBB[(int)PieceBBType.eWhite_King] ) & pieceBB[(int)PieceBBType.eBlack];
		}
		else {
			
			return KingAttacks( pieceBB[(int)PieceBBType.eWhite_King] ) & pieceBB[(int)PieceBBType.eWhite];
		}
		
		return (ulong)0;
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
	
	
	
	
	
	// ray move/attack
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
		
	   const ulong one = 1;
	   return 2*( (one << (sq|7)) - (one << sq) );
	}
	 
	ulong NortWestMaskEx(int sq) {
		
	   return (ulong)0x0101010101010100 << sq;
	}
	
	ulong SoutEastMaskEx(int sq) {
		
	   const ulong one = 1;
	   return (one << sq) - (one << (sq&56));
	}
	 
	ulong SoutWestMaskEx(int sq) {
		
	   return (ulong)0x0080808080808080 >> (sq ^ 63);
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
}



























