using System;
using _0.DucLib.Scripts.Common;
using _0.DucLib.Scripts.Machine;
using _0.OW.Scripts.Player.PlayerState;
using KinematicCharacterController.Examples;
using ThirdPersonCamera;
using UnityEngine;

namespace _0.OW.Scripts.Player
{
    public class OW_PlayerMovement : StateMachine
    {
        [SerializeField] private CameraController CameraController;
        
        private static readonly int Forward = Animator.StringToHash("Forward");
        private static readonly int OnGround = Animator.StringToHash("OnGround");
        
        private ExampleCharacterController Character;
        private Animator Animator;

        private Vector2 direction;
        private const string HorizontalInput = "Horizontal";
        private const string VerticalInput = "Vertical";
        public void OnJoyMove(Vector2 assist)
        {
            direction = assist;
        }

        /// <summary>
        /// Hàm được gọi khi joystick ngừng hoạt động.
        /// </summary>
        public void OnJoyEnd()
        {
            direction = Vector2.zero;
        }

        private void Awake()
        {
            Character = GetComponent<ExampleCharacterController>();
            Animator = GetComponent<Animator>();

            if (CameraController == null)
            {
                CameraController = FindObjectOfType<CameraController>();

                if (CameraController == null)
                {
                    enabled = false;
                }
            }
        }

        private void FixedUpdate()
        {
            HandleCharacterInput();
        }

        public void StopMovement()
        {
           
        }

        public void ResumeMovement()
        {
           
        }
        private void HandleCharacterInput()
        {
            Vector2 combinedInput = new Vector2(
                direction.x + Input.GetAxisRaw(HorizontalInput),
                direction.y + Input.GetAxisRaw(VerticalInput)
            ).normalized;

            if(OWManager.instance.playerStop) combinedInput = Vector2.zero;
            if (combinedInput != Vector2.zero)
            {
                if (Character.MaxStableMoveSpeed < Character.MaxMoveSpeed)
                {
                    Character.MaxStableMoveSpeed += (Time.deltaTime * 2);
                    ChangeState<PlayerWalk>();
                }
                else
                {
                    ChangeState<PlayerRun>();
                }
               
            }
            else
            {
                Character.MaxStableMoveSpeed = Character.MaxStableMoveSpeedBase;
                ChangeState<PlayerIdle>();
            }
            PlayerCharacterInputs characterInputs = new PlayerCharacterInputs
            {
                // Cấu trúc CharacterInputs
                MoveAxisForward = combinedInput.y,
                MoveAxisRight = combinedInput.x,
                JumpDown = Input.GetKeyDown(KeyCode.Space),
                CrouchDown = Input.GetKeyDown(KeyCode.C),
                CrouchUp = Input.GetKeyUp(KeyCode.C),
                CameraRotation = CameraController.transform.rotation
            };

            Character.SetInputs(ref characterInputs);
            
        }

      
    }
}