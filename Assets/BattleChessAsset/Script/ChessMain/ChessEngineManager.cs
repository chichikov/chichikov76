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
    }
	
    public void RequestStop()
    {
        _shouldStop = true;
    }
	
	public void Clear()
    {
        swWRiter.Close();
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
	
	ChessEngineConfig configData;
	
	// property
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
		configData = null;		
	}	
	
	// interface
	public IEnumerator Start() {	
		
		// clear received command respond que
		queReceived = new Queue<string>();		
		
		procEngine = new Process();
		procEngine.StartInfo.FileName = strProcPath;
		procEngine.StartInfo.Arguments = "uci";
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
		configData = new ChessEngineConfig();
		
		// wait for 2.0 sec for process thread running
		yield return new WaitForSeconds(2.0f);
		//yield return new WaitForSeconds(5.0f);
		
		sendWorker = new SendWorker( procEngine.StandardInput );
		threadSend = new Thread(sendWorker.ProcessSend);
		threadSend.Start();
		
		// send command to chess engine
		// 첫번째 명령이 안먹는 이유는?????
		// 실행 파라미터로 "uci"를 주어야 하는데 스트림 리다이렉션때문에 이게 안먹힘...	
		// 
		//Send( "Ping Test" );
		//Send( "uci" );	
		
		//Send( "isready" );
	}
	
	public void End() {
			
		queReceived.Clear();		
		
		//swWRiter.Close();		
		sendWorker.RequestStop();
		sendWorker.Clear();
		
		threadSend.Join();
		threadSend = null;
		sendWorker = null;

		// 비동기 로딩 스트림은 클로즈 하면 안된다!!!
		//srReader.Close();
		//srErrReader.Close();
		procEngine.CancelOutputRead();
		procEngine.CancelErrorRead();
		
		procEngine.Kill();
		procEngine.Close();				
		procEngine = null;	
		
		cmdParser = null;
		configData = null;
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
	
	public bool SetConfigCommand( CommandBase.CommandData commandData ) {
		
		if( commandData != null ) {
			
			return configData.SetConfigOption( commandData );			
		}
		
		return false;
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
}
//}
