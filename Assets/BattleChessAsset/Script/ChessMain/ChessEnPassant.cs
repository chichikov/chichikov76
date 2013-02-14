using UnityEngine;
using System.Collections;

public struct ChessEnPassant {

	public ulong enpassantCapturSqBB { get; set; }	
	
	
	
	public void Clear() {
		
		enpassantCapturSqBB = 0;
	}
	
	public string GetFenString() {		
		
		string strRetFen = " ";
		if( enpassantCapturSqBB > 0 ) {
			
			int nSquare = ChessBitBoard.BitScanForward( enpassantCapturSqBB );
			
			int nECTSRank, nECTSFile;
			nECTSRank = nSquare % ChessData.nNumRank;
			nECTSFile = nSquare / ChessData.nNumPile;
			
			strRetFen += ChessData.GetRankPileToString( nECTSRank, nECTSFile );
		}
		else {
			
			strRetFen += "-";
		}
		
		return strRetFen;
	}
}
