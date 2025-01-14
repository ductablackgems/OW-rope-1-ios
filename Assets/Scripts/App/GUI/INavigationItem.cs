using UnityEngine;

namespace App.GUI
{
	public interface INavigationItem
	{
		Vector3 Position
		{
			get;
		}

		bool ShowOnMap
		{
			get;
		}
	}
}
