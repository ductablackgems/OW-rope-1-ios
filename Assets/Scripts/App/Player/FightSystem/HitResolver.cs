using App.Player.FightSystem.Definition;
using System.Linq;
using UnityEngine;

namespace App.Player.FightSystem
{
	public class HitResolver : MonoBehaviour
	{
		public FightMovementDefinitions definitions;

		private FightMovementDefinition[,,] basicHits = new FightMovementDefinition[3, 3, 4];

		private bool initialized;

		public FightMovementDefinition ResolveHit(FightHitDefinition fightHitDefinition, Transform agressor, Transform victim)
		{
			if (!initialized)
			{
				Init();
			}
			Vector3 toDirection = agressor.position - victim.position;
			toDirection.y = 0f;
			Quaternion quaternion = Quaternion.FromToRotation(new Vector3(victim.forward.x, 0f, victim.forward.z), toDirection);
			if (fightHitDefinition.yAngleOffset != 0f)
			{
				quaternion = Quaternion.Euler(0f, quaternion.eulerAngles.y + fightHitDefinition.yAngleOffset, 0f);
			}
			float y = quaternion.eulerAngles.y;
			BasicHitSide basicHitSide = (fightHitDefinition.power == FightHitPower.Stagger && fightHitDefinition.height == FightHitHeight.High) ? ((!(y < 90f) && !(y > 270f)) ? BasicHitSide.Back : BasicHitSide.Front) : ((!(y < 45f) && !(y > 315f)) ? ((y < 135f) ? BasicHitSide.Right : ((y < 225f) ? BasicHitSide.Back : BasicHitSide.Left)) : BasicHitSide.Front);
			return basicHits[(int)fightHitDefinition.height, (int)fightHitDefinition.power, (int)basicHitSide];
		}

		public FightMovementDefinition ResolveCustomMovementDefinition(string tid)
		{
			return definitions.customHitDefinitions.First((FightMovementDefinition hit) => hit.tid.Equals(tid));
		}

		private void Awake()
		{
			if (!initialized)
			{
				Init();
			}
		}

		private void Init()
		{
			initialized = true;
			InitBasicHit("HitHighFrontWeak", FightHitHeight.High, FightHitPower.Weak, BasicHitSide.Front);
			InitBasicHit("HitHighBackWeak", FightHitHeight.High, FightHitPower.Weak, BasicHitSide.Back);
			InitBasicHit("HitHighLeftWeak", FightHitHeight.High, FightHitPower.Weak, BasicHitSide.Left);
			InitBasicHit("HitHighRightWeak", FightHitHeight.High, FightHitPower.Weak, BasicHitSide.Right);
			InitBasicHit("HitMidFrontWeak", FightHitHeight.Mid, FightHitPower.Weak, BasicHitSide.Front);
			InitBasicHit("HitMidBackWeak", FightHitHeight.Mid, FightHitPower.Weak, BasicHitSide.Back);
			InitBasicHit("HitMidLeftWeak", FightHitHeight.Mid, FightHitPower.Weak, BasicHitSide.Left);
			InitBasicHit("HitMidRightWeak", FightHitHeight.Mid, FightHitPower.Weak, BasicHitSide.Right);
			InitBasicHit("HitHighFrontStagger", FightHitHeight.High, FightHitPower.Stagger, BasicHitSide.Front);
			InitBasicHit("HitHighBackStagger", FightHitHeight.High, FightHitPower.Stagger, BasicHitSide.Back);
			InitBasicHit("HitMidFrontStagger", FightHitHeight.Mid, FightHitPower.Stagger, BasicHitSide.Front);
			InitBasicHit("HitMidBackStagger", FightHitHeight.Mid, FightHitPower.Stagger, BasicHitSide.Back);
			InitBasicHit("HitMidLeftStagger", FightHitHeight.Mid, FightHitPower.Stagger, BasicHitSide.Left);
			InitBasicHit("HitMidRightStagger", FightHitHeight.Mid, FightHitPower.Stagger, BasicHitSide.Right);
			InitBasicHit("HitMidFrontMedium", FightHitHeight.Mid, FightHitPower.Medium, BasicHitSide.Front);
			InitBasicHit("HitMidBackMedium", FightHitHeight.Mid, FightHitPower.Medium, BasicHitSide.Back);
			InitBasicHit("HitMidLeftMedium", FightHitHeight.Mid, FightHitPower.Medium, BasicHitSide.Left);
			InitBasicHit("HitMidRightMedium", FightHitHeight.Mid, FightHitPower.Medium, BasicHitSide.Right);
		}

		private void InitBasicHit(string tid, FightHitHeight height, FightHitPower power, BasicHitSide side)
		{
			basicHits[(int)height, (int)power, (int)side] = FindHitByTid(tid);
		}

		private FightMovementDefinition FindHitByTid(string tid)
		{
			return definitions.hitDefinitions.First((FightMovementDefinition hit) => hit.tid.Equals(tid));
		}
	}
}
