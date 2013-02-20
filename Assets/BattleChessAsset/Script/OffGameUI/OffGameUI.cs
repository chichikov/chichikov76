using UnityEngine;
using System.Collections;

public class OffGameUI : MonoBehaviour, IProcessChessEngine {

	// Use this for initialization
	void Start () {
		
		// retrieve tagged(panel) GameObject and Add GUIManager
		GameObject [] aPanel = GameObject.FindGameObjectsWithTag( "Panel" );
		foreach( GameObject panel in aPanel ) {
			
			GUIManager.Instance.AddPanel( panel.name, panel );
		}
		
		// first init menu select
		GUIManager.Instance.ShowAllPanel(false);
		GUIManager.Instance.ShowPanel( "InitPanel", true );
		
		// chess engine start
		ChessEngineManager.Instance.EngineCmdExecuter = this;
		ChessEngineManager.Instance.Start();		
	}
	
	// Update is called once per frame
	void Update () {
		
		// process engine command respond
		ChessEngineManager.Instance.ProcessEngineCommand();			
	}
	
	void OnApplicationQuit()
	{
		ChessEngineManager.Instance.End();		
	}
	
	
	
	
	
	// on Process Engine command
	public bool OnInitStockfishCommand( CommandBase.CommandData cmdData )
	{
		// why can't i send first command????????
		// 엔진에서 첫번째 명령 스트링을 초기화 안해서....
		ChessEngineManager.Instance.Send( "\n" );
		ChessEngineManager.Instance.Send( "uci" );
		return true;				
	}
	
	public bool OnIdCommand( CommandBase.CommandData cmdData )
	{		
		return true;		
	}
	public bool OnUciOkCommand( CommandBase.CommandData cmdData )
	{
		// send setoption command!!!		
		
		return true;
	}
	
	public bool OnReadyOkCommand( CommandBase.CommandData cmdData )
	{		
		return true;	
	}
	
	public bool OnCopyProtectionCommand( CommandBase.CommandData cmdData )
	{
		
		return true;
	}
	
	public bool OnRegistrationCommand( CommandBase.CommandData cmdData )
	{
		
		return true;
	}
	
	public bool OnOptionCommand( CommandBase.CommandData cmdData )
	{
		
		return true;
	}
	
	public bool OnInfoCommand( CommandBase.CommandData cmdData )
	{
		
		return true;
	}
	
	public bool OnBestMoveCommand( CommandBase.CommandData cmdData )
	{		
		return true;				
	}
}
