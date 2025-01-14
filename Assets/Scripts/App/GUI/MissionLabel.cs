using UnityEngine;

namespace App.GUI
{
	public class MissionLabel : MonoBehaviour
	{
		private UILabel label;

		private DurationTimer hideTimer = new DurationTimer();

		private bool initialized;

		public void Show(string text, float hideAfter = 0f)
		{
			try
			{
				bool flag = base.gameObject == null;
			}
			catch
			{
				return;
			}
			Init();
			label.text = text;
			base.gameObject.SetActive(value: true);
			if (hideAfter > 0f)
			{
				hideTimer.Run(hideAfter);
			}
			else
			{
				hideTimer.Stop();
			}
		}

		public void Hide()
		{
			base.gameObject.SetActive(value: false);
			hideTimer.Stop();
		}

		private void Awake()
		{
			Init();
		}

		private void Update()
		{
			if (hideTimer.Done())
			{
				Hide();
			}
		}

		private void Init()
		{
			if (!initialized)
			{
				initialized = true;
				label = this.GetComponentSafe<UILabel>();
				Hide();
			}
		}

		public void setMissionText(int Textid, int missionParametr)
		{
			GetComponent<LocalizedTextNGUI>().GetDisplayText(Textid, missionParametr);
		}
	}
}
