using App.Effects;
using App.GUI;
using App.Interaction;
using App.Player;
using App.Util;
using UnityEngine;

namespace App.Shop
{
	public class AirplaneShoppingZone : InteractiveObject
	{
		[SerializeField]
		private GameplayEffect effectPrefab;

		[SerializeField]
		private float effectDistance = 100f;

		private Pauser pauser;

		private PlayerModel player;

		private GameplayEffect effect;

		private EffectsManager effectsManager;

		private PanelsManager panelsManager;

		private AirplaneSelectionPanel dialog;

		private void Awake()
		{
			Initialize();
		}

		protected override void OnInitialized()
		{
			base.OnInitialized();
			pauser = ServiceLocator.Get<Pauser>();
			player = ServiceLocator.GetPlayerModel();
			effectsManager = ServiceLocator.Get<EffectsManager>();
			panelsManager = ServiceLocator.Get<PanelsManager>();
		}

		protected override void OnInteract()
		{
			base.OnInteract();
			if (!(dialog != null))
			{
				dialog = (panelsManager.ShowPanel(PanelType.AirplaneSelection) as AirplaneSelectionPanel);
				dialog.Close += OnDialogClose;
			}
		}

		protected override void OnUpdate()
		{
			base.OnUpdate();
			if (isInitialized)
			{
				UpdateEffect();
			}
		}

		private void OnDialogClose()
		{
			panelsManager.ShowPanel(panelsManager.PreviousPanel.GetPanelType());
			dialog.Close -= OnDialogClose;
			dialog = null;
		}

		private void UpdateEffect()
		{
			Vector3 vector = base.transform.position - player.Transform.position;
			float magnitude = vector.magnitude;
			if (vector.magnitude > effectDistance)
			{
				HideEffect();
			}
			else
			{
				ShowEffect();
			}
		}

		private void HideEffect()
		{
			if (!(effect == null))
			{
				effect.Deactivate();
				effect = null;
			}
		}

		private void ShowEffect()
		{
			if (!(effect != null))
			{
				effect = effectsManager.GetEffect(effectPrefab);
				effect.Activate(base.transform.position);
			}
		}
	}
}
