using UnityEngine;

namespace com.ootii.Timing
{
	public class TimeManager
	{
		public static float Relative60FPSDeltaTime;

		private static int mSampleCount;

		public static float _AverageDeltaTime;

		public static TimeManagerCore Core;

		private static float[] mSamples;

		private static int mSampleIndex;

		public static int SampleCount => mSampleCount;

		public static float AverageDeltaTime => _AverageDeltaTime;

		public static float SmoothedDeltaTime
		{
			get
			{
				if (Time.deltaTime <= _AverageDeltaTime)
				{
					return Time.deltaTime;
				}
				return _AverageDeltaTime;
			}
		}

		static TimeManager()
		{
			Relative60FPSDeltaTime = 1f;
			mSampleCount = 30;
			_AverageDeltaTime = Time.fixedDeltaTime;
			mSamples = new float[mSampleCount];
			mSampleIndex = 0;
			Core = Object.FindObjectOfType<TimeManagerCore>();
			if (Core == null)
			{
				Core = new GameObject("TimeManagerCore", typeof(TimeManagerCore))
				{
					hideFlags = HideFlags.HideInHierarchy
				}.GetComponent<TimeManagerCore>();
			}
		}

		public static void Initialize()
		{
			for (int i = 0; i < mSampleCount; i++)
			{
				mSamples[i] = Time.deltaTime;
			}
		}

		public static void Update()
		{
			Relative60FPSDeltaTime = Time.deltaTime / 0.01666f;
			mSamples[mSampleIndex++] = Time.deltaTime;
			if (mSampleIndex >= mSampleCount)
			{
				mSampleIndex = 0;
			}
			float num = 0f;
			for (int i = 0; i < mSampleCount; i++)
			{
				num += mSamples[i];
			}
			_AverageDeltaTime = num / (float)mSampleCount;
		}
	}
}
