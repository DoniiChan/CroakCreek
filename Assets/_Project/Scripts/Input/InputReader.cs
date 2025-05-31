using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;
using static PlayerInputActions;

namespace CroakCreek
{
    [CreateAssetMenu(fileName = "InputReader", menuName = "Platformer/InputReader")]
    public class InputReader : ScriptableObject, IPlayerActions
    {
        public event UnityAction<Vector2> Move = delegate { };
        public event UnityAction<Vector2, bool> Look = delegate { };
        public event UnityAction EnableMouseControlCamera = delegate { };
        public event UnityAction DisableMouseControlCamera = delegate { };

        private PlayerInputActions inputActions;

        public Vector3 Direction => inputActions?.Player.Move.ReadValue<Vector2>() ?? Vector2.zero;

        void OnEnable()
        {
            if (inputActions == null)
            {
                inputActions = new PlayerInputActions();
                inputActions.Player.SetCallbacks(this);
            }

            inputActions.Enable();
        }

        public void Disable()
        {
            if (inputActions != null)
            {
                inputActions.Disable();
                inputActions.Dispose();
                inputActions = null;
            }
        }

        public void OnMove(InputAction.CallbackContext context)
        {
            Move.Invoke(context.ReadValue<Vector2>());
        }

        public void OnLook(InputAction.CallbackContext context)
        {
            Look.Invoke(context.ReadValue<Vector2>(), IsDeviceMouse(context));
        }

        bool IsDeviceMouse(InputAction.CallbackContext context) => context.control.device.name == "Mouse";

        public void OnJump(InputAction.CallbackContext context) { /* noop */ }
        public void OnFire(InputAction.CallbackContext context) { /* noop */ }
        public void OnRun(InputAction.CallbackContext context) { /* noop */ }

        public void OnMouseControlCamera(InputAction.CallbackContext context)
        {
            switch (context.phase)
            {
                case InputActionPhase.Started:
                    EnableMouseControlCamera.Invoke();
                    break;
                case InputActionPhase.Canceled:
                    DisableMouseControlCamera.Invoke();
                    break;
            }
        }
    }
}
