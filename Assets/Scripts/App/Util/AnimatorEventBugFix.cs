namespace App.Util
{
	public class AnimatorEventBugFix
	{
		private bool[] eventFired;

		public AnimatorEventBugFix(int size)
		{
			eventFired = new bool[size];
		}

		public void Clear()
		{
			for (int i = 0; i < eventFired.Length; i++)
			{
				eventFired[i] = false;
			}
		}

		public bool SetFiredAndTest(int eventIndex)
		{
			if (eventFired[eventIndex])
			{
				return false;
			}
			eventFired[eventIndex] = true;
			return true;
		}
	}
}
