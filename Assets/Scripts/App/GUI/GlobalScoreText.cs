using _0.DucLib.Scripts.Common;
using App.SaveSystem;
using UnityEngine;

namespace App.GUI
{
	[RequireComponent(typeof(UILabel))]
	public class GlobalScoreText : MonoBehaviour
	{
		private PlayerSaveEntity playerSave;

		private UILabel uiLabel;

		protected void Awake()
		{
			playerSave = ServiceLocator.Get<SaveEntities>().PlayerSave;
			uiLabel = this.GetComponentSafe<UILabel>();

			// wayu
			//playerSave.score += 20000;
			//playerSave.Save();
		}

		protected void OnEnable()
		{
			playerSave.OnSave += OnSave;
			UpdateScore();
		}

		protected void OnDisable()
		{
			playerSave.OnSave -= OnSave;
		}

		private void OnSave(AbstractSaveEntity entity)
		{
			UpdateScore();
		}

		private void UpdateScore()
		{
			uiLabel.text = CommonHelper.AbbreviateNumber(playerSave.score);
		}
	}
}
