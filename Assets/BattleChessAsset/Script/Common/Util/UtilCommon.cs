using UnityEngine;
using System.Collections;

using System.Collections.Generic;

public class IDIntGenerator<T> {

	static int id = 0;
	
	public static int NextID {
		
		get {
			
			return id++;
		}
	}
}