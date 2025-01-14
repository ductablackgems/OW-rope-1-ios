using App.Player.Definition;
using App.Player.FightSystem.Definition;
using App.Util;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace App.Player.FightSystem
{
    public class AdvancedFightController : MonoBehaviour, ICharacterModule
    {
        public GameObject[] effects;

        public GameObject[] effectPositions;

        public AudioClip[] shouts;

        public string[] magicShieldNames;

        public bool isPlayer;

        public float castRadius = 1.7f;

        public float lostTargetDistance = 2.2f;

        public float rotateTowardsTargetSpeed = 10f;

        [Space]
        public FightMovementDefinitions movementDefinitions;

        public string[] movementTids = new string[1];

        public List<string> magicTids;

        [Space]
        public AudioSource audioSource;

        public FightSoundsScriptableObject fightSounds;

        [Space]
        public int debugIndex = -1;

        private Animator animator;

        private RigidbodyHelper rigidbodyHelper;

        private PlayerAnimatorHandler animatorHandler;

        private RagdollHelper ragdollHelper;

        private HitHandler hitHandler;

        private CharacterControl characterControl;

        private AttackZone attackZone;

        private AnimationSimulator animationSimulator;

        private FightMovementHandler movementHandler;

        private FightMovementDefinition[] definitions;

        private bool running;

        private Transform targetEnemy;

        private Transform targetBone;

        private Health targetHealth;

        private RagdollHelper targetRagdollHelper;

        private Health victimHealth;

        private HitHandler victimHitHandler;

        private DurationTimer checkTargetTimer = new DurationTimer();

        private DurationTimer fastStopTimer = new DurationTimer();

        private DurationTimer slowStopTimer = new DurationTimer();

        private DurationTimer attackButtonTimer = new DurationTimer();

        public event Action OnStartCombo;

        public void OnSwing()
        {
            if (audioSource != null)
            {
                audioSource.PlayOneShot(fightSounds.missClips[UnityEngine.Random.Range(0, fightSounds.missClips.Length)]);
            }
        }

        public void OnStrike(int index)
        {
            if (hitHandler.Running() || movementHandler.Definition == null)
            {
                return;
            }
            bool flag = false;
            if (movementHandler.Definition.useCustomHit)
            {
                FightHitDefinition fightHitDefinition = movementHandler.Definition.hits[index];
                victimHealth.ApplyDamage(fightHitDefinition.damage, 1, base.gameObject);
                flag = true;
            }
            else if (attackZone.IsIn)
            {
                FightHitDefinition fightHitDefinition2 = movementHandler.Definition.hits[index];
                HitHandler component = attackZone.TargetHealth.GetComponent<HitHandler>();
                flag = (component == null || component.HandleHit(fightHitDefinition2, base.transform));
                if (flag)
                {
                    attackZone.TargetHealth.ApplyDamage(fightHitDefinition2.damage, 1, base.gameObject);
                }
            }
            if (audioSource != null)
            {
                if (flag)
                {
                    audioSource.PlayOneShot(fightSounds.strikeClips[UnityEngine.Random.Range(0, fightSounds.strikeClips.Length)]);
                }
                else
                {
                    audioSource.PlayOneShot(fightSounds.missClips[UnityEngine.Random.Range(0, fightSounds.missClips.Length)]);
                }
            }
        }

        public void OnEffectStart(string info)
        {
            MagicShield component = GetComponent<MagicShield>();
            if (!(component == null))
            {
                component.ActivateShockWave();
            }
        }

        public void OnSoundStart(int index)
        {
            if (shouts != null && shouts.Length != 0 && index < shouts.Length)
            {
                audioSource.PlayOneShot(shouts[index]);
            }
        }

        public void Run()
        {
            if (!running)
            {
                running = true;
                checkTargetTimer.Run(0.5f);
                if (isPlayer)
                {
                    UpdateTarget();
                }
            }
        }

        public bool Running()
        {
            return running;
        }

        public void Stop()
        {
            if (running)
            {
                running = false;
                if (movementHandler.Definition != null && movementHandler.Definition.useCustomHit)
                {
                    victimHitHandler.Interrupt();
                }
                movementHandler.Clear();
                if (!ragdollHelper.Ragdolled && !hitHandler.Running())
                {
                    animatorHandler.GroundedState.RunCrossFixed(0.25f);
                }
            }
        }

        public void Control(bool attackPressed, bool attackPressedDown = false, bool isMagic = false)
        {
            if (!running)
            {
                return;
            }
            if (isPlayer)
            {
                if (attackPressedDown)
                {
                    attackButtonTimer.Run(0.3f);
                }
                if (!attackPressed && attackButtonTimer.InProgress())
                {
                    attackPressed = true;
                }
            }
            animatorHandler.ContinueFightCombo = attackPressed;
            if (hitHandler.Running())
            {
                if (movementHandler.Definition != null && movementHandler.Definition.useCustomHit)
                {
                    victimHitHandler.Interrupt();
                }
                movementHandler.Clear();
            }
            if (movementHandler.Definition != null)
            {
                movementHandler.UpdateStates();
            }
            if (isPlayer)
            {
                if (!checkTargetTimer.Running())
                {
                    checkTargetTimer.Run(0.5f);
                }
                else if (checkTargetTimer.Done())
                {
                    checkTargetTimer.Run(0.5f);
                    UpdateTarget();
                }
            }
            if (targetEnemy != null && (movementHandler.AnimationDefinition == null || !movementHandler.AnimationDefinition.kinematic))
            {
                float num = (movementHandler.AnimationDefinition == null) ? 1f : ((!movementHandler.AnimationDefinition.preventYRotation) ? 0.3f : 0.1f);
                Transform transform = targetRagdollHelper.VeryRagdolled ? targetBone : targetEnemy;
                Quaternion to = Quaternion.LookRotation(transform.transform.position - new Vector3(base.transform.position.x, transform.transform.position.y, base.transform.position.z));
                base.transform.rotation = Quaternion.RotateTowards(base.transform.rotation, to, rotateTowardsTargetSpeed * Time.deltaTime * num);
            }
            if (attackPressed && (!hitHandler.Running() || hitHandler.MustExit()) && (characterControl.Grounded | isMagic) && movementHandler.Definition == null)
            {
                if (hitHandler.Running())
                {
                    hitHandler.Clear();
                }
                RunRandomMovement(isMagic);
            }
            else if (movementHandler.Definition != null && movementHandler.MustExit())
            {
                movementHandler.Clear();
                animatorHandler.GroundedState.RunCrossFixed(0.25f);
            }
            if (isPlayer)
            {
                UpdateStopping();
            }
        }

        public bool NeedStrafeWalk()
        {
            if (running)
            {
                return targetEnemy != null;
            }
            return false;
        }

        public bool RunningCustomMovement()
        {
            if (running && movementHandler.Definition != null)
            {
                return movementHandler.Definition.useCustomHit;
            }
            return false;
        }

        public void UpdateStrafe(Vector3 cameraForward, float horizontalAxis, float verticalAxis)
        {
            Vector3 point = new Vector3(horizontalAxis, 0f, verticalAxis);
            Vector3 vector = base.transform.InverseTransformDirection(Quaternion.LookRotation(cameraForward) * point);
            animatorHandler.StrafeX.BlendTo(vector.x);
            animatorHandler.StrafeY.BlendTo(vector.z);
        }

        private void Awake()
        {
            rigidbodyHelper = this.GetComponentSafe<RigidbodyHelper>();
            animatorHandler = this.GetComponentSafe<PlayerAnimatorHandler>();
            animator = this.GetComponentSafe<Animator>();
            ragdollHelper = this.GetComponentSafe<RagdollHelper>();
            hitHandler = this.GetComponentSafe<HitHandler>();
            characterControl = this.GetComponentSafe<CharacterControl>();
            attackZone = this.GetComponentInChildrenSafe<AttackZone>();
            animationSimulator = ServiceLocator.Get<AnimationSimulator>(showError: false);
            Health componentInChildrenSafe = this.GetComponentInChildrenSafe<Health>();
            movementHandler = new FightMovementHandler(animator, componentInChildrenSafe);
            definitions = new FightMovementDefinition[movementTids.Length + magicTids.Count];
            int i;
            for (i = 0; i < movementTids.Length; i++)
            {
                definitions[i] = movementDefinitions.definitions.First((FightMovementDefinition definition) => definition.tid.Equals(movementTids[i]));
            }
            int j;
            for (j = 0; j < magicTids.Count; j++)
            {
                definitions[movementTids.Length + j] = movementDefinitions.definitions.First((FightMovementDefinition definition) => definition.tid.Equals(magicTids[j]));
            }
            movementHandler.OnAnimationChange += OnAnimationChange;
        }

        private void OnDestroy()
        {
            movementHandler.OnAnimationChange -= OnAnimationChange;
        }

        private void LateUpdate()
        {
            if (movementHandler.AnimationDefinition != null && movementHandler.AnimationDefinition.kinematic)
            {
                base.transform.position = animationSimulator.animator.transform.position;
                base.transform.rotation = animationSimulator.animator.transform.rotation;
            }
        }

        private void OnDrawGizmos()
        {
            if (isPlayer)
            {
                Gizmos.color = Color.yellow;
                Gizmos.DrawWireSphere(base.transform.position + Vector3.up, castRadius);
            }
        }

        private void RunRandomMovement(bool isMagic = false)
        {
            attackButtonTimer.Stop();
            FightMovementDefinition fightMovementDefinition = null;
            float num = (attackZone.IsIn && attackZone.TargetHealth.GetCurrentHealthNumeric() <= 50f) ? 0.7f : 0.3f;
            if (isMagic)
            {
                num = 0f;
            }
            if (isPlayer && attackZone.IsIn && (UnityEngine.Random.Range(0f, 1f) < num || debugIndex != -1) && (movementHandler.Definition == null || !movementHandler.Definition.useCustomHit) && attackZone.TargetHealth.CompareTag("Enemy"))
            {
                HitHandler componentSafe = attackZone.TargetHealth.GetComponentSafe<HitHandler>();
                RagdollHelper componentSafe2 = attackZone.TargetHealth.GetComponentSafe<RagdollHelper>();
                if (!componentSafe.WillRagdoll() && !componentSafe2.Ragdolled)
                {
                    int num2 = UnityEngine.Random.Range(0, movementDefinitions.grabDefinitions.Length - magicTids.Count);
                    if (debugIndex != -1)
                    {
                        num2 = debugIndex;
                    }
                    fightMovementDefinition = ((!movementDefinitions.grabDefinitions[num2].Equals(movementHandler.LastDefinition) || debugIndex != -1) ? movementDefinitions.grabDefinitions[num2] : ((num2 != 0) ? movementDefinitions.grabDefinitions[num2 - 1] : movementDefinitions.grabDefinitions[movementDefinitions.grabDefinitions.Length - 1]));
                    Transform transform1 = animationSimulator.animator.transform;
                    Transform transform2 = animationSimulator.victimAnimator.transform;
                    transform2.position = attackZone.TargetHealth.transform.position;
                    transform1.position = base.transform.position;
                    transform1.position = new Vector3(transform1.position.x, transform2.position.y, transform1.position.z);
                    transform2.LookAt(transform1);
                    transform1.LookAt(transform2);
                    transform2.position = transform1.position + transform1.forward;
                    animationSimulator.cameraAnchor.position = transform1.position + transform1.forward / 2f;
                    animationSimulator.cameraAnchor.rotation = transform1.rotation;
                    if (fightMovementDefinition.testSpace)
                    {
                        Vector3 point = animationSimulator.cameraAnchor.position + animationSimulator.cameraAnchor.TransformDirection(fightMovementDefinition.testSpaceCapsule.start);
                        Vector3 point2 = animationSimulator.cameraAnchor.position + animationSimulator.cameraAnchor.TransformDirection(fightMovementDefinition.testSpaceCapsule.end);
                        Collider[] array = Physics.OverlapCapsule(point, point2, fightMovementDefinition.testSpaceCapsule.radius);
                        foreach (Collider collider in array)
                        {
                            if (collider.gameObject.isStatic || WhoIs.Compare(collider, WhoIs.Entities.Vehicle))
                            {
                                fightMovementDefinition = null;
                                break;
                            }
                        }
                    }
                }
            }
            if (fightMovementDefinition == null)
            {
                int num3 = UnityEngine.Random.Range(0, definitions.Length - magicTids.Count);
                if (isMagic)
                {
                    num3 = UnityEngine.Random.Range(definitions.Length - magicTids.Count, definitions.Length);
                }
                fightMovementDefinition = ((!definitions[num3].Equals(movementHandler.LastDefinition)) ? definitions[num3] : ((num3 != 0) ? definitions[num3 - 1] : definitions[definitions.Length - 1]));
                if (!IsMagicIndex(fightMovementDefinition.tid) && isMagic)
                {
                    num3 = UnityEngine.Random.Range(definitions.Length - magicTids.Count, definitions.Length);
                    fightMovementDefinition = definitions[num3];
                }
            }
            if (fightMovementDefinition.useCustomHit)
            {
                victimHealth = attackZone.TargetHealth;
                victimHitHandler = attackZone.TargetHealth.GetComponentSafe<HitHandler>();
                victimHitHandler.RunCustomMovement(fightMovementDefinition.customHitTid);
                base.transform.position = animationSimulator.animator.transform.position;
                base.transform.rotation = animationSimulator.animator.transform.rotation;
                Transform transform3 = attackZone.TargetHealth.transform;
                transform3.position = animationSimulator.victimAnimator.transform.position;
                transform3.rotation = animationSimulator.victimAnimator.transform.rotation;
            }
            if ((bool)GetComponent<MagicShield>())
            {
                for (int j = 0; j < magicShieldNames.Length; j++)
                {
                    if (fightMovementDefinition.tid == magicShieldNames[j])
                    {
                        GetComponent<MagicShield>().ActivateLightningStorm();
                        return;
                    }
                }
            }
            movementHandler.Run(fightMovementDefinition);
            if (fightMovementDefinition.animations.Length > 1 && this.OnStartCombo != null)
            {
                this.OnStartCombo();
            }
        }

        private bool IsMagicIndex(string tid)
        {
            for (int i = 0; i < magicTids.Count; i++)
            {
                if (magicTids[i] == tid)
                {
                    return true;
                }
            }
            return false;
        }

        private void UpdateTarget()
        {
            Collider[] array = Physics.OverlapSphere(base.transform.position + Vector3.up, castRadius, 4096);
            float num = (targetEnemy == null) ? 0f : Vector3.Distance(base.transform.position, targetBone.position);
            bool flag = !(targetEnemy == null) && targetEnemy.GetComponentSafe<Health>().Dead();
            if ((num >= lostTargetDistance) | flag)
            {
                targetEnemy = null;
                targetBone = null;
                num = 0f;
            }
            bool flag2 = false;
            Collider[] array2 = array;
            foreach (Collider collider in array2)
            {
                WhoIsResult whoIsResult = WhoIs.Resolve(collider, WhoIs.Entities.Enemy);
                Health health = whoIsResult.IsEmpty ? null : whoIsResult.transform.GetComponentSafe<Health>();
                if (!whoIsResult.IsEmpty && !health.Dead())
                {
                    float num2 = Vector3.Distance(base.transform.position, collider.transform.position);
                    if (targetEnemy == null || num > num2)
                    {
                        flag2 = true;
                        targetEnemy = collider.transform;
                        targetHealth = health;
                        num = num2;
                    }
                }
            }
            if (flag2)
            {
                targetBone = targetHealth.GetTargetBone();
                targetRagdollHelper = targetEnemy.GetComponentSafe<RagdollHelper>();
            }
        }

        private void UpdateStopping()
        {
            if (movementHandler.Definition == null)
            {
                if (targetEnemy == null)
                {
                    if (!fastStopTimer.Running())
                    {
                        fastStopTimer.Run(4f);
                    }
                }
                else
                {
                    fastStopTimer.Stop();
                }
                if (!slowStopTimer.Running())
                {
                    slowStopTimer.Run(9f);
                }
                if (fastStopTimer.Done() || slowStopTimer.Done())
                {
                    Stop();
                }
            }
            else
            {
                fastStopTimer.Stop();
                slowStopTimer.Stop();
            }
        }

        private void OnAnimationChange(AnimationDefinition oldAnimation, AnimationDefinition newAnimation)
        {
            if (oldAnimation != null && oldAnimation.kinematic)
            {
                animationSimulator.StopSimulation();
            }
            if (newAnimation != null)
            {
                rigidbodyHelper.SetKinematic(newAnimation.kinematic, this);
                if (newAnimation.kinematic)
                {
                    animationSimulator.RunSimulation(newAnimation.hash);
                }
            }
            else
            {
                rigidbodyHelper.SetKinematic(kinematic: false, this);
            }
        }
    }
}
