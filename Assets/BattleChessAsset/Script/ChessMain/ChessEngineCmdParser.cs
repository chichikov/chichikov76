using UnityEngine;
using System.Collections;
using System.Collections.Generic;


//namespace BattleChess {


public class CommandBase {
	
	// command parse exception
	class CmdException : System.Exception
	{
	    public CmdException( string message ) : base( message )
	    {
	
	    }
	}
	
	class CmdParseException : CmdException
	{
	    public CmdParseException( string message ) : base( message )
	    {
	
	    }
	}
	
	
		
	// inner struct
	public class CommandData {
		
		// command/value pair
		public string StrCmd { get; set; }		
		
		Queue<string> queueStrValue;
		public Queue<string> QueueStrValue 
		{ 
			get {
				
				return queueStrValue;
			}
			
			set {
				
				queueStrValue = value;
			}
		}		
		
		// sub command		
		Queue<CommandData> queueSubCmdData;
		public Queue<CommandData> QueueSubCmdData { 
			get {
				
				return queueSubCmdData;
			} 
			
			set {
				
				queueSubCmdData = value;
			}
		}
		
		// whether invalid command or not
		public bool InvalidCmd { get; set; }		
		
		
		public CommandData() {
			
			queueStrValue = new Queue<string>();
			queueSubCmdData = new Queue<CommandData>();
			
			InvalidCmd = true;
		}			
	}
	
	
	
	// command source line
	public string StrCmdSrc { get; set; }		
	
	// command data
	public CommandData CmdData { get; set; }
	
	
	
	
	// interface
	// init
	public void Init( string strCommandSrc )
	{
		StrCmdSrc = strCommandSrc;
		CmdData = new CommandData();		
	}
	
	// parse command line
	public bool ParseCommand() {		
		
		try {
			// split token
			char[] delimiterChars = { ' ' };
	        string[] str_tokens = StrCmdSrc.Split(delimiterChars);					
			
			// get command token	
			CmdData.StrCmd = str_tokens[0];				
			
			switch( CmdData.StrCmd ) {
				
			case "id":		
				ParseIdCommand( CmdData, str_tokens );		
			break;
					
			case "uciok":
				ParseUciOkCommand( CmdData, str_tokens );		
			break;
				
			case "readyok":		
				ParseReadyOkCommand( CmdData, str_tokens );		
			break;		
			
			case "copyprotection":
				ParseCopyProtectionCommand( CmdData, str_tokens );		
			break;
			
			case "registration":
				ParseRegistrationCommand( CmdData, str_tokens );			
			break;
				
			case "option":
				ParseOptionCommand( CmdData, str_tokens );			
			break;
			
			case "info":
				ParseInfoCommand( CmdData, str_tokens );		
			break;
				
			case "bestmove":
				ParseBestMoveCommand( CmdData, str_tokens );			
			break;
				
			case "Stockfish":
				ParseInitStockfishCommand( CmdData, str_tokens );			
			break;
			
				
			default:												
				return false;						
				
			} // switch		
			
			if( CmdData.InvalidCmd ) {
				
				UnityEngine.Debug.Log( "Parsed Unknown Command or Sub Command Error" + " " + CmdData.StrCmd );				
				return false;
			}
			
			return true;
			
		} // switch		
		catch ( CmdException ex ) {
		
			UnityEngine.Debug.Log( ex.ToString() );			
		}
		finally {
			
						
		}
		
		return false;
	}	
	
	// helper function
	public void PrintCommand() {
		
		if( CmdData != null ) {
			
			string strCommand = CmdData.StrCmd;										
			
			PrintCommandValue( CmdData.QueueStrValue, ref strCommand );
			
			PrintSubCommand( CmdData.QueueSubCmdData, ref strCommand );			
			
			UnityEngine.Debug.Log( strCommand );
		}
		else {
			
			UnityEngine.Debug.Log( "Not Created Command" );
		}
	}
	
	
	// 
	void PrintCommandValue( Queue<string> queStrValue, ref string strTotalCmd ) {
		
		if( queStrValue.Count > 0 ) {
			foreach( string strCurrValue in queStrValue ) {				
				strTotalCmd += " " + strCurrValue;
			}	
		}
	}
	
	void PrintSubCommand( Queue<CommandData> queSubCmd, ref string strTotalCmd ) {
		
		if( queSubCmd.Count > 0 ) {
			foreach( CommandData cmdData in queSubCmd ) {								
				strTotalCmd += " " + cmdData.StrCmd;
				PrintCommandValue( cmdData.QueueStrValue, ref strTotalCmd );
				PrintSubCommand( cmdData.QueueSubCmdData, ref strTotalCmd );	
			}	
		}		
	}	
	
	string[] GetNextTokens( string [] str_tokens ) {
		
		if( str_tokens != null && str_tokens.Length > 1 ) {
			
			string [] str_next_tokens = new string[str_tokens.Length - 1];
			int nCnt = 0;
			foreach( string currToken in str_tokens ) {
				
				if( nCnt >= 1 ) {
					str_next_tokens[nCnt-1] = currToken;
				}
				
				nCnt++;
			}
			return str_next_tokens;
		}
		
		throw new CmdParseException( "GetNextTokens() - Invalid Token Exception Throw!!!" );
		//return null;
	}	
	
	string[] GetNextCommandTokens( string [] str_tokens, string strNextCmd, out string strCurrValueTokens ) {
		
		string [] str_next_tokens = null;
		if( str_tokens != null && str_tokens.Length > 1 ) {		
			
			// extract current's command value
			bool bEndExtracted = false;
			bool bCreatedExtra = false;
			
			int nCurrToken = 0;
			int nCurrNextToken = 0;
			
			strCurrValueTokens = "";
			
			foreach( string strToken in str_tokens ) {
				
				if( strToken == strNextCmd )			
					bEndExtracted = true;
				
				// 
				if( bEndExtracted ) {
					
					if( bCreatedExtra == false ) {
						
						str_next_tokens = new string[str_tokens.Length - nCurrToken];	
						bCreatedExtra = true;
					}
					
					if( bCreatedExtra )
						str_next_tokens[nCurrNextToken++] = str_tokens[nCurrToken];		
				}
				else {
					
					strCurrValueTokens += " " + strToken;				
				}	
				
				nCurrToken++;
			}
			
			if( str_next_tokens != null && str_next_tokens.Length > 0 )
				return str_next_tokens;
			
			throw new CmdParseException( "GetNextCommandTokens() - Invalid parameter Exception Throw!!!" );
			
			//return null;
		}
		else {
			
			throw new CmdParseException( "GetNextCommandTokens() - Invalid parameter Exception Throw!!!" );			
		}
		
		//return null;
	}	
	
	string GetTokensToString( string [] str_tokens ) {
		
		if( str_tokens != null && str_tokens.Length > 0 ) {
			
			string strRet = "";
			foreach( string strToken in str_tokens ) {
				strRet += " " + strToken;				
			}
			
			return strRet;
		}
		
		throw new CmdParseException( "GetTokensToString() - No Ecist Token Exception Throw!!!" );		
	}
	
	
	// parse
	// parse sub command
	void ParseIdCommand( CommandData cmdData, string [] str_tokens ) {	
		
		// sub command 1, name <x>, 2, author <x>
		string [] str_next_tokens = GetNextTokens( str_tokens );	
		//str_next_tokens = GetNextTokens( str_next_tokens );
		string strSubCmd = str_next_tokens[0];
		
		// should have sub command!!!
		if( strSubCmd == "name" )
		{
			// sub command
			CommandData subCmdData = new CommandData();
			subCmdData.StrCmd = strSubCmd;			
			string strValue = GetTokensToString( str_next_tokens );
			subCmdData.QueueStrValue.Enqueue( strValue );
			subCmdData.InvalidCmd = false;			
			
			cmdData.QueueSubCmdData.Enqueue(subCmdData);
						
			cmdData.InvalidCmd = false;
		}		
		else if( strSubCmd == "author" )
		{
			// sub command
			CommandData subCmdData = new CommandData();
			subCmdData.StrCmd = strSubCmd;
			string strValue = GetTokensToString( str_next_tokens );
			subCmdData.QueueStrValue.Enqueue( strValue );
			subCmdData.InvalidCmd = false;			
			
			cmdData.QueueSubCmdData.Enqueue(subCmdData);
						
			cmdData.InvalidCmd = false;
		}
		else {
			
			throw new CmdParseException( "ParseIdCommand() - Invalid Sub Command Exception Throw!!!" );
		}
	}	
	
	void ParseUciOkCommand( CommandData cmdData, string [] str_tokens ) {		
		
		cmdData.InvalidCmd = false;
	}
	
	void ParseReadyOkCommand( CommandData cmdData, string [] str_tokens ) {		
		
		cmdData.InvalidCmd = false;
	}
	
	void ParseCopyProtectionCommand( CommandData cmdData, string [] str_tokens ) {		
		
		if( str_tokens != null && str_tokens.Length > 1 ) {
			
			string [] str_next_tokens = GetNextTokens( str_tokens );
			
			// value 1, checking, 2, ok, 3, error
			cmdData.QueueStrValue.Clear();				
			cmdData.QueueStrValue.Enqueue( str_next_tokens[0] );
			cmdData.QueueSubCmdData.Clear();
			
			cmdData.InvalidCmd = false;
		}
		else {
			
			throw new CmdParseException( "ParseCopyProtectionCommand() - Invalid Command Value Exception Throw!!!" );			
		}
	}
	
	void ParseRegistrationCommand( CommandData cmdData, string [] str_tokens ) {
		
		if( str_tokens != null && str_tokens.Length > 1 ) {
			
			string [] str_next_tokens = GetNextTokens( str_tokens );
			
			// value 1, checking, 2, ok, 3, error
			cmdData.QueueStrValue.Clear();				
			cmdData.QueueStrValue.Enqueue( str_next_tokens[0] );
			cmdData.QueueSubCmdData.Clear();
			
			cmdData.InvalidCmd = false;
		}
		else {
			
			throw new CmdParseException( "ParseRegistrationCommand() - Invalid Command Value Exception Throw!!!" );			
		}
	}
	
	void ParseOptionCommand( CommandData cmdData, string [] str_tokens ) {					
		
		if( str_tokens != null && str_tokens.Length > 1 ) {
		
			string [] str_next_tokens = GetNextTokens( str_tokens );			
			//str_next_tokens = GetNextTokens( str_next_tokens );
			string strSubCmd = str_next_tokens[0];		
			
			// should have sub command!!!		
			if( strSubCmd == "name" ) {	
				
				CommandData subCmdData = new CommandData();			
				subCmdData.StrCmd = strSubCmd;			
				string strCmdValue;
				str_next_tokens = GetNextCommandTokens( str_next_tokens, "type", out strCmdValue );
				subCmdData.QueueStrValue.Enqueue( strCmdValue );
				subCmdData.InvalidCmd = false;			
				
				cmdData.QueueSubCmdData.Enqueue( subCmdData );
				
				strSubCmd = str_next_tokens[0];
				if( strSubCmd == "type" ) {
					
					subCmdData = new CommandData();	
					subCmdData.StrCmd = strSubCmd;				
					str_next_tokens = GetNextTokens( str_next_tokens );				
					strCmdValue = str_next_tokens[0];				
					subCmdData.QueueStrValue.Enqueue( strCmdValue );
					subCmdData.InvalidCmd = false;	
					
					cmdData.QueueSubCmdData.Enqueue( subCmdData );					
				}			
				
				if( str_next_tokens != null && str_next_tokens.Length > 1 ) {
					
					str_next_tokens = GetNextTokens( str_next_tokens );
					strSubCmd = str_next_tokens[0];
					if( strSubCmd == "default" ) {
						
						subCmdData = new CommandData();
						subCmdData.StrCmd = strSubCmd;				
						str_next_tokens = GetNextTokens( str_next_tokens );
						strCmdValue = str_next_tokens[0];
						subCmdData.QueueStrValue.Enqueue( strCmdValue );
						subCmdData.InvalidCmd = false;	
						
						cmdData.QueueSubCmdData.Enqueue( subCmdData );							
					}			
					
					if( str_next_tokens != null && str_next_tokens.Length > 1 ) {
						str_next_tokens = GetNextTokens( str_next_tokens );	
						strSubCmd = str_next_tokens[0];
						
						if( strSubCmd == "var" ) {
							
							subCmdData = new CommandData();
							subCmdData.StrCmd = strSubCmd;				
							str_next_tokens = GetNextTokens( str_next_tokens );
							strCmdValue = str_next_tokens[0];
							subCmdData.QueueStrValue.Enqueue( strCmdValue );				
							subCmdData.InvalidCmd = false;	
							
							while( str_next_tokens.Length > 1 ) {
								
								str_next_tokens = GetNextTokens( str_next_tokens );
								str_next_tokens = GetNextTokens( str_next_tokens );	
								strCmdValue = str_next_tokens[0];
								subCmdData.QueueStrValue.Enqueue( strCmdValue );						
							}
							
							cmdData.QueueSubCmdData.Enqueue( subCmdData );						
						}
						else {
							
							if( strSubCmd == "min" ) {
								
								subCmdData = new CommandData();
								subCmdData.StrCmd = strSubCmd;				
								str_next_tokens = GetNextTokens( str_next_tokens );
								strCmdValue = str_next_tokens[0];
								subCmdData.QueueStrValue.Enqueue( strCmdValue );				
								subCmdData.InvalidCmd = false;	
								
								cmdData.QueueSubCmdData.Enqueue( subCmdData );
								
								str_next_tokens = GetNextTokens( str_next_tokens );				
							}
										
							strSubCmd = str_next_tokens[0];
							if( strSubCmd == "max" ) {
								
								subCmdData = new CommandData();
								subCmdData.StrCmd = strSubCmd;
								str_next_tokens = GetNextTokens( str_next_tokens );
								strCmdValue = str_next_tokens[0];				
								subCmdData.QueueStrValue.Enqueue( strCmdValue );			
								subCmdData.InvalidCmd = false;	
								
								cmdData.QueueSubCmdData.Enqueue( subCmdData );									
							}
						}
					}
				}
				
				cmdData.InvalidCmd = false;
			}
			else {
				
				throw new CmdParseException( "ParseOptionCommand() - Invalid Command Value Exception Throw!!!" );			
			}
		}
		else {
			
			throw new CmdParseException( "ParseOptionCommand() - Invalid Parameter Exception Throw!!!" );			
		}
	}
		
		
	void ParseInfoCommand( CommandData cmdData, string [] str_tokens ) {																	
		
		if( str_tokens != null && str_tokens.Length > 1 ) {
			
			string [] str_next_tokens = GetNextTokens( str_tokens );
			
			string strSubCmd = str_next_tokens[0];
			str_next_tokens = GetNextTokens( str_next_tokens );			
			
			CommandData subCmdData = null;
			string strValue = null;
			
			while( strSubCmd != null ) {
				
				// should have sub command!!!		
				switch( strSubCmd ) {
					
					case "depth":
					{
						subCmdData = new CommandData();
						subCmdData.StrCmd = strSubCmd;
						strValue = str_next_tokens[0];
						subCmdData.QueueStrValue.Enqueue( strValue );																
						subCmdData.InvalidCmd = false;				
						
						cmdData.QueueSubCmdData.Enqueue( subCmdData );						
					}
					break;
					
					case "seldepth":
					{
						subCmdData = new CommandData();
						subCmdData.StrCmd = strSubCmd;
						strValue = str_next_tokens[0];
						subCmdData.QueueStrValue.Enqueue( strValue );																
						subCmdData.InvalidCmd = false;				
						
						cmdData.QueueSubCmdData.Enqueue( subCmdData );								
					}
					break;
					
					case "time":
					{
						subCmdData = new CommandData();
						subCmdData.StrCmd = strSubCmd;
						strValue = str_next_tokens[0];
						subCmdData.QueueStrValue.Enqueue( strValue );																
						subCmdData.InvalidCmd = false;				
						
						cmdData.QueueSubCmdData.Enqueue( subCmdData );
					}
					break;
					
					case "nodes":
					{
						subCmdData = new CommandData();
						subCmdData.StrCmd = strSubCmd;
						strValue = str_next_tokens[0];
						subCmdData.QueueStrValue.Enqueue( strValue );																
						subCmdData.InvalidCmd = false;				
						
						cmdData.QueueSubCmdData.Enqueue( subCmdData );								
					}
					break;
					
					case "pv":
					{
						
					}
					break;
					
					case "multipv":
					{
						subCmdData = new CommandData();
						subCmdData.StrCmd = strSubCmd;
						strValue = str_next_tokens[0];
						subCmdData.QueueStrValue.Enqueue( strValue );																
						subCmdData.InvalidCmd = false;				
						
						cmdData.QueueSubCmdData.Enqueue( subCmdData );					
					}
					break;
					
					case "score":
					{
						
					}
					break;
					
					case "currmove":
					{
						subCmdData = new CommandData();
						subCmdData.StrCmd = strSubCmd;
						strValue = str_next_tokens[0];
						subCmdData.QueueStrValue.Enqueue( strValue );																
						subCmdData.InvalidCmd = false;				
						
						cmdData.QueueSubCmdData.Enqueue( subCmdData );			
					}
					break;
					
					case "currmovenumber":
					{
						subCmdData = new CommandData();
						subCmdData.StrCmd = strSubCmd;
						strValue = str_next_tokens[0];
						subCmdData.QueueStrValue.Enqueue( strValue );																
						subCmdData.InvalidCmd = false;				
						
						cmdData.QueueSubCmdData.Enqueue( subCmdData );					
					}
					break;
					
					case "hashfull":
					{
						subCmdData = new CommandData();
						subCmdData.StrCmd = strSubCmd;
						strValue = str_next_tokens[0];
						subCmdData.QueueStrValue.Enqueue( strValue );																
						subCmdData.InvalidCmd = false;				
						
						cmdData.QueueSubCmdData.Enqueue( subCmdData );			
					}
					break;
					
					case "nps":
					{
						subCmdData = new CommandData();
						subCmdData.StrCmd = strSubCmd;
						strValue = str_next_tokens[0];
						subCmdData.QueueStrValue.Enqueue( strValue );																
						subCmdData.InvalidCmd = false;				
						
						cmdData.QueueSubCmdData.Enqueue( subCmdData );					
					}
					break;
					
					case "tbhits":
					{
						subCmdData = new CommandData();
						subCmdData.StrCmd = strSubCmd;
						strValue = str_next_tokens[0];
						subCmdData.QueueStrValue.Enqueue( strValue );																
						subCmdData.InvalidCmd = false;				
						
						cmdData.QueueSubCmdData.Enqueue( subCmdData );					
					}
					break;
					
					case "sbhits":
					{
						subCmdData = new CommandData();
						subCmdData.StrCmd = strSubCmd;
						strValue = str_next_tokens[0];
						subCmdData.QueueStrValue.Enqueue( strValue );																
						subCmdData.InvalidCmd = false;				
						
						cmdData.QueueSubCmdData.Enqueue( subCmdData );				
					}
					break;
					
					case "cpuload":
					{
						subCmdData = new CommandData();
						subCmdData.StrCmd = strSubCmd;
						strValue = str_next_tokens[0];
						subCmdData.QueueStrValue.Enqueue( strValue );																
						subCmdData.InvalidCmd = false;				
						
						cmdData.QueueSubCmdData.Enqueue( subCmdData );				
					}
					break;
					
					case "string":
					{
						subCmdData = new CommandData();
						subCmdData.StrCmd = strSubCmd;
						strValue = str_next_tokens[0];
						subCmdData.QueueStrValue.Enqueue( strValue );																
						subCmdData.InvalidCmd = false;				
						
						cmdData.QueueSubCmdData.Enqueue( subCmdData );				
					}
					break;
					
					case "refutation":
					{
						
					}
					break;
					
					case "currline":
					{
						
					}
					break;	
				}	
				
				if( str_next_tokens != null && str_next_tokens.Length > 1 ) {
					
					str_next_tokens = GetNextTokens( str_next_tokens );
					strSubCmd = str_next_tokens[0];
				}
				else {
					
					strSubCmd = null;				
				}
			}		
			
			cmdData.InvalidCmd = false;
		}
		else {
			
			throw new CmdParseException( "ParseInfoCommand() - Invalid Parameter Exception Throw!!!" );			
		}
	}	

	void ParseBestMoveCommand( CommandData cmdData, string [] str_tokens ) {
		
		if( str_tokens != null && str_tokens.Length > 1 ) {
			
			string [] str_next_tokens = GetNextTokens( str_tokens );
			
			// bestmove <move1> [ ponder <move2> ]
			string strValue = str_next_tokens[0];
			cmdData.QueueStrValue.Enqueue( strValue );														
			
			if( str_next_tokens.Length > 1 ) {
				// sub command - ponder <move2>		
				str_next_tokens = GetNextTokens( str_next_tokens );
				string strSubCmd = str_next_tokens[0];
				
				if( strSubCmd == "ponder" )
				{
					// subsub command
					CommandData subCmdData = new CommandData();			
					subCmdData.StrCmd = strSubCmd;			
					strValue = str_next_tokens[0];
					subCmdData.QueueStrValue.Enqueue( strValue );	
					subCmdData.InvalidCmd = false;
					
					cmdData.QueueSubCmdData.Enqueue( subCmdData );
				}
			}
			
			cmdData.InvalidCmd = false;			
		}
		else {
			
			throw new CmdParseException( "ParseBestMoveCommand() - Invalid Parameter Exception Throw!!!" );						
		}
	}
	
	void ParseInitStockfishCommand( CommandData cmdData, string [] str_tokens ) {
		
		if( str_tokens != null && str_tokens.Length > 1 ) {
			
			string [] str_next_tokens = GetNextTokens( str_tokens );
			
			
			string strValue = GetTokensToString( str_next_tokens );
			cmdData.QueueStrValue.Enqueue( strValue );																	
			cmdData.InvalidCmd = false;			
		}
		else {
			
			throw new CmdParseException( "ParseBestMoveCommand() - Invalid Parameter Exception Throw!!!" );						
		}		
	}
}








public class EngineToGuiCommand : CommandBase {	
}







// engine to gui command
public class ChessEngineCmdParser {
	
	public EngineToGuiCommand Cmd { get; set; }	
	
	public bool Parse( string strCommandLine ) {		
		
		Cmd = new EngineToGuiCommand();
		Cmd.Init( strCommandLine );
		
		return Cmd.ParseCommand();		
	}
}

//}

