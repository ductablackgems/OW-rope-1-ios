using App.Player.Definition;
using App.Spawn;
using App.Util;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace App.Player
{
	public class RagdollHelper : MonoBehaviour, IResetable
	{
		public delegate void OnRaggdolledEventHandler(bool ragdolled);

		public class BodyPart
		{
			public Transform transform;

			public Collider collider;

			public Rigidbody rigidbody;

			public Vector3 storedPosition;

			public Quaternion storedRotation;
		}

		public float ragdollToMecanimBlendTime = 0.5f;

		public HipsForwardDirection hipsForwardDirection;

		private PlayerAnimatorHandler animatorHandler;

		private Animator animator;

		private Rigidbody _rigidbody;

		private RigidbodyHelper rigidbodyHelper;

		private Collider _collider;

		private RagdollState state;

		private float ragdollingEndTime = -100f;

		private Vector3 ragdolledHipPosition;

		private Vector3 ragdolledHeadPosition;

		private Vector3 ragdolledFeetPosition;

		private List<BodyPart> bodyParts = new List<BodyPart>();

		private bool fixPositionsAfterRagdoll;

		private bool standUpStarted;

		public Transform pelvis;

		private Health Dead;

		private float dmg;

		public List<BodyPart> BodyParts => bodyParts;

		public bool VeryRagdolled => state == RagdollState.ragdolled;

		public bool Ragdolled
		{
			get
			{
				if (state == RagdollState.animated)
				{
					return standUpStarted;
				}
				return true;
			}
			set
			{
				if (value)
				{
					if (state == RagdollState.animated)
					{
						SetKinematic(ragdollPartsAreKinematic: false);
						animator.cullingMode = AnimatorCullingMode.AlwaysAnimate;
						animator.enabled = false;
						state = RagdollState.ragdolled;
					}
				}
				else if (state == RagdollState.ragdolled)
				{
					SetKinematic(ragdollPartsAreKinematic: true);
					ragdollingEndTime = Time.time;
					fixPositionsAfterRagdoll = true;
					animator.enabled = true;
					state = RagdollState.blendToAnim;
					foreach (BodyPart bodyPart in bodyParts)
					{
						bodyPart.storedRotation = bodyPart.transform.rotation;
						bodyPart.storedPosition = bodyPart.transform.position;
					}
					ragdolledFeetPosition = 0.5f * (animator.GetBoneTransform(HumanBodyBones.LeftToes).position + animator.GetBoneTransform(HumanBodyBones.RightToes).position);
					ragdolledHeadPosition = animator.GetBoneTransform(HumanBodyBones.Head).position;
					ragdolledHipPosition = animator.GetBoneTransform(HumanBodyBones.Hips).position;
					Vector3 vector = Vector3.zero;
					switch (hipsForwardDirection)
					{
					case HipsForwardDirection.Up:
						vector = animator.GetBoneTransform(HumanBodyBones.Hips).up;
						break;
					case HipsForwardDirection.Down:
						vector = -animator.GetBoneTransform(HumanBodyBones.Hips).up;
						break;
					case HipsForwardDirection.Forward:
						vector = -animator.GetBoneTransform(HumanBodyBones.Hips).forward;
						break;
					}
					if (vector.y < 0f)
					{
						animatorHandler.StandUpFromBackState.RunPrompt();
					}
					else
					{
						animatorHandler.StandUpFromBellyState.RunPrompt();
					}
					standUpStarted = true;
				}
				if (this.OnRaggdolled != null)
				{
					this.OnRaggdolled(value);
				}
			}
		}

		public bool StandingUp
		{
			get
			{
				if (state != RagdollState.blendToAnim && !animatorHandler.StandUpFromBackState.Running)
				{
					return animatorHandler.StandUpFromBellyState.Running;
				}
				return true;
			}
		}

		public Transform HipsTransform => animator.GetBoneTransform(HumanBodyBones.Hips);

		public event OnRaggdolledEventHandler OnRaggdolled;

		public event Action OnStandUpDone;

		public void ResetStates()
		{
			state = RagdollState.animated;
			ragdollingEndTime = -100f;
			fixPositionsAfterRagdoll = false;
			standUpStarted = false;
			SetKinematic(ragdollPartsAreKinematic: true);
		}

		protected void Awake()
		{
			Dead = GetComponent<Health>();
			animatorHandler = this.GetComponentSafe<PlayerAnimatorHandler>();
			animator = GetComponent<Animator>();
			_rigidbody = GetComponent<Rigidbody>();
			rigidbodyHelper = this.GetComponentSafe<RigidbodyHelper>();
			_collider = GetComponent<Collider>();
			pelvis = animator.GetBoneTransform(HumanBodyBones.Hips);
			Transform[] componentsInChildren = GetComponentsInChildren<Transform>();
			foreach (Transform transform in componentsInChildren)
			{
				if (transform.CompareTag("RagdollPart"))
				{
					BodyPart bodyPart = new BodyPart();
					bodyPart.transform = transform;
					bodyPart.rigidbody = transform.GetComponent<Rigidbody>();
					bodyPart.collider = transform.GetComponent<Collider>();
					bodyParts.Add(bodyPart);
				}
			}
		}

		protected void Start()
		{
			SetKinematic(ragdollPartsAreKinematic: true, changeMainComponents: false);
		}

		protected void Update()
		{
			if (standUpStarted && !StandingUp)
			{
				standUpStarted = false;
				if (this.OnStandUpDone != null)
				{
					this.OnStandUpDone();
				}
			}
		}

		protected void LateUpdate()
		{
			if (state != RagdollState.blendToAnim)
			{
				return;
			}
			if (fixPositionsAfterRagdoll)
			{
				fixPositionsAfterRagdoll = false;
				Vector3 b = ragdolledHipPosition - animator.GetBoneTransform(HumanBodyBones.Hips).position;
				Vector3 vector = base.transform.position + b;
				RaycastHit[] array = Physics.RaycastAll(new Ray(vector, Vector3.down));
				vector.y = 0f;
				RaycastHit[] array2 = array;
				for (int i = 0; i < array2.Length; i++)
				{
					RaycastHit raycastHit = array2[i];
					if (!raycastHit.transform.IsChildOf(base.transform))
					{
						vector.y = Mathf.Max(vector.y, raycastHit.point.y);
					}
				}
				base.transform.position = vector;
				_rigidbody.velocity = Vector3.zero;
				Vector3 vector2 = ragdolledHeadPosition - ragdolledFeetPosition;
				Vector3 b2 = 0.5f * (animator.GetBoneTransform(HumanBodyBones.LeftFoot).position + animator.GetBoneTransform(HumanBodyBones.RightFoot).position);
				Vector3 vector3 = animator.GetBoneTransform(HumanBodyBones.Head).position - b2;
				if (animatorHandler.StandUpFromBackState.Running)
				{
					base.transform.forward = -vector2.normalized;
				}
				else
				{
					base.transform.forward = vector2.normalized;
				}
				base.transform.rotation = Quaternion.Euler(0f, base.transform.rotation.eulerAngles.y, 0f);
			}
			float value = 1f - (Time.time - ragdollingEndTime) / ragdollToMecanimBlendTime;
			value = Mathf.Clamp01(value);
			foreach (BodyPart bodyPart in bodyParts)
			{
				if (bodyPart.transform == animator.GetBoneTransform(HumanBodyBones.Hips))
				{
					bodyPart.transform.position = Vector3.Lerp(bodyPart.transform.position, bodyPart.storedPosition, value);
				}
				bodyPart.transform.rotation = Quaternion.Slerp(bodyPart.transform.rotation, bodyPart.storedRotation, value);
			}
			if (value == 0f)
			{
				state = RagdollState.animated;
				animator.cullingMode = AnimatorCullingMode.CullUpdateTransforms;
			}
		}

		public void SetRagdollVelocity(Vector3 velocity)
		{
			foreach (BodyPart bodyPart in bodyParts)
			{
				if (bodyPart.rigidbody != null)
				{
					bodyPart.rigidbody.velocity = velocity;
				}
			}
		}

		private void OnEnable()
		{
			if (pelvis != null)
			{
				pelvis.GetComponent<Rigidbody>().isKinematic = true;
				pelvis.GetComponent<BoxCollider>().isTrigger = true;
				pelvis.GetComponent<Rigidbody>().mass = 8f;
			}
		}

		private void SetKinematic(bool ragdollPartsAreKinematic, bool changeMainComponents = true)
		{
			foreach (BodyPart bodyPart in bodyParts)
			{
				if (bodyPart.rigidbody != null)
				{
					bodyPart.rigidbody.isKinematic = ragdollPartsAreKinematic;
				}
				if (bodyPart.collider != null)
				{
					bodyPart.collider.enabled = !ragdollPartsAreKinematic;
				}
			}
			if (pelvis != null)
			{
				pelvis.GetComponent<BoxCollider>().isTrigger = ragdollPartsAreKinematic;
				pelvis.GetComponent<BoxCollider>().enabled = true;
				pelvis.GetComponent<Rigidbody>().mass = 8f;
				AddForceBody(dmg);
			}
			if (changeMainComponents)
			{
				rigidbodyHelper.SetKinematic(!ragdollPartsAreKinematic, this);
				_collider.enabled = ragdollPartsAreKinematic;
			}
		}

		public void AddForceBody(float damage)
		{
			dmg = damage;
			if (pelvis != null && Dead.Dead() && dmg > 0f)
			{
				pelvis.GetComponent<Rigidbody>().isKinematic = false;
				pelvis.GetComponent<BoxCollider>().isTrigger = false;
				pelvis.GetComponent<BoxCollider>().enabled = true;
				pelvis.GetComponent<Rigidbody>().mass = 4f;
				Vector3 a = UnityEngine.Camera.main.transform.forward;
				if (CompareTag("Player"))
				{
					a *= -1f;
				}
				pelvis.GetComponent<Rigidbody>().AddForce(a * damage * 15f, ForceMode.Impulse);
				dmg = 0f;
			}
		}

		public void AddForceBodyLife(float damage)
		{
			if (pelvis != null)
			{
				pelvis.GetComponent<Rigidbody>().isKinematic = false;
				pelvis.GetComponent<BoxCollider>().isTrigger = false;
				pelvis.GetComponent<BoxCollider>().enabled = true;
				pelvis.GetComponent<Rigidbody>().mass = 4f;
				Ragdolled = true;
				Vector3 forward = UnityEngine.Camera.main.transform.forward;
				pelvis.GetComponent<Rigidbody>().AddForce(forward * damage * 5f, ForceMode.Impulse);
				pelvis.GetComponent<Rigidbody>().AddExplosionForce(500f, base.transform.position, 10f, 3f, ForceMode.Impulse);
			}
		}

		public void Explosion(Vector3 explosionPos)
		{
			if (pelvis != null && Dead.Dead())
			{
				pelvis.GetComponent<Rigidbody>().isKinematic = false;
				pelvis.GetComponent<BoxCollider>().isTrigger = false;
				pelvis.GetComponent<BoxCollider>().enabled = true;
				pelvis.GetComponent<Rigidbody>().mass = 4f;
				pelvis.GetComponent<Rigidbody>().AddExplosionForce(200f, explosionPos, 10f, 3f, ForceMode.Impulse);
			}
		}
	}
}
