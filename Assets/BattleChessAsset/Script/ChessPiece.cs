using UnityEngine;
using System.Collections;


//namespace BattleChess {	
	
public class ChessPiece {
	
	public GameObject gameObject;
	
	public PlayerSide playerSide;
	public PieceType pieceType;
	public PiecePlayerType piecePlayerType;		
	
	public bool bEnPassantCapture;
	
	public ParticleSystem movablePSystem;
	
	
	public ChessPiece() {
		
		gameObject = null;		
		playerSide = PlayerSide.e_NoneSide;
		pieceType = PieceType.e_None;
		piecePlayerType = PiecePlayerType.eNone_Piece;		
		bEnPassantCapture = false;
	}	
			
	public ChessPiece( GameObject gameObject, PlayerSide playerSide, 
		PiecePlayerType piecePlayerType ) {
		
		this.gameObject = gameObject;
		this.playerSide = playerSide;
		this.pieceType = ChessUtility.GetPieceType( piecePlayerType );
		this.piecePlayerType = piecePlayerType;	
		
		this.bEnPassantCapture = false;		
	}	
	
	public void SetPiece( ChessPiece chessPiece ) {
		
		this.gameObject = chessPiece.gameObject;
		this.playerSide = chessPiece.playerSide;
		this.pieceType = chessPiece.pieceType;
		this.piecePlayerType = chessPiece.piecePlayerType;
		this.bEnPassantCapture = chessPiece.bEnPassantCapture;		
	}	
	
	public void Clear( bool bClearGameObject = false ) {	
		
		if( bClearGameObject && gameObject )
			gameObject.transform.position = new Vector3(0.0f, -10.0f, 0.0f);		
							
		bEnPassantCapture = false;		
	}		
	
	public void CopyFrom( ChessPiece chessPiece ) {
		
		this.gameObject = chessPiece.gameObject;
		this.playerSide = chessPiece.playerSide;
		this.pieceType = chessPiece.pieceType;
		this.piecePlayerType = chessPiece.piecePlayerType;		
		this.bEnPassantCapture = chessPiece.bEnPassantCapture;
	}
	
	public void SetPosition( Vector3 vPos ) {
		
		if( IsBlank() == false ) {
			
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

