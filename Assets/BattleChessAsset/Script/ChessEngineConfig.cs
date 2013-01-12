using UnityEngine;
using System.Collections;

using System.Collections.Generic;


//namespace BattleChess {	
	
	public class ChessEngineConfig {	
		
		public class Option {		
			
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
			
			public Queue<string> queueVar;
			
			public Option() {
				
				queueVar = new Queue<string>();
			}
			
			public void AddVar( string strVar ) {
				
				queueVar.Enqueue( strVar );
			}
			
			public void ClearAllVar() {
				
				queueVar.Clear();
			}
		}
		
		public string Name {
			get; set;
		}
		
		public string Authur {
			get; set;
		}
		
		Dictionary<string, Option> mapOption;
		
		
		
		public ChessEngineConfig() {
			
			mapOption = new Dictionary<string, Option>();		
		}
		
		public void AddOption( Option option ) {
			
			mapOption[option.Name] = option;		
		}
		
		public void ClearAllOption( Option option ) {
			
			mapOption.Clear();
		}
		
		public bool SetConfigCommand( CommandBase.CommandData commandData ) {
			
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
				
				Option option = new Option();
				
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
