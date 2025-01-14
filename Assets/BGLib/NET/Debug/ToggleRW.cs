using BG_Library.NET;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace BG_Library.DEBUG
{
	public class ToggleRW : MonoBehaviour
	{
		public Toggle t;

		private void Awake()
		{
			t.onValueChanged.AddListener(b =>
			{
				AdsManager.ignoreReward = !b;
			});
		}
	}
}