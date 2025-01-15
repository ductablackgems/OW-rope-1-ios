using UnityEngine;

namespace ThirdPersonCamera.DemoSceneScripts
{
    public class BallController : MonoBehaviour
    {
        public float speed = 500;
        public float jumpStrength = 1000;
        public Transform parent;
        public LayerMask layerMask;

        private const string CHAxis = "Horizontal";
        private const string CVAxis = "Vertical";

        private Rigidbody rb;
        private Vector3 movement;
        private bool inputJump;
        public Vector3 forwardVector;
        private bool grounded;
        private Camera mainCamera;

        private void Start()
        {
            mainCamera = Camera.main;
            inputJump = false;
            movement = Vector3.zero;
            grounded = false;

            rb = GetComponent<Rigidbody>();
            forwardVector = transform.forward;
            parent.transform.rotation = Quaternion.LookRotation(forwardVector);
        }

        private void FixedUpdate()
        {
            float moveHorizontal = Input.GetAxis(CHAxis);
            float moveVertical = Input.GetAxis(CVAxis);

            Vector3 tmpForward = mainCamera.transform.forward;
            tmpForward.y = 0;
            
            movement = Quaternion.LookRotation(tmpForward) * new Vector3(moveHorizontal, 0.0f, moveVertical);

            rb.AddForce(movement * speed);

            grounded = Physics.Raycast(transform.position, Vector3.down, 0.6f, layerMask);

            if (inputJump && grounded)
            {
                rb.AddForce(Vector3.up * jumpStrength);
                inputJump = false;
            }
            else
            {
                inputJump = false;
            }
        }

        private void Update()
        {
            parent.transform.position = transform.position;

            if (Input.GetKeyUp(KeyCode.Space))
            {
                inputJump = true;
            }

            if (Input.GetKeyUp(KeyCode.Q))
            {
                forwardVector = Quaternion.AngleAxis(-90.0f, Vector3.up) * forwardVector;
                parent.transform.rotation = Quaternion.LookRotation(forwardVector);
            }
            
            if (Input.GetKeyUp(KeyCode.E))
            {
                forwardVector = Quaternion.AngleAxis(90.0f, Vector3.up) * forwardVector;
                parent.transform.rotation = Quaternion.LookRotation(forwardVector);
            }
        }
    }
}