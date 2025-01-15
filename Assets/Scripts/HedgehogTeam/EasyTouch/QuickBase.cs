using UnityEngine;

namespace HedgehogTeam.EasyTouch
{
	public class QuickBase : MonoBehaviour
	{
		protected enum GameObjectType
		{
			Auto,
			Obj_3D,
			Obj_2D,
			UI
		}

		public enum DirectAction
		{
			None,
			Rotate,
			RotateLocal,
			Translate,
			TranslateLocal,
			Scale
		}

		public enum AffectedAxesAction
		{
			X,
			Y,
			Z,
			XY,
			XZ,
			YZ,
			XYZ
		}

		public string quickActionName;

		public bool isMultiTouch;

		public bool is2Finger;

		public bool isOnTouch;

		public bool enablePickOverUI;

		public bool resetPhysic;

		public DirectAction directAction;

		public AffectedAxesAction axesAction;

		public float sensibility = 1f;

		public CharacterController directCharacterController;

		public bool inverseAxisValue;

		protected Rigidbody cachedRigidBody;

		protected bool isKinematic;

		protected Rigidbody2D cachedRigidBody2D;

		protected bool isKinematic2D;

		protected GameObjectType realType;

		protected int fingerIndex = -1;

		private void Awake()
		{
			cachedRigidBody = GetComponent<Rigidbody>();
			if ((bool)cachedRigidBody)
			{
				isKinematic = cachedRigidBody.isKinematic;
			}
			cachedRigidBody2D = GetComponent<Rigidbody2D>();
			if ((bool)cachedRigidBody2D)
			{
				isKinematic2D = cachedRigidBody2D.isKinematic;
			}
		}

		public virtual void Start()
		{
			EasyTouch.SetEnableAutoSelect(value: true);
			realType = GameObjectType.Obj_3D;
			if ((bool)GetComponent<Collider>())
			{
				realType = GameObjectType.Obj_3D;
			}
			else if ((bool)GetComponent<Collider2D>())
			{
				realType = GameObjectType.Obj_2D;
			}
			else if ((bool)GetComponent<CanvasRenderer>())
			{
				realType = GameObjectType.UI;
			}
			switch (realType)
			{
			case GameObjectType.Obj_3D:
				EasyTouch.Set3DPickableLayer((int)EasyTouch.Get3DPickableLayer() | (1 << base.gameObject.layer));
				break;
			case GameObjectType.Obj_2D:
				EasyTouch.SetEnable2DCollider(value: true);
				EasyTouch.Set2DPickableLayer((int)EasyTouch.Get2DPickableLayer() | (1 << base.gameObject.layer));
				break;
			case GameObjectType.UI:
				EasyTouch.instance.enableUIMode = true;
				EasyTouch.SetUICompatibily(value: false);
				break;
			}
			if (enablePickOverUI)
			{
				EasyTouch.instance.enableUIMode = true;
				EasyTouch.SetUICompatibily(value: false);
			}
		}

		public virtual void OnEnable()
		{
		}

		public virtual void OnDisable()
		{
		}

		protected Vector3 GetInfluencedAxis()
		{
			Vector3 result = Vector3.zero;
			switch (axesAction)
			{
			case AffectedAxesAction.X:
				result = new Vector3(1f, 0f, 0f);
				break;
			case AffectedAxesAction.Y:
				result = new Vector3(0f, 1f, 0f);
				break;
			case AffectedAxesAction.Z:
				result = new Vector3(0f, 0f, 1f);
				break;
			case AffectedAxesAction.XY:
				result = new Vector3(1f, 1f, 0f);
				break;
			case AffectedAxesAction.XYZ:
				result = new Vector3(1f, 1f, 1f);
				break;
			case AffectedAxesAction.XZ:
				result = new Vector3(1f, 0f, 1f);
				break;
			case AffectedAxesAction.YZ:
				result = new Vector3(0f, 1f, 1f);
				break;
			}
			return result;
		}

		protected void DoDirectAction(float value)
		{
			Vector3 influencedAxis = GetInfluencedAxis();
			switch (directAction)
			{
			case DirectAction.Rotate:
				base.transform.Rotate(influencedAxis * value, Space.World);
				break;
			case DirectAction.RotateLocal:
				base.transform.Rotate(influencedAxis * value, Space.Self);
				break;
			case DirectAction.Translate:
			{
				if (directCharacterController == null)
				{
					base.transform.Translate(influencedAxis * value, Space.World);
					break;
				}
				Vector3 motion2 = influencedAxis * value;
				directCharacterController.Move(motion2);
				break;
			}
			case DirectAction.TranslateLocal:
			{
				if (directCharacterController == null)
				{
					base.transform.Translate(influencedAxis * value, Space.Self);
					break;
				}
				Vector3 motion = directCharacterController.transform.TransformDirection(influencedAxis) * value;
				directCharacterController.Move(motion);
				break;
			}
			case DirectAction.Scale:
				base.transform.localScale += influencedAxis * value;
				break;
			}
		}

		public void EnabledQuickComponent(string quickActionName)
		{
			QuickBase[] components = GetComponents<QuickBase>();
			foreach (QuickBase quickBase in components)
			{
				if (quickBase.quickActionName == quickActionName)
				{
					quickBase.enabled = true;
				}
			}
		}

		public void DisabledQuickComponent(string quickActionName)
		{
			QuickBase[] components = GetComponents<QuickBase>();
			foreach (QuickBase quickBase in components)
			{
				if (quickBase.quickActionName == quickActionName)
				{
					quickBase.enabled = false;
				}
			}
		}

		public void DisabledAllSwipeExcepted(string quickActionName)
		{
			QuickSwipe[] array = UnityEngine.Object.FindObjectsOfType(typeof(QuickSwipe)) as QuickSwipe[];
			foreach (QuickSwipe quickSwipe in array)
			{
				if (quickSwipe.quickActionName != quickActionName || (quickSwipe.quickActionName == quickActionName && quickSwipe.gameObject != base.gameObject))
				{
					quickSwipe.enabled = false;
				}
			}
		}
	}
}
