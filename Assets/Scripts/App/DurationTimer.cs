using UnityEngine;

namespace App
{
	public class DurationTimer
	{
		private bool useFixedTime;

		private float doneAt = -1f;

		private float startedAt;

		private float duration;

		public DurationTimer(bool useFixedTime = false)
		{
			this.useFixedTime = useFixedTime;
		}

		public void Run(float duration)
		{
			float time = GetTime();
			doneAt = time + duration;
			startedAt = time;
			this.duration = duration;
		}

		public bool Done()
		{
			float time = GetTime();
			if (Running())
			{
				return doneAt <= time;
			}
			return false;
		}

		public bool Done(float progress)
		{
			float time = GetTime();
			if (Running())
			{
				return startedAt + progress * duration <= time;
			}
			return false;
		}

		public void FakeDone(float fakeDuration)
		{
			duration = fakeDuration;
			startedAt = Time.time - fakeDuration;
			doneAt = Time.time;
		}

		public bool Running()
		{
			return doneAt >= 0f;
		}

		public bool InProgress()
		{
			float time = GetTime();
			if (Running())
			{
				return doneAt > time;
			}
			return false;
		}

		public void Stop()
		{
			doneAt = -1f;
		}

		public float GetProgress()
		{
			if (!Running())
			{
				return 0f;
			}
			float time = GetTime();
			if (doneAt < time)
			{
				return 1f;
			}
			return (time - startedAt) / duration;
		}

		public float GetEndTime()
		{
			return startedAt + duration;
		}

		private float GetTime()
		{
			if (!useFixedTime)
			{
				return Time.time;
			}
			return Time.fixedTime;
		}
	}
}
