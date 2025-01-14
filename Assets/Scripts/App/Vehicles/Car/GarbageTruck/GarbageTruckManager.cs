using App.Player;
using App.Vehicles.Car.Navigation;
using System.Collections.Generic;
using UnityEngine;

namespace App.Vehicles.Car.GarbageTruck
{
    public class GarbageTruckManager : MonoBehaviour
    {
        public Animator animator;

        public GameObject containerPosition;

        public GameObject trashPrefab;

        public GameObject trashPosition;

        public List<ParticleSystem> effects;

        public Transform Front;

        public Transform Back;

        public MissionPiece actualMissionPiece;

        private AnimationPitchforkPiece actualAnimationPitchforkPiece;

        private AnimationCargoPiece actualAnimationCargoPiece;

        private bool canLiftContainer = true;

        private int actualCapacity;

        private CameraPositionRotator cameraPositionRotator;

        private ContainerParameters liftedContainer;

        private GarbageTruckDispatching garbageTruckDispathing;

        private ContainerDetectorController containerDetector;

        private GarbageAudioController garbageAudioController;

        private VehicleModesHandler vehicleModesHandler;

        private VehicleComponents vehicleComponents;

        private DurationTimer animationPitchforkTimer = new DurationTimer();

        private DurationTimer animationCargoTimer = new DurationTimer();

        private DurationTimer pitchforkUpAudioTimer = new DurationTimer();

        private DurationTimer pitchforkDownAudioTimer = new DurationTimer();

        private DurationTimer cargoUpAudioTimer = new DurationTimer();

        private DurationTimer cargoDownAudioTimer = new DurationTimer();

        private DurationTimer checkerTimer = new DurationTimer();

        private DurationTimer animatorEnableTimer = new DurationTimer();

        private DurationTimer dumperTimer = new DurationTimer();

        private float defaultMaxVerticalAngle;

        private float nextMaxVerticalAngle;

        private float trashDelay;

        private float startDump;

        private bool canDoAction;

        private bool pitchforkLifting;

        private bool cargoMovingDown;

        private bool waitingForCollect;

        private bool lifting;

        private float startDumpingTime;

        private float endStartDumpingTime;

        public int MaxContainerCapacity
        {
            get;
            set;
        }

        public int AllCapacity
        {
            get;
            private set;
        }

        public bool IsFull => AllCapacity == MaxContainerCapacity;

        public bool InLandfill
        {
            get;
            set;
        }

        public bool InFront
        {
            get;
            set;
        }

        public bool PitchforkIsDown
        {
            get;
            set;
        }

        public bool CargoIsDown
        {
            get;
            set;
        }

        public bool CargoIsUp
        {
            get;
            set;
        }

        public bool InDumpsterArea
        {
            get;
            set;
        }

        public bool IsMissionCompletedWithThisCar
        {
            get;
            set;
        }

        private bool IsEmpty => AllCapacity <= 0;

        private float DumpingTime => Mathf.Clamp(endStartDumpingTime - startDumpingTime, 0f, 3f);

        private void Awake()
        {
            CargoIsUp = false;
            CargoIsDown = true;
            IsMissionCompletedWithThisCar = false;
            garbageTruckDispathing = base.gameObject.GetComponent<GarbageTruckDispatching>();
            cameraPositionRotator = base.gameObject.GetComponentInChildren<CameraPositionRotator>();
            containerDetector = base.gameObject.GetComponentInChildren<ContainerDetectorController>();
            garbageAudioController = (GetComponent<VehicleComponents>().carSounds as GarbageAudioController);
            vehicleModesHandler = this.GetComponentInChildrenSafe<VehicleModesHandler>();
            vehicleComponents = GetComponent<VehicleComponents>();
            animator.enabled = false;
            defaultMaxVerticalAngle = cameraPositionRotator.maxVerticalAngle;
        }

        private void Update()
        {
            if (CargoIsUp && (bool)liftedContainer && actualAnimationPitchforkPiece == AnimationPitchforkPiece.Lifting)
            {
                liftedContainer.EndLif();
                liftedContainer.ActivatePhysic();
                garbageAudioController.recycle.Stop();
                liftedContainer.DeactivateMissionDumpster();
                liftedContainer = null;
            }
            if (canDoAction)
            {
                if (PitchforkIsDown)
                {
                    if (nextMaxVerticalAngle > cameraPositionRotator.maxVerticalAngle)
                    {
                        cameraPositionRotator.maxVerticalAngle = Mathf.LerpUnclamped(cameraPositionRotator.maxVerticalAngle, nextMaxVerticalAngle, Time.deltaTime * 2f);
                    }
                    else
                    {
                        cameraPositionRotator.maxVerticalAngle = nextMaxVerticalAngle;
                        canDoAction = false;
                    }
                }
                else if (nextMaxVerticalAngle < cameraPositionRotator.maxVerticalAngle)
                {
                    cameraPositionRotator.maxVerticalAngle = Mathf.LerpUnclamped(cameraPositionRotator.maxVerticalAngle, nextMaxVerticalAngle, Time.deltaTime * 2f);
                }
                else
                {
                    cameraPositionRotator.maxVerticalAngle = nextMaxVerticalAngle;
                    canDoAction = false;
                }
            }
            if (animatorEnableTimer.Done())
            {
                animatorEnableTimer.Stop();
                animator.enabled = false;
            }
            if (animatorEnableTimer.Running() && !animator.enabled)
            {
                animator.enabled = true;
            }
            if (animationCargoTimer.Done())
            {
                animationCargoTimer.Stop();
                if (actualAnimationCargoPiece == AnimationCargoPiece.CargoDown)
                {
                    if (waitingForCollect && (bool)liftedContainer && !lifting)
                    {
                        if (liftedContainer.Collected)
                        {
                            liftedContainer.EndLif();
                            if (liftedContainer.IsFull && !liftedContainer.PhysicActivated && liftedContainer.Collect())
                            {
                                AllCapacity = Mathf.Clamp(AllCapacity + 1, 0, MaxContainerCapacity);
                            }
                        }
                        if (liftedContainer.InMission)
                        {
                            liftedContainer.SetAsMissionDumpster();
                        }
                        liftedContainer.ResetParent();
                        liftedContainer = null;
                        waitingForCollect = false;
                    }
                    cargoMovingDown = false;
                    CargoIsDown = true;
                    CargoIsUp = false;
                    containerDetector.gameObject.SetActive(value: false);
                    containerDetector.gameObject.SetActive(value: true);
                }
                if (actualAnimationCargoPiece == AnimationCargoPiece.CargoUp)
                {
                    CargoIsUp = true;
                }
            }
            if (animationPitchforkTimer.Done())
            {
                animationPitchforkTimer.Stop();
                if (actualAnimationPitchforkPiece == AnimationPitchforkPiece.PitchforkDown)
                {
                    PitchforkIsDown = true;
                    canDoAction = true;
                }
                else if (actualAnimationPitchforkPiece == AnimationPitchforkPiece.PitchforkUp)
                {
                    PitchforkIsDown = false;
                    canDoAction = true;
                }
                if (actualAnimationPitchforkPiece == AnimationPitchforkPiece.StartLift)
                {
                    if ((bool)liftedContainer && liftedContainer.IsFull && !liftedContainer.PhysicActivated)
                    {
                        garbageAudioController.recycle.Play();
                    }
                    animationPitchforkTimer.Run(3f);
                    actualAnimationPitchforkPiece = AnimationPitchforkPiece.Lifting;
                }
                else if (actualAnimationPitchforkPiece == AnimationPitchforkPiece.Lifting)
                {
                    animator.enabled = true;
                    animator.SetInteger("PitchforkState", 4);
                    garbageAudioController.pitchforkDown.Play();
                    animationPitchforkTimer.Run(4f);
                    actualAnimationPitchforkPiece = AnimationPitchforkPiece.EndLift;
                }
                else if (actualAnimationPitchforkPiece == AnimationPitchforkPiece.EndLift)
                {
                    canLiftContainer = true;
                    lifting = false;
                    garbageAudioController.pitchforkDown.Stop();
                    garbageAudioController.pitchforkUp.Stop();
                    if (CargoIsDown && (bool)liftedContainer)
                    {
                        liftedContainer.EndLif();
                        if (liftedContainer.IsFull && !liftedContainer.PhysicActivated && liftedContainer.Collect())
                        {
                            AllCapacity = Mathf.Clamp(AllCapacity + 1, 0, MaxContainerCapacity);
                        }
                        liftedContainer = null;
                    }
                    else if ((bool)liftedContainer)
                    {
                        liftedContainer.SetAsCollected();
                    }
                }
            }
            if (dumperTimer.Done())
            {
                GameObject gameObject = UnityEngine.Object.Instantiate(trashPrefab);
                if ((bool)gameObject)
                {
                    gameObject.transform.position = trashPosition.transform.position;
                }
                dumperTimer.Run(5f / (3f * (float)MaxContainerCapacity * 2f));
                float num = InFront ? 1f : 5f;
                if (startDumpingTime + num <= Time.time)
                {
                    if (!InFront)
                    {
                        dumperTimer.Stop();
                        actualCapacity = 0;
                        if (AllCapacity == MaxContainerCapacity && actualCapacity <= 0)
                        {
                            actualMissionPiece = MissionPiece.Done;
                        }
                        AllCapacity = 0;
                        GarbageCapacity.Instance.ResetSlider();
                    }
                    else
                    {
                        dumperTimer.Stop();
                        actualCapacity = 0;
                        if (AllCapacity == MaxContainerCapacity && actualCapacity <= 0)
                        {
                            garbageTruckDispathing.MissionFailedInTruck(5f);
                        }
                        AllCapacity = 0;
                        GarbageCapacity.Instance.ResetSlider();
                    }
                    return;
                }
            }
            if (checkerTimer.Done())
            {
                if (actualCapacity > 0)
                {
                    if (!IsEmpty)
                    {
                        for (int i = 0; i < effects.Count; i++)
                        {
                            effects[i].Play();
                        }
                    }
                    int num2 = 1;
                    if (InLandfill)
                    {
                        num2 = MaxContainerCapacity * 2;
                        dumperTimer.Run(5f / (3f * (float)num2));
                        checkerTimer.Stop();
                        startDumpingTime = Time.time;
                        GarbageCapacity.Instance.Dump(InFront ? 1f : 5f);
                        return;
                    }
                    if ((bool)trashPrefab && (bool)trashPosition)
                    {
                        for (int j = 0; j < 3 * num2; j++)
                        {
                            GameObject gameObject2 = UnityEngine.Object.Instantiate(trashPrefab);
                            if ((bool)gameObject2)
                            {
                                gameObject2.transform.position = trashPosition.transform.position;
                            }
                        }
                    }
                }
                GarbageCapacity.Instance.Activate();
                if (!InLandfill)
                {
                    actualCapacity--;
                    AllCapacity = Mathf.Clamp(AllCapacity - 1, 0, MaxContainerCapacity);
                }
                else
                {
                    actualCapacity = 0;
                    GarbageCapacity.Instance.SetValue(0, 2f, isUp: false);
                    if (AllCapacity == MaxContainerCapacity && actualCapacity <= 0)
                    {
                        actualMissionPiece = MissionPiece.Done;
                    }
                    AllCapacity = 0;
                }
                if (actualCapacity > 0)
                {
                    checkerTimer.Run(2f);
                }
                else
                {
                    checkerTimer.Stop();
                    for (int k = 0; k < effects.Count; k++)
                    {
                        effects[k].Stop();
                    }
                }
            }
            if (pitchforkUpAudioTimer.Done())
            {
                garbageAudioController.pitchforkUp.Stop();
                pitchforkUpAudioTimer.Stop();
            }
            if (pitchforkDownAudioTimer.Done())
            {
                garbageAudioController.pitchforkDown.Stop();
                pitchforkDownAudioTimer.Stop();
            }
            if (cargoUpAudioTimer.Done())
            {
                garbageAudioController.cargoUp.Stop();
                cargoUpAudioTimer.Stop();
            }
            if (cargoDownAudioTimer.Done())
            {
                garbageAudioController.cargoDown.Stop();
                cargoDownAudioTimer.Stop();
            }
        }

        public void Collect(bool isPlayer = false)
        {
            if ((bool)containerDetector && ((bool)containerDetector.ActualContainer || waitingForCollect) && canLiftContainer)
            {
                if (isPlayer)
                {
                    CallAdsManager.ShowInterstial(CallAdsManager.PositionAds.gameplay_vehicle_dump, () =>
                    {
                        canLiftContainer = false;
                        lifting = true;
                        if (!liftedContainer)
                        {
                            liftedContainer = containerDetector.ActualContainer;
                        }
                        animator.enabled = true;
                        liftedContainer.StartLift(containerPosition.transform);
                        animator.SetInteger("PitchforkState", 3);
                        if (animatorEnableTimer.GetEndTime() <= Time.time + 3.7f + 3f + 4f + 0.2f)
                        {
                            animatorEnableTimer.Run(10.9f);
                        }
                        garbageAudioController.pitchforkUp.Play();
                        animationPitchforkTimer.Run(3.7f);
                        actualAnimationPitchforkPiece = AnimationPitchforkPiece.StartLift;
                    });
                }
                else
                {
                    canLiftContainer = false;
                    lifting = true;
                    if (!liftedContainer)
                    {
                        liftedContainer = containerDetector.ActualContainer;
                    }
                    animator.enabled = true;
                    liftedContainer.StartLift(containerPosition.transform);
                    animator.SetInteger("PitchforkState", 3);
                    if (animatorEnableTimer.GetEndTime() <= Time.time + 3.7f + 3f + 4f + 0.2f)
                    {
                        animatorEnableTimer.Run(10.9f);
                    }
                    garbageAudioController.pitchforkUp.Play();
                    animationPitchforkTimer.Run(3.7f);
                    actualAnimationPitchforkPiece = AnimationPitchforkPiece.StartLift;
                }
            }
        }

        public void SetActualContainer(ContainerParameters actualContainer)
        {
            containerDetector.ActualContainer = actualContainer;
        }

        public void LiftDown(bool isPlayer = false)
        {
            if (!pitchforkLifting)
            {
                if (isPlayer)
                {
                    CallAdsManager.ShowInterstial(CallAdsManager.PositionAds.gameplay_vehicle_dump, () =>
                    {
                        pitchforkLifting = true;
                        animator.enabled = true;
                        animator.SetInteger("PitchforkState", 1);
                        if (animatorEnableTimer.GetEndTime() <= Time.time + 2f + 0.2f)
                        {
                            animatorEnableTimer.Run(2.2f);
                        }
                        garbageAudioController.pitchforkDown.Play();
                        pitchforkDownAudioTimer.Run(2f);
                        actualAnimationPitchforkPiece = AnimationPitchforkPiece.PitchforkDown;
                        nextMaxVerticalAngle = 35f;
                        if (vehicleModesHandler.mode == VehicleMode.Player)
                        {
                            animationPitchforkTimer.Run(2f);
                        }
                        if (vehicleModesHandler.mode == VehicleMode.AI)
                        {
                            PitchforkIsDown = true;
                        }
                    });
                }
                else
                {
                    pitchforkLifting = true;
                    animator.enabled = true;
                    animator.SetInteger("PitchforkState", 1);
                    if (animatorEnableTimer.GetEndTime() <= Time.time + 2f + 0.2f)
                    {
                        animatorEnableTimer.Run(2.2f);
                    }
                    garbageAudioController.pitchforkDown.Play();
                    pitchforkDownAudioTimer.Run(2f);
                    actualAnimationPitchforkPiece = AnimationPitchforkPiece.PitchforkDown;
                    nextMaxVerticalAngle = 35f;
                    if (vehicleModesHandler.mode == VehicleMode.Player)
                    {
                        animationPitchforkTimer.Run(2f);
                    }
                    if (vehicleModesHandler.mode == VehicleMode.AI)
                    {
                        PitchforkIsDown = true;
                    }
                }
            }
        }

        public void LiftUp(bool isPlayer = false)
        {
            if (pitchforkLifting)
            {
                if (isPlayer)
                {
                    CallAdsManager.ShowInterstial(CallAdsManager.PositionAds.gameplay_vehicle_dump, () =>
                    {
                        pitchforkLifting = false;
                        animator.enabled = true;
                        canLiftContainer = true;
                        animator.SetInteger("PitchforkState", 2);
                        if (animatorEnableTimer.GetEndTime() <= Time.time + 2f + 0.2f)
                        {
                            animatorEnableTimer.Run(2.2f);
                        }
                        garbageAudioController.pitchforkUp.Play();
                        pitchforkUpAudioTimer.Run(2f);
                        actualAnimationPitchforkPiece = AnimationPitchforkPiece.PitchforkUp;
                        nextMaxVerticalAngle = defaultMaxVerticalAngle;
                        if (vehicleModesHandler.mode == VehicleMode.Player)
                        {
                            animationPitchforkTimer.Run(2f);
                        }
                        if (vehicleModesHandler.mode == VehicleMode.AI)
                        {
                            PitchforkIsDown = false;
                        }
                    });
                }
                else
                {
                    pitchforkLifting = false;
                    animator.enabled = true;
                    canLiftContainer = true;
                    animator.SetInteger("PitchforkState", 2);
                    if (animatorEnableTimer.GetEndTime() <= Time.time + 2f + 0.2f)
                    {
                        animatorEnableTimer.Run(2.2f);
                    }
                    garbageAudioController.pitchforkUp.Play();
                    pitchforkUpAudioTimer.Run(2f);
                    actualAnimationPitchforkPiece = AnimationPitchforkPiece.PitchforkUp;
                    nextMaxVerticalAngle = defaultMaxVerticalAngle;
                    if (vehicleModesHandler.mode == VehicleMode.Player)
                    {
                        animationPitchforkTimer.Run(2f);
                    }
                    if (vehicleModesHandler.mode == VehicleMode.AI)
                    {
                        PitchforkIsDown = false;
                    }
                }
            }
        }

        public void StartDump(bool isPlayer = false)
        {
            if (isPlayer)
            {
                CallAdsManager.ShowInterstial(CallAdsManager.PositionAds.gameplay_vehicle_dump, () =>
                {
                    animator.enabled = true;
                    animator.SetInteger("CargoState", 5);
                    garbageAudioController.cargoUp.Play();
                    animationCargoTimer.Run(3f);
                    if (animatorEnableTimer.GetEndTime() <= Time.time + 3f + 0.2f)
                    {
                        animatorEnableTimer.Run(3.2f);
                    }
                    cargoUpAudioTimer.Run(3f);
                    checkerTimer.Run(3f);
                    startDumpingTime = Time.time;
                    actualCapacity = AllCapacity;
                    actualAnimationCargoPiece = AnimationCargoPiece.CargoUp;
                    CargoIsDown = false;
                    CargoIsUp = false;
                    if (containerDetector.ActualContainer != null)
                    {
                        waitingForCollect = true;
                        liftedContainer = containerDetector.ActualContainer;
                        liftedContainer.SetParent(containerPosition.transform);
                        liftedContainer.DeactivateEmmiters();
                    }
                    if (lifting)
                    {
                        waitingForCollect = true;
                    }
                });
            }
            else
            {
                animator.enabled = true;
                animator.SetInteger("CargoState", 5);
                garbageAudioController.cargoUp.Play();
                animationCargoTimer.Run(3f);
                if (animatorEnableTimer.GetEndTime() <= Time.time + 3f + 0.2f)
                {
                    animatorEnableTimer.Run(3.2f);
                }
                cargoUpAudioTimer.Run(3f);
                checkerTimer.Run(3f);
                startDumpingTime = Time.time;
                actualCapacity = AllCapacity;
                actualAnimationCargoPiece = AnimationCargoPiece.CargoUp;
                CargoIsDown = false;
                CargoIsUp = false;
                if (containerDetector.ActualContainer != null)
                {
                    waitingForCollect = true;
                    liftedContainer = containerDetector.ActualContainer;
                    liftedContainer.SetParent(containerPosition.transform);
                    liftedContainer.DeactivateEmmiters();
                }
                if (lifting)
                {
                    waitingForCollect = true;
                }
            }
        }

        public void EndDump(bool isPlayer = false)
        {
            if (!cargoMovingDown)
            {
                if (isPlayer)
                {
                    CallAdsManager.ShowInterstial(CallAdsManager.PositionAds.gameplay_vehicle_dump, () =>
                    {
                        CargoIsUp = false;
                        cargoMovingDown = true;
                        endStartDumpingTime = Time.time;
                        animator.enabled = true;
                        animator.SetInteger("CargoState", 6);
                        animationCargoTimer.Run(DumpingTime);
                        if (animatorEnableTimer.GetEndTime() <= Time.time + DumpingTime + 0.2f)
                        {
                            animatorEnableTimer.Run(DumpingTime + 0.2f);
                        }
                        garbageAudioController.cargoDown.Play();
                        cargoDownAudioTimer.Run(DumpingTime);
                        trashDelay = 0f;
                        checkerTimer.Stop();
                        actualAnimationCargoPiece = AnimationCargoPiece.CargoDown;
                        checkerTimer.Stop();
                        for (int i = 0; i < effects.Count; i++)
                        {
                            effects[i].Stop();
                        }
                    });
                }
                else
                {
                    CargoIsUp = false;
                    cargoMovingDown = true;
                    endStartDumpingTime = Time.time;
                    animator.enabled = true;
                    animator.SetInteger("CargoState", 6);
                    animationCargoTimer.Run(DumpingTime);
                    if (animatorEnableTimer.GetEndTime() <= Time.time + DumpingTime + 0.2f)
                    {
                        animatorEnableTimer.Run(DumpingTime + 0.2f);
                    }
                    garbageAudioController.cargoDown.Play();
                    cargoDownAudioTimer.Run(DumpingTime);
                    trashDelay = 0f;
                    checkerTimer.Stop();
                    actualAnimationCargoPiece = AnimationCargoPiece.CargoDown;
                    checkerTimer.Stop();
                    for (int i = 0; i < effects.Count; i++)
                    {
                        effects[i].Stop();
                    }
                }
            }
        }

        public bool CanLift()
        {
            if (containerDetector.ActualContainer != null || liftedContainer != null)
            {
                return PitchforkIsDown;
            }
            return false;
        }

        public bool CanPitchforkUp()
        {
            if (PitchforkIsDown && !CanLift())
            {
                return canLiftContainer;
            }
            return false;
        }

        public bool CanPitchforkDown()
        {
            if (!PitchforkIsDown && !CanLift())
            {
                return canLiftContainer;
            }
            return false;
        }

        public bool CanDumpStart()
        {
            return CargoIsDown;
        }

        public bool CanDumpEnd()
        {
            return !CargoIsDown;
        }

        public void ResetCapacity()
        {
            AllCapacity = 0;
        }
    }
}
