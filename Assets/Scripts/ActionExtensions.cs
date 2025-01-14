using System;

public static class ActionExtensions
{
	public static void SafeInvoke(this Action action)
	{
		action?.Invoke();
	}

	public static void SafeInvoke<T>(this Action<T> action, T arg1)
	{
		action?.Invoke(arg1);
	}

	public static void SafeInvoke<T1, T2>(this Action<T1, T2> action, T1 arg1, T2 arg2)
	{
		action?.Invoke(arg1, arg2);
	}

	public static void SafeInvoke<T1, T2, T3>(this Action<T1, T2, T3> action, T1 arg1, T2 arg2, T3 arg3)
	{
		action?.Invoke(arg1, arg2, arg3);
	}

	public static void SafeInvoke<T1, T2, T3, T4>(this Action<T1, T2, T3, T4> action, T1 arg1, T2 arg2, T3 arg3, T4 arg4)
	{
		action?.Invoke(arg1, arg2, arg3, arg4);
	}
}
