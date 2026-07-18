
#if DistortionsX_HAS_URP && DistortionsX_HAS_INPUT_SYSTEM
using UnityEngine;
using UnityEngine.InputSystem;

namespace DistortionsPro_20X
{
    [RequireComponent(typeof(CharacterController))]
    public sealed class FirstPersonController : MonoBehaviour
    {
        [Header("Movement")]
        [SerializeField] float moveSpeed = 5f;
        [SerializeField] float jumpForce = 5f;   // treated as jump velocity, same behavior as your old script
        [SerializeField] float gravity = -9.81f;

        [Header("Look")]
        [SerializeField] float mouseSensitivity = 0.08f;
        [SerializeField] float gamepadLookSpeed = 180f;
        [SerializeField] Transform cameraTransform;
        [SerializeField] float minPitch = -80f;
        [SerializeField] float maxPitch = 80f;
        [SerializeField] bool lockCursorOnStart = true;

        float pitch;
        CharacterController cc;
        Vector3 velocity;

        InputAction moveAction;
        InputAction lookAction;
        InputAction jumpAction;

        void Awake()
        {
            cc = GetComponent<CharacterController>();

            if (cameraTransform == null)
            {
                Camera mainCam = Camera.main;
                if (mainCam != null)
                    cameraTransform = mainCam.transform;
            }

            if (cameraTransform != null)
                pitch = NormalizePitch(cameraTransform.localEulerAngles.x);

            CreateActions();

            if (lockCursorOnStart)
                SetCursorLocked(true);
        }

        void OnEnable()
        {
            moveAction?.Enable();
            lookAction?.Enable();
            jumpAction?.Enable();
        }

        void OnDisable()
        {
            moveAction?.Disable();
            lookAction?.Disable();
            jumpAction?.Disable();

            if (lockCursorOnStart)
                SetCursorLocked(false);
        }

        void OnDestroy()
        {
            moveAction?.Dispose();
            lookAction?.Dispose();
            jumpAction?.Dispose();
        }

        void Update()
        {
            if (cameraTransform == null)
                return;

            HandleCursorToggle();
            HandleLook();
            HandleMovement();
        }

        void CreateActions()
        {
            if (moveAction != null)
                return;

            moveAction = new InputAction("Move", InputActionType.Value);
            moveAction.AddCompositeBinding("2DVector")
                .With("Up", "<Keyboard>/w")
                .With("Down", "<Keyboard>/s")
                .With("Left", "<Keyboard>/a")
                .With("Right", "<Keyboard>/d");
            moveAction.AddCompositeBinding("2DVector")
                .With("Up", "<Keyboard>/upArrow")
                .With("Down", "<Keyboard>/downArrow")
                .With("Left", "<Keyboard>/leftArrow")
                .With("Right", "<Keyboard>/rightArrow");
            moveAction.AddBinding("<Gamepad>/leftStick");

            lookAction = new InputAction("Look", InputActionType.Value);
            lookAction.AddBinding("<Mouse>/delta");
            lookAction.AddBinding("<Gamepad>/rightStick");

            jumpAction = new InputAction("Jump", InputActionType.Button);
            jumpAction.AddBinding("<Keyboard>/space");
            jumpAction.AddBinding("<Gamepad>/buttonSouth");
        }

        void HandleCursorToggle()
        {
            if (!lockCursorOnStart)
                return;

            if (Keyboard.current != null && Keyboard.current.escapeKey.wasPressedThisFrame)
                SetCursorLocked(false);

            if (Mouse.current != null &&
                Mouse.current.leftButton.wasPressedThisFrame &&
                Cursor.lockState != CursorLockMode.Locked)
            {
                SetCursorLocked(true);
            }
        }

        void HandleLook()
        {
            if (lockCursorOnStart && Cursor.lockState != CursorLockMode.Locked)
                return;

            Vector2 look = lookAction.ReadValue<Vector2>();

            bool fromMouse = lookAction.activeControl != null && lookAction.activeControl.device is Mouse;

            float yawDelta;
            float pitchDelta;

            if (fromMouse)
            {
                yawDelta = look.x * mouseSensitivity;
                pitchDelta = look.y * mouseSensitivity;
            }
            else
            {
                yawDelta = look.x * gamepadLookSpeed * Time.deltaTime;
                pitchDelta = look.y * gamepadLookSpeed * Time.deltaTime;
            }

            transform.Rotate(0f, yawDelta, 0f);

            pitch -= pitchDelta;
            pitch = Mathf.Clamp(pitch, minPitch, maxPitch);
            cameraTransform.localRotation = Quaternion.Euler(pitch, 0f, 0f);
        }

        void HandleMovement()
        {
            Vector2 input = moveAction.ReadValue<Vector2>();

            Vector3 move = transform.right * input.x + transform.forward * input.y;
            cc.Move(move * moveSpeed * Time.deltaTime);

            if (cc.isGrounded)
            {
                if (velocity.y < 0f)
                    velocity.y = -2f;

                if (jumpAction.WasPressedThisFrame())
                    velocity.y = jumpForce;
            }

            velocity.y += gravity * Time.deltaTime;
            cc.Move(velocity * Time.deltaTime);
        }

        void SetCursorLocked(bool locked)
        {
            Cursor.lockState = locked ? CursorLockMode.Locked : CursorLockMode.None;
            Cursor.visible = !locked;
        }

        static float NormalizePitch(float angle)
        {
            if (angle > 180f)
                angle -= 360f;

            return angle;
        }
    }
}
#endif
