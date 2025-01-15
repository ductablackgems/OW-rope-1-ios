using KinematicCharacterController.Examples;
using UnityEngine;

namespace ThirdPersonCamera.Demo
{
    [RequireComponent(typeof(ExampleCharacterController))]
    [RequireComponent(typeof(Animator))]
    public class TPC_ExamplePlayer : MonoBehaviour
    {
        public CameraController CameraController;
        
        private static readonly int Forward = Animator.StringToHash("Forward");
        private static readonly int OnGround = Animator.StringToHash("OnGround");
        
        private ExampleCharacterController Character;
        private Animator Animator;

        private const string HorizontalInput = "Horizontal";
        private const string VerticalInput = "Vertical";

        private void Awake()
        {
            Character = GetComponent<ExampleCharacterController>();
            Animator = GetComponent<Animator>();

            if (CameraController == null)
            {
                CameraController = FindObjectOfType<CameraController>();

                if (CameraController == null)
                {
                    Debug.LogError("No CameraController found!");
                    enabled = false;
                }
            }
        }

        private void Update()
        {
            HandleCharacterInput();
            
            UpdateAnimator();
        }

        private void HandleCharacterInput()
        {
            PlayerCharacterInputs characterInputs = new PlayerCharacterInputs
            {
                // Build the CharacterInputs struct
                MoveAxisForward = Input.GetAxisRaw(VerticalInput),
                MoveAxisRight = Input.GetAxisRaw(HorizontalInput),
                JumpDown = Input.GetKeyDown(KeyCode.Space),
                CrouchDown = Input.GetKeyDown(KeyCode.C),
                CrouchUp = Input.GetKeyUp(KeyCode.C),
                CameraRotation = CameraController.transform.rotation
            };

            // Apply inputs to character
            Character.SetInputs(ref characterInputs);
        }

        private void UpdateAnimator()
        {
            // var isGrounded = Character.Motor.GroundingStatus.IsStableOnGround;
            // var forwardAmount = Character.Motor.BaseVelocity.magnitude;
            //
            // // update the animator parameters
            // Animator.SetFloat(Forward, forwardAmount, 0.1f, Time.deltaTime);
            // Animator.SetBool(OnGround, isGrounded);
        }
    }
}