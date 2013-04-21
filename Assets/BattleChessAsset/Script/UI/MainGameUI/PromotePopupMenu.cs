using UnityEngine;
using System.Collections;

public class PromotePopupMenu : MonoBehaviour {
	
	// field
	public UIPopupList popupList;
	
	// property
	public string CurrentSelItem { get; private set; }
		
		
	
	// Use this for first initialization	
	void Awake() {	
		
		popupList.selection = popupList.items[0];
		CurrentSelItem = popupList.items[0];
	}
	
	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}
	
	
	// evnet handler
	public void OnSelectionChange( string item ) {
		
		CurrentSelItem = item;
		
		// promote piece
	}
}
