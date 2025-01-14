using App;
using App.Player.FightSystem;
using UnityEngine;

public class MagicController : MonoBehaviour
{
	private AdvancedFightController advancedFightController;

	private DurationTimer checkMagicDelayTimer = new DurationTimer();

	private bool action;

	private void Awake()
	{
		advancedFightController = GetComponent<AdvancedFightController>();
	}

	private void Update()
	{
		if (action && checkMagicDelayTimer.Done())
		{
			StopMagicAction();
		}
	}

	public bool MagicRun()
	{
		return action;
	}

	public void Run()
	{
		StartMagicAction();
	}

	private void StartMagicAction()
	{
		action = true;
		advancedFightController.Run();
		advancedFightController.Control(attackPressed: true, attackPressedDown: true, isMagic: true);
		checkMagicDelayTimer.Run(2.8f);
	}

	private void StopMagicAction()
	{
		action = false;
		checkMagicDelayTimer.Stop();
	}
}
