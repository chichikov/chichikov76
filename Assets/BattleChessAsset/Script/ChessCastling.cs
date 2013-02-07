using UnityEngine;
using System.Collections;


//namespace BattleChess {	

public struct ChessCastling {
	
	// catling
	public CastlingState CastlingWKSide { get; set; }
	public CastlingState CastlingWQSide { get; set; }
	public CastlingState CastlingBKSide { get; set; }
	public CastlingState CastlingBQSide { get; set; }
	
	
	
	public void Clear() {
		
		CastlingWKSide = CastlingState.eCastling_Temporary_Disable_State;
		CastlingWQSide = CastlingState.eCastling_Temporary_Disable_State;
		CastlingBKSide = CastlingState.eCastling_Temporary_Disable_State;
		CastlingBQSide = CastlingState.eCastling_Temporary_Disable_State;
	}
	
	public bool IsWhiteKingSideAvailable() {
		
		if( CastlingWKSide == CastlingState.eCastling_Enable_State )
			return true;
		return false;
	}
	
	public bool IsWhiteQueenSideAvailable() {
		
		if( CastlingWQSide == CastlingState.eCastling_Enable_State )
			return true;
		return false;
	}
	
	public bool IsBlackKingSideAvailable() {
		
		if( CastlingBKSide == CastlingState.eCastling_Enable_State )
			return true;
		return false;
	}
	
	public bool IsBlackQueenSideAvailable() {
		
		if( CastlingBQSide == CastlingState.eCastling_Enable_State )
			return true;
		return false;
	}
	
	public string GetFenString() {
	
		bool bBlank = true;
		string strRetFen = " ";
		if( IsWhiteKingSideAvailable() ) {
			strRetFen += "K";
			bBlank = false;
		}
		
		if( IsWhiteQueenSideAvailable() ) {
			strRetFen += "Q";
			bBlank = false;
		}
		
		if( IsWhiteQueenSideAvailable() ) {
			strRetFen += "k";
			bBlank = false;
		}
		
		if( IsWhiteQueenSideAvailable() ) {
			strRetFen += "q";
			bBlank = false;
		}
		
		if( bBlank ) {
			
			strRetFen += "-";			
		}
		
		return strRetFen;
	}
}


//}