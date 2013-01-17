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
	
	public const int NumPieceBB = 14;
	
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
	}
	
	public void InitKingAttackPattern() {	
		
		ulong sqBB = 1;
		for( int sq = 0; sq < 64; sq++, sqBB <<= 1 )
		   arrKingAttacksBB[sq] = KingAttacks(sqBB);
	}
	
	public void InitKnightAttackPattern() {			
		
	}
	
	public void InitPawnAttackPattern() {			
		
	}
	
	
	
	/*
	public void InitKingAttackPattern() {		
		
		arrKingAttacksBB = new ulong[64];						
		for( int j=0; j<ChessData.nNumPile; j++ ) {
			for( int k=0; k<ChessData.nNumRank; k++ ) {								
					
				int nPos = j * ChessData.nNumRank + k;
				ulong ulCurr = (ulong)1 << nPos;					
				
				// left/bottom 기준 counter-clockwise order
				ulong [] aAttackCandi = new ulong[8];						
				aAttackCandi[0] = ulCurr >> (ChessData.nNumRank + 1);
				aAttackCandi[1] = ulCurr >> ChessData.nNumRank;
				aAttackCandi[2] = ulCurr >> (ChessData.nNumRank - 1);
				aAttackCandi[3] = ulCurr << 1;
				aAttackCandi[4] = ulCurr << (ChessData.nNumRank + 1);
				aAttackCandi[5] = ulCurr << ChessData.nNumRank;
				aAttackCandi[6] = ulCurr >> 1;
				aAttackCandi[7] = ulCurr << (ChessData.nNumRank + 1);					
				
				// 4 corner
				// left/bottom
				if( k == 0 && j == 0 ) {
					
					arrKingAttacksBB[nPos] = aAttackCandi[3] | aAttackCandi[4] | aAttackCandi[5];
				}
				// right/bottom
				else if( k == 7 && j == 0 ) {
					
					arrKingAttacksBB[nPos] = aAttackCandi[5] | aAttackCandi[6] | aAttackCandi[7];
				}
				// left/top
				else if( k == 0 && j == 7 ) {
					
					arrKingAttacksBB[nPos] = aAttackCandi[1] | aAttackCandi[2] | aAttackCandi[3];
				}
				// right/top
				else if( k == 7 && j == 7 ) {
					
					arrKingAttacksBB[nPos] = aAttackCandi[7] | aAttackCandi[0] | aAttackCandi[1];
				}
				// bottom
				else if( j == 0 ) {
					
					arrKingAttacksBB[nPos] = aAttackCandi[3] | aAttackCandi[4] | aAttackCandi[5] | aAttackCandi[6] | aAttackCandi[7];
				}
				// left
				else if( k == 0 ) {
					
					arrKingAttacksBB[nPos] = aAttackCandi[1] | aAttackCandi[2] | aAttackCandi[3] | aAttackCandi[4] | aAttackCandi[5];
				}
				// right
				else if( k == 7 ) {
					
					arrKingAttacksBB[nPos] = aAttackCandi[5] | aAttackCandi[6] | aAttackCandi[7] | aAttackCandi[0] | aAttackCandi[1];
				}
				// top
				else if( j == 7 ) {
					
					arrKingAttacksBB[nPos] = aAttackCandi[3] | aAttackCandi[4] | aAttackCandi[5] | aAttackCandi[6] | aAttackCandi[7];
				}									
			}
		}
				
	}
	
	public void InitKnightAttackPattern() {		
		
		arrKnightAttacksBB = new ulong[64];						
		for( int j=0; j<ChessData.nNumPile; j++ ) {
			for( int k=0; k<ChessData.nNumRank; k++ ) {												
					
				int nPos = j * ChessData.nNumRank + k;
				ulong ulCurr = (ulong)1 << nPos;
				
				arrKingAttacksBB[nPos] = 0;
				
				// left/bottom 기준 counter-clockwise order
				ulong [] aAttackCandi = new ulong[8];						
				aAttackCandi[0] = ulCurr - 17;
				aAttackCandi[1] = ulCurr - 15;
				aAttackCandi[2] = ulCurr - 6;
				aAttackCandi[3] = ulCurr + 10;
				aAttackCandi[4] = ulCurr + 17;
				aAttackCandi[5] = ulCurr + 15;
				aAttackCandi[6] = ulCurr + 6;
				aAttackCandi[7] = ulCurr - 10;
				
				int nAttackPile, nAttackRank;
				
				nAttackRank = k - 1;
				nAttackPile = j - 2;					
				if( nAttackRank >= 0 && nAttackPile >= 0 ) {
					
					arrKingAttacksBB[nPos] |= aAttackCandi[0];
				}
				
				nAttackRank = k + 1;
				nAttackPile = j - 2;					
				if( nAttackRank < ChessData.nNumRank && nAttackPile >= 0 ) {
					
					arrKingAttacksBB[nPos] |= aAttackCandi[1];
				}
				
				nAttackRank = k + 2;
				nAttackPile = j - 1;					
				if( nAttackRank < ChessData.nNumRank && nAttackPile >= 0 ) {
					
					arrKingAttacksBB[nPos] |= aAttackCandi[2];
				}
				
				nAttackRank = k + 2;
				nAttackPile = j + 1;					
				if( nAttackRank < ChessData.nNumRank && nAttackPile < ChessData.nNumPile ) {
					
					arrKingAttacksBB[nPos] |= aAttackCandi[3];
				}
				
				nAttackRank = k + 1;
				nAttackPile = j + 2;					
				if( nAttackRank < ChessData.nNumRank && nAttackPile < ChessData.nNumPile ) {
					
					arrKingAttacksBB[nPos] |= aAttackCandi[4];
				}
				
				nAttackRank = k - 1;
				nAttackPile = j + 2;					
				if( nAttackRank >= 0 && nAttackPile < ChessData.nNumPile ) {
					
					arrKingAttacksBB[nPos] |= aAttackCandi[5];
				}
				
				nAttackRank = k - 2;
				nAttackPile = j + 1;					
				if( nAttackRank >= 0 && nAttackPile < ChessData.nNumPile ) {
					
					arrKingAttacksBB[nPos] |= aAttackCandi[6];
				}
				
				nAttackRank = k - 2;
				nAttackPile = j - 1;					
				if( nAttackRank >= 0 && nAttackPile >= 0 ) {
					
					arrKingAttacksBB[nPos] |= aAttackCandi[7];
				}
			}
		}
		
	}
	
	public void InitPawnAttackPattern() {		
		
		arrPawnAttacksBB = new ulong[2,64];		
		for( int i=0; i<2; i++ ) {			
			for( int j=0; j<ChessData.nNumPile; j++ ) {
				for( int k=0; k<ChessData.nNumRank; k++ ) {					
						
					int nPos = j * ChessData.nNumRank + k;
					ulong ulCurr = (ulong)1 << nPos;
					
					arrPawnAttacksBB[i, nPos] = 0;
					
					// white pawn
					if( i == (int)PieceBBType.eWhite ) {					
						
						// 0 pile, 7 pile
						if( j != 0 && j != 7 ) {
							
							// east attack
							// check border
							if( k != ChessData.nNumRank - 1 ) {
								
								arrPawnAttacksBB[i, nPos] = ulCurr << (ChessData.nNumRank + 1);								
							}
							// west attack
							if( k != 0 ) {
								
								arrPawnAttacksBB[i, nPos] |= ulCurr << (ChessData.nNumRank - 1);								
							}														
						}						
					}
					// black pawn
					else {
						
						// 0 pile, 7 pile
						if( j != 0 && j != 7 ) {
							
							// east attack
							// check border
							if( k != ChessData.nNumRank - 1 ) {
								
								arrPawnAttacksBB[i, nPos] = ulCurr >> (ChessData.nNumRank - 1);								
							}
							// west attack
							if( k != 0 ) {
								
								arrPawnAttacksBB[i, nPos] |= ulCurr >> (ChessData.nNumRank + 1);								
							}														
						}	
					}
				}
			}
		}		
	}
	*/
	
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



























