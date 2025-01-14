using App.Player.Clothes;
using App.Player.ColorManager;
using App.Player.FightSystem;
using App.Player.GrenadeThrow;
using App.Util;
using UnityEngine;

namespace App.Player
{
	public class PlayerModel
	{
		public delegate void SwitchEventHandler(PlayerModel previousModel);

		private PlayerModel previousModel;

		private bool isRoot;

		public GameObject GameObject
		{
			get;
			private set;
		}

		public RagdollHelper RagdollHelper
		{
			get;
			private set;
		}

		public WeaponInventory WeaponInventory
		{
			get;
			private set;
		}

		public AdvancedFightController FightController
		{
			get;
			private set;
		}

		public ShotController ShotController
		{
			get;
			private set;
		}

		public GrenadeThrowController GrenadeThrowController
		{
			get;
			private set;
		}

		public RopeController RopeController
		{
			get;
			private set;
		}

		public ClimbController ClimbController
		{
			get;
			private set;
		}

		public GlideController GlideController
		{
			get;
			private set;
		}

		public Rigidbody Rigidbody
		{
			get;
			private set;
		}

		public Transform Transform
		{
			get;
			private set;
		}

		public ClothesManager ClothesManager
		{
			get;
			private set;
		}

		public PlayerColorManager ColorManager
		{
			get;
			private set;
		}

		public ItemsCollector ItemsCollector
		{
			get;
			private set;
		}

		public PlayerMonitor PlayerMonitor
		{
			get;
			private set;
		}

		public Transform PelvisTransform
		{
			get;
			private set;
		}

		public Health Health
		{
			get;
			private set;
		}

		public event SwitchEventHandler OnSwitch;

		public PlayerModel(GameObject gameObject, bool isRoot = true)
		{
			this.isRoot = isRoot;
			if (isRoot)
			{
				previousModel = new PlayerModel(null, isRoot: false);
			}
			if (gameObject != null)
			{
				SetPlayer(gameObject);
			}
		}

		public void SetPlayer(GameObject gameObject)
		{
			PlayerModel playerModel = null;
			if (isRoot && GameObject != null)
			{
				playerModel = previousModel;
				playerModel.SetPlayer(GameObject);
			}
			GameObject = gameObject;
			RagdollHelper = gameObject.GetComponentSafe<RagdollHelper>();
			WeaponInventory = gameObject.GetComponentSafe<WeaponInventory>();
			FightController = gameObject.GetComponentSafe<AdvancedFightController>();
			ShotController = gameObject.GetComponentSafe<ShotController>();
			GrenadeThrowController = gameObject.GetComponentSafe<GrenadeThrowController>();
			RopeController = gameObject.GetComponent<RopeController>();
			ClimbController = gameObject.GetComponent<ClimbController>();
			GlideController = gameObject.GetComponentSafe<GlideController>();
			Rigidbody = gameObject.GetComponentSafe<Rigidbody>();
			Transform = gameObject.GetComponentSafe<Transform>();
			ClothesManager = gameObject.GetComponent<ClothesManager>();
			ColorManager = gameObject.GetComponent<PlayerColorManager>();
			ItemsCollector = gameObject.GetComponentSafe<ItemsCollector>();
			PlayerMonitor = gameObject.GetComponentSafe<PlayerMonitor>();
			PelvisTransform = gameObject.GetComponentSafe<Animator>().GetBoneTransform(HumanBodyBones.Hips);
			Health = gameObject.GetComponent<Health>();
			if (isRoot && this.OnSwitch != null)
			{
				this.OnSwitch(playerModel);
			}
		}
	}
}
