using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace BG_Library.Common 
{
	public class AnimLoadSceneDemo : AnimLoadSceneBase
	{
		[SerializeField] CanvasGroup panelCanvas;
		[SerializeField] Text sceneText;

		public override void StartLoad(string sceneName)
		{
			panelCanvas.gameObject.SetActive(true);
			sceneText.text = sceneName;

			panelCanvas.alpha = 1;
		}

		public override void EndLoad(string sceneName, System.Action onComplete = null)
		{
			DOTweenManager.Ins.TweenDelay(1, () =>
			{
				_ = DOTweenManager.Ins.TweenChangeAlphaCanvasGroup(panelCanvas, 1, 0, 0.5f)
				.OnComplete(() =>
				{
					panelCanvas.gameObject.SetActive(false);
					onComplete?.Invoke();
				});
			});
		}
	}
}