using System;
using UnityEngine;

namespace Forge3D
{
	public class F3DFXController : MonoBehaviour
	{
		public static F3DFXController instance;

		private string[] fxTypeName = new string[12]
		{
			"Vulcan",
			"Sologun",
			"Sniper",
			"Shotgun",
			"Seeker",
			"Railgun",
			"Plasmagun",
			"Plasma beam",
			"Heavy plasma beam",
			"Lightning gun",
			"Flamethrower",
			"Pulse laser"
		};

		private int curSocket;

		private int timerID = -1;

		[Header("Turret setup")]
		public Transform[] TurretSocket;

		public ParticleSystem[] ShellParticles;

		public F3DFXType DefaultFXType;

		[Header("Vulcan")]
		public Transform vulcanProjectile;

		public Transform vulcanMuzzle;

		public Transform vulcanImpact;

		public float vulcanOffset;

		[Header("Solo gun")]
		public Transform soloGunProjectile;

		public Transform soloGunMuzzle;

		public Transform soloGunImpact;

		public float soloGunOffset;

		[Header("Sniper")]
		public Transform sniperBeam;

		public Transform sniperMuzzle;

		public Transform sniperImpact;

		public float sniperOffset;

		[Header("Shotgun")]
		public Transform shotGunProjectile;

		public Transform shotGunMuzzle;

		public Transform shotGunImpact;

		public float shotGunOffset;

		[Header("Seeker")]
		public Transform seekerProjectile;

		public Transform seekerMuzzle;

		public Transform seekerImpact;

		public float seekerOffset;

		[Header("Rail gun")]
		public Transform railgunBeam;

		public Transform railgunMuzzle;

		public Transform railgunImpact;

		public float railgunOffset;

		[Header("Plasma gun")]
		public Transform plasmagunProjectile;

		public Transform plasmagunMuzzle;

		public Transform plasmagunImpact;

		public float plasmaGunOffset;

		[Header("Plasma beam")]
		public Transform plasmaBeam;

		public float plasmaOffset;

		[Header("Plasma beam heavy")]
		public Transform plasmaBeamHeavy;

		public float plasmaBeamHeavyOffset;

		[Header("Lightning gun")]
		public Transform lightningGunBeam;

		public float lightingGunBeamOffset;

		[Header("Flame")]
		public Transform flameRed;

		public float flameOffset;

		[Header("Laser impulse")]
		public Transform laserImpulseProjectile;

		public Transform laserImpulseMuzzle;

		public Transform laserImpulseImpact;

		public float laserImpulseOffset;

		private void Awake()
		{
			instance = this;
		}

		private void OnGUI()
		{
			GUIStyle gUIStyle = new GUIStyle(GUI.skin.label);
			gUIStyle.fontSize = 25;
			gUIStyle.fontStyle = FontStyle.Bold;
			gUIStyle.wordWrap = false;
			GUIStyle gUIStyle2 = new GUIStyle(GUI.skin.label);
			gUIStyle2.fontSize = 11;
			gUIStyle2.wordWrap = false;
			GUILayout.BeginArea(new Rect(Screen.width / 2 - 150, Screen.height - 150, 300f, 120f));
			GUILayout.BeginVertical();
			GUILayout.FlexibleSpace();
			GUILayout.BeginHorizontal();
			GUILayout.FlexibleSpace();
			GUILayout.Label(fxTypeName[(int)DefaultFXType], gUIStyle);
			GUILayout.FlexibleSpace();
			GUILayout.EndHorizontal();
			GUILayout.FlexibleSpace();
			GUILayout.BeginHorizontal();
			GUILayout.FlexibleSpace();
			GUILayout.Label("Press Left / Right arrow keys to switch", gUIStyle2);
			GUILayout.FlexibleSpace();
			GUILayout.EndHorizontal();
			GUILayout.FlexibleSpace();
			GUILayout.BeginHorizontal();
			GUILayout.FlexibleSpace();
			if (GUILayout.Button("Previous", GUILayout.Width(90f), GUILayout.Height(30f)))
			{
				PrevWeapon();
			}
			GUILayout.FlexibleSpace();
			if (GUILayout.Button("Next", GUILayout.Width(90f), GUILayout.Height(30f)))
			{
				NextWeapon();
			}
			GUILayout.FlexibleSpace();
			GUILayout.EndHorizontal();
			GUILayout.FlexibleSpace();
			GUILayout.EndVertical();
			GUILayout.EndArea();
		}

		private void Update()
		{
			if (UnityEngine.Input.GetKeyDown(KeyCode.RightArrow))
			{
				NextWeapon();
			}
			else if (UnityEngine.Input.GetKeyDown(KeyCode.LeftArrow))
			{
				PrevWeapon();
			}
		}

		private void NextWeapon()
		{
			if ((int)DefaultFXType < Enum.GetNames(typeof(F3DFXType)).Length - 1)
			{
				DefaultFXType++;
			}
		}

		private void PrevWeapon()
		{
			if (DefaultFXType > F3DFXType.Vulcan)
			{
				DefaultFXType--;
			}
		}

		private void AdvanceSocket()
		{
			curSocket++;
			if (curSocket >= TurretSocket.Length)
			{
				curSocket = 0;
			}
		}

		public void Fire()
		{
			switch (DefaultFXType)
			{
			case F3DFXType.Vulcan:
				timerID = F3DTime.time.AddTimer(0.05f, Vulcan);
				Vulcan();
				break;
			case F3DFXType.SoloGun:
				timerID = F3DTime.time.AddTimer(0.2f, SoloGun);
				SoloGun();
				break;
			case F3DFXType.Sniper:
				timerID = F3DTime.time.AddTimer(0.3f, Sniper);
				Sniper();
				break;
			case F3DFXType.ShotGun:
				timerID = F3DTime.time.AddTimer(0.3f, ShotGun);
				ShotGun();
				break;
			case F3DFXType.Seeker:
				timerID = F3DTime.time.AddTimer(0.2f, Seeker);
				Seeker();
				break;
			case F3DFXType.RailGun:
				timerID = F3DTime.time.AddTimer(0.2f, RailGun);
				RailGun();
				break;
			case F3DFXType.PlasmaGun:
				timerID = F3DTime.time.AddTimer(0.2f, PlasmaGun);
				PlasmaGun();
				break;
			case F3DFXType.PlasmaBeam:
				PlasmaBeam();
				break;
			case F3DFXType.PlasmaBeamHeavy:
				PlasmaBeamHeavy();
				break;
			case F3DFXType.LightningGun:
				LightningGun();
				break;
			case F3DFXType.FlameRed:
				FlameRed();
				break;
			case F3DFXType.LaserImpulse:
				timerID = F3DTime.time.AddTimer(0.15f, LaserImpulse);
				LaserImpulse();
				break;
			}
		}

		public void Stop()
		{
			if (timerID != -1)
			{
				F3DTime.time.RemoveTimer(timerID);
				timerID = -1;
			}
			switch (DefaultFXType)
			{
			case F3DFXType.PlasmaBeam:
				F3DAudioController.instance.PlasmaBeamClose(base.transform.position);
				break;
			case F3DFXType.PlasmaBeamHeavy:
				F3DAudioController.instance.PlasmaBeamHeavyClose(base.transform.position);
				break;
			case F3DFXType.LightningGun:
				F3DAudioController.instance.LightningGunClose(base.transform.position);
				break;
			case F3DFXType.FlameRed:
				F3DAudioController.instance.FlameGunClose(base.transform.position);
				break;
			}
		}

		private void Vulcan()
		{
			Quaternion lhs = Quaternion.Euler(UnityEngine.Random.onUnitSphere);
			F3DPoolManager.Pools["GeneratedPool"].Spawn(vulcanMuzzle, TurretSocket[curSocket].position, TurretSocket[curSocket].rotation, TurretSocket[curSocket]);
			F3DProjectile component = F3DPoolManager.Pools["GeneratedPool"].Spawn(vulcanProjectile, TurretSocket[curSocket].position + TurretSocket[curSocket].forward, lhs * TurretSocket[curSocket].rotation, null).gameObject.gameObject.GetComponent<F3DProjectile>();
			if ((bool)component)
			{
				component.SetOffset(vulcanOffset);
			}
			if (ShellParticles.Length != 0)
			{
				ShellParticles[curSocket].Emit(1);
			}
			F3DAudioController.instance.VulcanShot(TurretSocket[curSocket].position);
			AdvanceSocket();
		}

		public void VulcanImpact(Vector3 pos)
		{
			F3DPoolManager.Pools["GeneratedPool"].Spawn(vulcanImpact, pos, Quaternion.identity, null);
			F3DAudioController.instance.VulcanHit(pos);
		}

		private void SoloGun()
		{
			Quaternion lhs = Quaternion.Euler(UnityEngine.Random.onUnitSphere);
			F3DPoolManager.Pools["GeneratedPool"].Spawn(soloGunMuzzle, TurretSocket[curSocket].position, TurretSocket[curSocket].rotation, TurretSocket[curSocket]);
			F3DProjectile component = F3DPoolManager.Pools["GeneratedPool"].Spawn(soloGunProjectile, TurretSocket[curSocket].position + TurretSocket[curSocket].forward, lhs * TurretSocket[curSocket].rotation, null).gameObject.GetComponent<F3DProjectile>();
			if ((bool)component)
			{
				component.SetOffset(soloGunOffset);
			}
			F3DAudioController.instance.SoloGunShot(TurretSocket[curSocket].position);
			AdvanceSocket();
		}

		public void SoloGunImpact(Vector3 pos)
		{
			F3DPoolManager.Pools["GeneratedPool"].Spawn(soloGunImpact, pos, Quaternion.identity, null);
			F3DAudioController.instance.SoloGunHit(pos);
		}

		private void Sniper()
		{
			Quaternion lhs = Quaternion.Euler(UnityEngine.Random.onUnitSphere);
			F3DPoolManager.Pools["GeneratedPool"].Spawn(sniperMuzzle, TurretSocket[curSocket].position, TurretSocket[curSocket].rotation, TurretSocket[curSocket]);
			F3DBeam component = F3DPoolManager.Pools["GeneratedPool"].Spawn(sniperBeam, TurretSocket[curSocket].position, lhs * TurretSocket[curSocket].rotation, null).gameObject.GetComponent<F3DBeam>();
			if ((bool)component)
			{
				component.SetOffset(sniperOffset);
			}
			F3DAudioController.instance.SniperShot(TurretSocket[curSocket].position);
			if (ShellParticles.Length != 0)
			{
				ShellParticles[curSocket].Emit(1);
			}
			AdvanceSocket();
		}

		public void SniperImpact(Vector3 pos)
		{
			F3DPoolManager.Pools["GeneratedPool"].Spawn(sniperImpact, pos, Quaternion.identity, null);
			F3DAudioController.instance.SniperHit(pos);
		}

		private void ShotGun()
		{
			Quaternion lhs = Quaternion.Euler(UnityEngine.Random.onUnitSphere);
			F3DPoolManager.Pools["GeneratedPool"].Spawn(shotGunMuzzle, TurretSocket[curSocket].position, TurretSocket[curSocket].rotation, TurretSocket[curSocket]);
			F3DPoolManager.Pools["GeneratedPool"].Spawn(shotGunProjectile, TurretSocket[curSocket].position, lhs * TurretSocket[curSocket].rotation, null);
			F3DAudioController.instance.ShotGunShot(TurretSocket[curSocket].position);
			if (ShellParticles.Length != 0)
			{
				ShellParticles[curSocket].Emit(1);
			}
			AdvanceSocket();
		}

		private void Seeker()
		{
			Quaternion lhs = Quaternion.Euler(UnityEngine.Random.onUnitSphere);
			F3DPoolManager.Pools["GeneratedPool"].Spawn(seekerMuzzle, TurretSocket[curSocket].position, TurretSocket[curSocket].rotation, TurretSocket[curSocket]);
			F3DProjectile component = F3DPoolManager.Pools["GeneratedPool"].Spawn(seekerProjectile, TurretSocket[curSocket].position, lhs * TurretSocket[curSocket].rotation, null).gameObject.GetComponent<F3DProjectile>();
			if ((bool)component)
			{
				component.SetOffset(seekerOffset);
			}
			F3DAudioController.instance.SeekerShot(TurretSocket[curSocket].position);
			AdvanceSocket();
		}

		public void SeekerImpact(Vector3 pos)
		{
			F3DPoolManager.Pools["GeneratedPool"].Spawn(seekerImpact, pos, Quaternion.identity, null);
			F3DAudioController.instance.SeekerHit(pos);
		}

		private void RailGun()
		{
			Quaternion lhs = Quaternion.Euler(UnityEngine.Random.onUnitSphere);
			F3DPoolManager.Pools["GeneratedPool"].Spawn(railgunMuzzle, TurretSocket[curSocket].position, TurretSocket[curSocket].rotation, TurretSocket[curSocket]);
			F3DBeam component = F3DPoolManager.Pools["GeneratedPool"].Spawn(railgunBeam, TurretSocket[curSocket].position, lhs * TurretSocket[curSocket].rotation, null).gameObject.GetComponent<F3DBeam>();
			if ((bool)component)
			{
				component.SetOffset(railgunOffset);
			}
			F3DAudioController.instance.RailGunShot(TurretSocket[curSocket].position);
			if (ShellParticles.Length != 0)
			{
				ShellParticles[curSocket].Emit(1);
			}
			AdvanceSocket();
		}

		public void RailgunImpact(Vector3 pos)
		{
			F3DPoolManager.Pools["GeneratedPool"].Spawn(railgunImpact, pos, Quaternion.identity, null);
			F3DAudioController.instance.RailGunHit(pos);
		}

		private void PlasmaGun()
		{
			Quaternion lhs = Quaternion.Euler(UnityEngine.Random.onUnitSphere);
			F3DPoolManager.Pools["GeneratedPool"].Spawn(plasmagunMuzzle, TurretSocket[curSocket].position, TurretSocket[curSocket].rotation, TurretSocket[curSocket]);
			F3DProjectile component = F3DPoolManager.Pools["GeneratedPool"].Spawn(plasmagunProjectile, TurretSocket[curSocket].position, lhs * TurretSocket[curSocket].rotation, null).gameObject.GetComponent<F3DProjectile>();
			if ((bool)component)
			{
				component.SetOffset(plasmaOffset);
			}
			F3DAudioController.instance.PlasmaGunShot(TurretSocket[curSocket].position);
			AdvanceSocket();
		}

		public void PlasmaGunImpact(Vector3 pos)
		{
			F3DPoolManager.Pools["GeneratedPool"].Spawn(plasmagunImpact, pos, Quaternion.identity, null);
			F3DAudioController.instance.PlasmaGunHit(pos);
		}

		private void PlasmaBeam()
		{
			for (int i = 0; i < TurretSocket.Length; i++)
			{
				F3DPoolManager.Pools["GeneratedPool"].Spawn(plasmaBeam, TurretSocket[i].position, TurretSocket[i].rotation, TurretSocket[i]);
			}
			F3DAudioController.instance.PlasmaBeamLoop(base.transform.position, base.transform.parent);
		}

		private void PlasmaBeamHeavy()
		{
			for (int i = 0; i < TurretSocket.Length; i++)
			{
				F3DPoolManager.Pools["GeneratedPool"].Spawn(plasmaBeamHeavy, TurretSocket[i].position, TurretSocket[i].rotation, TurretSocket[i]);
			}
			F3DAudioController.instance.PlasmaBeamHeavyLoop(base.transform.position, base.transform.parent);
		}

		private void LightningGun()
		{
			for (int i = 0; i < TurretSocket.Length; i++)
			{
				F3DPoolManager.Pools["GeneratedPool"].Spawn(lightningGunBeam, TurretSocket[i].position, TurretSocket[i].rotation, TurretSocket[i]);
			}
			F3DAudioController.instance.LightningGunLoop(base.transform.position, base.transform);
		}

		private void FlameRed()
		{
			for (int i = 0; i < TurretSocket.Length; i++)
			{
				F3DPoolManager.Pools["GeneratedPool"].Spawn(flameRed, TurretSocket[i].position, TurretSocket[i].rotation, TurretSocket[i]);
			}
			F3DAudioController.instance.FlameGunLoop(base.transform.position, base.transform);
		}

		private void LaserImpulse()
		{
			Quaternion lhs = Quaternion.Euler(UnityEngine.Random.onUnitSphere);
			F3DPoolManager.Pools["GeneratedPool"].Spawn(laserImpulseMuzzle, TurretSocket[curSocket].position, TurretSocket[curSocket].rotation, TurretSocket[curSocket]);
			F3DProjectile component = F3DPoolManager.Pools["GeneratedPool"].Spawn(laserImpulseProjectile, TurretSocket[curSocket].position, lhs * TurretSocket[curSocket].rotation, null).gameObject.GetComponent<F3DProjectile>();
			if ((bool)component)
			{
				component.SetOffset(laserImpulseOffset);
			}
			F3DAudioController.instance.LaserImpulseShot(TurretSocket[curSocket].position);
			AdvanceSocket();
		}

		public void LaserImpulseImpact(Vector3 pos)
		{
			F3DPoolManager.Pools["GeneratedPool"].Spawn(laserImpulseImpact, pos, Quaternion.identity, null);
			F3DAudioController.instance.LaserImpulseHit(pos);
		}
	}
}
