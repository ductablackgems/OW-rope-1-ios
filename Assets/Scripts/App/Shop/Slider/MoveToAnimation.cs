using System.Collections;
using UnityEngine;

namespace App.Shop.Slider
{
	public class MoveToAnimation : MonoBehaviour
	{
		private float speed;

		private IEnumerator coroutine;

		public void MoveTo(Vector3 moveToPosition, float speed)
		{
			this.speed = speed;
			coroutine = _MoveTo(moveToPosition);
			StartCoroutine(coroutine);
		}

		public void Stop()
		{
			if (coroutine != null)
			{
				StopCoroutine(coroutine);
				coroutine = null;
			}
		}

		private IEnumerator _MoveTo(Vector3 moveToPosition)
		{
			while (true)
			{
				base.transform.position = Vector3.MoveTowards(base.transform.localPosition, moveToPosition, speed * Time.deltaTime);
				if (!(base.transform.position == moveToPosition))
				{
					yield return null;
					continue;
				}
				break;
			}
		}
	}
}
