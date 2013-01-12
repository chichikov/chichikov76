using UnityEngine;
using System.Collections;

public struct ChessEnPassant {

	public int Rank { get; set; }
	public int Pile { get; set; }
	public bool Available { get; set; }	
	
	
	public string GetFenString() {		
		
		string strRetFen = " ";
		if( Available ) {
			
			strRetFen += ChessData.GetRankPileToString( Rank, Pile );
		}
		else {
			
			strRetFen += "-";
		}
		
		return strRetFen;
	}
}
