using UnityEngine;
using Sirenix.OdinInspector;

public abstract class Singleton<T> : MonoBehaviour where T : MonoBehaviour
{
	public static T Inst { get; private set; }
	
	protected virtual void Awake()
	{
		if (Inst == null) { Inst = this as T; }
		else
		{
			Destroy(this);
		}
	}
	
	protected void OnApplicationQuit()
	{
		#if UNITY_EDITOR
		Inst = null;
		#endif
	}
}

public abstract class EditorSingleton<T> : MonoBehaviour where T : MonoBehaviour
{
	//static instance
	public static T Inst
	{
		get => GetOrSetInstance();
	}
	private static T _instance;


	private static T GetOrSetInstance()
	{
		if (_instance) { return _instance; }
            
		_instance = FindObjectOfType<T>();
		return _instance;
	}
	
	protected virtual void Awake()
	{
		if (_instance == null) { _instance = this as T; }
		else
		{
			Destroy(this);
		}
	}
	
	protected void OnApplicationQuit()
	{
		#if UNITY_EDITOR
		_instance = null;
		#endif
	}
}

public abstract class SerializedSingleton<T> : SerializedMonoBehaviour  where T : SerializedMonoBehaviour 
{
	public static T Inst { get; private set; }
	
	protected virtual void Awake()
	{
		if (Inst == null) { Inst = this as T; }
		else
		{
			Destroy(this);
		}
	}
	
	protected void OnApplicationQuit()
	{
		#if UNITY_EDITOR
		Inst = null;
		#endif
	}
}
public abstract class EditorSerializedSingleton<T> : SerializedMonoBehaviour where T : SerializedMonoBehaviour
{
	//static instance
	public static T Inst
	{
		get => GetOrSetInstance();
	}
	private static T _instance;


	private static T GetOrSetInstance()
	{
		if (_instance) return _instance;
            
		_instance = FindObjectOfType<T>();
		return _instance;
	}
	
	protected virtual void Awake()
	{
		if (_instance != null && _instance != this){ Destroy(this); }
		else
		{
			_instance = this as T;
		}
	}
	
	protected void OnApplicationQuit()
	{
		#if UNITY_EDITOR
		_instance = null;
		#endif
	}
}