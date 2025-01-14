using App.GUI;
using App.Player.Definition;
using App.Util;
using UnityEngine;

namespace App.Player
{
	public class FlyController : MonoBehaviour, ICharacterModule
	{
		public enum FlyStyle
		{
			Mechanic,
			Wings
		}

		public FlyStyle flyStyle;

		[Range(0f, 1f)]
		public float flyType;

		public float upForce = 100f;

		public float downForce = 200f;

		public float horizontalForce = 200f;

		public float drag = 1f;

		public float movingTurnSpeed = 360f;

		public float stationaryTurnSpeed = 180f;

		public ParticleSystem[] particleSystems;

		public AudioSource flyAudioSource;

		private Rigidbody _rigidbody;

		private WingsController wingsController;

		private PlayerAnimatorHandler animatorHandler;

		private WingsAnimationHandler wingsAnimatorHandler;

		private CharacterControl characterControl;

		private EnergyScript energy;

		private CharacterRotator characterRotator;

		private PanelsManager panelsManager;

		private Transform cameraTransform;

		private DurationTimer liftUpTimer = new DurationTimer(useFixedTime: true);

		private float initialDrag;

		private float liftInput;

		private float horizontalInput;

		private float verticalInput;

		private float UpDownInput;

		private Vector2 smoothInput;

		private bool running;

		public bool Runnable()
		{
			return energy.GetCurrentEnergy() > 0.3f;
		}

		public void Run()
		{
			if (Runnable())
			{
				running = true;
				smoothInput = Vector3.zero;
				UpdateAnimatorAxes();
				animatorHandler.Fly = true;
				_rigidbody.useGravity = false;
				_rigidbody.drag = drag;
				ParticleSystem[] array = particleSystems;
				for (int i = 0; i < array.Length; i++)
				{
					array[i].Play();
				}
				liftUpTimer.Run(0.4f);
				panelsManager.ShowPanel(PanelType.Flying);
				if ((bool)flyAudioSource)
				{
					flyAudioSource.pitch = 1f;
					flyAudioSource.Play();
				}
			}
		}

		public bool Running()
		{
			return running;
		}

		public void Stop()
		{
			if (Running())
			{
				running = false;
				animatorHandler.Fly = false;
				_rigidbody.useGravity = true;
				_rigidbody.drag = initialDrag;
				liftInput = 0f;
				horizontalInput = 0f;
				verticalInput = 0f;
				liftUpTimer.Stop();
				ParticleSystem[] array = particleSystems;
				for (int i = 0; i < array.Length; i++)
				{
					array[i].Stop();
				}
				if (panelsManager.CompareActivePanel(PanelType.Flying))
				{
					panelsManager.ShowPanel(PanelType.Game);
				}
				if ((bool)flyAudioSource)
				{
					flyAudioSource.Stop();
				}
				if (flyStyle == FlyStyle.Wings && (bool)wingsController)
				{
					wingsAnimatorHandler.FwdGlide = false;
					wingsController.DeactivateAll();
				}
			}
		}

		private void Awake()
		{
			_rigidbody = this.GetComponentSafe<Rigidbody>();
			wingsController = GetComponent<WingsController>();
			animatorHandler = this.GetComponentSafe<PlayerAnimatorHandler>();
			wingsAnimatorHandler = GetComponent<WingsAnimationHandler>();
			characterControl = this.GetComponentSafe<CharacterControl>();
			energy = this.GetComponentSafe<EnergyScript>();
			characterRotator = this.GetComponentInChildrenSafe<CharacterRotator>();
			panelsManager = ServiceLocator.Get<PanelsManager>();
			cameraTransform = ServiceLocator.GetGameObject("MainCamera").transform;
			initialDrag = _rigidbody.drag;
		}

		private void Start()
		{
			animatorHandler.FlyType = flyType;
		}

		public void Control(bool flyPressedDown, bool upPressed, bool downPressed, float horizontalInput, float verticalInput)
		{
			if (!Running())
			{
				return;
			}
			if (energy.GetCurrentEnergy() < 0.05f)
			{
				Stop();
				return;
			}
			if (flyPressedDown && animatorHandler.FlyState.Running)
			{
				Stop();
				return;
			}
			if (!liftUpTimer.Running() && characterControl.Grounded && characterControl.IsGroundedRaw(out RaycastHit _))
			{
				Stop();
				return;
			}
			if (upPressed)
			{
				liftInput = 1f;
			}
			else if (downPressed)
			{
				liftInput = -1f;
			}
			else
			{
				liftInput = 0f;
			}
			this.horizontalInput = horizontalInput;
			this.verticalInput = verticalInput;
			UpDownInput = (upPressed ? 1f : (downPressed ? (-1f) : 0f));
			UpdateAnimatorAxes(Time.deltaTime);
			if (upPressed || Mathf.Abs(horizontalInput) > 0.2f || Mathf.Abs(verticalInput) > 0.2f || liftUpTimer.Running())
			{
				if ((bool)flyAudioSource)
				{
					flyAudioSource.pitch = MoveFloatTowards(flyAudioSource.pitch, 2f, Time.deltaTime * 2f);
				}
			}
			else if ((bool)flyAudioSource)
			{
				flyAudioSource.pitch = MoveFloatTowards(flyAudioSource.pitch, 1f, Time.deltaTime * 1.5f);
			}
		}

		public void FixedUpdate()
		{
			if (!Running())
			{
				return;
			}
			if (flyStyle == FlyStyle.Wings)
			{
				if ((bool)wingsController)
				{
					if (wingsController.frontWingsAnimationSettings.Running)
					{
						if (!wingsController.frontWingsAnimationSettings.Glide)
						{
							energy.ConsumeFlyEnergy(Time.fixedDeltaTime);
						}
					}
					else
					{
						energy.ConsumeFlyEnergy(Time.fixedDeltaTime);
					}
				}
			}
			else
			{
				energy.ConsumeFlyEnergy(Time.fixedDeltaTime);
			}
			if (liftUpTimer.Done() || (liftInput != 0f && liftUpTimer.Running()))
			{
				liftUpTimer.Stop();
			}
			if (liftInput != 0f || horizontalInput != 0f || verticalInput != 0f || liftUpTimer.Running())
			{
				float num = liftInput;
				if (liftUpTimer.Running())
				{
					num = 1f;
				}
				Vector3 forward = cameraTransform.forward;
				Vector3 vector = forward;
				vector.y = 0f;
				forward.Normalize();
				vector.Normalize();
				Vector3 right = cameraTransform.right;
				right.y = 0f;
				right.Normalize();
				Vector3 a = Vector3.zero;
				if (flyStyle != FlyStyle.Wings)
				{
					a = ((!(verticalInput < 0f)) ? (forward * verticalInput + right * horizontalInput) : (vector * verticalInput + right * horizontalInput));
				}
				else if ((bool)wingsController)
				{
					if (verticalInput < 0f)
					{
						if (horizontalInput < 0f)
						{
							if (wingsController.frontWingsAnimationSettings.Running)
							{
								wingsAnimatorHandler.FwdGlide = false;
								wingsController.frontWingsAnimationSettings.End();
							}
							if (!wingsController.leftWingsAnimationSettings.Running)
							{
								wingsAnimatorHandler.FwdGlide = false;
								if (num == 0f)
								{
									wingsController.DeactivateOther(wingsController.leftWingsAnimationSettings);
									wingsController.leftWingsAnimationSettings.Begin();
								}
							}
							if (wingsController.leftWingsAnimationSettings.CanAddForce)
							{
								a = vector * verticalInput + right * horizontalInput;
							}
						}
						else if (horizontalInput > 0f)
						{
							if (!wingsController.rightWingsAnimationSettings.Running)
							{
								wingsAnimatorHandler.FwdGlide = false;
								if (num == 0f)
								{
									wingsController.DeactivateOther(wingsController.rightWingsAnimationSettings);
									wingsController.rightWingsAnimationSettings.Begin();
								}
							}
							if (wingsController.rightWingsAnimationSettings.CanAddForce)
							{
								a = vector * verticalInput + right * horizontalInput;
							}
						}
						else
						{
							if (!wingsController.backWingsAnimationSettings.Running)
							{
								wingsAnimatorHandler.FwdGlide = false;
								if (num == 0f)
								{
									wingsController.DeactivateOther(wingsController.backWingsAnimationSettings);
									wingsController.backWingsAnimationSettings.Begin();
								}
							}
							if (wingsController.backWingsAnimationSettings.CanAddForce)
							{
								a = vector * verticalInput + right * horizontalInput;
							}
						}
					}
					else if (verticalInput > 0f)
					{
						if (horizontalInput < -0.8f)
						{
							if (!wingsController.leftWingsAnimationSettings.Running)
							{
								wingsAnimatorHandler.FwdGlide = false;
								if (num == 0f)
								{
									wingsController.DeactivateOther(wingsController.leftWingsAnimationSettings);
									wingsController.leftWingsAnimationSettings.Begin();
								}
							}
							if (wingsController.leftWingsAnimationSettings.CanAddForce)
							{
								a = forward * verticalInput + right * horizontalInput;
							}
						}
						else if (horizontalInput > 0.8f)
						{
							if (!wingsController.rightWingsAnimationSettings.Running)
							{
								wingsAnimatorHandler.FwdGlide = false;
								if (num == 0f)
								{
									wingsController.DeactivateOther(wingsController.rightWingsAnimationSettings);
									wingsController.rightWingsAnimationSettings.Begin();
								}
							}
							if (wingsController.rightWingsAnimationSettings.CanAddForce)
							{
								a = forward * verticalInput + right * horizontalInput;
							}
						}
						else
						{
							if (!wingsController.frontWingsAnimationSettings.Running)
							{
								wingsAnimatorHandler.FwdGlide = false;
								if (num == 0f)
								{
									wingsController.DeactivateOther(wingsController.frontWingsAnimationSettings);
									wingsController.frontWingsAnimationSettings.Begin();
								}
							}
							if (wingsController.frontWingsAnimationSettings.Glide)
							{
								if (num == 0f)
								{
									wingsAnimatorHandler.FwdGlide = true;
									a = 0.4f * forward * verticalInput + right * horizontalInput;
								}
							}
							else
							{
								wingsAnimatorHandler.FwdGlide = false;
								if (wingsController.frontWingsAnimationSettings.CanAddForce)
								{
									a = forward * verticalInput + right * horizontalInput;
								}
							}
						}
					}
					else
					{
						if (wingsController.frontWingsAnimationSettings.Running)
						{
							wingsAnimatorHandler.FwdGlide = false;
							wingsController.frontWingsAnimationSettings.End();
						}
						if (horizontalInput < 0f)
						{
							if (!wingsController.leftWingsAnimationSettings.Running)
							{
								wingsAnimatorHandler.FwdGlide = false;
								if (num == 0f)
								{
									wingsController.DeactivateOther(wingsController.leftWingsAnimationSettings);
									wingsController.leftWingsAnimationSettings.Begin();
								}
							}
							if (wingsController.leftWingsAnimationSettings.CanAddForce)
							{
								a = vector * verticalInput + right * horizontalInput;
							}
						}
						else if (horizontalInput > 0f)
						{
							if (!wingsController.rightWingsAnimationSettings.Running)
							{
								wingsAnimatorHandler.FwdGlide = false;
								if (num == 0f)
								{
									wingsController.DeactivateOther(wingsController.rightWingsAnimationSettings);
									wingsController.rightWingsAnimationSettings.Begin();
								}
							}
							if (wingsController.rightWingsAnimationSettings.CanAddForce)
							{
								a = vector * verticalInput + right * horizontalInput;
							}
						}
					}
				}
				Vector3 b = a * horizontalForce;
				if (flyStyle == FlyStyle.Wings)
				{
					if (!liftUpTimer.Running())
					{
						if ((bool)wingsController)
						{
							if (num > 0f)
							{
								if (wingsController.frontWingsAnimationSettings.Running)
								{
									wingsAnimatorHandler.FwdGlide = false;
									wingsController.frontWingsAnimationSettings.End();
								}
								if (!wingsController.upWingsAnimationSettings.Running)
								{
									wingsAnimatorHandler.FwdGlide = false;
									wingsController.DeactivateOther(wingsController.upWingsAnimationSettings);
									wingsController.upWingsAnimationSettings.Begin();
								}
								if (wingsController.upWingsAnimationSettings.CanAddForce)
								{
									_rigidbody.AddForce(wingsController.upWingsAnimationSettings.moveForce * num * Vector3.up + b);
								}
							}
							else if (num == 0f)
							{
								if (wingsController.upWingsAnimationSettings.Running)
								{
									wingsAnimatorHandler.FwdGlide = false;
									wingsController.DeactivateOther(wingsController.upWingsAnimationSettings);
								}
								_rigidbody.AddForce(downForce * num * Vector3.up + b);
							}
							else
							{
								wingsAnimatorHandler.FwdGlide = false;
								wingsController.DeactivateAll();
								_rigidbody.AddForce(downForce * num * Vector3.up + b);
							}
						}
					}
					else
					{
						_rigidbody.AddForce(upForce * num * Vector3.up + b);
					}
				}
				else if (num > 0f)
				{
					_rigidbody.AddForce(upForce * num * Vector3.up + b);
				}
				else
				{
					_rigidbody.AddForce(downForce * num * Vector3.up + b);
				}
				if (!characterRotator.FixingRotation)
				{
					Quaternion to = Quaternion.LookRotation(vector);
					base.transform.rotation = Quaternion.RotateTowards(base.transform.rotation, to, movingTurnSpeed);
				}
			}
			else if (flyStyle == FlyStyle.Wings && (bool)wingsController)
			{
				if (!wingsController.idleWingsAnimationSettings.Running)
				{
					wingsAnimatorHandler.FwdGlide = false;
					wingsController.DeactivateOther(wingsController.idleWingsAnimationSettings);
					wingsController.idleWingsAnimationSettings.Begin();
				}
				if (wingsController.idleWingsAnimationSettings.CanAddForce)
				{
					_rigidbody.AddForce(wingsController.idleWingsAnimationSettings.moveForce * Vector3.up);
				}
				else
				{
					_rigidbody.AddForce((0f - wingsController.idleWingsAnimationSettings.moveForce) * Vector3.up);
				}
			}
		}

		private void UpdateAnimatorAxes(float deltaTime = 0f)
		{
			int num = 2;
			smoothInput.x = MoveFloatTowards(smoothInput.x, horizontalInput, deltaTime * (float)num);
			smoothInput.y = MoveFloatTowards(smoothInput.y, verticalInput, deltaTime * (float)num);
			animatorHandler.FlyX = smoothInput.x;
			animatorHandler.FlyZ = smoothInput.y;
			animatorHandler.FlyY = MoveFloatTowards(animatorHandler.FlyY, UpDownInput, deltaTime * (float)num);
			if ((bool)wingsAnimatorHandler)
			{
				wingsAnimatorHandler.FlyX = smoothInput.x;
				wingsAnimatorHandler.FlyZ = smoothInput.y;
				wingsAnimatorHandler.FlyY = MoveFloatTowards(wingsAnimatorHandler.FlyY, UpDownInput, deltaTime * (float)num);
			}
		}

		private float MoveFloatTowards(float origin, float target, float ammount)
		{
			if (target > origin)
			{
				return Mathf.Min(origin + ammount, target);
			}
			if (target < origin)
			{
				return Mathf.Max(origin - ammount, target);
			}
			return origin;
		}
	}
}
