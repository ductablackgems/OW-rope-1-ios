using UnityEngine;

namespace App.Player.FightSystem
{
	public class AnimationSimulator : MonoBehaviour
	{
		public Animator animator;

		public Animator victimAnimator;

		public Transform cameraAnchor;

		[Space]
		public int debugIndex = -1;

		public FightMovementDefinitions definitions;

		public Vector3 capsuleStart;

		public Vector3 capsuleEnd;

		public float capsuleRadius = 1f;

		private int stopAnimatorHelper;

		private int stopVictimAnimatorHelper;

		private int tposeHash = Animator.StringToHash("Base Layer.tpose");

		public void RunSimulation(int stateHash)
		{
			animator.gameObject.SetActive(value: true);
			animator.Play(stateHash);
			stopAnimatorHelper = 0;
		}

		public void RunVictimSimulation(int victimStateHash)
		{
			victimAnimator.gameObject.SetActive(value: true);
			victimAnimator.Play(victimStateHash);
			stopVictimAnimatorHelper = 0;
		}

		public void StopSimulation()
		{
			animator.Play(tposeHash);
			stopAnimatorHelper = 2;
		}

		public void StopVictimSimulation()
		{
			victimAnimator.Play(tposeHash);
			stopVictimAnimatorHelper = 2;
		}

		private void Awake()
		{
			animator.Play(tposeHash);
			victimAnimator.Play(tposeHash);
			stopAnimatorHelper = 2;
			stopVictimAnimatorHelper = 2;
		}

		private void Update()
		{
			if (stopAnimatorHelper > 0)
			{
				stopAnimatorHelper--;
				if (stopAnimatorHelper == 0)
				{
					animator.gameObject.SetActive(value: false);
				}
			}
			if (stopVictimAnimatorHelper > 0)
			{
				stopVictimAnimatorHelper--;
				if (stopVictimAnimatorHelper == 0)
				{
					victimAnimator.gameObject.SetActive(value: false);
				}
			}
		}

		private void OnDrawGizmos()
		{
			Vector3 start = cameraAnchor.position + cameraAnchor.TransformDirection(capsuleStart);
			Vector3 end = cameraAnchor.position + cameraAnchor.TransformDirection(capsuleEnd);
			Debug2.DrawCapsule(start, end, Color.blue, capsuleRadius);
		}
	}
}
