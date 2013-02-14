using UnityEngine;
using System.Collections;

public class EtcMenu : MonoBehaviour {

	public void OnBackClick() {		
		
		GUIManager.Instance.ShowPanel( gameObject.name, false );
		GUIManager.Instance.ShowPanel( "InitPanel", true );
	}	
}
