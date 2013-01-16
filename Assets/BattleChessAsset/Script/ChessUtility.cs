using UnityEngine;
using System.Collections;


//namespace BattleChess {
	
	
public class ChessUtility {

	
	// Utiliy Method	
	public static PieceType GetPieceType( PiecePlayerType piecePlayerType ) {
		
		if( piecePlayerType >= PiecePlayerType.eWhite_King && piecePlayerType <= PiecePlayerType.eWhite_Pawn8 ) {
			
			switch( piecePlayerType )
			{
				case PiecePlayerType.eWhite_King:
					return PieceType.e_King;				
				
				case PiecePlayerType.eWhite_Queen:
					return PieceType.e_Queen;				
				
				case PiecePlayerType.eWhite_RookLeft:				
				case PiecePlayerType.eWhite_RookRight:
					return PieceType.e_Rook;
								
				case PiecePlayerType.eWhite_BishopLeft:
				case PiecePlayerType.eWhite_BishopRight:
					return PieceType.e_Bishop;				
				
				case PiecePlayerType.eWhite_KnightLeft:
				case PiecePlayerType.eWhite_KnightRight:
					return PieceType.e_Knight;				
				
				case PiecePlayerType.eWhite_Pawn1:
				case PiecePlayerType.eWhite_Pawn2:
				case PiecePlayerType.eWhite_Pawn3:
				case PiecePlayerType.eWhite_Pawn4:
				case PiecePlayerType.eWhite_Pawn5:
				case PiecePlayerType.eWhite_Pawn6:
				case PiecePlayerType.eWhite_Pawn7:
				case PiecePlayerType.eWhite_Pawn8:
					return PieceType.e_Pawn;						
			}
		}
		else if( piecePlayerType >= PiecePlayerType.eBlack_King && piecePlayerType <= PiecePlayerType.eBlack_Pawn8 ) {
			
			switch( piecePlayerType )
			{
				case PiecePlayerType.eBlack_King:
					return PieceType.e_King;				
				
				case PiecePlayerType.eBlack_Queen:
					return PieceType.e_Queen;				
				
				case PiecePlayerType.eBlack_RookLeft:				
				case PiecePlayerType.eBlack_RookRight:
					return PieceType.e_Rook;				
				
				case PiecePlayerType.eBlack_BishopLeft:
				case PiecePlayerType.eBlack_BishopRight:
					return PieceType.e_Bishop;				
				
				case PiecePlayerType.eBlack_KnightLeft:
				case PiecePlayerType.eBlack_KnightRight:
					return PieceType.e_Knight;				
				
				case PiecePlayerType.eBlack_Pawn1:
				case PiecePlayerType.eBlack_Pawn2:
				case PiecePlayerType.eBlack_Pawn3:
				case PiecePlayerType.eBlack_Pawn4:
				case PiecePlayerType.eBlack_Pawn5:
				case PiecePlayerType.eBlack_Pawn6:
				case PiecePlayerType.eBlack_Pawn7:
				case PiecePlayerType.eBlack_Pawn8:
					return PieceType.e_Pawn;				
			}
		}
		else {
			
			return PieceType.e_None;	
		}
		
		return PieceType.e_None;
	}




	static ChessUtility() {
	}
} // ChessUtiliy
	
	
//}
