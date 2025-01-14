using UnityEngine;

namespace App.Util
{
	public class AutoBlend
	{
		private int hash;

		private float speed;

		private Animator animator;

		public float TargetValue
		{
			get;
			private set;
		}

		public bool Done
		{
			get;
			private set;
		}

		public AutoBlend(int hash, float speed, Animator animator)
		{
			this.hash = hash;
			this.speed = speed;
			this.animator = animator;
			TargetValue = animator.GetFloat(hash);
			Done = true;
		}

		public void ForceValue(float value)
		{
			TargetValue = value;
			Done = true;
			animator.SetFloat(hash, value);
		}

		public void BlendTo(float value)
		{
			TargetValue = value;
			Done = false;
		}

		public void Update(float deltaTime)
		{
			if (Done)
			{
				return;
			}
			float @float = animator.GetFloat(hash);
			if (TargetValue > @float)
			{
				@float += deltaTime * speed;
				if (@float >= TargetValue)
				{
					@float = TargetValue;
					Done = true;
				}
			}
			else
			{
				@float -= deltaTime * speed;
				if (@float <= TargetValue)
				{
					@float = TargetValue;
					Done = true;
				}
			}
			animator.SetFloat(hash, @float);
		}
	}
}
