using UnityEngine;

namespace App.Player.Climbing
{
	public class ClimbColliderResizer
	{
		private CapsuleCollider collider;

		private float defaultHeight;

		private float shrinkedHeight;

		private ColliderResizerState state;

		private float speed;

		public ClimbColliderResizer(CapsuleCollider collider, float defaultHeight, float shrinkedHeight)
		{
			this.collider = collider;
			this.defaultHeight = defaultHeight;
			this.shrinkedHeight = shrinkedHeight;
			state = ColliderResizerState.Default;
		}

		public void Grow(float speed)
		{
			state = ColliderResizerState.Growing;
			this.speed = speed;
		}

		public void Shrink(float speed)
		{
			state = ColliderResizerState.Shrinking;
			this.speed = speed;
		}

		public void Update(float deltaTime)
		{
			if (state != 0 && state != ColliderResizerState.Shrinked)
			{
				float num = (state == ColliderResizerState.Growing) ? defaultHeight : shrinkedHeight;
				collider.height = Mathf.MoveTowards(collider.height, num, deltaTime * speed);
				if (collider.height == num)
				{
					state = ((state != ColliderResizerState.Growing) ? ColliderResizerState.Shrinked : ColliderResizerState.Default);
				}
			}
		}
	}
}
