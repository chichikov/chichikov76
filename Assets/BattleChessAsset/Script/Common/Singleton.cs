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


// c# generic singleton
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


// unity game object singleton
public class SingletonGameObjectException
  : Exception
{	
	public SingletonGameObjectException()	{
	}	
	
	public SingletonGameObjectException(string message)
	 : base(message) {
	}
		
	public SingletonGameObjectException(Exception innerException)
		: base(null, innerException) {
	}	
	
	public SingletonGameObjectException(string message, Exception innerException)
	 : base(message, innerException) {
	}
}


public static class SingletonGameObject<T>
	where T : UnityEngine.MonoBehaviour
{
	static volatile T _instance;
	static object _lock = new object();	
	
	static SingletonGameObject() {
	}
	
	public static void AddSingleton( T singleton ) {
		
		if( _instance == null ) {
			
			lock (_lock) {
				if( _instance == null ) {
					
					_instance = singleton;
					
					if( _instance is UnityEngine.MonoBehaviour ) {
						
						UnityEngine.Object.DontDestroyOnLoad( _instance.gameObject );						
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

