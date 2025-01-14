using UnityEngine;

namespace App.Util
{
	public class AnimatorState
	{
		private int hash;

		private Animator animator;

		private int layerIndex;

		public bool Running => animator.GetCurrentAnimatorStateInfo(layerIndex).fullPathHash == hash;

		public bool RunningNext => animator.GetNextAnimatorStateInfo(layerIndex).fullPathHash == hash;

		public bool RunningNowOrNext
		{
			get
			{
				if (!Running)
				{
					return RunningNext;
				}
				return true;
			}
		}

		public AnimatorState(string name, Animator animator, int layerIndex = 0)
		{
			hash = Animator.StringToHash(name);
			this.animator = animator;
			this.layerIndex = layerIndex;
		}

		public void RunPrompt()
		{
			animator.Play(hash);
		}

		public void RunCross(float transitionDuration)
		{
			animator.CrossFade(hash, transitionDuration);
		}

		public void RunCrossFixed(float transitionDuration)
		{
			animator.CrossFadeInFixedTime(hash, transitionDuration);
		}
	}
}
