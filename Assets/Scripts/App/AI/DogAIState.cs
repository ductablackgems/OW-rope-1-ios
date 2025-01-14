using App.Dogs;
using App.Settings;

namespace App.AI
{
	public abstract class DogAIState : AIState
	{
		protected Dog Dog
		{
			get;
			private set;
		}

		protected DogSettingsItem Settings
		{
			get;
			private set;
		}

		public DogAIState(IAIEntity entity)
			: base(entity)
		{
			Dog = (entity as DogAIController).Dog;
			Settings = Dog.Settings;
		}
	}
}
