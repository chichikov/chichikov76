using UnityEngine;
using System.Collections;


public class Modal2D : MonoBehaviour {
	
	
	
	UIModal currentModal = null;
	public UIModal CurrentModal {
		
		get { return currentModal; }
		set { 
			
			if( currentModal != null )
				Object.Destroy( currentModal.gameObject );
				
			currentModal = value; 
		}
	}
	
	
	
	
	
	
	
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
	
	
	
	
	// modal dialog management
	public void CommonBtnDelegate( GameObject go ) {
		
		if( currentModal != null ) {
			
			UnityEngine.Object.Destroy( currentModal.gameObject );
			currentModal = null;
		}
	}	
	
	public void ShowModal( GUIManager.ModalType modalType, string strMessage, UIEventListener.VoidDelegate[] btnDelegates ) {
		
		if( modalType < GUIManager.ModalType.OneButtonModal || modalType > GUIManager.ModalType.ThreeButtonModal )
			return;
		
		string strModalPrefab = modalType.ToString();
		GameObject modal = GUIManager.Instance.InstantiateModalToParent( gameObject, strModalPrefab );	
		if( modal == null ) {
			
			Debug.Log( "Modal2D.ShowModal() - couldn't instantiate modal prefab" );
			return;
		}
		
		// set current modal			
		NGUITools.SetActive( modal, true ); 
		CurrentModal = modal.GetComponent<UIModal>();		
		
		CurrentModal.InitModal( modalType, strMessage, btnDelegates, CommonBtnDelegate );		
	}	
	
	public void ShowWaitModal( GUIManager.ModalType modalType, string strMessage, 
								float fDuration = 0.0f, System.Action timeOutCallBack = null ) {			
		
		if( modalType != GUIManager.ModalType.WaitModal )
			return;
		
		string strModalPrefab = modalType.ToString();
		GameObject modal = GUIManager.Instance.InstantiateModalToParent( gameObject, strModalPrefab );			
		if( modal == null ) {
			
			Debug.Log( "Modal2D.ShowWaitModal() - couldn't instantiate modal prefab" );
			return;
		}							
		
		// set current modal			
		NGUITools.SetActive( modal, true ); 
		CurrentModal = modal.GetComponent<UIModal>();			
		
		StartCoroutine( CurrentModal.InitWaitModal( modalType, strMessage, fDuration, timeOutCallBack ) );
	}
	
	public void ShowWaitCoroutineModal( GUIManager.ModalType modalType, string strMessage, 
										YieldInstruction instruction, float fDuration = 0.0f, 
										System.Action waitCoroutineCallBack = null, System.Action waitCallBack = null ) {
		
		if( modalType != GUIManager.ModalType.WaitCoroutineModal )
			return;
		
		string strModalPrefab = modalType.ToString();
		GameObject modal = GUIManager.Instance.InstantiateModalToParent( gameObject, strModalPrefab );			
		if( modal == null ) {
			
			Debug.Log( "Modal2D.ShowWaitCoroutineModal() - couldn't instantiate modal prefab" );
			return;
		}							
		
		// set current modal			
		NGUITools.SetActive( modal, true ); 
		CurrentModal = modal.GetComponent<UIModal>();		
		
		StartCoroutine( CurrentModal.InitWaitCoroutineModal( modalType, strMessage, instruction, fDuration, waitCoroutineCallBack, waitCallBack ) );
	}
	
	
	
	
}
