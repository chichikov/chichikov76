using UnityEngine;
using System.Collections;

public class WheelPhysics : MonoBehaviour {
	
	public Vector3 eulerAngleVelocity = new Vector3(0, 100, 0);
		
	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}
	
	void FixedUpdate() { 	
		
		Quaternion deltaRotation = Quaternion.Euler(eulerAngleVelocity * Time.deltaTime);
		rigidbody.MoveRotation(rigidbody.rotation * deltaRotation);		
	}
}
