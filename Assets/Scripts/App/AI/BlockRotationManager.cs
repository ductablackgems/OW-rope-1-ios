using App.Player;

namespace App.AI
{
	public class BlockRotationManager : AbstractAIScript
	{
		private RigidbodyHelper rigidbodyHelper;

		private NavmeshWalker walker;

		private void Awake()
		{
			rigidbodyHelper = base.ComponentsRoot.GetComponentSafe<RigidbodyHelper>();
			walker = this.GetComponentSafe<NavmeshWalker>();
			rigidbodyHelper.OnKinematic += OnKinematic;
		}

		private void OnDestroy()
		{
			rigidbodyHelper.OnKinematic -= OnKinematic;
		}

		private void OnKinematic(bool kinematic)
		{
			walker.BlockRotationUpdate(kinematic);
		}
	}
}
