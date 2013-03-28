using UnityEngine;
using System.Collections;

using System.Collections.Generic;


[RequireComponent(typeof(UIDraggablePanelEx))]
public class FriendListScrollPanel : MonoBehaviour, IDraggablePanelAdapter {		
	
	public class FriendInfo
	{
		public int nRank;
		public string strPitureLabel;
		public string strName;
		
		public FriendInfo( int nRank, string strPictureLabel, string strName )
		{
			this.nRank = nRank;
			this.strPitureLabel = strPictureLabel;
			this.strName = strName;
		}
	}
	
	List<FriendInfo> friendList;	
	
	UIDraggablePanelEx dragPanel;
	UIGridEx grid;
	
	// Use this for first initialization	
	void Awake() {	
		
		// init friend list
		friendList = new List<FriendInfo>();
		for( int i=0; i<1000; i++ )
		{
			FriendInfo friendInfo = new FriendInfo( i, i.ToString(), "dummy_name_" + i.ToString() );	
			friendList.Add( friendInfo );
		}		
		
		friendList.Sort( SortByRank );
		
		// draggable panel init
		dragPanel = GetComponentInChildren<UIDraggablePanelEx>() as UIDraggablePanelEx;
		grid = GetComponentInChildren<UIGridEx>() as UIGridEx;
		
		FillGrid ();		
	}
	
	
	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}
	
	
	// sort for FriendInfo
	static public int SortByName (FriendInfo a, FriendInfo b) { return string.Compare(a.strName, b.strName); }
	static public int SortByRank (FriendInfo a, FriendInfo b) { return (a.nRank < b.nRank ? 1 : -1); }
	
	
	// fill grid scroll item contents
	public void FillGrid()
	{
		dragPanel.Init( this, friendList.Count, 50 );		
	}
	
	
	public void OnNotifyFill( int nSlotID, GameObject slotObj ) {
		
		
	}
}
