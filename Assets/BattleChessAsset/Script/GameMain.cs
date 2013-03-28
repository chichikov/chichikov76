using UnityEngine;
using System.Collections;



public class GameMain : MonoBehaviour, IProcessChessEngine {	
	
	
	
	// property
	public IProcessChessEngine EngineCmdExecuterProxy {
		
		get; set;
	}
	
	
	public GameMain() {
		
		EngineCmdExecuterProxy = null;
	}
	
	// Use this for first initialization
	void Awake() {
	
		// enroll game object singleton
		SingletonGameObject<GameMain>.AddSingleton( this ); 
		
		// chess engine start
		ChessEngineManager.Instance.EngineCmdExecuter = this;		
	}
	
	// Use this for second initialization
	void Start() {					
		
		GameState.Instance.currentState = GameState.eGameState.Intro;			
	}
	
	// Update is called once per frame
	void Update() {		
		// process engine command respond
		ChessEngineManager.Instance.ProcessEngineCommand();		
	}
	
	void OnDestroy () {	
		
		ChessEngineManager.Instance.End();	
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
		
		if( EngineCmdExecuterProxy != null )
			return EngineCmdExecuterProxy.OnInitStockfishCommand( cmdData );
		
		return true;				
	}
	
	public bool OnIdCommand( CommandBase.CommandData cmdData )
	{	
		
		if( EngineCmdExecuterProxy != null )
			return EngineCmdExecuterProxy.OnIdCommand( cmdData );
		
		return true;		
	}
	public bool OnUciOkCommand( CommandBase.CommandData cmdData )
	{
		// enable init menu's start and option button
		OffGameUI offGameUIScript = GUIManager.Instance.GetGUIGameObjectScript<OffGameUI>( "OffGameUI" );		
		offGameUIScript.initPanelScript.startBtn.isEnabled = true;
		offGameUIScript.initPanelScript.optionBtn.isEnabled = true;		
		
		if( EngineCmdExecuterProxy != null )
			return EngineCmdExecuterProxy.OnUciOkCommand( cmdData );
		
		return true;
	}
	
	public bool OnReadyOkCommand( CommandBase.CommandData cmdData )
	{		
		if( EngineCmdExecuterProxy != null )
			return EngineCmdExecuterProxy.OnReadyOkCommand( cmdData );
		
		return true;	
	}
	
	public bool OnCopyProtectionCommand( CommandBase.CommandData cmdData )
	{
		
		if( EngineCmdExecuterProxy != null )
			return EngineCmdExecuterProxy.OnCopyProtectionCommand( cmdData );
		
		return true;
	}
	
	public bool OnRegistrationCommand( CommandBase.CommandData cmdData )
	{
		
		if( EngineCmdExecuterProxy != null )
			return EngineCmdExecuterProxy.OnRegistrationCommand( cmdData );
		
		return true;
	}
	
	public bool OnOptionCommand( CommandBase.CommandData cmdData )
	{
		// enable/disable ui engine option
		
		// setting default option				
		ChessEngineOption option = ChessEngineManager.Instance.DefaultConfigData.GetConfigOption( cmdData.GetSubCommandValue("name") );
		if( option != null ) {
			
			OffGameUI offGameUIScript = GUIManager.Instance.GetGUIGameObjectScript<OffGameUI>( "OffGameUI" );			
			offGameUIScript.optionScrollPanelScript.SetOption( option );
			return true;
		}
		
		
		if( EngineCmdExecuterProxy != null )
			return EngineCmdExecuterProxy.OnOptionCommand( cmdData );
				
		return false;
	}
	
	public bool OnInfoCommand( CommandBase.CommandData cmdData )
	{
		
		if( EngineCmdExecuterProxy != null )
			return EngineCmdExecuterProxy.OnInfoCommand( cmdData );
		
		return true;
	}
	
	public bool OnBestMoveCommand( CommandBase.CommandData cmdData )
	{
		
		if( EngineCmdExecuterProxy != null )
			return EngineCmdExecuterProxy.OnBestMoveCommand( cmdData );
		
		return true;				
	}
	
	
	
	
	
	
	
	// static
	public static GameMain Instance {
		
		get { 
			return SingletonGameObject<GameMain>.Instance; 
		} 
	}
}
