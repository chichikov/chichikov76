using UnityEngine;
using System.Collections;

using System.Collections.Generic;

// for extention method
using UnityExtention;

// unity game object singleton
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




public class GUIHouse {
	
	// field
	string strName;
	GameObject guiObj;
	bool b3DUI;
	
	Dictionary<string, GameObject> mapPanel;
	
	
	// property
	public string Name { 
		
		get {			
			return strName;
		}
		
		private set {
		}
	}
	
	public GameObject GUIObject { 
		
		get {			
			return guiObj;
		}
		
		private set {
		}
	}
	
	public bool UI3D { 
		
		get {			
			return b3DUI;
		}
		
		private set {
		}
	}
	
	
	// method
	private GUIHouse() {
	}
	
	public GUIHouse( string strName, GameObject guiObj, bool b3DUI ) {
		
		this.strName = strName;
		this.guiObj = guiObj;
		this.b3DUI = b3DUI;
		
		mapPanel = new Dictionary<string, GameObject>();
		
		// populate Gui Panel!!!
		GameObject [] aPanel = this.guiObj.GetChildrenGameObjectWithTag( "Panel" );
		if( aPanel == null ) {
			
			throw new GUIManagerException( string.Format( "No panel exist - '{0}'", this.strName) ); 
		}
		else {
			
			foreach( GameObject currPanel in aPanel ) {
				
				mapPanel.Add( currPanel.name, currPanel );
			}
		}		
	}	
	
	public void DetroySelf() {
		
		Object.Destroy( guiObj );
		guiObj = null;
	}
	
	
	public GameObject GetPanel( string strName ) {
		
		if( mapPanel.ContainsKey(strName) ) {
			
			return mapPanel[strName];
		}
		
		return null;
	}	
	
	
	
	
	public bool AddPanel( string strName, GameObject gameObj ) {
		
		if( mapPanel.ContainsKey(strName) )			
			return false;		
		
		mapPanel.Add(strName, gameObj);			
		return true;
	}
	
	public bool RemovePanel( string strName ) {
		
		if( mapPanel.ContainsKey(strName) ) {					
			
			Object.Destroy( mapPanel[strName] );
			mapPanel.Remove(strName);			
			return true;
		}	
			
		return false;
	}	
	
	
	
	
	
	
	public void ShowPanel( string strPanel, bool bShow ) {		
		
		if( mapPanel.ContainsKey( strPanel ) ) {
			
			 NGUITools.SetActive( mapPanel[strPanel], bShow ); 
		}
	}
	
	public void ShowAllPanel( bool bShow ) {
		
		foreach( KeyValuePair<string, GameObject> pair in mapPanel ) {
			
			 NGUITools.SetActive( pair.Value, bShow ); 
		}
	}	
}






public class GUIManager : MonoBehaviour {
	
	public struct sGUIRef {
		
		public bool b3DUI;
		public string strName;
		public GameObject GUIPrefab;
		
		public sGUIRef( bool b3DUI, string strName, GameObject GUIPrefab )	{
			
			this.b3DUI = b3DUI;
			this.strName = strName;
			this.GUIPrefab = GUIPrefab;
		}
	}
	
	// field
	public GameObject [] arrayGUI2DPrefab;	
	public GameObject [] arrayGUI3DPrefab;	
	
	Dictionary<string, sGUIRef> mapGUIRef;
	Dictionary<string, GUIHouse> mapGUI;
	
	public UIRoot UIRoot2D;
	public UICamera UICamera2D;
	public UIAnchor UIAnchor2D;
	
	public UIRoot UIRoot3D;
	public UICamera UICamera3D;
	public UIAnchor UIAnchor3D;
	
		
	
	// property
		
	
	
	void Awake() {		
		
		// enroll game object singleton
		SingletonGameObject<GUIManager>.AddSingleton( this ); 
		
		// construct GUI House object from arrayGUIPrefab for convenient and optimizing
		mapGUIRef = new Dictionary<string, sGUIRef>();		
		
		foreach( GameObject currPrefab2D in arrayGUI2DPrefab ) {
			
			sGUIRef guiRef2D = new sGUIRef( false, currPrefab2D.name, currPrefab2D );
			mapGUIRef.Add( currPrefab2D.name, guiRef2D );			
		}	
		
		foreach( GameObject currPrefab3D in arrayGUI3DPrefab ) {
			
			sGUIRef guiRef3D = new sGUIRef( true, currPrefab3D.name, currPrefab3D );
			mapGUIRef.Add( currPrefab3D.name, guiRef3D );			
		}
		
		// populate GUI House object
		mapGUI = new Dictionary<string, GUIHouse>();		
	}
	
	void Start() {				
		
	}	
	
	void Update() {			
		
	}	
	
	
	
	// private method
	
	
	
	
	
	// public method
	public GUIHouse GetGUI( string strName ) {
		
		if( mapGUI.ContainsKey(strName) ) {
			
			return mapGUI[strName];
		}
		
		// instantiate and create GUIHouse object and return
		if( mapGUIRef.ContainsKey(strName) == false ) {
			
			throw new GUIManagerException( string.Format( "No GUIRef object exist - '{0}'", strName) );
		}					
		
		// set parent to GUIManager and Disable unloading		
		GameObject guiGameObj = NGUITools.AddChild( mapGUIRef[strName].b3DUI ? UIAnchor3D.gameObject : UIAnchor2D.gameObject,
													mapGUIRef[strName].GUIPrefab );
		
		if( guiGameObj == null ) {			
			
			throw new GUIManagerException( string.Format( "Cannot Create Gui Game Object - '{0}'", strName) );
		}
		
		guiGameObj.name = strName;	
		Object.DontDestroyOnLoad( guiGameObj );					
		
		GUIHouse guiHouse = new GUIHouse( strName, guiGameObj, mapGUIRef[strName].b3DUI );
		
		mapGUI.Add( guiHouse.Name, guiHouse );
		
		return guiHouse;
	}	
	
	public GameObject GetGUIGameObject( string strName ) {
		
		GUIHouse guiHouse = GetGUI(strName);	
		if( guiHouse == null ) {
			
			throw new GUIManagerException( string.Format( "No GUIHouse object exist - '{0}'", strName) );			
		}
		
		return guiHouse.GUIObject;				
	}
	
	public T GetGUIGameObjectScript<T>( string strName ) where T : MonoBehaviour {
		
		GUIHouse guiHouse = GetGUI(strName);	
		if( guiHouse == null ) {
			
			throw new GUIManagerException( string.Format( "No GUIHouse object exist - '{0}'", strName) );				
		}
		
		return guiHouse.GUIObject.GetComponentInChildren<T>();			
	}		
	
	public bool AddGUI( string strName, GUIHouse guiHouse ) {
		
		if( mapGUI.ContainsKey(strName) )			
			return false;
		
		Object.DontDestroyOnLoad( guiHouse.GUIObject );
		guiHouse.GUIObject.transform.parent = guiHouse.UI3D ? UIAnchor3D.transform : UIAnchor2D.transform;
		
		mapGUI.Add(strName, guiHouse);			
		return true;
	}
	
	public bool AddGUI( string strName, GameObject guiGameObj, bool b3DUI ) {
		
		if( mapGUI.ContainsKey(strName) || guiGameObj == null )			
			return false;		
		
		Object.DontDestroyOnLoad( guiGameObj );
		guiGameObj.transform.parent = b3DUI ? UIAnchor3D.transform : UIAnchor2D.transform;
		
		GUIHouse guiHouse = new GUIHouse( strName, guiGameObj, b3DUI );
		mapGUI.Add(strName, guiHouse);			
		return true;
	}
	
	public bool RemoveGUI( string strName ) {
		
		if( mapGUI.ContainsKey(strName) ) {			
			
			mapGUI[strName].DetroySelf();
			mapGUI.Remove(strName);			
			return true;
		}	
			
		return false;
	}	
	
	
	
	
	
	public GameObject GetPanel( string strGuihouse, string strPanel ) {	
		
		GUIHouse guiHouse = GetGUI( strGuihouse );		
		if( guiHouse == null ) {
			
			throw new GUIManagerException( string.Format( "Cannot access GUIHouse object - '{0}'", strGuihouse) );										
		}		
		
		return guiHouse.GetPanel( strPanel );		
	}
	
	public T GetPanelScript<T>( string strGuihouse, string strPanel ) where T : MonoBehaviour {	
		
		GUIHouse guiHouse = GetGUI( strGuihouse );		
		if( guiHouse == null ) {
			
			throw new GUIManagerException( string.Format( "Cannot access GUIHouse object - '{0}'", strGuihouse) );															
		}
		
		return guiHouse.GetPanel( strPanel ).GetComponentInChildren<T>();
	}	
	
	public void ShowPanel( string strGuihouse, string strPanel, bool bShow ) {		
		
		GUIHouse guiHouse = GetGUI( strGuihouse );		
		if( guiHouse == null ) {		
									
			throw new GUIManagerException( string.Format( "Cannot access GUIHouse object - '{0}'", strGuihouse) );
		}		
		
		guiHouse.ShowPanel(strPanel, bShow);	
	}
	
	public void ShowAllPanel( string strGuihouse, bool bShow ) {
		
		GUIHouse guiHouse = GetGUI( strGuihouse );		
		if( guiHouse == null ) {
			
			throw new GUIManagerException( string.Format( "Cannot access GUIHouse object - '{0}'", strGuihouse) );									
		}	
		
		guiHouse.ShowAllPanel(bShow);	
	}	
	
	
	// static
	public static GUIManager Instance {
		
		get { 
			return SingletonGameObject<GUIManager>.Instance; 
		} 	
	}	
}
