namespace App.Util
{
	public class AnimatorMachine
	{
		private AnimatorState[] states;

		public bool Running
		{
			get
			{
				AnimatorState[] array = states;
				for (int i = 0; i < array.Length; i++)
				{
					if (array[i].Running)
					{
						return true;
					}
				}
				return false;
			}
		}

		public bool RunningNext
		{
			get
			{
				AnimatorState[] array = states;
				for (int i = 0; i < array.Length; i++)
				{
					if (array[i].RunningNext)
					{
						return true;
					}
				}
				return false;
			}
		}

		public bool RunningNowOrNext
		{
			get
			{
				AnimatorState[] array = states;
				for (int i = 0; i < array.Length; i++)
				{
					if (array[i].RunningNowOrNext)
					{
						return true;
					}
				}
				return false;
			}
		}

		public AnimatorMachine(params AnimatorState[] states)
		{
			this.states = states;
		}
	}
}
