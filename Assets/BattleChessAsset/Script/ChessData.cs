using UnityEngine;
using System.Collections;

using System;
using System.Collections.Generic;



//namespace BattleChess {	
	
// Chess Data, Type, Enum, Etc....	
public enum PlayerSide {
	e_NoneSide = 0,
	e_White,
	e_Black	
};

public enum PieceType {
	e_None = -1,
	e_King = 0,
	e_Queen,
	e_Look,
	e_Bishop,
	e_Knight,
	e_Pawn,		
};	

public enum PiecePlayerType : int {
	
	eNone_Piece = -1,
	eWhite_King = 0,
	eWhite_Queen,
	eWhite_LookLeft,
	eWhite_LookRight,
	eWhite_BishopLeft,
	eWhite_BishopRight,
	eWhite_KnightLeft,
	eWhite_KnightRight,
	eWhite_Pawn1,
	eWhite_Pawn2,
	eWhite_Pawn3,
	eWhite_Pawn4,
	eWhite_Pawn5,
	eWhite_Pawn6,
	eWhite_Pawn7,
	eWhite_Pawn8,
	
	eBlack_King,
	eBlack_Queen,
	eBlack_LookLeft,
	eBlack_LookRight,
	eBlack_BishopLeft,
	eBlack_BishopRight,
	eBlack_KnightLeft,
	eBlack_KnightRight,
	eBlack_Pawn1,
	eBlack_Pawn2,
	eBlack_Pawn3,
	eBlack_Pawn4,
	eBlack_Pawn5,
	eBlack_Pawn6,
	eBlack_Pawn7,
	eBlack_Pawn8
};

[Flags]
public enum BoardPositionType {
	
	eNone = 0x00,
	eInside = 0x01,
	eLeft = 0x02,
	eRight = 0x04,
	eTop = 0x08,
	eBottom = 0x10
};

public enum BoardPosition : byte {

    //BoardPositions   
    A1 = 0x00, B1 = 0x01, C1 = 0x02, D1 = 0x03, E1 = 0x04, F1 = 0x05, G1 = 0x06, H1 = 0x07,
    A2 = 0x08, B2 = 0x09, C2 = 0x0A, D2 = 0x0B, E2 = 0x0C, F2 = 0x0D, G2 = 0x0E, H2 = 0x0F,
    A3 = 0x10, B3 = 0x11, C3 = 0x12, D3 = 0x13, E3 = 0x14, F3 = 0x15, G3 = 0x16, H3 = 0x17,
    A4 = 0x18, B4 = 0x19, C4 = 0x1A, D4 = 0x1B, E4 = 0x1C, F4 = 0x1D, G4 = 0x1E, H4 = 0x1F,
    A5 = 0x20, B5 = 0x21, C5 = 0x22, D5 = 0x23, E5 = 0x24, F5 = 0x25, G5 = 0x26, H5 = 0x27,
    A6 = 0x28, B6 = 0x29, C6 = 0x2A, D6 = 0x2B, E6 = 0x2C, F6 = 0x2D, G6 = 0x2E, H6 = 0x2F,
    A7 = 0x30, B7 = 0x31, C7 = 0x32, D7 = 0x33, E7 = 0x34, F7 = 0x35, G7 = 0x36, H7 = 0x37,
    A8 = 0x38, B8 = 0x39, C8 = 0x3A, D8 = 0x3B, E8 = 0x3C, F8 = 0x3D, G8 = 0x3E, H8 = 0x3F,
    BoardBits = 0x3F, InvalidPosition = 0x40
};

public enum CastlingState : byte {
	
	eCastling_Init_State = 0,
	eCastling_Enable_State,
	eCastling_Disable_State,
}
	
public class ChessData {
	// number of board square
	public static readonly int nNumBoardSquare = 64;
	// number of pieces white = 16, black = 16
	public static readonly int nNumWholePiece = 32;
	public static readonly int nNumWhitePiece = 16;
	public static readonly int nNumBlackPiece = 16;	
	// number of pile and rank
	public static readonly int nNumPile = 8;
	public static readonly int nNumRank = 8;
	
	public static readonly float fTileWidth = 1.0f;
	public static readonly float fBoardWidth = nNumRank * fTileWidth;		
	
	public static PiecePlayerType [,] aStartPiecePos = new PiecePlayerType[8,8] {
		
		{PiecePlayerType.eWhite_LookLeft, PiecePlayerType.eWhite_KnightLeft, PiecePlayerType.eWhite_BishopLeft, PiecePlayerType.eWhite_Queen, PiecePlayerType.eWhite_King, PiecePlayerType.eWhite_BishopRight, PiecePlayerType.eWhite_KnightRight, PiecePlayerType.eWhite_LookRight},
		{PiecePlayerType.eWhite_Pawn1, PiecePlayerType.eWhite_Pawn2, PiecePlayerType.eWhite_Pawn3, PiecePlayerType.eWhite_Pawn4, PiecePlayerType.eWhite_Pawn5, PiecePlayerType.eWhite_Pawn6, PiecePlayerType.eWhite_Pawn7, PiecePlayerType.eWhite_Pawn8},
		
		{PiecePlayerType.eNone_Piece, PiecePlayerType.eNone_Piece, PiecePlayerType.eNone_Piece, PiecePlayerType.eNone_Piece, PiecePlayerType.eNone_Piece, PiecePlayerType.eNone_Piece, PiecePlayerType.eNone_Piece, PiecePlayerType.eNone_Piece},
		{PiecePlayerType.eNone_Piece, PiecePlayerType.eNone_Piece, PiecePlayerType.eNone_Piece, PiecePlayerType.eNone_Piece, PiecePlayerType.eNone_Piece, PiecePlayerType.eNone_Piece, PiecePlayerType.eNone_Piece, PiecePlayerType.eNone_Piece},
		{PiecePlayerType.eNone_Piece, PiecePlayerType.eNone_Piece, PiecePlayerType.eNone_Piece, PiecePlayerType.eNone_Piece, PiecePlayerType.eNone_Piece, PiecePlayerType.eNone_Piece, PiecePlayerType.eNone_Piece, PiecePlayerType.eNone_Piece},
		{PiecePlayerType.eNone_Piece, PiecePlayerType.eNone_Piece, PiecePlayerType.eNone_Piece, PiecePlayerType.eNone_Piece, PiecePlayerType.eNone_Piece, PiecePlayerType.eNone_Piece, PiecePlayerType.eNone_Piece, PiecePlayerType.eNone_Piece},
		
		{PiecePlayerType.eBlack_Pawn1, PiecePlayerType.eBlack_Pawn2, PiecePlayerType.eBlack_Pawn3, PiecePlayerType.eBlack_Pawn4, PiecePlayerType.eBlack_Pawn5, PiecePlayerType.eBlack_Pawn6, PiecePlayerType.eBlack_Pawn7, PiecePlayerType.eBlack_Pawn8},
		{PiecePlayerType.eBlack_LookLeft, PiecePlayerType.eBlack_KnightLeft, PiecePlayerType.eBlack_BishopLeft, PiecePlayerType.eBlack_Queen, PiecePlayerType.eBlack_King, PiecePlayerType.eBlack_BishopRight, PiecePlayerType.eBlack_KnightRight, PiecePlayerType.eBlack_LookRight}
	};
	
	public static Dictionary<PiecePlayerType, string> pieceFenStringDic;	
	public static string GetPieceFenString( PiecePlayerType piecePlayerType ) {
		
		return pieceFenStringDic[piecePlayerType];
	}
	
	// rank/pile to string
	public static string GetRankPileToString( int nRank, int nPile ) {			
		
		if( nRank >= 0 && nRank < ChessData.nNumRank && nPile >= 0 && nPile < ChessData.nNumPile ) {			
			
			char [] aRankPile = new char[2];
			aRankPile[0] = (char)(nRank + (int)'a');
			aRankPile[1] = (char)(nPile + (int)'1');
			
			string strRet = new string(aRankPile);	
			
			return strRet;
		}		
		
		UnityEngine.Debug.LogError( "GetRankPileToString() - invalid nRank, nPile" );
		return null;
	}
	
	// string to rank/pile
	public static bool GetStringToRankPile( string strPosition, out int nRank, out int nPile ) {
		
		if( strPosition.Length == 2 ) {
			nRank = strPosition[0] - 'a';
			nPile = strPosition[1] - '1';
			
			//UnityEngine.Debug.Log( "GetStringToRankPile() - strPosition" + nRank + " , " + nPile );
			
			return true;
		}
		
		nRank = -1;
		nPile = -1;
		
		UnityEngine.Debug.LogError( "GetStringToRankPile() - invalid strPosition" );
		
		return false;
	}
	
	// get oppsite Side
	public static PlayerSide GetOppositeSide( PlayerSide side ) {
		
		if( side == PlayerSide.e_White )
			return PlayerSide.e_Black;
		
		return PlayerSide.e_White;
	}
		
	static ChessData() {
		
		pieceFenStringDic = new Dictionary<PiecePlayerType, string>();
		
		pieceFenStringDic[PiecePlayerType.eWhite_King] = "K";
		pieceFenStringDic[PiecePlayerType.eWhite_Queen] = "Q";
		pieceFenStringDic[PiecePlayerType.eWhite_LookLeft] = "R";
		pieceFenStringDic[PiecePlayerType.eWhite_LookRight] = "R";
		pieceFenStringDic[PiecePlayerType.eWhite_BishopLeft] = "B";
		pieceFenStringDic[PiecePlayerType.eWhite_BishopRight] = "B";
		pieceFenStringDic[PiecePlayerType.eWhite_KnightLeft] = "N";
		pieceFenStringDic[PiecePlayerType.eWhite_KnightRight] = "N";
		pieceFenStringDic[PiecePlayerType.eWhite_Pawn1] = "P";
		pieceFenStringDic[PiecePlayerType.eWhite_Pawn2] = "P";
		pieceFenStringDic[PiecePlayerType.eWhite_Pawn3] = "P";
		pieceFenStringDic[PiecePlayerType.eWhite_Pawn4] = "P";
		pieceFenStringDic[PiecePlayerType.eWhite_Pawn5] = "P";
		pieceFenStringDic[PiecePlayerType.eWhite_Pawn6] = "P";
		pieceFenStringDic[PiecePlayerType.eWhite_Pawn7] = "P";
		pieceFenStringDic[PiecePlayerType.eWhite_Pawn8] = "P";	
		
		pieceFenStringDic[PiecePlayerType.eBlack_King] = "k";
		pieceFenStringDic[PiecePlayerType.eBlack_Queen] = "q";
		pieceFenStringDic[PiecePlayerType.eBlack_LookLeft] = "r";
		pieceFenStringDic[PiecePlayerType.eBlack_LookRight] = "r";
		pieceFenStringDic[PiecePlayerType.eBlack_BishopLeft] = "b";
		pieceFenStringDic[PiecePlayerType.eBlack_BishopRight] = "b";
		pieceFenStringDic[PiecePlayerType.eBlack_KnightLeft] = "n";
		pieceFenStringDic[PiecePlayerType.eBlack_KnightRight] = "n";
		pieceFenStringDic[PiecePlayerType.eBlack_Pawn1] = "p";
		pieceFenStringDic[PiecePlayerType.eBlack_Pawn2] = "p";
		pieceFenStringDic[PiecePlayerType.eBlack_Pawn3] = "p";
		pieceFenStringDic[PiecePlayerType.eBlack_Pawn4] = "p";
		pieceFenStringDic[PiecePlayerType.eBlack_Pawn5] = "p";
		pieceFenStringDic[PiecePlayerType.eBlack_Pawn6] = "p";
		pieceFenStringDic[PiecePlayerType.eBlack_Pawn7] = "p";
		pieceFenStringDic[PiecePlayerType.eBlack_Pawn8] = "p";		
	}
}
//}
