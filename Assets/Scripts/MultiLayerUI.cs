using HedgehogTeam.EasyTouch;
using UnityEngine;

public class MultiLayerUI : MonoBehaviour
{
	public void SetAutoSelect(bool value)
	{
		EasyTouch.SetEnableAutoSelect(value);
	}

	public void SetAutoUpdate(bool value)
	{
		EasyTouch.SetAutoUpdatePickedObject(value);
	}

	public void Layer1(bool value)
	{
		LayerMask mask = EasyTouch.Get3DPickableLayer();
		if (value)
		{
			mask = ((int)mask | 0x100);
		}
		else
		{
			mask = ~(int)mask;
			mask = ~((int)mask | 0x100);
		}
		EasyTouch.Set3DPickableLayer(mask);
	}

	public void Layer2(bool value)
	{
		LayerMask mask = EasyTouch.Get3DPickableLayer();
		if (value)
		{
			mask = ((int)mask | 0x200);
		}
		else
		{
			mask = ~(int)mask;
			mask = ~((int)mask | 0x200);
		}
		EasyTouch.Set3DPickableLayer(mask);
	}

	public void Layer3(bool value)
	{
		LayerMask mask = EasyTouch.Get3DPickableLayer();
		if (value)
		{
			mask = ((int)mask | 0x400);
		}
		else
		{
			mask = ~(int)mask;
			mask = ~((int)mask | 0x400);
		}
		EasyTouch.Set3DPickableLayer(mask);
	}
}
