using UnityEngine;
using System.Collections;

public class GameMain : MonoBehaviour {
	
	
	
	
	// Use this for first initialization
	void Awake() {
	
		// enroll game object singleton
		SingletonGameObject<GameMain>.AddSingleton( this ); 
	}
	
	// Use this for second initialization
	void Start() {
	
	}
	
	// Update is called once per frame
	void Update() {
	
	}	
	
	// static
	public static GameMain Instance {
		
		get { 
			return SingletonGameObject<GameMain>.Instance; 
		} 
	}
}
