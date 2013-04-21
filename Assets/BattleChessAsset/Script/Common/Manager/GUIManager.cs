using UnityEngine;
using System.Collections;

using System.Collections.Generic;

// for extention method
using UnityExtention;


// gui exception
public class GUIManagerException : System.Exception
{	
	public GUIManagerException()	{
	}	
	
	public GUIManagerException(string message)
	 : base(message) {
	}
		
	public GUIManagerException(System.Exception innerException)
		: base(null, innerException) {
	}	
	
	public GUIManagerException(string message, System.Exception innerException)
	 : base(message, innerException) {
	}
}




public class GUIManager : MonoBehaviour {
	
	
	public enum ModalType {
		
		OneButtonModal,
		TwoButtonModal,
		ThreeButtonModal,
		WaitModal,
		WaitCoroutineModal
	}
	
	
	public struct UIHouseRef {
		
		public bool is3DUI;
		public string name;
		public GameObject uiHousePrefab;		
	}
	
	// field
	public GameObject [] uiHouse2DPrefabs;	
	public GameObject [] uiHouse3DPrefabs;	
	public GameObject [] uiModalPrefabs;	
	
	public UIRoot UIRoot2D;
	public UICamera UICamera2D;
	public UIAnchor UIAnchor2D;
	
	public UIRoot UIRoot3D;
	public UICamera UICamera3D;
	public UIAnchor UIAnchor3D;			
	
	// common 2d panel
	public Common2D commonPanels;
	
	// modal 2d dialog
	public Modal2D modalDialogs;
	
	
	
	Dictionary<string, UIHouseRef> mapUIHouseRef;
	Dictionary<string, UIHouseRef> mapModalRef;
	
	Dictionary<string, UIHouse> mapUIHouse;	
	
	
	
	// current UIHouse 
	UIHouse currUIHouse2D;	
	UIHouse currUIHouse3D;	
	
		
	
	// property
	public Dictionary<string, UIHouse> MapUIHouse { get { return mapUIHouse; } private set {} }	
	
	
	
	
	
	void Awake() {		
		
		// enroll game object singleton
		SingletonGameObject<GUIManager>.AddSingleton( this ); 
		
		// construct GUI House object from arrayGUIPrefab for convenient and optimizing
		
		// 2d gui prefab
		mapUIHouseRef = new Dictionary<string, UIHouseRef>();		
		
		foreach( GameObject curr2DPrefab in uiHouse2DPrefabs ) {
			
			UIHouseRef uiHouse2DRef = new UIHouseRef() { is3DUI = false, name = curr2DPrefab.name, uiHousePrefab = curr2DPrefab };
			mapUIHouseRef.Add( curr2DPrefab.name, uiHouse2DRef );			
		}	
		
		// 3d gui prefab
		foreach( GameObject curr3DPrefab in uiHouse3DPrefabs ) {
			
			UIHouseRef uiHouse3DRef = new UIHouseRef() { is3DUI = true, name = curr3DPrefab.name, uiHousePrefab = curr3DPrefab };
			mapUIHouseRef.Add( curr3DPrefab.name, uiHouse3DRef );			
		}		
		
		// init modal 
		mapModalRef = new Dictionary<string, UIHouseRef>();
		
		foreach( GameObject currModalPrefab in uiModalPrefabs ) {
			
			UIHouseRef uiModalRef = new UIHouseRef() { is3DUI = false, name = currModalPrefab.name, uiHousePrefab = currModalPrefab };
			mapModalRef.Add( currModalPrefab.name, uiModalRef );			
		}
		
		// populate GUI House object
		mapUIHouse = new Dictionary<string, UIHouse>();		
		
		currUIHouse2D = null;		
		currUIHouse3D = null;		
	}
	
	void Start() {				
		
	}	
	
	void Update() {			
		
	}	
	
	
	
	// private method
	
	
	
	
	
	// public method	
	public UIHouse GetUIHouse( string strName, bool bLoad ) {
		
		if( mapUIHouse.ContainsKey(strName) )			
			return mapUIHouse[strName];		
		
		if( bLoad = false )
			return null;
		
		// instantiate and create GUIHouse object and return
		if( mapUIHouseRef.ContainsKey(strName) == false )			
			throw new GUIManagerException( string.Format( "No UIHouseRef object exist - '{0}'", strName) );						
		
		// set parent to GUIManager and Disable unloading		
		UIHouse uiHouse = InstantiateToRoot( mapUIHouseRef[strName] );		
		if( uiHouse == null )			
			throw new GUIManagerException( string.Format( "Cannot Create ui House Object - '{0}'", strName) );				
		
		mapUIHouse.Add( uiHouse.nameID, uiHouse );
		
		return uiHouse;
	}	
	
	public T GetUIHouseScript<T>( string strName ) where T : MonoBehaviour {
		
		UIHouse uiHouse = GetUIHouse(strName, false);	
		if( uiHouse != null )			
			return uiHouse.gameObject.GetComponentInChildren<T>();			
		
		return null;
	}		
	
	public bool AddUIHouse( UIHouse uiHouse ) {
		
		if( mapUIHouse.ContainsKey( uiHouse.nameID ) )			
			return false;
		
		AttachToRoot( uiHouse );
		
		mapUIHouse.Add(uiHouse.nameID, uiHouse);			
		return true;
	}
	
	public bool RemoveUIHouse( UIHouse uiHouse ) {
		
		if( uiHouse != null )			
			RemoveUIHouse( uiHouse.nameID );			
		
		return false;
	}	
	
	public bool RemoveUIHouse( string strName ) {
		
		if( mapUIHouse.ContainsKey(strName) ) {			
			
			Object.Destroy( mapUIHouse[strName].gameObject );
			mapUIHouse.Remove(strName);			
			return true;
		}	
			
		return false;
	}
	
	public void RemoveAllUIHouse() {
		
		foreach( KeyValuePair<string, UIHouse> pair in mapUIHouse )			
			Object.Destroy( pair.Value.gameObject );							
		
		mapUIHouse.Clear();
	}	
	
	
	
	
	public UIPanel GetPanel( string strPanel ) {	
		
		foreach( KeyValuePair<string, UIHouse> pair in MapUIHouse ) {
			
			UIPanel panel = pair.Value.GetPanel( strPanel );
			if( panel != null )			
				return panel;
		}
		
		return null;
	}
	
	public UIPanel GetPanel( string strUIHouse, string strPanel ) {	
		
		UIHouse uiHouse = GetUIHouse( strUIHouse, false );		
		if( uiHouse != null )
			return uiHouse.GetPanel( strPanel );		
		
		return null;
	}
	
	public T GetPanelScript<T>( string strUIHouse, string strPanel ) where T : MonoBehaviour {	
		
		UIPanel panel = GetPanel( strUIHouse, strPanel );		
		if( panel != null )			
			return panel.GetComponentInChildren<T>();
		
		return null;
	}
	
	
	
	
	// UIHouse
	public void ShowPanel( string strPanel, bool bShow, bool b3D = false, bool bAll = false ) {
		
		if( bAll ) {
			foreach( KeyValuePair<string, UIHouse> pair in MapUIHouse ) {
				
				UIPanel panel = pair.Value.GetPanel( strPanel );
				if( panel != null )					
					pair.Value.ShowPanel( strPanel, bShow );			
			}
		}
		else {
			
			if( b3D ) {
				
				if( currUIHouse3D != null )
					currUIHouse3D.ShowPanel( strPanel, bShow );
			}
			else {
				
				if( currUIHouse2D != null )
					currUIHouse2D.ShowPanel( strPanel, bShow );
			}
		}
	}
	
	public void ShowPanel( string strUIHouse, string strPanel, bool bShow ) {		
		
		bool bLoad = bShow ? true : false;
		UIHouse uiHouse = GetUIHouse( strUIHouse, bLoad );		
		if( uiHouse != null )		
			uiHouse.ShowPanel(strPanel, bShow);			
	}
	
	public void ShowMainPanel( bool bShow, bool b3D = false ) {		
		
		if( b3D ) {
				
			if( currUIHouse3D != null )
				currUIHouse3D.ShowMainPanel( bShow );
		}
		else {
			
			if( currUIHouse2D != null )
				currUIHouse2D.ShowMainPanel( bShow );
		}		
	}
	
	public void ShowAllPanel( bool bShow, bool b3D = false ) {
		
		if( b3D ) {
			
			if( currUIHouse3D != null )
				currUIHouse3D.ShowAllPanel( bShow );
		}
		else {
			
			if( currUIHouse2D != null )
				currUIHouse2D.ShowAllPanel( bShow );
		}		
	}
	
	public void ShowAllPanel( string strUIHouse, bool bShow ) {
		
		bool bLoad = bShow ? true : false;
		UIHouse uiHouse = GetUIHouse( strUIHouse, bLoad );		
		if( uiHouse != null )
			uiHouse.ShowAllPanel(bShow);	
	}
	
	public void SetCurrentUIHouse( string strUIHouse, bool bUnloadPrev, bool bShowMainPanel = true ) {
		
		bool bLoad = bShowMainPanel ? true : false;
		UIHouse uiHouse = GetUIHouse( strUIHouse, bLoad );		
		if( uiHouse != null ) {
			
			if( uiHouse.is3DUI ) {
				
				if( bUnloadPrev )					
					RemoveUIHouse( currUIHouse3D );				
			
				currUIHouse3D = uiHouse;						
			}
			else {
				
				if( bUnloadPrev )					
					RemoveUIHouse( currUIHouse2D );	
				
				currUIHouse2D = uiHouse;				
			}
			
			uiHouse.ShowMainPanel( bShowMainPanel );
		}
	}	
	
	
	// common panel
	
	
	// modal/message dialog method
	public void ShowModal( ModalType modalType, string strMessage, params UIEventListener.VoidDelegate[] btnDelegates ) {
		
		modalDialogs.ShowModal( modalType, strMessage, btnDelegates );
	}
	
	
	
	
	
	// utility method
	public void AttachToRoot( UIHouse uiHouse ) {		
		
		if( uiHouse != null )
		{
			GameObject parent = uiHouse.is3DUI ? UIAnchor3D.gameObject : UIAnchor2D.gameObject;
			if( parent != null ) {
			
				Transform t = uiHouse.gameObject.transform;
				t.parent = parent.transform;
				t.localPosition = Vector3.zero;
				t.localRotation = Quaternion.identity;
				t.localScale = Vector3.one;
				uiHouse.gameObject.layer = parent.layer;				
				Object.DontDestroyOnLoad( uiHouse.gameObject );				
			}
		}	
	}
	
	public UIHouse InstantiateToRoot( UIHouseRef uiHouseRef ) {		
		
		if( uiHouseRef.uiHousePrefab != null ) {
			
			GameObject go = Instantiate( uiHouseRef.uiHousePrefab ) as GameObject;	
			if( go != null )
			{	
				UIHouse uiHouse = go.GetComponent<UIHouse>();				
				if( uiHouse ) {
					
					GameObject parent = uiHouse.is3DUI ? UIAnchor3D.gameObject : UIAnchor2D.gameObject;				
					if( parent ) {
						
						Transform t = go.transform;
						t.parent = parent.transform;
						t.localPosition = Vector3.zero;
						t.localRotation = Quaternion.identity;
						t.localScale = Vector3.one;
						go.layer = parent.layer;
						go.name = uiHouse.nameID;
						Object.DontDestroyOnLoad( go );						
						
						return uiHouse;
					}
				}				
			}		
		}
		
		return null;
	}
	
	public GameObject InstantiateModalToParent( GameObject parent, string strPrefab ) {
	
		if( mapModalRef.ContainsKey( strPrefab ) == false ) {
			
			Debug.Log( "GUIManager.InstantiateModalToParent() - Has no Modal prefab reference" );
			return null;
		}
		
		GameObject modal = Instantiate( mapModalRef[strPrefab].uiHousePrefab ) as GameObject;

		if( modal != null && parent != null )
		{
			Transform t = modal.transform;
			t.parent = parent.transform;
			// scale
			t.localScale = Vector3.one;				
			// rotation
			t.rotation = Quaternion.identity;				
			// position and depth
			t.position = new Vector3( t.position.x, t.position.y, CommonZDepth.ModalDialogF );	
			
			modal.layer = parent.layer;						
			
			// add whole screen size box collider
			NGUITools.AddWidgetCollider( modal );				
			modal.GetComponent<BoxCollider>().size = new Vector3( Screen.width, Screen.height, 0 );					
			modal.GetComponent<BoxCollider>().center = new Vector3( 0, 0, CommonZDepth.ModalDialogF + 1 );
			
			Object.DontDestroyOnLoad( modal );	
		}
		
		return modal;
	}
	
	
	
	
	
	
	
	// static
	// attach to parent	
	public static void AttachToParent( GameObject parent, GameObject child ) {	

		if( parent != null )
		{
			Transform t = child.transform;
			t.parent = parent.transform;
			t.localPosition = Vector3.zero;
			t.localRotation = Quaternion.identity;
			t.localScale = Vector3.one;
			child.layer = parent.layer;
			
			Object.DontDestroyOnLoad( child );	
		}	
	}
	
	public static GameObject InstantiateToParent( GameObject parent, GameObject prefab ) {
	
		GameObject go = GameObject.Instantiate(prefab) as GameObject;

		if( go != null && parent != null )
		{
			Transform t = go.transform;
			t.parent = parent.transform;
			t.localPosition = Vector3.zero;
			t.localRotation = Quaternion.identity;
			t.localScale = Vector3.one;
			go.layer = parent.layer;
			
			Object.DontDestroyOnLoad( go );	
		}
		
		return go;
	}
	
	
	
	
	public static GUIManager Instance {
		
		get { 
			return SingletonGameObject<GUIManager>.Instance; 
		} 	
	}	
}
