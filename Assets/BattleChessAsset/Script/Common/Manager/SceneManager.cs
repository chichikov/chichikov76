using UnityEngine;
using System.Collections;

using System.Collections.Generic;


public class SceneManager : MonoBehaviour {	
	
	public enum eLoadState {
		
		eLOAD_DONE,
		eLOAD_SYNC_LOADING,
		eLOAD_ASYNC_LOADING
	}
	
	public string currentSceneName;
	public int currentSceneIndex;		
	
	string prevSceneName;
	int prevSceneIndex;
	
	AsyncOperation currentAsyncOP;
	
	
	public bool IsLoadingDone { get; private set; }
	
	public eLoadState CurrentState { 
		
		get {	
			
			if( IsLoadingDone )
				return eLoadState.eLOAD_DONE;
			
			if( currentAsyncOP == null ) {				
					
				if( Application.isLoadingLevel )
					return eLoadState.eLOAD_SYNC_LOADING;
				
				return eLoadState.eLOAD_DONE;
			}
			
			if( currentAsyncOP.isDone )
				return eLoadState.eLOAD_DONE;
			
			return eLoadState.eLOAD_ASYNC_LOADING;
		}
	}
	
	
	
	
	
	void Awake() {		
		
		// enroll game object singleton
		SingletonGameObject<SceneManager>.AddSingleton( this );				
		
		prevSceneName = currentSceneName = Application.loadedLevelName;
		prevSceneIndex = currentSceneIndex = 0;
		
		currentAsyncOP = null;
	}
	
	void Start() {				
		
	}	
	
	void Update() {			
		
	}
	
	void OnLevelWasLoaded( int nSceneIndex ) {
		
		currentSceneIndex = nSceneIndex;
		currentSceneName = Application.loadedLevelName;
		
		IsLoadingDone = true;
	}
	
	
	
	// scene management	
	public IEnumerator AsyncLoadLevelAdditively( string strScene ) {
		
		prevSceneName = currentSceneName;
		currentSceneName = strScene;
		
		prevSceneIndex = currentSceneIndex;
		
		currentAsyncOP = Application.LoadLevelAdditiveAsync( strScene );       
		yield return currentAsyncOP;
	}	
	
	public IEnumerator AsyncLoadLevelAdditively( int nSceneIndex ) {
		
		prevSceneIndex = currentSceneIndex;	
		currentSceneIndex = nSceneIndex;
		
		prevSceneName = currentSceneName;	
		
		currentAsyncOP = Application.LoadLevelAdditiveAsync( nSceneIndex );       
		yield return currentAsyncOP;
	}
	
	public IEnumerator AsyncLoadLevel( string strScene ) {
		
		prevSceneName = currentSceneName;
		currentSceneName = strScene;
		
		prevSceneIndex = currentSceneIndex;
		
		currentAsyncOP = Application.LoadLevelAsync( strScene );       
		yield return currentAsyncOP;
	}
	
	public IEnumerator AsyncLoadLevel( int nSceneIndex ) {
		
		prevSceneIndex = currentSceneIndex;	
		currentSceneIndex = nSceneIndex;
		
		prevSceneName = currentSceneName;	
		
		currentAsyncOP = Application.LoadLevelAsync( nSceneIndex );       
		yield return currentAsyncOP;
	}
	
	public void LoadLevel( string strScene ) {		
		
		prevSceneName = currentSceneName;
		currentSceneName = strScene;
		
		prevSceneIndex = currentSceneIndex;		
		
		currentAsyncOP = null;
		
		Application.LoadLevel( currentSceneName );		
	}
	
	public void LoadLevel( int nSceneIndex ) {
		
		prevSceneIndex = currentSceneIndex;	
		currentSceneIndex = nSceneIndex;
		
		prevSceneName = currentSceneName;		
		
		currentAsyncOP = null;
		
		Application.LoadLevel( nSceneIndex );		
	}
	
	
	// static
	public static SceneManager Instance {
		
		get { 
			return SingletonGameObject<SceneManager>.Instance; 
		} 	
	}	
}
