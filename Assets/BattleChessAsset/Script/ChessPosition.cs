using UnityEngine;
using System.Collections;


//namespace BattleChess {	
	
	
public struct ChessPosition {
		
	public BoardPosition pos;
	public int nRank;
	public int nPile;
	public BoardPositionType posType;	
	
	public ChessPosition( int nPosRank, int nPosPile ) {
		
		this.posType = GetPositionType( nPosRank, nPosPile );
		if( this.posType != BoardPositionType.eNone ) {
		
			this.nRank = nPosRank;
			this.nPile = nPosPile;
			
			this.pos = (BoardPosition)(nPosPile * ChessData.nNumRank + nPosRank);
		}
		else {
			
			this.nRank = -1;
			this.nPile = -1;
			this.pos = BoardPosition.InvalidPosition;						
		}	
	}
	
	public ChessPosition( BoardPosition boardPos ) {
		
		int nPosRank = 0, nPosPile = 0;
		this.posType = ChessPosition.CalcPositionIndex( boardPos, ref nPosRank, ref nPosPile );
		if( this.posType != BoardPositionType.eNone ) {				
			
			this.nRank = nPosRank;
			this.nPile = nPosPile;
			
			this.pos = boardPos;			
		}
		else {
			
			this.nRank = -1;
			this.nPile = -1;
			
			this.pos = BoardPosition.InvalidPosition;			
		}	
	}
	
	public ChessPosition( ChessPosition chessPos ) {
		
		this.posType = chessPos.posType;			
		this.nRank = chessPos.nRank;
		this.nPile = chessPos.nPile;		
		this.pos = chessPos.pos;		
	}		
	
	
	public bool SetPosition( int nPosRank, int nPosPile ) {
		
		this.posType = GetPositionType( nPosRank, nPosPile );
		if( this.posType != BoardPositionType.eNone ) {
		
			this.nRank = nPosRank;
			this.nPile = nPosPile;
			
			this.pos = (BoardPosition)(nPosPile * ChessData.nNumRank + nPosRank);
			return true;
		}
		else {
			
			this.nRank = -1;
			this.nPile = -1;
			
			this.pos = BoardPosition.InvalidPosition;			
		}	
		
		return false;
	}
	
	public bool SetPosition( BoardPosition boardPos ) {
		
		int nPosRank = 0, nPosPile = 0;
		this.posType = ChessPosition.CalcPositionIndex( boardPos, ref nPosRank, ref nPosPile );
		if( this.posType != BoardPositionType.eNone ) {				
			
			this.nRank = nPosRank;
			this.nPile = nPosPile;
				
			this.pos = boardPos;
			return true;
		}
		else {
			
			this.nRank = -1;
			this.nPile = -1;
			
			this.pos = BoardPosition.InvalidPosition;			
		}	
		
		return false;
	}
	
	public bool SetPosition( ChessPosition chessPos ) {			
		
		this.posType = chessPos.posType;			
		this.nRank = chessPos.nRank;
		this.nPile = chessPos.nPile;		
		this.pos = chessPos.pos;	
		
		if( chessPos.posType == BoardPositionType.eNone ) {		
			
			return false;
		}		
		
		return true;
	}
	
	public Vector3 Get3DPosition() {
		
		Vector3 vRet = Vector3.zero;
		if( IsInvalidPos() )
			return vRet;	
		
		vRet.x = nRank - 3.5f;
		vRet.z = nPile - 3.5f;
		
		return vRet;
	}
	
	public bool MovePosition( int nRankMove, int nPileMove ) {			
				
		int nMovedRank, nMovedPile;		
		nMovedRank = nRank + nRankMove;
		nMovedPile = nPile + nPileMove;
			
		if( nMovedRank >= 0 && nMovedRank < ChessData.nNumRank &&
			nMovedPile >= 0 && nMovedPile < ChessData.nNumPile ) {
			
			SetPosition( nMovedRank, nMovedPile);
			return true;
		}
		
		pos = BoardPosition.InvalidPosition;
		posType = BoardPositionType.eNone;
		nRank = -1;
		nPile = -1;
		
		return false;
	}
	
	public bool IsInvalidPos() {
		
		if( pos >= BoardPosition.InvalidPosition || posType == BoardPositionType.eNone ||
			nRank < 0 || nRank >= ChessData.nNumRank || nPile < 0 || nPile >= ChessData.nNumPile )
			return true;
		return false;
	}		
	
	public bool IsLeftBoundary() {
		
		if( (posType & BoardPositionType.eLeft) == BoardPositionType.eLeft )
			return true;
		return false;
	}
	
	public bool IsRightBoundary() {
		
		if( (posType & BoardPositionType.eRight) == BoardPositionType.eRight )
			return true;
		return false;
	}
	
	public bool IsTopBoundary() {
		
		if( (posType & BoardPositionType.eTop) == BoardPositionType.eTop )
			return true;
		return false;
	}
	
	public bool IsBottomBoundary() {
		
		if( (posType & BoardPositionType.eBottom) == BoardPositionType.eBottom )
			return true;
		return false;
	}
	
	public bool IsLeftTopBoundary() {
		
		if( IsLeftBoundary() && IsTopBoundary() )
			return true;
		return false;
	}
	
	public bool IsLeftBottomBoundary() {
		
		if( IsLeftBoundary() && IsBottomBoundary() )
			return true;
		return false;
	}
	
	public bool IsRightTopBoundary() {
		
		if( IsRightBoundary() && IsTopBoundary() )
			return true;
		return false;
	}
	
	public bool IsRightBottomBoundary() {
		
		if( IsRightBoundary() && IsBottomBoundary() )
			return true;
		return false;
	}
	
	public bool IsBoundary() {
		
		if( IsLeftBoundary() || IsRightBoundary() || 
			IsTopBoundary() || IsBottomBoundary() )
			return true;
		return false;
	}	
	
	
	
	public BoardPositionType GetPositionIndex( ref int nPosRank, ref int nPosPile ) {
		
		nPosRank = this.nRank;
		nPosPile = this.nPile;
		
		return posType;		
	}
	
	public BoardPositionType CalcPositionIndex( ref int nPosRank, ref int nPosPile ) {
		
		BoardPositionType retBoardPos = GetPositionType( nPosRank, nPosPile );
		if( retBoardPos != BoardPositionType.eNone ) {				
		
			int nPos = (int)pos;
			nPosRank = nPos % ChessData.nNumRank;
			nPosPile = nPos / ChessData.nNumPile;
		}
		
		return retBoardPos;
	}
	
	
	
	// static function		
	public static BoardPositionType CalcPositionIndex( BoardPosition pos, ref int nPosRank, ref int nPosPile ) {
		
		BoardPositionType retBoardPos = GetPositionType( nPosRank, nPosPile );
		if( retBoardPos != BoardPositionType.eNone ) {				
		
			int nPos = (int)pos;
			nPosRank = nPos % ChessData.nNumRank;
			nPosPile = nPos / ChessData.nNumPile;
		}
		
		return retBoardPos;
	}
	
	public static bool CalcPositionIndex( Vector3 vPos, ref int nPosRank, ref int nPosPile ) {		
	
		int nBoardWidth = (int)ChessData.fBoardWidth;
		
		nPosRank = (int)(vPos.x + 4.0f);																				
		nPosPile = (int)(vPos.z + 4.0f);	
		
		if( nPosRank >= 0 && nPosRank < nBoardWidth && 
			nPosPile >= 0 && nPosPile < nBoardWidth ) {												
			
			return true;
		}		
		
		return false;
	}
	
	public static BoardPositionType GetPositionType( int nPosRank, int nPosPile ) {
		
		BoardPositionType retPosType = BoardPositionType.eNone;
		if( IsInvalidPositionIndex(nPosRank, nPosPile)== false ) {
			
			retPosType |= BoardPositionType.eInside;
				
			if( nPosRank == 0 )
				retPosType |= BoardPositionType.eLeft;
			
			if( nPosRank == ChessData.nNumRank - 1 )
				retPosType |= BoardPositionType.eRight;
			
			if( nPosPile == 0 )
				retPosType |= BoardPositionType.eBottom;
			
			if( nPosPile == 0 )
				retPosType |= BoardPositionType.eTop;								
		}
		
		return retPosType;
	}
	
	public static BoardPositionType GetPositionType( BoardPosition pos ) {
		
		BoardPositionType retPosType = BoardPositionType.eNone;
		
		int nPosRank = 0, nPosPile = 0;
		retPosType = ChessPosition.CalcPositionIndex( pos, ref nPosRank, ref nPosPile );			
		return retPosType;			
	}
	
	public static bool IsInvalidPositionIndex( int nPosRank, int nPosPile ) {
		
		if( (nPosRank < 0 || nPosRank >= ChessData.nNumRank) || (nPosPile < 0 || nPosPile >= ChessData.nNumPile) )
			return true;
		return false;
	}	
	
	
	
	//
	public override bool Equals(System.Object obj)
    {
        // If parameter cannot be cast to Chess return false:
		try {		
	        	
	        // Return true if the fields match:
	        return this == (ChessPosition)obj;	
		}
		catch {
			
			return false;
		}
    }

    public bool Equals(ChessPosition rho)
    {	
        // Return true if the fields match:
         return (pos == rho.pos);// && (posType == rho.posType) && (nRank == rho.nRank) && (nPile == rho.nPile));
    }
	
	public override int GetHashCode()
    {
        return (int)pos ^ (int)posType ^ nRank ^ nPile;
    }

	
	public static bool operator ==( ChessPosition lho, ChessPosition rho )
	{	    
	    // Return true if the fields match:
	    return (lho.pos == rho.pos);// && (lho.posType == rho.posType) && (lho.nRank == rho.nPile) && (lho.nRank == rho.nPile));
	}
	
	public static bool operator !=( ChessPosition lho, ChessPosition rho)
	{
	    return !(lho == rho);
	}	
}
//}