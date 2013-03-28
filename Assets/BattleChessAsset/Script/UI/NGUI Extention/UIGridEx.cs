//----------------------------------------------
//            NGUI: Next-Gen UI kit
// Copyright © 2011-2012 Tasharen Entertainment
//----------------------------------------------

using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// All children added to the game object with this script will be repositioned to be on a grid of specified dimensions.
/// If you want the cells to automatically set their scale based on the dimensions of their content, take a look at UITable.
/// </summary>

[ExecuteInEditMode]
[AddComponentMenu("NGUI/Interaction/Grid Ex")]
public class UIGridEx : MonoBehaviour
{
	public enum Arrangement
	{
		Horizontal,
		Vertical,
	}

	public Arrangement arrangement = Arrangement.Horizontal;	
	public float cellWidth = 200f;
	public float cellHeight = 200f;
	public bool repositionNow = false;	
	public bool hideInactive = true;
	
	// addition to ngui for uniform draggable scroll panel
	public GameObject gridItemPrefab;
	
	private List<GameObject> gridItemList = new List<GameObject>();
	public List<GameObject> GridItemList { get { return gridItemList;  } }
	
	int numGridItem;
	int realNumGridItem;
	
	int currentPos = 0;
	int prevPos = 0;
	
	int startSlotID;
	int endSlotID;	
	
	IDraggablePanelAdapter panelAdapter = null;	
	
	Dictionary<int, GameObject> gridItemSlotMap = new Dictionary<int, GameObject>();
	
	private bool isFilled = false;
	public bool IsFilled { get { return isFilled; }	}
	// fin

	bool mStarted = false;

	void Start ()
	{
		mStarted = true;
		Reposition();
	}

	void Update ()
	{
		if (repositionNow)
		{
			repositionNow = false;
			Reposition();
		}
	}	
	
	public void Reposition ()
	{
		if (!mStarted)
		{
			repositionNow = true;
			return;
		}		
		
		foreach( KeyValuePair<int, GameObject> pair in gridItemSlotMap )			
		{
			int nSlotID = pair.Key;
			GameObject gridItemObj = pair.Value;
			
			if( !hideInactive || NGUITools.GetActive( gridItemObj ) ) 
			{
				if( arrangement == Arrangement.Horizontal )				
				{					
					float depth = gridItemObj.transform.localPosition.z;
					gridItemObj.transform.localPosition = new Vector3( cellWidth * nSlotID, 0, depth );				
				}
				else
				{
					float depth = gridItemObj.transform.localPosition.z;
					gridItemObj.transform.localPosition = new Vector3( 0, -cellHeight * nSlotID, depth );	
				}
			}
		}
		
		UIDraggablePanelEx drag = NGUITools.FindInParents<UIDraggablePanelEx>(gameObject);
		if (drag != null) 
			drag.UpdateScrollbars(true);				
	}	
	
	public void Init( IDraggablePanelAdapter adapter, int nNumGridItem, int nRealGridItem )
	{
		if( gridItemPrefab == null || adapter == null )
		{
			Debug.LogError( "UIGridEx::Init() - Can't initialize UIGridEx because gridItemPrefab or panelAdapter is not set" );
			return;
		}
		
		currentPos = prevPos = 0;		
		
		numGridItem = nNumGridItem;
		realNumGridItem = nRealGridItem;		
		panelAdapter = adapter;
		
		startSlotID = 0;
		endSlotID = realNumGridItem - 1;
		
		int nCurrNumChildItem = gridItemList.Count;
		int nNumWould = nRealGridItem - nCurrNumChildItem;
		// no create or delete
		if( nNumWould == 0 )
		{
			return;
		}
		// create
		else if( nNumWould > 0 )
		{
			for( int i=0; i<nNumWould; i++ )
			{
				GameObject obj = Instantiate( gridItemPrefab, Vector3.zero, Quaternion.identity ) as GameObject; 				
				if( obj == null )
				{
					Debug.LogError( "UIGridEx::Init() - Can't initialize UIGridEx because gridItemPrefab is not instantiated" );
					return;					
				}
				
				obj.transform.parent = gameObject.transform;
				obj.layer = gameObject.layer;
				
				obj.transform.localPosition = Vector3.zero;
				obj.transform.localScale = Vector3.one;
				// 이름 초기화 이거 같은게 있으면 트랜스폼의 GetChild()가 안먹는다??? 확인!!!!
				obj.name = (nCurrNumChildItem + i).ToString( "0000" );
				
				gridItemList.Add( obj );
			}
			
			// fill slot map 
			int nCnt = 0;
			gridItemSlotMap.Clear();			
			foreach( GameObject gridItemObj in gridItemList )
			{
				gridItemSlotMap.Add( nCnt, gridItemObj );					
				panelAdapter.OnNotifyFill( nCnt++, gridItemObj );				
			}
		}
		// delete
		else
		{
			List<GameObject> removeObjList = new List<GameObject>();
			for( int i=0; i<-nNumWould; i++ )
			{
				GameObject currentObj = gridItemList[ nCurrNumChildItem - 1 - i ];
				removeObjList.Add( currentObj );						
			}
			
			foreach( GameObject obj in removeObjList )
			{
				gridItemList.Remove( obj );				
				GameObject.Destroy( obj );				
			}
			
			// fill slot map 
			int nCnt = 0;
			gridItemSlotMap.Clear();			
			foreach( GameObject gridItemObj in gridItemList )
			{
				gridItemSlotMap.Add( nCnt, gridItemObj );				
				panelAdapter.OnNotifyFill( nCnt++, gridItemObj );	
			}
		}		
		
		Reposition();
		
		isFilled = true;
	}
	
	public GameObject GetGridItem( int nIdx )
	{
		if( gridItemPrefab == null )
		{
			Debug.LogError( "UIGridEx::GetGridItem() - Can't initialize UIGridEx because gridItemPrefab is not set" );
			return null;
		}		
		
		if( nIdx < 0 || nIdx > gridItemList.Count )
		{
			Debug.LogError( "UIGridEx::GetGridItem() - Invalid Index argument" );
			return null;
		}		
		
		return gridItemList[nIdx];
	}
	
	public void ClearGridItem()
	{		
		foreach( GameObject obj in gridItemList )
		{					
			GameObject.Destroy( obj );				
		}
		
		gridItemList.Clear();
		gridItemSlotMap.Clear();
		
		isFilled = false;
	}
	
	public GameObject GetGridSlotItem( int nSlotID )
	{
		if( gridItemSlotMap.ContainsKey(nSlotID) )
			return gridItemSlotMap[nSlotID];
		
		return null;
	}
	
	public void SetCurrentPosition( int nPos )
	{	
		prevPos = currentPos;
		currentPos = Mathf.Clamp( nPos, 0, numGridItem - 1 );	
		
		int nDragStart, nDragEnd;
			
		int nDragThresholdStart = realNumGridItem / 2;
		int nDragThresholdEnd = numGridItem - realNumGridItem / 2;
		
		int nDir = currentPos - prevPos;
		int nNumDragItem = Mathf.Abs( nDir );
		
		// nothing to do
		if( nDir == 0 )
		{
			return;
		}
		// drag
		else 
		{
			if( currentPos < nDragThresholdStart )
			{
				nDragStart = 0;
				nDragEnd = realNumGridItem - 1;
			}
			else if( currentPos > nDragThresholdEnd )
			{
				nDragStart = numGridItem - realNumGridItem;
				nDragEnd = numGridItem - 1;
			}
			else
			{
				// drag to bottom or top dir					
				nDragStart = Mathf.Clamp( currentPos - nDragThresholdStart, 0, numGridItem - 1 );
				nDragEnd = Mathf.Clamp( nDragStart + realNumGridItem - 1, 0, numGridItem - 1 );		
			}
			
			List< KeyValuePair<int, GameObject> > needFillList = new List< KeyValuePair<int, GameObject> >();
			
			ReArarange( nDragStart, nDragEnd, needFillList );
			
			if( panelAdapter != null )
			{
				foreach( KeyValuePair<int, GameObject> pair in needFillList )
				{
					panelAdapter.OnNotifyFill( pair.Key, pair.Value );				
				}
			}
		}			
	}
	
	void ReArarange( int nStart, int nEnd, List< KeyValuePair<int, GameObject> > needFillList )
	{
		int nNumMoveSlot = nStart - startSlotID;
		
		// nothing to do
		if( nNumMoveSlot == 0 )
		{
			return;
		}
		
		// entire
		int nNumMoveSlotAbs = Mathf.Abs( nNumMoveSlot );
		if( nNumMoveSlotAbs >= realNumGridItem  )
		{			
			for( int i=0, nMoveDstSlotID = nStart; i<realNumGridItem; i++, nMoveDstSlotID++ )
			{
				int nMoveSrcSlotID = startSlotID + i;				
				gridItemSlotMap.Add( nMoveDstSlotID, gridItemSlotMap[nMoveSrcSlotID] );				
				
				needFillList.Add( new KeyValuePair<int, GameObject>( nMoveDstSlotID, gridItemSlotMap[nMoveSrcSlotID] ) );	
				
				gridItemSlotMap.Remove( nMoveSrcSlotID );
			}				
		}
		// partial
		else
		{		
			// drag to bottom or right dir		
			if( nNumMoveSlot > 0 )
			{	
				// add new move drag item
				for( int i=0, nMoveDstSlotID = endSlotID + 1; i<nNumMoveSlot; i++, nMoveDstSlotID++ )
				{
					int nMoveSrcSlotID = startSlotID + i;				
					gridItemSlotMap.Add( nMoveDstSlotID, gridItemSlotMap[nMoveSrcSlotID] );				
					
					needFillList.Add( new KeyValuePair<int, GameObject>( nMoveDstSlotID, gridItemSlotMap[nMoveSrcSlotID] ) );	
					
					gridItemSlotMap.Remove( nMoveSrcSlotID );
				}			
			}
			// drag to top or left dir
			else if( nNumMoveSlot < 0 )
			{			
				// add new move drag item
				nNumMoveSlot = -nNumMoveSlot;
				for( int i=0, nMoveDstSlotID = startSlotID - nNumMoveSlot; i<nNumMoveSlot; i++, nMoveDstSlotID++ )
				{
					int nMoveSrcSlotID = endSlotID - nNumMoveSlot + 1 + i;
					gridItemSlotMap.Add( nMoveDstSlotID, gridItemSlotMap[nMoveSrcSlotID] );
					
					needFillList.Add( new KeyValuePair<int, GameObject>( nMoveDstSlotID, gridItemSlotMap[nMoveSrcSlotID] ) );	
					
					gridItemSlotMap.Remove( nMoveSrcSlotID );
				}			
			}
		}
		
		startSlotID = nStart;
		endSlotID = nEnd;
		
		Reposition();
	}	
	// fin
}

