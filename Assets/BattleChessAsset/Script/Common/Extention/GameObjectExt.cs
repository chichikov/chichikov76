using UnityEngine;
using System.Collections;

using System.Collections.Generic;


namespace UnityExtention
{
	// GameObject Extention
	public static class GameObjectExt {	
		
		public static GameObject [] GetChildrenGameObjectWithTag( this GameObject gameObj, string strTag, bool bIncludeMe = false )
		{
			if( gameObj == null )
				return null;
		
			List<GameObject> listFindObject = new List<GameObject>();			
			
			// for all child in hierachy
			Transform[] allChildren = gameObj.GetComponentsInChildren<Transform>();
			foreach( Transform currChildTrans in allChildren ) {
				
				if( currChildTrans.gameObject.tag == strTag ) {
					
					listFindObject.Add( currChildTrans.gameObject );
				}
			}
			
			if( bIncludeMe )
				listFindObject.Add( gameObj );
			
			return listFindObject.Count > 0 ? listFindObject.ToArray() : null;		    
		}	       
	} 
	
	// AnimationState Extention
	public static class AnimationStateExt {	
		
		public static IEnumerator WaitForAnimation( this AnimationState animState, float fRatio ) {
			
			animState.wrapMode = WrapMode.ClampForever;
			animState.enabled = true;
			animState.speed = animState.speed == 0 ? 1 : animState.speed;
			var t = animState.time;
			while( ( t / animState.length ) + float.Epsilon < fRatio )
			{
				t += Time.deltaTime * animState.speed;
				yield return null;
			}
		}
	} 	
}