using App.GUI;
using App.Player;
using App.Prefabs;
using App.Quests;
using App.SaveSystem;
using App.Spawn;
using App.Util;
using App.Vehicles;
using App.Weapons;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace App.Rewards
{
    public class RewardManager : MonoBehaviour
    {
        [Range(0f, 1f)]
        [SerializeField]
        private float armyRewardChance = 0.01f;

        [SerializeField]
        private GameObject[] armyVehicles;

        [SerializeField]
        private string fallbackVehicleName = "Rare Car";

        [SerializeField]
        private string weaponText = "{0} + Ammo";

        [SerializeField]
        private string ammoOnlyText = "Ammo for {0}";

        private PanelsManager panelsManager;

        private PlayerRespawner playerRespawner;

        private SaveEntities saveEntities;

        private WeaponInventory weaponInventory;

        private GunPrefabsScriptableObject gunPrefabs;

        private TankSpawner tankSpawner;

        private VehicleSpawner vehicleSpawner;

        private MechSpawner mechSpawner;

        private int layerMask;

        private List<GunType> m_AvailableGunList = new List<GunType>(16);

        private int currentRewardIndex;

        private VehicleComponents vehicleRewardPrefab;

        private List<string> rewards = new List<string>(4);

        private int hackingReward;

        private Pauser pauser;

        public List<string> Rewards => rewards;

        public event Action<int, RewardState> DataChanged;

        public void ShowRewardSelectionScreen()
        {
            panelsManager.ShowPanel(PanelType.RewardSelection);
        }

        public void OnRewardSelectionLeaveRequest()
        {
            panelsManager.ShowPanel(PanelType.Respawn);
        }

        public void AssignWaitingRewards()
        {
            if (!(vehicleRewardPrefab == null))
            {
                AssignVehicle(vehicleRewardPrefab);
                vehicleRewardPrefab = null;
            }
        }

        public void ResetRewardStates()
        {
            vehicleRewardPrefab = null;
            rewards.Clear();
        }

        public void AssignQuestReward(Quest quest)
        {
            PlayerSaveEntity playerSave = saveEntities.PlayerSave;
            playerSave.score += quest.Settings.FinishReward.Amount;
            playerSave.Save();
        }

        int m_hackcount = 1;
        public void AssignHackingReward(int reward, int hackcount = 1)
        {
            m_hackcount = hackcount;
            hackingReward = reward;
            HackingResultPanel obj = pauser.PauseWithDialog(PanelType.HackingResult) as HackingResultPanel;
            obj.Show(reward, CallAdsManager.RewardedIsReady(), OnHackingResultPanelClosed, hackcount);
        }

        private void Awake()
        {
            Initialize_Internal();
        }

        private void OnHackingResultPanelClosed(HackingResultPanel.Result result)
        {
            pauser.Resume();
            if (result == HackingResultPanel.Result.Close)
            {
                saveEntities.PlayerSave.score += hackingReward;
                saveEntities.PlayerSave.Save();
                hackingReward = 0;
            }
            else
            {
                saveEntities.PlayerSave.score += hackingReward * (m_hackcount + 1);
                saveEntities.PlayerSave.Save();
                hackingReward = 0;
            }
        }

        //private void OnRewardSelectionAdResult(Reklamaa.ResultType result)
        //{
        //	if (result == Reklamaa.ResultType.Completed)
        //	{
        //		SetState(currentRewardIndex, RewardState.Finished);
        //		AssignRewardToPlayer(currentRewardIndex);
        //		if (currentRewardIndex + 1 < rewardData.Length)
        //		{
        //			SetState(currentRewardIndex + 1, RewardState.Ready);
        //		}
        //	}
        //}

        private void Initialize_Internal()
        {
            panelsManager = ServiceLocator.Get<PanelsManager>();
            playerRespawner = ServiceLocator.Get<PlayerRespawner>();
            saveEntities = ServiceLocator.Get<SaveEntities>();
            weaponInventory = ServiceLocator.Get<WeaponInventory>();
            gunPrefabs = ServiceLocator.Get<PrefabsContainer>().gunPrefabs;
            tankSpawner = ServiceLocator.Get<TankSpawner>();
            vehicleSpawner = ServiceLocator.Get<VehicleSpawner>();
            mechSpawner = ServiceLocator.Get<MechSpawner>(showError: false);
            pauser = ServiceLocator.Get<Pauser>();
            layerMask = LayerMask.GetMask("Enemy", "Impact", "Player", "Climbable", "Ignore Raycast");
            GameObject gameObject = ServiceLocator.GetGameObject("MusicPlayer", showError: false);
        }


        public void AssignRewardToPlayer(int rewardIndex)
        {
            switch (rewardIndex)
            {
                case 0:
                    playerRespawner.ReturnLostItems();
                    break;
                case 1:
                    AssignRandomWeapon();
                    break;
                case 2:
                    {
                        playerRespawner.ReturnLostItems();
                        GameObject randomVehiclePrefab = GetRandomVehiclePrefab();
                        vehicleRewardPrefab = ((randomVehiclePrefab != null) ? randomVehiclePrefab.GetComponent<VehicleComponents>() : null);
                        if (vehicleRewardPrefab != null)
                        {
                            string displayName = vehicleRewardPrefab.displayName;
                            displayName = (string.IsNullOrEmpty(displayName) ? fallbackVehicleName : displayName);
                            rewards.Add(displayName);
                        }
                        AssignVehicle(vehicleRewardPrefab);
                        break;
                    }
            }
        }

        private void AssignRandomWeapon()
        {
            PlayerSaveEntity playerSave = saveEntities.PlayerSave;
            Dictionary<GunType, GunSaveEntity> gun2 = playerSave.guns;
            m_AvailableGunList.Clear();
            foreach (KeyValuePair<GunType, GunSaveEntity> gun in playerSave.guns)
            {
                if (gun.Value.IsGun)
                {
                    m_AvailableGunList.Add(gun.Key);
                }
            }
            int index = UnityEngine.Random.Range(0, m_AvailableGunList.Count);
            GunType gunType = m_AvailableGunList[index];
            GunSaveEntity gunSave = playerSave.GetGunSave(gunType);
            bool buyed = gunSave.buyed;
            gunSave.buyed = true;
            Dictionary<GunType, GameObject> sortedWeaponLauncherPrefabs = gunPrefabs.GetSortedWeaponLauncherPrefabs();
            GameObject value = null;
            sortedWeaponLauncherPrefabs.TryGetValue(gunType, out value);
            IWeapon component = value.GetComponent<IWeapon>();
            gunSave.ammo += component.GetAmmo();
            gunSave.ammoReserve += component.GetAmmoCommonReserve();
            gunSave.Save();
            string gunName = component.GetGunName();
            string format = buyed ? ammoOnlyText : weaponText;
            format = string.Format(format, gunName);
            rewards.Add(format);
        }

        private GameObject GetRandomVehiclePrefab()
        {
            float num = UnityEngine.Random.Range(0f, 1f);
            if (armyVehicles.Length != 0 && num <= armyRewardChance)
            {
                int num2 = UnityEngine.Random.Range(0, armyVehicles.Length);
                return armyVehicles[num2];
            }
            return vehicleSpawner.GetRandomSportCarPrefab();
        }

        private void AssignVehicle(VehicleComponents prefab)
        {
            if (!(prefab == null))
            {
                Transform transform = ServiceLocator.GetGameObject("Player").transform;
                if (prefab.type == VehicleType.Tank)
                {
                    Vector3 validSpherePosition = PhysicsUtils.GetValidSpherePosition(transform.position, transform.forward, 6f, 6.5f, 30f, 30f, layerMask);
                    tankSpawner.SpawnVehicle(validSpherePosition, prefab.gameObject);
                }
                else if (prefab.type == VehicleType.Car)
                {
                    Vector3 validSpherePosition2 = PhysicsUtils.GetValidSpherePosition(transform.position, transform.forward, 6f, 6.5f, 30f, 30f, layerMask);
                    vehicleSpawner.SpawnVehicle(validSpherePosition2, prefab.gameObject);
                }
                else if (prefab.type == VehicleType.Mech && !(mechSpawner == null))
                {
                    Vector3 validSpherePosition3 = PhysicsUtils.GetValidSpherePosition(transform.position, transform.forward, 6f, 6.5f, 30f, 30f, layerMask);
                    mechSpawner.SpawnVehicle(validSpherePosition3, prefab.gameObject);
                }
            }
        }
    }
}
