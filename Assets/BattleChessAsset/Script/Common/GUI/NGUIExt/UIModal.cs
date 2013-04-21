using UnityEngine;
using System.Collections;



[ExecuteInEditMode]
[AddComponentMenu("NGUI/UI/Modal")]
public class UIModal : MonoBehaviour {		
	
	GUIManager.ModalType modalType;		
	UILabel messageLabel;	
	UIEventListener [] buttonListeners;
	System.Action waitCallBack = null;
	System.Action waitCoroutineCallBack = null;
	YieldInstruction instruction;
	
	
	float fDuration = 0;
	float fStartTime = 0;	
	
	
	
	// MonoBehaviour method
	// Use this for frist initialization
	void Awake() {	
	}
	
	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {			
	}	
	
	
	public void InitModal( GUIManager.ModalType modalType, string strMessage,
							UIEventListener.VoidDelegate[] btnDelegates, 
							UIEventListener.VoidDelegate CommonBtnDelegate ) {
		
		this.modalType = modalType;
		
		// set label		
		if( messageLabel != null )
			messageLabel.text = strMessage;
		
		// set event handler		
		if( buttonListeners.Length >= 1 ) {			
			buttonListeners[0].onClick = btnDelegates[0]; 
			buttonListeners[0].onClick += CommonBtnDelegate;	
		}
		
		if( buttonListeners.Length >= 2 ) {		
			buttonListeners[1].onClick = btnDelegates[1];
			buttonListeners[1].onClick += CommonBtnDelegate;	
		}
		
		if( buttonListeners.Length >= 3 ) {			
			buttonListeners[2].onClick = btnDelegates[2];
			buttonListeners[2].onClick += CommonBtnDelegate;
		}
	}
	
	public IEnumerator InitWaitModal( GUIManager.ModalType modalType, string strMessage, 
								float fDuration = 0.0f, System.Action waitCallBack = null ) {
		
		this.modalType = modalType;
		
		this.fDuration = fDuration;
		this.fStartTime = Time.time;
		
		this.waitCallBack = waitCallBack;
		
		// set label		
		if( messageLabel != null )
			messageLabel.text = strMessage;		
		
		if( Time.time < fStartTime + fDuration ) {
			
			yield return null;
		}		
		else {			
			
			if( waitCallBack != null )
				waitCallBack();							
		}	
		
		GUIManager.Instance.modalDialogs.CurrentModal = null;
	}
	
	public IEnumerator InitWaitCoroutineModal( GUIManager.ModalType modalType, string strMessage, 
										YieldInstruction instruction, float fDuration = 0.0f, 
										System.Action waitCoroutineCallBack = null, System.Action waitCallBack = null ) {
		
		this.modalType = modalType;
		
		instruction = instruction;
		
		this.fDuration = fDuration;
		this.fStartTime = Time.time;
		
		this.waitCallBack = waitCallBack;
		this.waitCoroutineCallBack = waitCoroutineCallBack;		
		
		// set label		
		if( messageLabel != null )
			messageLabel.text = strMessage;
		
		yield return StartCoroutine( WaitForCoroutine() );
		
		GUIManager.Instance.modalDialogs.CurrentModal = null;		
	}
	
	IEnumerator WaitForCoroutine() {
		
		if( Time.time < fStartTime + fDuration ) {
			
			yield return instruction;
			
			if( waitCoroutineCallBack != null )
				waitCoroutineCallBack();	
		}		
		else {			
			
			if( waitCallBack != null )
				waitCallBack();							
		}				
	}
}
