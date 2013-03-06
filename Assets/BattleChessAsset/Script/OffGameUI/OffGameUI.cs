using UnityEngine;
using System.Collections;

public class OffGameUI : MonoBehaviour, IProcessChessEngine {	
	
	
	public InitMenu initPanelScript;
	public ModeMenu modePanelScript;
	public OptionMenu optionPanelScript;
	public OptionScrollPanel optionScrollPanelScript;
	public EtcMenu etcPanelScript;
	
	
	// Use this for first initialization	
	void Awake() {	
		
		// first init menu select
		GUIManager.Instance.ShowAllPanel(false);
		GUIManager.Instance.ShowPanel( "InitPanel", true );
		
		// chess engine start
		ChessEngineManager.Instance.EngineCmdExecuter = this;
		ChessEngineManager.Instance.Start();
	}
	
	// Use this for initialization
	void Start () {			
				
	}
	
	// Update is called once per frame
	void Update () {
		
		// process engine command respond
		ChessEngineManager.Instance.ProcessEngineCommand();			
	}
	
	void OnDestroy () {	
		
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
		// enable init menu's start and option button			
		initPanelScript.startBtn.isEnabled = true;
		initPanelScript.optionBtn.isEnabled = true;		
		
		return true;
	}
	
	public bool OnReadyOkCommand( CommandBase.CommandData cmdData )
	{
		Application.LoadLevel("BattleChessInfinity");
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
		// enable/disable ui engine option
		
		// setting default option				
		ChessEngineOption option = ChessEngineManager.Instance.DefaultConfigData.GetConfigOption( cmdData.GetSubCommandValue("name") );
		if( option != null ) {
			
			optionScrollPanelScript.SetOption( option );
			return true;
		}
		
				
		return false;
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
