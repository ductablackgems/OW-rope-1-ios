using UnityEngine;
using UnityEngine.UI;

namespace ThirdPersonCamera.DemoSceneScripts
{
    public class UIHandling : MonoBehaviour
    {
        public Toggle optionCameraEnabled;
        public Toggle optionSmartPivot;
        public Toggle optionOcclusionCheck;
        public Toggle optionThicknessCheck;
        public Toggle optionControllerInvertY;
        public Toggle optionMouseInvertY;

        public InputField inputDesiredDistance;
        public InputField inputMaxThickness;
        public InputField inputZoomOutStepValue;
        public InputField inputCollisionDistance;

        public Slider mouseSliderX;
        public Slider mouseSliderY;
        public Slider controllerSliderX;
        public Slider controllerSliderY;

        private const string ColliderName = "Collider";

        private CameraController cc;
        private FreeForm freeForm;
        
#if ENABLE_INPUT_SYSTEM
        private CameraInputSampling cameraInput;
#else
        private CameraInputSampling_FreeForm cameraInput;
#endif

        private bool ignoreChanges;

        private void Awake()
        {
            ignoreChanges = true;

            cc = FindObjectOfType<CameraController>();
#if ENABLE_INPUT_SYSTEM
            cameraInput = FindObjectOfType<CameraInputSampling>();
#else
            cameraInput = FindObjectOfType<CameraInputSampling_FreeForm>();
#endif
            freeForm = FindObjectOfType<FreeForm>();

            if (cc != null)
            {
                optionSmartPivot.isOn = cc.SmartPivot;
                optionOcclusionCheck.isOn = cc.OcclusionCheck;

                inputDesiredDistance.text = cc.DesiredDistance.ToString();
                inputMaxThickness.text = cc.MaxThickness.ToString();
                inputZoomOutStepValue.text = cc.ZoomOutStepValue.ToString();
                inputCollisionDistance.text = cc.CollisionDistance.ToString();

                optionThicknessCheck.isOn = cc.ThicknessCheck;
            }
            
#if ENABLE_INPUT_SYSTEM
            if (cameraInput != null)
            {
                ref var input = ref cameraInput.GetInput();
                
                optionCameraEnabled.isOn = freeForm.CameraEnabled;
                optionControllerInvertY.isOn = input.InputFreeForm.ControllerInvertY;
                optionMouseInvertY.isOn = input.InputFreeForm.MouseInvertY;

                mouseSliderX.value = input.InputFreeForm.MouseSensitivity.x;
                mouseSliderY.value = input.InputFreeForm.MouseSensitivity.y;

                controllerSliderX.value = input.InputFreeForm.ControllerSensitivity.x;
                controllerSliderY.value = input.InputFreeForm.ControllerSensitivity.y;
            }
#else

            if (cameraInput != null)
            {
                optionCameraEnabled.isOn = freeForm.CameraEnabled;
                optionControllerEnabled.isOn = cameraInput.controllerEnabled;
                optionControllerInvertY.isOn = cameraInput.controllerInvertY;
                optionMouseInvertY.isOn = cameraInput.mouseInvertY;

                if (cameraInput.cameraMode == CameraMode.Input)
                    optionCameraMode.value = 0;
                else
                    optionCameraMode.value = 1;

                mouseSliderX.value = cameraInput.mouseSensitivity.x;
                mouseSliderY.value = cameraInput.mouseSensitivity.y;

                controllerSliderX.value = cameraInput.controllerSensitivity.x;
                controllerSliderY.value = cameraInput.controllerSensitivity.y;
            }
#endif

            ignoreChanges = false;
        }

        public void HandleUI()
        {
            if (ignoreChanges)
                return;

            cc.SmartPivot = optionSmartPivot.isOn;

            if (!cc.SmartPivot)
            {
                cc.CameraNormalMode = true;
            }

            cc.OcclusionCheck = optionOcclusionCheck.isOn;
            cc.ThicknessCheck = optionThicknessCheck.isOn;

            if (float.TryParse(inputDesiredDistance.text, out var newDistance))
                cc.DesiredDistance = newDistance;

            if (float.TryParse(inputMaxThickness.text, out var newThickness))
                cc.MaxThickness = newThickness;

            if (float.TryParse(inputZoomOutStepValue.text, out var newStep))
                cc.ZoomOutStepValue = newStep;

            if (float.TryParse(inputCollisionDistance.text, out var newCd))
                cc.CollisionDistance = newCd;

#if ENABLE_INPUT_SYSTEM
            if (cameraInput != null)
            {
                ref var input = ref cameraInput.GetInput();
                
                input.InputFreeForm.ControllerInvertY = optionControllerInvertY.isOn;
                input.InputFreeForm.MouseInvertY = optionMouseInvertY.isOn;

                freeForm.CameraEnabled = optionCameraEnabled.isOn;
                input.InputFreeForm.MouseSensitivity.x = mouseSliderX.value;
                input.InputFreeForm.MouseSensitivity.y = mouseSliderY.value;

                input.InputFreeForm.ControllerSensitivity.x = controllerSliderX.value;
                input.InputFreeForm.ControllerSensitivity.y = controllerSliderY.value;
            }
#else
            if (cameraInput != null)
            {
                if (optionCameraMode.value == 0)
                    cameraInput.cameraMode = CameraMode.Input;
                else
                    cameraInput.cameraMode = CameraMode.Always;

                cameraInput.controllerEnabled = optionControllerEnabled.isOn;
                cameraInput.controllerInvertY = optionControllerInvertY.isOn;
                cameraInput.mouseInvertY = optionMouseInvertY.isOn;

                freeForm.CameraEnabled = optionCameraEnabled.isOn;
                cameraInput.mouseSensitivity.x = mouseSliderX.value;
                cameraInput.mouseSensitivity.y = mouseSliderY.value;

                cameraInput.controllerSensitivity.x = controllerSliderX.value;
                cameraInput.controllerSensitivity.y = controllerSliderY.value;
            }
#endif
        }

        public void Update()
        {
            if (cameraInput != null)
            {
                RaycastHit2D hit = Physics2D.Raycast(new Vector2(Input.mousePosition.x, Input.mousePosition.y), Vector2.zero);

                if (hit.collider != null && hit.collider.name == ColliderName)
                {
                    freeForm.CameraEnabled = false;
                    optionCameraEnabled.isOn = false;
                }
                else
                {
                    freeForm.CameraEnabled = true;
                    optionCameraEnabled.isOn = true;
                }

                if (Input.GetKey(KeyCode.Escape))
                {
                    Cursor.visible = true;
                    Cursor.lockState = CursorLockMode.None;
                }
            }

            ignoreChanges = true;
            inputDesiredDistance.text = cc.DesiredDistance.ToString();
            ignoreChanges = false;
        }
    }
}