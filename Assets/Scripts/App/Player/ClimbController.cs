using App.Player.Climbing;
using App.Player.Definition;
using System;
using UnityEngine;

namespace App.Player
{
	[RequireComponent(typeof(PlayerAnimatorHandler))]
	public class ClimbController : MonoBehaviour, ICharacterModule
	{
		public float moveSpeed = 1f;

		public float forwardForce = 1000f;

		public float rotationSpeed;

		public Transform bodyCenter;

		public Transform frontRotationPoint;

		public Transform backRotationPoint;

		public Transform standUpRotationPoint;

		[Space]
		public float colliderShrinkSpeed = 1f;

		public float colliderFlipGrowSpeed = 1f;

		public float colliderStandUpSpeed = 1f;

		public float colliderCommonGrowSpeed = 1f;

		private PlayerAnimatorHandler animatorHandler;

		private CharacterControl characterControl;

		private CharacterRotator characterRotator;

		private RopeController ropeController;

		private Rigidbody _rigidbody;

		private ClimbRaycasts raycasts;

		private ClimbColliderResizer climbColliderResizer;

		private TriggerMonitor wallTrigger = new TriggerMonitor();

		private Vector2 lastInput;

		private DurationTimer forcedRunTimer = new DurationTimer();

		private DurationTimer standUpTimer = new DurationTimer();

		private DurationTimer cacheNormalTimer = new DurationTimer();

		private DurationTimer recentInputTimer = new DurationTimer();

		private Vector3 cachedNormal;

		private bool fastStop;

		private bool notClimb;

		private Vector3 lastKnownFrontNormal;

		public bool Runnable()
		{
			return !notClimb;
		}

		public void RunForced(RaycastHit hit)
		{
			if (!notClimb)
			{
				forcedRunTimer.Run(0.65f);
				Run();
			}
		}

		public void Run()
		{
			if (!notClimb)
			{
				lastKnownFrontNormal = Vector3.zero;
				climbColliderResizer.Shrink(colliderShrinkSpeed);
				animatorHandler.Climb = true;
				_rigidbody.useGravity = false;
				_rigidbody.velocity = Vector3.zero;
				lastInput = Vector2.zero;
			}
		}

		public bool Running()
		{
			return animatorHandler.Climb;
		}

		public void FastStop()
		{
			if (Running())
			{
				fastStop = true;
				Stop();
			}
		}

		public void Stop()
		{
			if (!Running())
			{
				return;
			}
			standUpTimer.Stop();
			animatorHandler.Climb = false;
			animatorHandler.ClimbMove = false;
			animatorHandler.FastClimbExit = fastStop;
			_rigidbody.useGravity = true;
			forcedRunTimer.Stop();
			fastStop = false;
			bool flag = false;
			if (raycasts.topRaycast.Hit() && Vector3.Angle(Vector3.up, raycasts.topRaycast.GetRaycastHit().normal) < 45f)
			{
				flag = true;
			}
			if (!flag && raycasts.bottomRaycast.Hit() && Vector3.Angle(Vector3.up, raycasts.bottomRaycast.GetRaycastHit().normal) < 45f)
			{
				flag = true;
			}
			if (!flag)
			{
				Vector3 lastKnownFrontNormal2 = lastKnownFrontNormal;
				if (Vector3.Angle(Vector3.up, lastKnownFrontNormal) < 45f)
				{
					flag = true;
				}
			}
			if (flag)
			{
				climbColliderResizer.Grow(colliderStandUpSpeed);
				animatorHandler.ClimbExitBlend = 0f;
				float y = (base.transform.up.y >= 0f) ? base.transform.rotation.eulerAngles.y : (base.transform.rotation.eulerAngles.y + 180f);
				characterRotator.StandUp(Quaternion.Euler(0f, y, 0f), standUpRotationPoint);
			}
			else if (raycasts.headRaycast.Hit() && !raycasts.headRaycast.HitClimbable())
			{
				climbColliderResizer.Grow(colliderFlipGrowSpeed);
				animatorHandler.ClimbExitBlend = 2f;
				characterRotator.Flip(bodyCenter);
			}
			else
			{
				climbColliderResizer.Grow(colliderCommonGrowSpeed);
				animatorHandler.ClimbExitBlend = 1f;
				float y2 = (base.transform.up.y >= 0f) ? base.transform.rotation.eulerAngles.y : (base.transform.rotation.eulerAngles.y + 180f);
				characterRotator.CommonRotate(Quaternion.Euler(0f, y2, 0f), bodyCenter);
			}
		}

		public bool HittingWall()
		{
			if (!raycasts.topRaycast.HitClimbable())
			{
				return raycasts.bottomRaycast.HitClimbable();
			}
			return true;
		}

		private void Awake()
		{
			animatorHandler = this.GetComponentSafe<PlayerAnimatorHandler>();
			characterControl = this.GetComponentSafe<CharacterControl>();
			characterRotator = this.GetComponentSafe<CharacterRotator>();
			ropeController = GetComponent<RopeController>();
			_rigidbody = this.GetComponentSafe<Rigidbody>();
			raycasts = this.GetComponentInChildrenSafe<ClimbRaycasts>();
			CapsuleCollider componentSafe = this.GetComponentSafe<CapsuleCollider>();
			climbColliderResizer = new ClimbColliderResizer(componentSafe, componentSafe.height, 1.58f);
		}

		public void Control(float horizontalAxis, float verticalAxis)
		{
			wallTrigger.FixedUpdate();
			climbColliderResizer.Update(Time.fixedDeltaTime);
			if (notClimb)
			{
				Stop();
			}
			else
			{
				if (!animatorHandler.Climb)
				{
					return;
				}
				if (standUpTimer.Done())
				{
					standUpTimer.Stop();
					Stop();
					return;
				}
				if (horizontalAxis != 0f || verticalAxis != 0f)
				{
					lastInput = new Vector2(horizontalAxis, verticalAxis);
					recentInputTimer.Run(1.2f);
				}
				if (!animatorHandler.ClimbMachine.RunningNowOrNext)
				{
					return;
				}
				UpdateLastKnowNormal();
				float magnitude = new Vector2(horizontalAxis, verticalAxis).magnitude;
				animatorHandler.ClimbMove = (magnitude > 0.1f);
				Vector3 vector = CountNormal();
				float num = Vector3.Angle(Vector3.up, vector);
				if ((num < 10f || num > 170f || vector == Vector3.zero) && !standUpTimer.Running())
				{
					standUpTimer.Run((num > 170f) ? 0.55f : 0.3f);
				}
				else if (vector != Vector3.zero && num >= 10f && num <= 170f && standUpTimer.Running())
				{
					standUpTimer.Stop();
				}
				if (ropeController == null || !ropeController.FixingRotation)
				{
					if (raycasts.headRaycast.Hit() && !raycasts.headRaycast.HitClimbable())
					{
						Stop();
						return;
					}
					if (cacheNormalTimer.InProgress())
					{
						bool towardEdge = raycasts.headRaycast.Hit();
						ProcessRotation(vector, towardEdge);
					}
					_rigidbody.velocity = base.transform.up * moveSpeed * magnitude;
					if (!wallTrigger.IsTriggered() || recentInputTimer.InProgress() || forcedRunTimer.InProgress())
					{
						_rigidbody.AddForce(base.transform.forward * forwardForce);
					}
				}
				if (!forcedRunTimer.InProgress() && !HittingWall() && cacheNormalTimer.GetProgress() > 0.7f)
				{
					Stop();
				}
			}
		}

		private void UpdateLastKnowNormal()
		{
			if (raycasts.topRaycast.Hit() && raycasts.bottomRaycast.Hit())
			{
				float num = Vector3.Angle(Vector3.up, raycasts.topRaycast.GetRaycastHit().normal);
				float num2 = Vector3.Angle(Vector3.up, raycasts.bottomRaycast.GetRaycastHit().normal);
				lastKnownFrontNormal = ((num < num2) ? raycasts.topRaycast.GetRaycastHit().normal : raycasts.bottomRaycast.GetRaycastHit().normal);
			}
			else if (raycasts.topRaycast.Hit())
			{
				lastKnownFrontNormal = raycasts.topRaycast.GetRaycastHit().normal;
			}
			else if (raycasts.bottomRaycast.Hit())
			{
				lastKnownFrontNormal = raycasts.bottomRaycast.GetRaycastHit().normal;
			}
		}

		private bool ProcessRotation(Vector3 normal, bool towardEdge)
		{
			Vector3 b = towardEdge ? backRotationPoint.position : frontRotationPoint.position;
			Quaternion rotation = base.transform.rotation;
			base.transform.forward = new Vector3(0f - normal.x, 0f - normal.y, 0f - normal.z);
			if (lastInput != Vector2.zero)
			{
				float num = Vector2.Angle(Vector2.up, lastInput) * ((float)Math.PI / 180f);
				if (lastInput.x > 0f)
				{
					num *= -1f;
				}
				base.transform.RotateAround(base.transform.forward, num);
			}
			float num2 = Quaternion.Angle(rotation, base.transform.rotation);
			base.transform.rotation = Quaternion.RotateTowards(rotation, base.transform.rotation, Time.fixedDeltaTime * rotationSpeed);
			if (towardEdge)
			{
				base.transform.position = base.transform.position + b - backRotationPoint.position;
			}
			else
			{
				base.transform.position = base.transform.position + b - frontRotationPoint.position;
			}
			return num2 > 0f;
		}

		private Vector3 CountNormal()
		{
			bool num = raycasts.headRaycast.HitClimbable();
			bool flag = raycasts.topRaycast.HitClimbable();
			bool flag2 = raycasts.bottomRaycast.HitClimbable();
			bool flag3 = raycasts.topEdgeRaycast.HitClimbable();
			Vector3 vector;
			if (num)
			{
				vector = raycasts.headRaycast.GetRaycastHit().normal;
			}
			else if (flag && flag2)
			{
				vector = Vector3.Lerp(raycasts.topRaycast.GetRaycastHit().normal, raycasts.bottomRaycast.GetRaycastHit().normal, 0.5f);
			}
			else if (flag)
			{
				vector = raycasts.topRaycast.GetRaycastHit().normal;
			}
			else if (flag3 && flag2)
			{
				vector = Vector3.Lerp(raycasts.topEdgeRaycast.GetRaycastHit().normal, raycasts.bottomRaycast.GetRaycastHit().normal, 0.5f);
			}
			else if (flag3)
			{
				vector = raycasts.topEdgeRaycast.GetRaycastHit().normal;
			}
			else
			{
				if (!flag2)
				{
					if (cacheNormalTimer.InProgress())
					{
						return cachedNormal;
					}
					return Vector3.zero;
				}
				vector = raycasts.bottomRaycast.GetRaycastHit().normal;
			}
			if (Vector3.Angle(vector, Vector3.up) < 2f)
			{
				if (cacheNormalTimer.InProgress())
				{
					return cachedNormal * 0.05f + Vector3.up;
				}
				return Vector3.zero;
			}
			if (Vector3.Angle(vector, Vector3.down) < 2f)
			{
				if (cacheNormalTimer.InProgress())
				{
					return cachedNormal * 0.05f + Vector3.down;
				}
				return Vector3.zero;
			}
			CacheNormal(vector);
			return vector;
		}

		private void CacheNormal(Vector3 normal)
		{
			cachedNormal = normal;
			cacheNormalTimer.Run(0.5f);
		}

		private bool IsOnEdge()
		{
			if (raycasts.topRaycast.HitClimbable())
			{
				return false;
			}
			return raycasts.topEdgeRaycast.HitClimbable();
		}

		private void OnCollisionStay(Collision collision)
		{
			if (collision.gameObject.layer == 13)
			{
				wallTrigger.MarkTrigger(collision.transform);
			}
		}

		private void OnTriggerStay(Collider other)
		{
			if (other.CompareTag("notClimb"))
			{
				notClimb = true;
			}
		}

		private void OnTriggerEnter(Collider other)
		{
			if (other.CompareTag("notClimb"))
			{
				notClimb = true;
			}
		}

		private void OnTriggerExit(Collider other)
		{
			if (other.CompareTag("notClimb"))
			{
				notClimb = false;
			}
		}
	}
}
