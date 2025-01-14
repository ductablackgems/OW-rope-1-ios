using com.ootii.Collections;

namespace com.ootii.Utilities.Debug
{
	public class LogText
	{
		public string Text;

		public int X;

		public int Y;

		private static ObjectPool<LogText> sPool = new ObjectPool<LogText>(20, 5);

		public static int Length => sPool.Length;

		public static LogText Allocate()
		{
			LogText logText = sPool.Allocate();
			logText.Text = "";
			logText.X = 0;
			logText.Y = 0;
			return logText;
		}

		public static LogText Allocate(string rText, int rX, int rY)
		{
			LogText logText = sPool.Allocate();
			logText.Text = rText;
			logText.X = rX;
			logText.Y = rY;
			return logText;
		}

		public static void Release(LogText rInstance)
		{
			if (rInstance != null)
			{
				sPool.Release(rInstance);
			}
		}
	}
}
