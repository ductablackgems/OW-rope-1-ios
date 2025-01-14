using App.Player.Definition;
using App.Util;
using App.Vehicles.Bicycle;
using UnityEngine;

namespace App.Vehicles.Skateboard
{
	public class PlayerSkateboardController : MonoBehaviour
	{
		public Transform rootBone;

		public WheelCollider wheel;

		public float motorPower = 60f;

		public float brakeTorque = 100f;

		public float maxSpeed = 15f;

		[Space]
		public float minRotationSpeed = 30f;

		public float maxRotationSpeed = 150f;

		public float worstSteerSpeed = 10f;

		public float jumpRotationSpeed = 100f;

		[Space]
		public float minJumpForce = 100f;

		public float maxJumpForce = 150f;

		public float vertJumpForce = 20f;

		public float fixVertAirSpeed = 10f;

		public float preventCrashDuration = 0.1f;

		public float durationBeforeFall = 0.5f;

		[Space]
		public float downForce = 500f;

		public float changeDirectionDelay = 0.1f;

		public float sideVelocityCoeff = 0.95f;

		public float maxSideVelocityDamp = 0.5f;

		public SkateboardRotationFix rotationFix;

		[Space]
		public float minBigAirSpeed = 2f;

		public float maxBigAirSpeed = 15f;

		public float bigAirRotationFixSpeed = 0.6f;

		private Rigidbody _rigidbody;

		private SkateboardAnimatorHandler skateboardAnimatorHandler;

		private PlayerAnimatorHandler playerAnimatorHandler;

		private RampTracker rampTracker;

		private StreetVehicleCrasher crasher;

		private Vector3 initialRootPosition;

		private Quaternion initialRootRotation;

		private float horizontalInput;

		private DurationTimer pushTimer = new DurationTimer();

		private DurationTimer fallTimer = new DurationTimer();

		private DurationTimer changeDirectionTimer = new DurationTimer();

		private Vector3 groundNormal;

		private Transform groundHelper;

		public bool InAir
		{
			get;
			private set;
		}

		public bool IsVertJumping
		{
			get;
			private set;
		}

		public int Direction
		{
			get;
			private set;
		}

		public Vector3 VertNormal
		{
			get;
			private set;
		}

		private void Awake()
		{
			_rigidbody = this.GetComponentSafe<Rigidbody>();
			skateboardAnimatorHandler = this.GetComponentSafe<SkateboardAnimatorHandler>();
			playerAnimatorHandler = ServiceLocator.GetGameObject("Player").GetComponentSafe<PlayerAnimatorHandler>();
			rampTracker = this.GetComponentSafe<RampTracker>();
			crasher = this.GetComponentSafe<StreetVehicleCrasher>();
			initialRootPosition = rootBone.localPosition;
			initialRootRotation = rootBone.localRotation;
			Direction = 1;
		}

		private void Start()
		{
			groundHelper = new GameObject("_GroundHelper_").transform;
		}

		private void OnDestroy()
		{
			if (groundHelper != null)
			{
				UnityEngine.Object.Destroy(groundHelper.gameObject);
			}
		}

		private void OnEnable()
		{
			skateboardAnimatorHandler.Animator.enabled = true;
			fallTimer.Run(1f);
			Direction = 1;
			groundNormal = Vector3.up;
		}

		private void OnDisable()
		{
			InAir = false;
			skateboardAnimatorHandler.InAir = false;
			skateboardAnimatorHandler.GroundedState.RunPrompt();
			skateboardAnimatorHandler.Animator.enabled = false;
			rootBone.localPosition = initialRootPosition;
			rootBone.localRotation = initialRootRotation;
			rampTracker.Clear();
			IsVertJumping = false;
			changeDirectionTimer.Stop();
		}

		private void Update()
		{
			UpdateHorizontalInput();
			float verticalInput = GetVerticalInput();
			Vector3 vector = groundHelper.InverseTransformDirection(_rigidbody.velocity);
			Vector3 vector2 = vector;
			vector2.y = 0f;
			float magnitude = vector2.magnitude;
			float absoluteZVelocity = Mathf.Abs(vector.z);
			float magnitude2 = _rigidbody.velocity.magnitude;
			UpdateDirection(vector, magnitude);
			UpdateWheel(verticalInput, absoluteZVelocity);
			bool justJumped = UpdateJumping(magnitude2);
			UpdatePushing(verticalInput, absoluteZVelocity);
			UpdateSteer(horizontalInput, magnitude2, justJumped);
			UpdateAnimators(verticalInput, vector, absoluteZVelocity);
		}

		private void FixedUpdate()
		{
			bool flag = wheel.isGrounded;
			if (flag && !playerAnimatorHandler.SkateboardJumpState.RunningNowOrNext && !playerAnimatorHandler.SkateboardFallState.RunningNext)
			{
				wheel.GetGroundHit(out WheelHit hit);
				if (hit.normal.y > 0.02f || hit.collider.CompareTag("LoopRamp"))
				{
					groundNormal = hit.normal;
					rampTracker.MarkHit(hit);
					fallTimer.Stop();
				}
				else
				{
					flag = false;
				}
			}
			else if (!fallTimer.Running())
			{
				fallTimer.Run(durationBeforeFall);
			}
			bool flag2 = !flag && rampTracker.RampTracked() && rampTracker.IsAboveRamp() && Vector3.Angle(Vector3.up, _rigidbody.velocity) < 80f && _rigidbody.velocity.y > 1f;
			InAir = (fallTimer.Done() | flag2);
			bool flag3 = playerAnimatorHandler.Animator.IsInTransition(0);
			bool flag4 = (flag3 && (playerAnimatorHandler.SkateboardGroundedState.RunningNext || playerAnimatorHandler.SkateboardPushState.RunningNext || playerAnimatorHandler.SkateboardLandState.RunningNext)) || (!flag3 && (playerAnimatorHandler.SkateboardGroundedState.Running || playerAnimatorHandler.SkateboardPushState.Running || playerAnimatorHandler.SkateboardLandState.Running));
			if (InAir)
			{
				UpdateRotation();
				if (IsVertJumping && rampTracker.IsAboveRamp())
				{
					base.transform.position = Vector3.MoveTowards(base.transform.position, rampTracker.GetVertAirPosition(), Time.fixedDeltaTime * fixVertAirSpeed);
				}
				if (!flag4)
				{
					return;
				}
				VertJumpData vertJumpData = flag2 ? rampTracker.ResolveJump() : default(VertJumpData);
				if (vertJumpData.isVertJump)
				{
					skateboardAnimatorHandler.JumpState.RunCrossFixed(0.15f);
					playerAnimatorHandler.SkateboardJumpState.RunCrossFixed(0.15f);
					IsVertJumping = true;
					VertNormal = vertJumpData.vertNormal;
					_rigidbody.velocity = _rigidbody.velocity.magnitude * vertJumpData.jumpDirection;
					if (!vertJumpData.isCorner)
					{
						float b = vertJumpForce;
						b = Mathf.Lerp(0f, b, vertJumpData.jumpDirection.y);
						_rigidbody.AddForce(vertJumpData.jumpDirection * b, ForceMode.Acceleration);
					}
				}
				else
				{
					playerAnimatorHandler.SkateboardFallState.RunCrossFixed(0.3f);
					skateboardAnimatorHandler.FallState.RunCrossFixed(0.3f);
					groundNormal = base.transform.up;
				}
				return;
			}
			IsVertJumping = false;
			UpdateRotation();
			if (flag)
			{
				Vector3 vector = groundHelper.InverseTransformDirection(_rigidbody.velocity);
				Vector3 vector2 = vector;
				vector2.y = 0f;
				if (vector2 != Vector3.zero)
				{
					float magnitude = vector2.magnitude;
					float x = vector2.x;
					vector2.x *= sideVelocityCoeff;
					if (Mathf.Abs(x - vector2.x) > maxSideVelocityDamp * Time.fixedDeltaTime)
					{
						vector2.x = x - maxSideVelocityDamp * (float)((!(x < 0f)) ? 1 : (-1));
					}
					vector2.z += Mathf.Abs(x - vector2.x) * (float)((!(vector2.z < 0f)) ? 1 : (-1));
					vector2 = vector2.normalized * magnitude;
					_rigidbody.velocity = groundHelper.TransformDirection(new Vector3(vector2.x, vector.y, vector2.z));
				}
			}
			if (flag4 && (flag || !rampTracker.RampTracked() || !rampTracker.IsAboveRamp()))
			{
				if (!flag && (playerAnimatorHandler.SkateboardLandState.RunningNowOrNext || playerAnimatorHandler.SkateboardFallState.RunningNowOrNext))
				{
					_rigidbody.AddForce(-groundNormal * downForce * 10f);
				}
				else
				{
					_rigidbody.AddForce(-groundNormal * downForce);
				}
			}
		}

		private void UpdateHorizontalInput()
		{
			bool num = ETCInput.GetButton("SteerRightButton") || UnityEngine.Input.GetAxis("Horizontal") > 0f;
			bool flag = ETCInput.GetButton("SteerLeftButton") || UnityEngine.Input.GetAxis("Horizontal") < 0f;
			float maxDelta = 5f * Time.deltaTime;
			if (num | flag)
			{
				if (flag)
				{
					horizontalInput = Mathf.MoveTowards(horizontalInput, -1f, maxDelta);
				}
				else
				{
					horizontalInput = Mathf.MoveTowards(horizontalInput, 1f, maxDelta);
				}
			}
			else
			{
				horizontalInput = Mathf.MoveTowards(horizontalInput, 0f, maxDelta);
			}
		}

		private float GetVerticalInput()
		{
			bool num = ETCInput.GetButton("VehicleForwardButton") || UnityEngine.Input.GetAxis("Vertical") > 0f;
			bool flag = ETCInput.GetButton("VehicleBackButton") || UnityEngine.Input.GetAxis("Vertical") < 0f;
			return num ? 1 : (flag ? (-1) : 0);
		}

		private void UpdateDirection(Vector3 localVelocity, float xzVelocityMagnitude)
		{
			if (InAir)
			{
				return;
			}
			if (Direction == 1 && localVelocity.z < 0f && xzVelocityMagnitude > 0.1f)
			{
				if (!changeDirectionTimer.InProgress())
				{
					Direction = -1;
					changeDirectionTimer.Run(changeDirectionDelay);
				}
			}
			else if (Direction == -1 && localVelocity.z > 0f && xzVelocityMagnitude > 0.1f && !changeDirectionTimer.InProgress())
			{
				Direction = 1;
				changeDirectionTimer.Run(changeDirectionDelay);
			}
		}

		private void UpdateWheel(float verticalInput, float absoluteZVelocity)
		{
			wheel.motorTorque = (((verticalInput > 0f && absoluteZVelocity > maxSpeed) || !wheel.isGrounded) ? 0f : (verticalInput * motorPower * (float)Direction));
			wheel.brakeTorque = ((verticalInput < 0f) ? brakeTorque : 0f);
		}

		private void UpdatePushing(float verticalInput, float absoluteZVelocity)
		{
			if (pushTimer.Running())
			{
				if (verticalInput <= 0f || absoluteZVelocity < 1f)
				{
					pushTimer.Stop();
				}
				else if (pushTimer.Done() && playerAnimatorHandler.SkateboardGroundedState.Running && !playerAnimatorHandler.Animator.IsInTransition(0))
				{
					if (UnityEngine.Random.Range(0f, 3f) < 1f)
					{
						pushTimer.Run(UnityEngine.Random.Range(0.9f, 1.1f));
					}
					else
					{
						pushTimer.Run(UnityEngine.Random.Range(2.5f, 4.5f));
					}
					playerAnimatorHandler.TriggerSkaterPush();
					skateboardAnimatorHandler.TriggerPush();
				}
			}
			else if (verticalInput > 0f && absoluteZVelocity >= 1f)
			{
				pushTimer.Run(1f);
			}
		}

		private bool UpdateJumping(float velocityMagnitude)
		{
			bool result = false;
			if (InputUtils.VehicleJump.IsDown && !InAir && (playerAnimatorHandler.SkateboardGroundedState.Running || playerAnimatorHandler.SkateboardPushState.Running))
			{
				VertJumpData vertJumpData = rampTracker.ResolveJump();
				result = true;
				skateboardAnimatorHandler.JumpState.RunCrossFixed(0.15f);
				playerAnimatorHandler.SkateboardJumpState.RunCrossFixed(0.15f);
				fallTimer.FakeDone(1f);
				InAir = true;
				if (vertJumpData.isVertJump)
				{
					crasher.PreventCrash(preventCrashDuration);
					IsVertJumping = true;
					VertNormal = vertJumpData.vertNormal;
					_rigidbody.velocity = _rigidbody.velocity.magnitude * vertJumpData.jumpDirection;
					if (!vertJumpData.isCorner)
					{
						float b = vertJumpForce;
						b = Mathf.Lerp(0f, b, vertJumpData.jumpDirection.y);
						_rigidbody.AddForce(vertJumpData.jumpDirection * b, ForceMode.Acceleration);
					}
				}
				else
				{
					groundNormal = base.transform.up;
					if (rampTracker.RampTracked())
					{
						crasher.PreventCrash(preventCrashDuration);
					}
					float d = Mathf.Lerp(minJumpForce, maxJumpForce, velocityMagnitude / maxSpeed);
					_rigidbody.AddForce(Vector3.Lerp(Vector3.up, base.transform.up, 0.6f) * d, ForceMode.Acceleration);
				}
			}
			return result;
		}

		private void UpdateSteer(float horizontalInput, float velocityMagnitude, bool justJumped)
		{
			if (horizontalInput != 0f)
			{
				float num = (!InAir && (!playerAnimatorHandler.SkateboardJumpState.RunningNowOrNext || justJumped)) ? Mathf.Lerp(maxRotationSpeed, minRotationSpeed, velocityMagnitude / worstSteerSpeed) : jumpRotationSpeed;
				float num2 = num * Time.deltaTime * horizontalInput;
				base.transform.Rotate(new Vector3(0f, num2, 0f));
				if (!InAir && (!playerAnimatorHandler.SkateboardJumpState.RunningNowOrNext || justJumped) && horizontalInput != 0f)
				{
					Quaternion rotation = Quaternion.AngleAxis(num2, base.transform.up);
					_rigidbody.velocity = rotation * _rigidbody.velocity;
				}
			}
		}

		private void UpdateAnimators(float verticalInput, Vector3 localVelocity, float absoluteZVelocity)
		{
			int num = (localVelocity.z >= 0f) ? 1 : (-1);
			if (absoluteZVelocity < 0.02f && verticalInput <= 0f)
			{
				num = 0;
			}
			skateboardAnimatorHandler.InAir = InAir;
			playerAnimatorHandler.SkaterInAir = InAir;
			skateboardAnimatorHandler.Direction.BlendTo(num);
			playerAnimatorHandler.SkaterDirection.BlendTo(num);
			playerAnimatorHandler.SkaterDirection.Update(Time.deltaTime);
			skateboardAnimatorHandler.Direction.Update(Time.deltaTime);
		}

		private void UpdateRotation()
		{
			if (IsVertJumping)
			{
				groundNormal = VertNormal;
				groundNormal.y = 0.3f;
				groundNormal.Normalize();
			}
			Vector3 vector = Vector3.zero;
			Vector3 axis = Vector3.zero;
			Vector3 vector2 = Vector3.zero;
			if (InAir && !IsVertJumping)
			{
				Vector3 velocity = _rigidbody.velocity;
				velocity.y = 0f;
				groundHelper.forward = velocity;
				vector = groundHelper.InverseTransformDirection(_rigidbody.velocity);
				axis = groundHelper.right;
				vector2 = groundHelper.InverseTransformDirection(groundNormal);
			}
			groundHelper.up = groundNormal;
			Vector3 direction = groundHelper.InverseTransformDirection(base.transform.forward);
			direction.y = 0f;
			Vector3 forward = groundHelper.TransformDirection(direction);
			groundHelper.rotation = Quaternion.LookRotation(forward, groundNormal);
			if (InAir && !IsVertJumping && vector2.z < 0f && vector.z > minBigAirSpeed)
			{
				Quaternion rotation = groundHelper.rotation;
				float num = Mathf.Lerp(0f, bigAirRotationFixSpeed, (vector.z - minBigAirSpeed) / (maxBigAirSpeed - minBigAirSpeed));
				groundHelper.RotateAround(axis, Time.fixedDeltaTime * num);
				groundNormal = groundHelper.up;
				groundHelper.rotation = rotation;
			}
			float angleDifference = Quaternion.Angle(groundHelper.rotation, base.transform.rotation);
			float num2 = (!playerAnimatorHandler.SkateboardLandState.RunningNowOrNext && (InAir || !playerAnimatorHandler.SkateboardFallState.RunningNowOrNext)) ? rotationFix.GetFixSpeed(angleDifference, _rigidbody.velocity.magnitude, !InAir && rampTracker.RampTracked() && !rampTracker.IsAboveRamp(), !InAir && (!rampTracker.RampTracked() || !rampTracker.IsAboveRamp())) : rotationFix.GetMaxSpeed();
			base.transform.rotation = Quaternion.RotateTowards(base.transform.rotation, groundHelper.rotation, Time.fixedDeltaTime * num2);
		}
	}
}
