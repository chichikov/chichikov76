using UnityEngine;
using System.Collections;


//namespace BattleChess {	
	
public class ChessPiece {
	
	public GameObject gameObject;
	
	public PlayerSide playerSide;
	public PieceType pieceType;
	public PiecePlayerType piecePlayerType;			
	
	
	public ParticleSystem movablePSystem;
	
	
	public ChessPiece() {
		
		gameObject = null;		
		playerSide = PlayerSide.e_NoneSide;
		pieceType = PieceType.e_None;
		piecePlayerType = PiecePlayerType.eNone_Piece;				
	}	
			
	public ChessPiece( GameObject gameObject, PlayerSide playerSide, 
		PiecePlayerType piecePlayerType ) {
		
		this.gameObject = gameObject;
		this.playerSide = playerSide;
		this.pieceType = ChessUtility.GetPieceType( piecePlayerType );
		this.piecePlayerType = piecePlayerType;				
	}	
	
	public void SetPiece( ChessPiece chessPiece ) {
		
		this.gameObject = chessPiece.gameObject;
		this.playerSide = chessPiece.playerSide;
		this.pieceType = chessPiece.pieceType;
		this.piecePlayerType = chessPiece.piecePlayerType;			
	}	
	
	public void Clear( bool bClearGameObject = false ) {	
		
		if( bClearGameObject && gameObject ) {
			//UnityEngine.Debug.LogError( "ChessPiece::Clear() - success!!!!" );
			gameObject.transform.position = new Vector3(0.0f, -10.0f, 0.0f);
		}		
	}		
	
	public void CopyFrom( ChessPiece chessPiece ) {
		
		this.gameObject = chessPiece.gameObject;
		this.playerSide = chessPiece.playerSide;
		this.pieceType = chessPiece.pieceType;
		this.piecePlayerType = chessPiece.piecePlayerType;				
	}
	
	public void SetPosition( Vector3 vPos ) {
		
		if( IsBlank() == false ) {
			//UnityEngine.Debug.LogError( "ChessBoardSquare::SetPiece() - piece type : " + pieceType );
			this.gameObject.transform.position = vPos;
		}
	}	
	
	
	public bool IsBlank() {
		
		if( gameObject == null || piecePlayerType == PiecePlayerType.eNone_Piece )
			return true;
		return false;			
	}
	
	public bool IsEnemy( PlayerSide otherPlayerSide ) {
		
		if( IsBlank() == false && playerSide != otherPlayerSide )
			return true;
		return false;			
	}	
}	
//}

