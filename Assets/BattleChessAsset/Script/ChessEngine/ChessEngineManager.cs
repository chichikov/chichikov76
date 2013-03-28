using UnityEngine;
using System.Collections;

// execute for chess engine process
using System.Diagnostics;
using System.IO; 
using System.Threading;
using System.Collections.Generic;



//namespace BattleChess {	

public class SendWorker
{
	// Volatile is used as hint to the compiler that this data
    // member will be accessed by multiple threads.
    volatile bool _shouldStop;	
	Queue<string> queSend;
	private readonly object queSyncRoot;

	
	public StreamWriter swWRiter;
	
	public SendWorker( StreamWriter streamWriter ) {
		
		queSend = new Queue<string>();
		queSyncRoot = new object();
		
		swWRiter = streamWriter;
	}
	
	public void Send( string strCmd ) {
		
		lock( queSyncRoot ) {
			
			queSend.Enqueue(strCmd);
		}
	}
	
    // This method will be called when the thread is started.
    public void ProcessSend()
    {
        while(!_shouldStop)
        {
			lock( queSyncRoot ) {
				
				if( queSend.Count > 0 ) {
					
					string strCmd = queSend.Dequeue() as string;
					
					swWRiter.WriteLine( strCmd );
					//swWRiter.FlushAsync();
					swWRiter.Flush();
				}
			}
            
			Thread.Sleep(100);
		} 
		
		// on sender thread exit, send lastly quit command
		swWRiter.WriteLine( "quit" );
		//swWRiter.FlushAsync();
		swWRiter.Flush();
		
		swWRiter.Close();
    }
	
    public void RequestStop()
    {
        _shouldStop = true;
    }	
	
	private SendWorker() {}
}

	
// chess engine manager class
public class ChessEngineManager {	
	
	// chess engine executive file path
	// Application.dataPath < == asset folder
	string strProcPath = @"ChessEngine\StockFish.exe"; 	
	
	// execute for chess engine process	
    Process procEngine;	
	
	// stream input/output
	//StreamWriter swWRiter;
	// 비동기 로딩 스트림은 레퍼런스를 가질 수 없다!!!
	//StreamReader srReader;
	//StreamReader srErrReader;	
	
	Thread threadSend;
	SendWorker sendWorker;
	
	// received command respond queue
	Queue<string> queReceived;
	private readonly object queSyncRoot;	
	
	
	// engine command parser
	ChessEngineCmdParser cmdParser;	
	
	
	
	public IProcessChessEngine EngineCmdExecuter { get; set; }
	
	// property
	public ChessEngineConfig DefaultConfigData { get; private set; }
	public ChessEngineConfig CurrentConfigData { get; private set; }
	
	public bool IsEngineInit { get; private set; }
	public bool IsEngineRunning { get; private set; }
	
	public bool IsPonderMode { get; private set; }
	public bool IsPonderFailed { get; set; }
	public string CurrentPonder { get; private set; }
	
	public bool IsForceStop { get; set; }
	public bool IsWaitforNewGame { get; set; }
	
	
	
	public static ChessEngineManager Instance { 
		get { 
			return Singleton<ChessEngineManager>.Instance; 
		} 
	}
	
	


	
	
	private ChessEngineManager() {		
		
		procEngine = null;
		queReceived = null;
		queSyncRoot = new object();
		//swWRiter = null;
		threadSend = null;
		sendWorker = null;
		//srReader = null;
		//srErrReader = null;
		
		cmdParser = null;		
		DefaultConfigData = null;		
		CurrentConfigData = null;
		IsEngineInit = false;
		IsEngineRunning = false;
		IsPonderMode = false;
		IsPonderFailed = false;
		IsWaitforNewGame = false;
		IsForceStop = false;
	}	
	
	// interface
	//public IEnumerator Start() {	
	public void Start() {	
		
		// clear received command respond que
		queReceived = new Queue<string>();		
		
		try
		{
			procEngine = new Process();
			procEngine.StartInfo.FileName = strProcPath;		
			//procEngine.StartInfo.WorkingDirectory = @"ChessEngine\";
			//procEngine.StartInfo.Arguments = "uci";
			procEngine.StartInfo.CreateNoWindow = true;
			procEngine.StartInfo.UseShellExecute = false;		
			procEngine.StartInfo.ErrorDialog = false;
			procEngine.StartInfo.RedirectStandardOutput = true;
			procEngine.StartInfo.RedirectStandardInput = true;
			procEngine.StartInfo.RedirectStandardError = true;		
			
			// Set our event handler to asynchronously read the sort output/err.
	        procEngine.OutputDataReceived += new DataReceivedEventHandler(StandardOutputHandler);		
			procEngine.ErrorDataReceived += new DataReceivedEventHandler(StandardErrorHandler);
			
			// start chess engine(stockfish)		
			procEngine.Start();
			
			// Start the asynchronous read of the output stream.
	        procEngine.BeginOutputReadLine();
			procEngine.BeginErrorReadLine();
			
			//swWRiter = procEngine.StandardInput;	
			//swWRiter.AutoFlush = true;
			//srReader = procEngine.StandardOutput;
			//srErrReader = procEngine.StandardError;
			
			cmdParser = new ChessEngineCmdParser() { Cmd = null };
			DefaultConfigData = new ChessEngineConfig();
			CurrentConfigData = new ChessEngineConfig();
			
			// wait for 2.0 sec for process thread running
			//yield return new WaitForSeconds(2.0f);
			//yield return new WaitForSeconds(5.0f);
			
			sendWorker = new SendWorker( procEngine.StandardInput );
			threadSend = new Thread(sendWorker.ProcessSend);
			threadSend.Start();	
			
			IsEngineRunning = true;
		}
		catch( System.Exception e ) {
			
			UnityEngine.Debug.LogError( "ChessEngineManager - Start() Failed!!! because of" + e.Message );           
		}		
	}
	
	public void End() {
		
		if( IsEngineRunning ) {
			
			queReceived.Clear();				
			
			sendWorker.RequestStop();	
			
			threadSend.Join();
			threadSend = null;
			sendWorker = null;
	
			// 비동기 로딩 스트림은 클로즈 하면 안된다!!!
			//srReader.Close();
			//srErrReader.Close();
			procEngine.CancelOutputRead();
			procEngine.CancelErrorRead();		
			
			procEngine.Close();				
			procEngine = null;	
			
			cmdParser = null;
			DefaultConfigData = null;
			CurrentConfigData = null;
			
			IsEngineRunning = false;
			IsEngineInit = false;
		}
	}
	
	public void Send( string strUciCmd ) {
		
		if (!string.IsNullOrEmpty( strUciCmd ))
        {		
			sendWorker.Send( strUciCmd );			
		}
	}	
	
	public string PopReceivedQueue() {		
		
		string strRet = null;		
		lock( queSyncRoot ) {			
			
			if( queReceived.Count > 0 )
			{			
				strRet = queReceived.Dequeue() as string;
				//UnityEngine.Debug.Log(strRet);
			}
		}				
			
		return strRet;
	}	
	
	public EngineToGuiCommand ParseCommand( string strCmdLine ) {
		
		if( cmdParser != null ) {			
			
			bool bParseSuccess = cmdParser.Parse( strCmdLine );			
			if( bParseSuccess ) {			
				
				return cmdParser.Cmd;
			}
		}
		
		return null;
	}
	
	public bool SetDefaultOptionCommand( CommandBase.CommandData commandData ) {
		
		if( commandData != null ) {
			
			return DefaultConfigData.SetConfigOption( commandData );			
		}
		
		return false;
	}
	
	public void SetCurrentBoolOption( string strOptionName, bool bValue ) {
		
		if( DefaultConfigData == null || DefaultConfigData.IsEmpty() ) {
			//UnityEngine.Debug.Log( "ChessEngineManager::SetCurrentBoolOption() - " + strOptionName );
			return;
		}
		
		ChessEngineOption defaultOption = DefaultConfigData.GetConfigOption( strOptionName );
		ChessEngineOption currentOption = new ChessEngineOption();	
		currentOption.CopyFrom( defaultOption );
		currentOption.SetBoolValue( bValue );
		CurrentConfigData.AddOption( currentOption );
	}
	
	public void SetCurrentRangeFloatOption( string strOptionName, float fValue ) {
		
		if( DefaultConfigData == null || DefaultConfigData.IsEmpty() ) {
			//UnityEngine.Debug.Log( "ChessEngineManager::SetCurrentRangeFloatOption() - " + strOptionName );
			return;
		}
		
		ChessEngineOption defaultOption = DefaultConfigData.GetConfigOption( strOptionName );
		ChessEngineOption currentOption = new ChessEngineOption();	
		currentOption.CopyFrom( defaultOption );
		currentOption.SetRangeFloatValue( fValue );
		CurrentConfigData.AddOption( currentOption );	
	}
	
	public void SetCurrentStringOption( string strOptionName, string strValue ) {
		
		if( DefaultConfigData == null || DefaultConfigData.IsEmpty() ) {
			//UnityEngine.Debug.Log( "ChessEngineManager::SetCurrentStringOption() - " + strOptionName );
			return;
		}
		
		ChessEngineOption defaultOption = DefaultConfigData.GetConfigOption( strOptionName );
		ChessEngineOption currentOption = new ChessEngineOption();	
		currentOption.CopyFrom( defaultOption );
		currentOption.SetStringValue( strValue );
		CurrentConfigData.AddOption( currentOption );	
	}
	
	// send current option to engine
	public void SendCurrentOption() {
		
		Dictionary<string, ChessEngineOption> optionMap = CurrentConfigData.GetOptionMap();
		if( optionMap != null ) {		
			
			foreach( KeyValuePair<string, ChessEngineOption> optionPair in optionMap ) {			
				
				Send( optionPair.Value.GetSendOptionString() );
			}
			
			// update engine ponder option state
			ChessEngineOption ponderOption = CurrentConfigData.GetConfigOption( "Ponder" );			
			if( ponderOption == null )
				ponderOption = DefaultConfigData.GetConfigOption( "Ponder" );
			
			if( ponderOption != null )				
				IsPonderMode = ponderOption.GetBoolValue();							
			else
				IsPonderMode = false;		
		}
	}	
	
	public void SendNewGame() {
		
		SendStop();
		
		Send( "ucinewgame" );
		Send( "isready" );
		
		IsWaitforNewGame = true;
	}
	
	public void SendStop() {
		
		IsForceStop = true;
		Send( "stop" );
	}

	
	// internal	
	
	// async std output/err read
	private void StandardOutputHandler( object objProcess, DataReceivedEventArgs outLine )
    {
        // Collect the command output. 
        if (!string.IsNullOrEmpty(outLine.Data))
        {			
			//UnityEngine.Debug.Log(outLine.Data);
			string strTrimOutLine = outLine.Data;
			char [] sTrim = { '\r', '\n' };			
			strTrimOutLine = strTrimOutLine.Trim( sTrim );
			
			lock( queSyncRoot ) {								
				
            	queReceived.Enqueue( strTrimOutLine );				
			}			
        }
    }	
	
	private void StandardErrorHandler( object objProcess, DataReceivedEventArgs outLine )
    {
        // Collect the Error.
		// print log unity debug log window
    }   
	
	
	
	
	
	// process engine command
	// process engine command respond
	// process engine command respond
	public void ProcessEngineCommand() {
		
		if( IsEngineRunning ) {
			// read one line
			string strCurCommandLine = PopReceivedQueue();
			while( strCurCommandLine != null ) {
				
				UnityEngine.Debug.Log(strCurCommandLine);
				
				// process one engine respond
				EngineToGuiCommand command = ParseCommand( strCurCommandLine );
				if( command != null ) {				
					
					//command.PrintCommand();
					SetDefaultOptionCommand( command.CmdData );
					
					ExcuteEngineCommand( command );								
				}
				
				strCurCommandLine = PopReceivedQueue();
			}	
		}
	}
	
	
	
	// 
	bool ExcuteIdCommand( CommandBase.CommandData cmdData ) {
		
		if( EngineCmdExecuter != null )
			return EngineCmdExecuter.OnIdCommand( cmdData );
		
		return false;
	}
	
	bool ExcuteUciOkCommand( CommandBase.CommandData cmdData ) {			
		
		if( EngineCmdExecuter != null ) {
			
			IsEngineInit = true;
			
			return EngineCmdExecuter.OnUciOkCommand( cmdData );
		}
		
		return false;
	}
	
	bool ExcuteReadyOkCommand( CommandBase.CommandData cmdData ) {		
		
		if( EngineCmdExecuter != null )
			return EngineCmdExecuter.OnReadyOkCommand( cmdData );
		
		return false;
	}
	
	bool ExcuteCopyProtectionCommand( CommandBase.CommandData cmdData ) {
		
		if( EngineCmdExecuter != null )
			return EngineCmdExecuter.OnCopyProtectionCommand( cmdData );
		
		return false;
	}
	
	bool ExcuteRegistrationCommand( CommandBase.CommandData cmdData ) {
		
		if( EngineCmdExecuter != null )
			return EngineCmdExecuter.OnRegistrationCommand( cmdData );
		
		return false;
	}
	
	bool ExcuteOptionCommand( CommandBase.CommandData cmdData ) {
		
		if( EngineCmdExecuter != null )
			return EngineCmdExecuter.OnOptionCommand( cmdData );
		
		return false;
	}
	
	bool ExcuteInfoCommand( CommandBase.CommandData cmdData ) {
		
		if( EngineCmdExecuter != null )
			return EngineCmdExecuter.OnInfoCommand( cmdData );
		
		return false;
	}
	
	bool ExcuteBestMoveCommand( CommandBase.CommandData cmdData ) {		
		
		if( EngineCmdExecuter != null ) {					
				
			if( IsPonderMode ) {
				
				// ignore command if ponder fail!!!		
				if( IsPonderFailed ) {
					
					CurrentPonder = null;
					IsPonderFailed = false;
					return false;
				}
				else {
					
					CurrentPonder = cmdData.GetSubCommandValue("ponder");
				}
			}
			else {
				
				if( IsForceStop ) {
					
					IsForceStop = false;
					CurrentPonder = null;
					return false;
				}				
					
				CurrentPonder = null;
				
			}
			
			return EngineCmdExecuter.OnBestMoveCommand( cmdData );
		}
		
		return false;
	}	
	
	bool ExcuteInitStockfishCommand( CommandBase.CommandData cmdData ) {		
		
		if( EngineCmdExecuter != null )
			return EngineCmdExecuter.OnInitStockfishCommand( cmdData );
		
		return false;
	}
	
	// delegate function for engine command
	public bool ExcuteEngineCommand( EngineToGuiCommand cmd ) {		
		
		if( cmd == null )
			return false;
		
		bool bValidCmd = !cmd.CmdData.InvalidCmd;		
		if( bValidCmd ) {
			
			string strCmd = cmd.CmdData.StrCmd;
			
			switch( strCmd ) {
					
				case "id":		
					return ExcuteIdCommand( cmd.CmdData );						
						
				case "uciok":	
					return ExcuteUciOkCommand( cmd.CmdData );				
					
				case "readyok":		
					return ExcuteReadyOkCommand( cmd.CmdData );							
				
				case "copyprotection":
					return ExcuteCopyProtectionCommand( cmd.CmdData );						
				
				case "registration":
					return ExcuteRegistrationCommand( cmd.CmdData );							
					
				case "option":
					return ExcuteOptionCommand( cmd.CmdData );							
				
				case "info":
					return ExcuteInfoCommand( cmd.CmdData );											
				
				case "bestmove":
					return ExcuteBestMoveCommand( cmd.CmdData );	
				
				case "Stockfish":
					return ExcuteInitStockfishCommand( cmd.CmdData );
					
				default:								
					return false;					
			} // switch	
			
			//return true;
		}	
		
		return false;
	}	
}
//}
