using UnityEngine;
using System.Collections;

using System.Collections.Generic;


namespace UnityExtention
{
	// GameObject Extention
	public static class GameObjectExt {	
		
		static void GetChildrenSub( this GameObject gameObj, List<GameObject> listChild )
		{
			if( gameObj == null )
				return;
				
			listChild.Add( gameObj );
			
			for (int i = 0, imax = gameObj.transform.GetChildCount(); i < imax; ++i)
			{
				Transform child = gameObj.transform.GetChild(i);
				child.gameObject.GetChildrenSub( listChild );
			}		    
		}
						
		public static GameObject [] GetChildren( this GameObject gameObj )
		{
			if( gameObj == null )
				return null;		
			
			// for all child in hierachy
			List<GameObject> listChild = new List<GameObject>();
			
			for (int i = 0, imax = gameObj.transform.GetChildCount(); i < imax; ++i)
			{
				Transform child = gameObj.transform.GetChild(i);
				child.gameObject.GetChildrenSub( listChild );
			}			
			
			return listChild.Count > 0 ? listChild.ToArray() : null;		    
		}		
		
		public static T [] GetChildrenComponent<T>( this GameObject gameObj )
			where T : Component
		{
			if( gameObj == null )
				return null;
			
			List<T> listFoundChild = new List<T>();		
			GameObject [] aChild = gameObj.GetChildren();															
			
			foreach( GameObject currChild in aChild ) {
				
				T currComponent = currChild.GetComponent<T>();
				if( currComponent != null )					
					listFoundChild.Add( currComponent );				
			}			
			
			return listFoundChild.Count > 0 ? listFoundChild.ToArray() : null;		    
		}		
		
		public static GameObject [] GetChildrenWithTag( this GameObject gameObj, string strTag )
		{
			if( gameObj == null )
				return null;
		
			List<GameObject> listFoundChild = new List<GameObject>();
			GameObject [] aChild = gameObj.GetChildren();									
			
			foreach( GameObject currChild in aChild ) {
				
				if( currChild.tag == strTag )					
					listFoundChild.Add( currChild );				
			}			
			
			return listFoundChild.Count > 0 ? listFoundChild.ToArray() : null;		    
		}		
		
		public static void AddChild( this GameObject gameObj, GameObject childObj )
		{
			if( gameObj == null || childObj == null )
				return;
			
			Transform t = childObj.transform;
			t.parent = gameObj.transform;						    
		}
		
		public static T AddChildInstantiate<T>( this GameObject gameObj, T prefab, Vector3 vPos, Quaternion rot )
			where T : Component
		{
			if( gameObj == null || prefab == null )
				return null;
			
			T instObj = MonoBehaviour.Instantiate( prefab, vPos, rot ) as T;
			if( instObj != null ) {
			
				Transform t = instObj.gameObject.transform;
				t.parent = gameObj.transform;						
				
				return instObj;
			}
			
			return null;
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