using UnityEngine;
using System;

public static class PauseController
{
	public static bool IsGamePaused { get; private set; }
	public static Action<bool> OnPause;

	[RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
	static void InitStaticFields()
	{
		IsGamePaused = false;
	}

	public static void PauseGame(bool pause)
	{
		IsGamePaused = pause;

		Time.timeScale = pause switch
		{
			true => 0,
			false => 1
		};

		OnPause?.Invoke(pause);
	}
}