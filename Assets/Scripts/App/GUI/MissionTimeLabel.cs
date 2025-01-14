using System;
using UnityEngine;

namespace App.GUI
{
	public class MissionTimeLabel : MonoBehaviour
	{
		private UILabel label;

		private float remainTime = -1f;

		private Action AfterTimeExceed;

		private int lastRemainTimeInt = -1;

		public void StartCountdown(float remainTime, Action AfterTimeExceed = null)
		{
			this.remainTime = remainTime;
			this.AfterTimeExceed = AfterTimeExceed;
			base.gameObject.SetActive(value: true);
		}

		public void AbortCountdown()
		{
			remainTime = -1f;
			lastRemainTimeInt = -1;
			AfterTimeExceed = null;
			base.gameObject.SetActive(value: false);
		}

		private void Awake()
		{
			label = this.GetComponentSafe<UILabel>();
			AbortCountdown();
		}

		private void Update()
		{
			if (!(remainTime > 0f))
			{
				return;
			}
			remainTime -= Time.deltaTime;
			int num = Mathf.RoundToInt(remainTime);
			if (lastRemainTimeInt != num)
			{
				lastRemainTimeInt = num;
				TimeSpan timeSpan = TimeSpan.FromSeconds(num);
				label.text = $"{timeSpan.Minutes:D2}:{timeSpan.Seconds:D2}";
			}
			if (remainTime <= 0f)
			{
				remainTime = -1f;
				lastRemainTimeInt = -1;
				if (AfterTimeExceed != null)
				{
					AfterTimeExceed();
					AfterTimeExceed = null;
				}
				base.gameObject.SetActive(value: false);
			}
		}
	}
}
