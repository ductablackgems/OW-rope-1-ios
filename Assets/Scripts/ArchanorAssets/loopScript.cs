using System.Collections;
using UnityEngine;

namespace ArchanorAssets
{
	public class loopScript : MonoBehaviour
	{
		public GameObject chosenEffect;

		public float loopTimeLimit = 2f;

		private void Start()
		{
			PlayEffect();
		}

		public void PlayEffect()
		{
			StartCoroutine("EffectLoop");
		}

		private IEnumerator EffectLoop()
		{
			GameObject effectPlayer = UnityEngine.Object.Instantiate(chosenEffect, base.transform.position, base.transform.rotation);
			yield return new WaitForSeconds(loopTimeLimit);
			UnityEngine.Object.Destroy(effectPlayer);
			PlayEffect();
		}
	}
}
