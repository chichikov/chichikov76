using UnityEngine;
using System.Collections;

public class OptionScrollPanel : MonoBehaviour {
	
	// ui script component	
	public UISlider aggressiveSlider;
	public UISlider cowardiceSlider;
	public UISlider EBTSlider;
	public UISlider EMHSlider;
	public UISlider EMTSlider;
	public UISlider hashSizeSlider;
	public UISlider MTSPSlider;
	public UISlider minSplitDepthSlider;
	public UISlider mobilityEndSlider;
	public UISlider mobilityMiddleSlider;
	public UISlider MTTSlider;
	public UISlider multiPVSlider;
	public UISlider passedPawnMiddleSlider;
	public UISlider passedPawnEndSlider;
	public UISlider skillLevelSlider;
	public UISlider slowMoveSlider;
	public UISlider spaceSlider;
	public UISlider threadSlider;	
	
	public UICheckbox analyseModeCheckbox;
	public UICheckbox ownBookCheckbox;
	public UICheckbox debugLogCheckbox;
	public UICheckbox searchLogCheckbox;
	public UICheckbox ponderCheckbox;
	public UICheckbox bestBookMoveCheckbox;
	public UICheckbox chess960Checkbox;
	public UICheckbox clearHashCheckbox;
	public UICheckbox useSleepingThreadCheckbox;
	
	public UIInput bookFileInput;
	public UIInput searchLogFileInput;
	
	
	// Use this for first initialization	
	void Awake() {	
		
		// Add to GUIManager
		GUIManager.Instance.AddGUI( this.gameObject.name, this.gameObject );		
	}
	
	// Use this for initialization
	void Start() {	
		
	}
	
	// Update is called once per frame
	void Update() {
	
	}
	
	void OnDestroy() {
		
		GUIManager.Instance.RemoveGUI( this.gameObject.name );
	}
	
	
	
	
	public void SetOption( ChessEngineOption option ) {						
		
		// for stockfish chess engine
		switch( option.Name ) {	   
			
		case "Use Debug Log":								
				
			debugLogCheckbox.isChecked = option.GetBoolDefault();
			
			break;
			
		case "Use Search Log":			
				
			searchLogCheckbox.isChecked = option.GetBoolDefault();	
			
			break;			
			
		case "Search Log Filename":
			
			searchLogFileInput.text = option.GetStringDefault();
			
			break;
		
		case "Book File":
			
			bookFileInput.text = option.GetStringDefault();
			
			break;
			
		case "Best Book Move":			
				
			bestBookMoveCheckbox.isChecked = option.GetBoolDefault();
			
			break;
			
		case "Mobility (Middle Game)":			
			
			mobilityMiddleSlider.sliderValue = option.GetRangeFloatDefault();
				
			break;
			
		case "Mobility (Endgame)":			
			
			mobilityEndSlider.sliderValue = option.GetRangeFloatDefault();	
			
			break;
			
		case "Passed Pawns (Middle Game)":			
			
			passedPawnMiddleSlider.sliderValue = option.GetRangeFloatDefault();				
			
			break;
		
		case "Passed Pawns (Endgame)":
			
			passedPawnEndSlider.sliderValue = option.GetRangeFloatDefault();
			
			break;
			
		case "Space":
			
			spaceSlider.sliderValue = option.GetRangeFloatDefault();
			
			break;
			
		case "Aggressiveness":
			
			aggressiveSlider.sliderValue = option.GetRangeFloatDefault();
			
			break;
			
		case "Cowardice":
			
			cowardiceSlider.sliderValue = option.GetRangeFloatDefault();
				
			break;
			
		case "Min Split Depth":
			
			minSplitDepthSlider.sliderValue = option.GetRangeFloatDefault();
			
			break;
		
		case "Max Threads per Split Point":
			
			MTSPSlider.sliderValue = option.GetRangeFloatDefault();
			
			break;
			
		case "Threads":
			
			threadSlider.sliderValue = option.GetRangeFloatDefault();
			
			break;
			
		case "Use Sleeping Threads":
			
			useSleepingThreadCheckbox.isChecked = option.GetBoolDefault();
			
			break;
			
		case "Hash":
			
			hashSizeSlider.sliderValue = option.GetRangeFloatDefault();
			
			break;
		
		case "Ponder":
			
			ponderCheckbox.isChecked = option.GetBoolDefault();
			
			break;
			
		case "OwnBook":
			
			ownBookCheckbox.isChecked = option.GetBoolDefault();
			
			break;
			
		case "MultiPV":
			
			multiPVSlider.sliderValue = option.GetRangeFloatDefault();
			
			break;
			
		case "Skill Level":
			
			skillLevelSlider.sliderValue = option.GetRangeFloatDefault();
			
			break;
		
		case "Emergency Move Horizon":
			
			EMHSlider.sliderValue = option.GetRangeFloatDefault();
			
			break;
			
		case "Emergency Base Time":
			
			EBTSlider.sliderValue = option.GetRangeFloatDefault();
			
			break;
			
		case "Emergency Move Time":
			
			EMTSlider.sliderValue = option.GetRangeFloatDefault();
			
			break;
			
		case "Minimum Thinking Time":
			
			MTTSlider.sliderValue = option.GetRangeFloatDefault();
			
			break;
			
		case "Slow Mover":
			
			slowMoveSlider.sliderValue = option.GetRangeFloatDefault();
			
			break;
			
		case "UCI_Chess960":
			
			chess960Checkbox.isChecked = option.GetBoolDefault();
			
			break;
			
		case "UCI_AnalyseMode":
			
			analyseModeCheckbox.isChecked = option.GetBoolDefault();
			
			break;
			
		case "Clear Hash":
			
			clearHashCheckbox.isChecked = option.GetBoolDefault();
			
			break;
							
		default:
			break;			
		}			
	}
	
	
	
	
	
	// ui event handler
	
	// slider event
	public void OnAggressiveSliderChange( float fVal ) {	
		
		ChessEngineManager.Instance.SetCurrentRangeFloatOption( "Aggressiveness", fVal );
	}
	
	public void OnCowardiceSliderChange( float fVal ) {
		
		ChessEngineManager.Instance.SetCurrentRangeFloatOption( "Cowardice", fVal );		
	}
	
	public void OnEBTSliderChange( float fVal ) {
		
		ChessEngineManager.Instance.SetCurrentRangeFloatOption( "Emergency Base Time", fVal );
	}
	
	public void OnEMHSliderChange( float fVal ) {
		
		ChessEngineManager.Instance.SetCurrentRangeFloatOption( "Emergency Move Horizon", fVal );		
	}
	
	public void OnEMTSliderChange( float fVal ) {
		
		ChessEngineManager.Instance.SetCurrentRangeFloatOption( "Emergency Move Time", fVal );
	}
	
	public void OnHashSizeSliderChange( float fVal ) {
		
		ChessEngineManager.Instance.SetCurrentRangeFloatOption( "Hash", fVal );
	}
	
	public void OnMTSPSliderChange( float fVal ) {
		
		ChessEngineManager.Instance.SetCurrentRangeFloatOption( "Max Threads per Split Point", fVal );
	}
	
	public void OnMinSplitDepthSliderChange( float fVal ) {
		
		ChessEngineManager.Instance.SetCurrentRangeFloatOption( "Min Split Depth", fVal );
	}
	
	public void OnMobilityEndSliderChange( float fVal ) {
		
		ChessEngineManager.Instance.SetCurrentRangeFloatOption( "Mobility (Endgame)", fVal );
	}
	
	public void OnMobilityMiddleSliderChange( float fVal ) {
		
		ChessEngineManager.Instance.SetCurrentRangeFloatOption( "Mobility (Middle Game)", fVal );
	}
	
	public void OnMTTSliderChange( float fVal ) {
		
		ChessEngineManager.Instance.SetCurrentRangeFloatOption( "Minimum Thinking Time", fVal );
	}
	
	public void OnMultiPVSliderChange( float fVal ) {
		
		ChessEngineManager.Instance.SetCurrentRangeFloatOption( "MultiPV", fVal );
	}
	
	public void OnPassedPawnEndSliderChange( float fVal ) {
		
		ChessEngineManager.Instance.SetCurrentRangeFloatOption( "Passed Pawns (Endgame)", fVal );
	}
	
	public void OnPassedPawnMiddleSliderChange( float fVal ) {
		
		ChessEngineManager.Instance.SetCurrentRangeFloatOption( "Passed Pawns (Middle Game)", fVal );
	}
	
	public void OnSkillLevelSliderChange( float fVal ) {
		
		ChessEngineManager.Instance.SetCurrentRangeFloatOption( "Skill Level", fVal );
	}
	
	public void OnSlowMoveSliderChange( float fVal ) {
		
		ChessEngineManager.Instance.SetCurrentRangeFloatOption( "Slow Mover", fVal );
	}
	
	public void OnSpaceSliderChange( float fVal ) {
		
		ChessEngineManager.Instance.SetCurrentRangeFloatOption( "Space", fVal );
	}
	
	public void OnThreadSliderChange( float fVal ) {
		
		ChessEngineManager.Instance.SetCurrentRangeFloatOption( "Threads", fVal );
	}
	
	
	
	// check box event
	public void OnAnalyseModeActivate( bool bState ) {
		
		ChessEngineManager.Instance.SetCurrentBoolOption( "UCI_AnalyseMode", bState );
	}
	
	public void OnOwnBookActivate( bool bState ) {
	
		ChessEngineManager.Instance.SetCurrentBoolOption( "OwnBook", bState );
	}
	
	public void OnClearHashActivate( bool bState ) {
		
		ChessEngineManager.Instance.SetCurrentBoolOption( "Clear Hash", bState );
	}
	
	public void OnUseSleepingThreadActivate( bool bState ) {
		
		ChessEngineManager.Instance.SetCurrentBoolOption( "Use Sleeping Threads", bState );
	}
	
	public void OnDebugLogActivate( bool bState ) {
		
		ChessEngineManager.Instance.SetCurrentBoolOption( "Use Debug Log", bState );
	}
	
	public void OnSearchLogActivate( bool bState ) {
		
		ChessEngineManager.Instance.SetCurrentBoolOption( "Use Search Log", bState );
	}
	
	public void OnPonderActivate( bool bState ) {
		
		ChessEngineManager.Instance.SetCurrentBoolOption( "Ponder", bState );
	}
	
	public void OnBestBookMoveActivate( bool bState ) {
		
		ChessEngineManager.Instance.SetCurrentBoolOption( "Best Book Move", bState );
	}
	
	public void OnChess960Activate( bool bState ) {
		
		ChessEngineManager.Instance.SetCurrentBoolOption( "UCI_Chess960", bState );
	}

	
	
	// input box event
	public void OnBookFileSubmit( string strInput ) {
		
		ChessEngineManager.Instance.SetCurrentStringOption( "Book File", strInput );
	}
	
	public void OnSearchLogSubmit( string strInput ) {
		
		ChessEngineManager.Instance.SetCurrentStringOption( "Search Log Filename", strInput );
	}
}
