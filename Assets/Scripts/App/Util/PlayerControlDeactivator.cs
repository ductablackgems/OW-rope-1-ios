using App.Player;
using UnityEngine;

namespace App.Util
{
	public class PlayerControlDeactivator : MonoBehaviour
	{
		private void Awake()
		{
			ServiceLocator.Get<PlayerController>().controlled = false;
			ServiceLocator.Get<PlayerCamera>().controlled = false;
		}
	}
}
