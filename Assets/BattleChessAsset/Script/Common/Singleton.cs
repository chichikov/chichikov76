using UnityEngine;
using System.Collections;

using System.Reflection;



   
public class SingletonException : System.Exception
{	
	public SingletonException()	{
	}	
	
	public SingletonException(string message)
	 : base(message) {
	}
		
	public SingletonException(System.Exception innerException)
		: base(null, innerException) {
	}	
	
	public SingletonException(string message, System.Exception innerException)
	 : base(message, innerException) {
	}
}


// c# generic singleton
public static class Singleton<T>
	where T : class
{
	static volatile T _instance = null;
	static object _lock = new object();
	
	
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
							            BindingFlags.NonPublic, null, new System.Type[0], null);
						}
						catch (System.Exception exception) {
							
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


// unity game object singleton
public class SingletonGameObjectException : System.Exception
{	
	public SingletonGameObjectException()	{
	}	
	
	public SingletonGameObjectException(string message)
	 : base(message) {
	}
		
	public SingletonGameObjectException(System.Exception innerException)
		: base(null, innerException) {
	}	
	
	public SingletonGameObjectException(string message, System.Exception innerException)
	 : base(message, innerException) {
	}
}


public static class SingletonGameObject<T> where T : MonoBehaviour
{
	static volatile T _instance = null;
	static object _lock = new object();		
	
	
	public static void AddSingleton( T singleton ) {
		
		if( _instance == null ) {
			
			lock (_lock) {
				if( _instance == null ) {
					
					_instance = singleton;					
					
					if( _instance is MonoBehaviour ) {
						
						Object.DontDestroyOnLoad( _instance.gameObject );						
					}
					else {
						
						throw new SingletonGameObjectException(string.Format("SingletonGameObject Type miss match!!!!!!", typeof(T).Name));
					}					
				}
				else {
					
					throw new SingletonGameObjectException(string.Format("SingletonGameObject already created!!!", typeof(T).Name));
				}
			}
		}
		else {
			
			throw new SingletonGameObjectException(string.Format("SingletonGameObject already created!!!", typeof(T).Name));
		}
	}
	
	public static T Instance
	{
		get {
			
			if (_instance == null) {
				
				lock (_lock) {
					
					if (_instance == null) {
						
						throw new SingletonGameObjectException(string.Format("SingletonGameObject dose not created!!!", typeof(T).Name));											
					}
				}				
			}
			
			return _instance;
		}
	}
}

