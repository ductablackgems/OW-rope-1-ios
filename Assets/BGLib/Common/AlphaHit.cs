using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace BG_Library.Common
{
	public class AlphaHit : MonoBehaviour
	{
		private void Awake()
		{
			GetComponent<Image>().alphaHitTestMinimumThreshold = 0.1f;
		}
	}
}