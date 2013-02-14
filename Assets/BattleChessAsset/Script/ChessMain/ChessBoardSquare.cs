using UnityEngine;
using System.Collections;

public class ChessBoardSquare {	
	
	public ChessPiece piece;		
	
	public ChessPosition position;
	
	public ParticleSystem movablePSystem;
	
	
	public ChessBoardSquare() {
		
		piece = null;		
		position = new ChessPosition(-1, -1);
		movablePSystem = null;
	}	
			
	public ChessBoardSquare( ChessPiece piece, ParticleSystem moveablePSystem, int nPile, int nRank ) {
		
		this.position = new ChessPosition( nRank, nPile );
		this.piece = piece;	
		if( this.piece != null )
			this.piece.SetPosition( this.position.Get3DPosition() );
		
		SetMovableEffect( moveablePSystem );
	}	
	
	public void SetPiece( ChessPiece chessPiece ) {
		
		if( piece != null ) {
			
			UnityEngine.Debug.Log( "-------------------------------------ChessBoardSquare::SetPiece() - piece type : " + piece.pieceType + 
				"   File : " +  position.nPile + "   Rank : " + position.nRank );
		}
		
		piece = chessPiece;
		if( piece != null )
			piece.SetPosition( this.position.Get3DPosition() );			
	}	
	
	public void ClearPiece( bool bClearGameObject = false ) {				
		
		if( IsBlank() )
			return;
		
		//if( bClearGameObject )
		//	UnityEngine.Debug.Log( "---------------------------------------ChessBoardSquare::Clear() - success!!!!    piece type : " + piece.pieceType + 
		//		"   File : " +  position.nPile + "   Rank : " + position.nRank );
			
		piece.Clear( bClearGameObject );
		
		piece = null;
	}
	
	
	
	
	public void SetMovableEffect( ParticleSystem movablePSystem ) {
		
		if( position.IsInvalidPos() )
			return;					
		
		this.movablePSystem = movablePSystem;							
		this.movablePSystem.Stop();			
		
		Vector3 pos = Vector3.zero;
		pos.x = position.nRank - 3.5f;
		pos.y = 0.01f;
		pos.z = position.nPile - 3.5f;
		Quaternion rot = Quaternion.identity;
		
		this.movablePSystem.gameObject.transform.position = pos;
		this.movablePSystem.gameObject.transform.rotation = rot;
		
		this.movablePSystem.gameObject.renderer.material.SetColor( "_Color", Color.blue );
	}
	
	public void ShowMovableEffect( bool bShow ) {
		
		if( movablePSystem == null )
			return;
		
		if( bShow ) {
			
			movablePSystem.renderer.enabled = true;
			movablePSystem.Play();				
		}
		else{
			
			movablePSystem.Stop();
			movablePSystem.renderer.enabled = false;
		}			
	}
	
	public bool IsBlank() {
		
		if( piece == null )
			return true;
		return false;			
	}	
	
	public bool IsEnemy( PlayerSide otherPlayerSide ) {
		
		if( IsBlank() )
			return false;
		
		return piece.IsEnemy( otherPlayerSide );			
	}	
	
	public bool IsInvalidPos() {
		
		return position.IsInvalidPos();
	}
	
	// override operator
	public override bool Equals(System.Object obj)
    {
        // If parameter is null return false.
        if (obj == null)
        {
            return false;
        }

        // If parameter cannot be cast to Point return false.
        ChessBoardSquare p = obj as ChessBoardSquare;
        if ((System.Object)p == null)
        {
            return false;
        }

        // Return true if the fields match:
        return (position == p.position) && ( piece == p.piece );
    }

    public bool Equals(ChessBoardSquare p)
    {
        // If parameter is null return false:
        if ((object)p == null)
        {
            return false;
        }

        // Return true if the fields match:
        return (position == p.position) && ( piece == p.piece );
    }

    public override int GetHashCode()
    {
        return (int)position.pos;
    }

}
