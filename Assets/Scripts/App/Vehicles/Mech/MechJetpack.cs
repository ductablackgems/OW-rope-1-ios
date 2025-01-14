using System.Collections;
using UnityEngine;

namespace App.Vehicles.Mech
{
	public sealed class MechJetpack : MechTechnology
	{
		public enum JetpackState
		{
			None,
			Launch,
			Flying,
			Landing
		}

		[SerializeField]
		private float LaunchForce = 8000f;

		[SerializeField]
		private float UpForce = 8000f;

		[SerializeField]
		private float DownForce = 8000f;

		[SerializeField]
		private float MoveForce = 1000f;

		[SerializeField]
		private AudioClip launchSound;

		[SerializeField]
		private AudioClip landSound;

		private readonly int AnimLandID = Animator.StringToHash("land");

		private readonly int AnimJetpackID = Animator.StringToHash("jetpack");

		private Coroutine launchCoroutine;

		private JetControl[] jets;

		private Rigidbody body;

		private MechSounds sounds;

		private MechController controller;

		private bool isFlyingUp;

		private bool isFlyingDown;

		private bool isDeactivationRequest;

		private float dragBackup;

		public override bool IsActive => State != JetpackState.None;

		public override bool CanActivate
		{
			get
			{
				if (State != 0)
				{
					return State == JetpackState.Landing;
				}
				return true;
			}
		}

		public override bool IsFlying => State == JetpackState.Flying;

		public override bool CanFly => true;

		public JetpackState State
		{
			get;
			private set;
		}

		protected override void OnActivate()
		{
			base.OnActivate();
			base.Animator.SetBool(AnimLandID, value: false);
			if (CanActivate)
			{
				switch (State)
				{
				case JetpackState.None:
					SetLaunchState();
					break;
				case JetpackState.Landing:
					SetFlyingState();
					break;
				}
				ActivateJets(isActive: true);
				controller.SetActiveFallingEffect(isActive: false);
			}
		}

		protected override void OnDeactivate()
		{
			base.OnDeactivate();
			if (State != 0)
			{
				if (State == JetpackState.Launch)
				{
					isDeactivationRequest = true;
					return;
				}
				body.drag = dragBackup;
				SetFallingState();
			}
		}

		protected override void OnMove(Vector2 move)
		{
			if (State == JetpackState.Flying)
			{
				float d = controller.MoveAccordingToCameraDiraction(Time.deltaTime).magnitude * base.MaxForwardSpeed;
				body.AddForce(base.transform.forward.normalized * MoveForce * d);
			}
		}

		protected override void OnMoveUp()
		{
			base.OnMoveUp();
			if (State == JetpackState.Flying)
			{
				isFlyingUp = true;
			}
		}

		protected override void OnMoveDown()
		{
			base.OnMoveDown();
			if (State == JetpackState.Flying)
			{
				isFlyingDown = true;
			}
		}

		protected override void OnInitialize()
		{
			base.OnInitialize();
			body = GetComponent<Rigidbody>();
			jets = GetComponentsInChildren<JetControl>(includeInactive: true);
			sounds = GetComponentInChildren<MechSounds>();
			controller = GetComponent<MechController>();
		}

		private void FixedUpdate()
		{
			if (isInitialized)
			{
				if (isFlyingUp)
				{
					body.AddForce(Vector3.up * UpForce);
					isFlyingUp = false;
				}
				else if (isFlyingDown)
				{
					body.AddForce(Vector3.down * DownForce);
					isFlyingDown = false;
				}
				if (State == JetpackState.Flying)
				{
					body.AddForce(Vector3.up * body.mass * Physics.gravity.magnitude);
				}
				CheckCanLand();
				base.Animator.SetBool(AnimJetpackID, State != JetpackState.None);
			}
		}

		private IEnumerator Launch_Coroutine(float force)
		{
			Vector3 startPosition = base.transform.position;
			float lastHeight = 0f;
			float height = 0f;
			sounds.Engine.PlayOneShot(launchSound);
			body.AddForce(Vector3.up * force, ForceMode.Impulse);
			while (height >= lastHeight)
			{
				height = base.transform.position.y - startPosition.y;
				if (height >= lastHeight)
				{
					lastHeight = height;
				}
				yield return null;
			}
			if (isDeactivationRequest)
			{
				SetFallingState();
			}
			else
			{
				SetFlyingState();
			}
			launchCoroutine = null;
		}

		private void CheckCanLand()
		{
			if (State != 0 && State != JetpackState.Launch && controller.TryLand())
			{
				base.Animator.SetBool(AnimLandID, value: true);
				isFlyingDown = false;
				isFlyingUp = false;
				State = JetpackState.None;
				ActivateJets(isActive: false);
				controller.SetActiveFallingEffect(isActive: false);
			}
		}

		private void SetFlyingState()
		{
			State = JetpackState.Flying;
			dragBackup = body.drag;
			body.drag = 1f;
		}

		private void SetFallingState()
		{
			if (launchCoroutine != null)
			{
				StopCoroutine(launchCoroutine);
				launchCoroutine = null;
			}
			State = JetpackState.Landing;
			isDeactivationRequest = false;
			ActivateJets(isActive: false);
			controller.SetActiveFallingEffect(isActive: true);
		}

		private void SetLaunchState()
		{
			State = JetpackState.Launch;
			launchCoroutine = StartCoroutine(Launch_Coroutine(LaunchForce));
		}

		private void ActivateJets(bool isActive)
		{
			JetControl[] array = jets;
			for (int i = 0; i < array.Length; i++)
			{
				array[i].IsActive = isActive;
			}
		}
	}
}
