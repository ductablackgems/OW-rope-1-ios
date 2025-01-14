using System;
using UnityEngine;

public class ShowRoom : MonoBehaviour
{
	private int _index;

	private void Update()
	{
		if (UnityEngine.Input.GetKeyDown(KeyCode.RightArrow))
		{
			Next();
		}
		if (UnityEngine.Input.GetKeyDown(KeyCode.LeftArrow))
		{
			Previous();
		}
	}

	public void Next()
	{
		base.transform.GetChild(_index).gameObject.SetActive(value: false);
		_index = GetCount(++_index, 0, base.transform.childCount - 1);
		base.transform.GetChild(_index).gameObject.SetActive(value: true);
	}

	public void Previous()
	{
		base.transform.GetChild(_index).gameObject.SetActive(value: false);
		_index = GetCount(--_index, 0, base.transform.childCount - 1);
		base.transform.GetChild(_index).gameObject.SetActive(value: true);
	}

	private static int GetCount(int currentValue, int min, int max)
	{
		int num = max - min + 1;
		if (currentValue < min)
		{
			int num2 = Math.Abs(currentValue) - Math.Abs(min);
			num2 %= num;
			currentValue = max + 1 - num2;
		}
		else if (currentValue > max)
		{
			int num3 = Math.Abs(Math.Abs(currentValue) - Math.Abs(max));
			num3 %= num;
			currentValue = min - 1 + num3;
		}
		return currentValue;
	}
}
