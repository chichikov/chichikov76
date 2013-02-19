using UnityEngine;
using System.Collections;

public interface IProcessChessEngine {	
	
	bool OnIdCommand( CommandBase.CommandData cmdData );			
	bool OnUciOkCommand( CommandBase.CommandData cmdData );			
	bool OnReadyOkCommand( CommandBase.CommandData cmdData );	
	bool OnCopyProtectionCommand( CommandBase.CommandData cmdData );						
	bool OnRegistrationCommand( CommandBase.CommandData cmdData );									
	bool OnOptionCommand( CommandBase.CommandData cmdData );							
	bool OnInfoCommand( CommandBase.CommandData cmdData );												
	bool OnBestMoveCommand( CommandBase.CommandData cmdData );	
		
}
