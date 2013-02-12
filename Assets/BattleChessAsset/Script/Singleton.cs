using UnityEngine;
using System.Collections;

using System;
using System.Reflection;

   
public class SingletonException
  : Exception
{	
	public SingletonException()	{
	}	
	
	public SingletonException(string message)
	 : base(message) {
	}
		
	public SingletonException(Exception innerException)
		: base(null, innerException) {
	}	
	
	public SingletonException(string message, Exception innerException)
	 : base(message, innerException) {
	}
}



public static class Singleton<T>
	where T : class
{
	static volatile T _instance;
	static object _lock = new object();
	
	static Singleton() {
	}
	
	public static T Instance
	{
		get {
			
			if (_instance == null) {
				
				lock (_lock) {
					if (_instance == null) {
						
						ConstructorInfo constructor = null;
						
						try	{
							// Binding flags exclude public constructors.
							constructor = typeof(T).GetConstructor(BindingFlags.Instance | 
							            BindingFlags.NonPublic, null, new Type[0], null);
						}
						catch (Exception exception) {
							
							throw new SingletonException(exception);
						}
						
						if (constructor == null || constructor.IsAssembly) {
							// Also exclude internal constructors.
							throw new SingletonException(string.Format("A private or " + 
							    "protected constructor is missing for '{0}'.", typeof(T).Name));
						}
						
						_instance = (T)constructor.Invoke(null);
					}
				}				
			}
			
			return _instance;
		}
	}
}