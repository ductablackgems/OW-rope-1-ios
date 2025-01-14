using com.ootii.Utilities.Debug;
using System.Collections.Generic;
using System.Diagnostics;

namespace com.ootii.Utilities
{
	public class Profiler
	{
		public string Tag = "";

		private string mSpacing = "";

		private int mCount;

		private float mRunTime;

		private float mTotalTime;

		private float mMinTime;

		private float mMaxTime;

		private Stopwatch mTimer = new Stopwatch();

		private float mTicksPerMillisecond;

		private static Dictionary<string, Profiler> sProfilers = new Dictionary<string, Profiler>();

		public float AverageTime
		{
			get
			{
				if (mCount == 0)
				{
					return 0f;
				}
				return mTotalTime / (float)mCount;
			}
		}

		public float MinTime => mMinTime;

		public float MaxTime => mMaxTime;

		public float TotalTime => mTotalTime;

		public float Time => mRunTime;

		public float ElapsedTime
		{
			get
			{
				if (mTimer.IsRunning)
				{
					return (float)mTimer.ElapsedTicks / mTicksPerMillisecond;
				}
				return mRunTime;
			}
		}

		public int Count => mCount;

		public Profiler(string rTag)
		{
			Tag = rTag;
			mTicksPerMillisecond = 10000f;
			mMinTime = 2.14748365E+09f;
			mMaxTime = -2.14748365E+09f;
		}

		public Profiler(string rTag, string rSpacing)
		{
			Tag = rTag;
			mSpacing = rSpacing;
			mTicksPerMillisecond = 10000f;
			mMinTime = 2.14748365E+09f;
			mMaxTime = -2.14748365E+09f;
		}

		public void Reset()
		{
			mCount = 0;
			mRunTime = 0f;
			mTotalTime = 0f;
			mMinTime = 0f;
			mMaxTime = 0f;
		}

		public void Start()
		{
			mTimer.Reset();
			mTimer.Start();
		}

		public float Stop()
		{
			mTimer.Stop();
			mRunTime = (float)mTimer.ElapsedTicks / mTicksPerMillisecond;
			mTotalTime += mRunTime;
			if (mMinTime == 0f || mRunTime < mMinTime)
			{
				mMinTime = mRunTime;
			}
			if (mMaxTime == 0f || mRunTime > mMaxTime)
			{
				mMaxTime = mRunTime;
			}
			mCount++;
			return mRunTime;
		}

		public override string ToString()
		{
			return $"{mSpacing} {Tag} - time:{mRunTime:f4}ms cnt:{mCount} avg:{AverageTime:f4}ms min:{mMinTime:f4}ms max:{mMaxTime:f4}ms";
		}

		public static Profiler Start(string rProfiler)
		{
			if (!sProfilers.ContainsKey(rProfiler))
			{
				sProfilers.Add(rProfiler, new Profiler(rProfiler, ""));
			}
			sProfilers[rProfiler].Start();
			return sProfilers[rProfiler];
		}

		public static Profiler Start(string rProfiler, string rSpacing)
		{
			if (!sProfilers.ContainsKey(rProfiler))
			{
				sProfilers.Add(rProfiler, new Profiler(rProfiler, rSpacing));
			}
			sProfilers[rProfiler].Start();
			return sProfilers[rProfiler];
		}

		public static float Stop(string rProfiler)
		{
			if (!sProfilers.ContainsKey(rProfiler))
			{
				return 0f;
			}
			return sProfilers[rProfiler].Stop();
		}

		public static float ProfilerTime(string rProfiler)
		{
			if (!sProfilers.ContainsKey(rProfiler))
			{
				return 0f;
			}
			return sProfilers[rProfiler].ElapsedTime;
		}

		public static string ToString(string rProfiler)
		{
			if (rProfiler.Length == 0)
			{
				float num = 0f;
				float num2 = 0f;
				foreach (Profiler value in sProfilers.Values)
				{
					num += value.Time;
					num2 += value.AverageTime;
				}
				string text = $"Profiles - Time:{num:f4}ms Avg:{num2:f4}ms\r\n";
				{
					foreach (Profiler value2 in sProfilers.Values)
					{
						text += $"{value2.ToString()} Prc:{value2.Time / num:f3} AvgPrc:{value2.AverageTime / num2:f3}\r\n";
					}
					return text;
				}
			}
			if (!sProfilers.ContainsKey(rProfiler))
			{
				return "";
			}
			return sProfilers[rProfiler].ToString();
		}

		public static void ScreenWrite(string rProfiler, int rLine)
		{
			if (rProfiler.Length == 0)
			{
				float num = 0f;
				float num2 = 0f;
				foreach (Profiler value in sProfilers.Values)
				{
					num += value.Time;
					num2 += value.AverageTime;
				}
				int num3 = 0;
				foreach (Profiler value2 in sProfilers.Values)
				{
					Log.ScreenWrite($"{value2.ToString()} Prc:{value2.Time / num:f3} AvgPrc:{value2.AverageTime / num2:f3}\r\n", rLine + num3);
					num3++;
				}
			}
			else if (sProfilers.ContainsKey(rProfiler))
			{
				Log.ScreenWrite(sProfilers[rProfiler].ToString(), rLine);
			}
		}
	}
}
