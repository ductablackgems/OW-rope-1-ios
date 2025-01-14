namespace App
{
	public class FrameCounter
	{
		public int FetchIndex
		{
			get;
			set;
		}

		public int CurrentFrameNum
		{
			get;
			private set;
		}

		public FrameCounter(int fetchIndex, int currentFrameNum = 0)
		{
			FetchIndex = fetchIndex;
			CurrentFrameNum = currentFrameNum;
		}

		public void Reset()
		{
			CurrentFrameNum = 0;
		}

		public void PrepareToTrueFetch()
		{
			CurrentFrameNum = FetchIndex;
		}

		public bool Fetch()
		{
			if (FetchIndex <= CurrentFrameNum)
			{
				CurrentFrameNum = 0;
				return true;
			}
			CurrentFrameNum++;
			return false;
		}
	}
}
