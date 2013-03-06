using UnityEngine;
using System.Collections;

using System.Collections.Generic;


//namespace BattleChess {	

public class ChessEngineOption {		
		
	public string Name {
		get; set;
	}
	
	public string Type {
		get; set;
	} 
	
	public string Default {
		get; set;
	} 
	
	public string Min {
		get; set;
	} 
	
	public string Max {
		get; set;
	} 
	
	public string Value {
		get; set;
	} 
	
	public Queue<string> queueVar;
	
	public ChessEngineOption() {
		
		queueVar = new Queue<string>();
	}
	
	public void AddVar( string strVar ) {
		
		queueVar.Enqueue( strVar );
	}	
	
	public bool GetBoolDefault() {
		
		if( Default == "true" )
			return true;
		
		return false;
	}	
	
	public void SetBoolValue( bool bValue ) {		
		
		if( bValue )
			Value = "true";
		else
			Value = "false";
	}
	
	public bool GetBoolValue() {		
		
		if( Value == "true" )
			return true;
		
		return false;	
	}
	
	public float GetRangeFloatDefault() {
		
		float fDefault, fMin, fMax;
		fDefault = float.Parse(Default);
		fMin = float.Parse(Min);
		fMax = float.Parse(Max);
		
		return ((fDefault - fMin) / (fMax - fMin));
	}
	
	public void SetRangeFloatValue( float fPropValue ) {		
		
		float fMin, fMax;			
		fMin = float.Parse(Min);
		fMax = float.Parse(Max);
		
		Value = (fPropValue * ( fMax - fMin )).ToString( "0.0000" );		
	}
	
	public float GetRangeFloatValue() {			
			
		return float.Parse( Value );		
	}	
	
	public string GetStringDefault() {			
		
		return Default;
	}
	
	public void SetStringValue( string strValue ) {			
		
		Value = strValue;
	}
	
	public string GetStringValue() {			
		
		return Value;
	}	
	
	public void CopyFrom( ChessEngineOption option ) {
		
		this.Name = option.Name;
		this.Type = option.Type;
		this.Default = option.Default;
		this.Min = option.Min;
		this.Max = option.Max;
		
		foreach( string strValue in option.queueVar ) {
			
			this.queueVar.Enqueue( strValue );				
		}
	}
	
	public string GetSendOptionString() {	
		
		return "setoption name " + Name + " value " + Value;		
	}
}
	









public class ChessEngineConfig {		
	
	
	public string Name {
		get; set;
	}
	
	public string Authur {
		get; set;
	}
	
	Dictionary<string, ChessEngineOption> mapOption;
	
	
	
	public ChessEngineConfig() {
		
		mapOption = new Dictionary<string, ChessEngineOption>();		
	}	
	
	
	
	public Dictionary<string, ChessEngineOption> GetOptionMap() {
		
		return mapOption;			
	}
	
	
	public bool IsEmpty() {
		
		if( mapOption.Count > 0 )
			return false;		
		return true;			
	}
	
	public void AddOption( ChessEngineOption option ) {
		
		mapOption[option.Name] = option;		
	}
	
	public void ClearAllOption( ChessEngineOption option ) {
		
		mapOption.Clear();
	}
	
	public ChessEngineOption GetConfigOption( string strOptionName ) {
		
		if( mapOption.ContainsKey( strOptionName ) ) {
			
			return mapOption[strOptionName];
		}
		
		//UnityEngine.Debug.Log( "ChessEngineOption::GetConfigOption() - " + strOptionName );
		return null;
	}
	
	public bool SetConfigOption( CommandBase.CommandData commandData ) {
		
		bool bRet = false;		
		if( commandData.StrCmd == "id" ) {
			
			CommandBase.CommandData subCmdData = commandData.QueueSubCmdData.Peek();
			if( subCmdData != null && subCmdData.StrCmd == "name" ) {
				Name = subCmdData.QueueStrValue.Peek();				
				bRet = true;
			}
			else if( subCmdData != null && subCmdData.StrCmd == "author" ) {
				Authur = subCmdData.QueueStrValue.Peek();
				bRet = true;
			}					
		}
		else if( commandData.StrCmd == "option" ) {
			
			ChessEngineOption option = new ChessEngineOption();
			
			foreach( CommandBase.CommandData subCmdData in commandData.QueueSubCmdData ) {
			
				if( subCmdData != null ) {					
					
					if( subCmdData.StrCmd == "name" ) {
						
						option.Name = subCmdData.QueueStrValue.Peek();												
					}
					else if( subCmdData.StrCmd == "type" ) {
						
						option.Type = subCmdData.QueueStrValue.Peek();						
					}
					else if( subCmdData.StrCmd == "default" ) {
						
						 option.Default = subCmdData.QueueStrValue.Peek();
					}
					else if( subCmdData.StrCmd == "min" ) {
						
						 option.Min = subCmdData.QueueStrValue.Peek();
					}
					else if( subCmdData.StrCmd == "max" ) {
						
						 option.Max = subCmdData.QueueStrValue.Peek();
					}
					else if( subCmdData.StrCmd == "var" ) {
						
						foreach( string strCurrVar in subCmdData.QueueStrValue )
							option.queueVar.Enqueue( strCurrVar );
					}
				}
			}
			
			AddOption( option );
			bRet = true;
		}
		
		return bRet;
	}
}
//}
