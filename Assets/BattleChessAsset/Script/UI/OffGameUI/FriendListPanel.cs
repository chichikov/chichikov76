using UnityEngine;
using System.Collections;

public class FriendListPanel : MonoBehaviour {
	

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}
	
	
	public void OnBackClick() {		
		
		GUIManager.Instance.ShowAllPanel( false );
		GUIManager.Instance.ShowMainPanel( true );
	}	
}
