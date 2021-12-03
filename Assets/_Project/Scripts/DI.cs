using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class DI
{

	private static Dictionary<Type, object> depedencies;

	public static void Reset()
	{
		if (depedencies != null) depedencies.Clear();
	}

	public static T Set<T>(T dep, bool substitute = false)
	{
		if (Exists<T>())
		{
			if (substitute)
			{
				depedencies.Remove(typeof(T));
			}
			else
			{
				return (T) depedencies[typeof(T)];
			}
		}

		depedencies.Add(typeof(T), dep);
		return dep;
	}

	public static T Remove<T>()
	{
		if (Exists<T>())
		{
			T comp = (T) depedencies[typeof(T)];
			depedencies.Remove(typeof(T));
			return comp;
		}
		else
		{
			return default(T);
		}
	}

	public static T Get<T>()
	{
		if (Exists<T>())
		{
			return (T) depedencies[typeof(T)];
		}
		else
		{
			return default(T);
		}
	}

	public static bool Exists<T>()
	{
		if (depedencies == null) depedencies = new Dictionary<Type, object>();
		return depedencies.ContainsKey(typeof(T));
	}

}
