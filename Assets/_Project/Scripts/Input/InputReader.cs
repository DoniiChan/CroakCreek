using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;
using static PlayerInputActions;

namespace CroakCreek
{
    [CreateAssetMenu(fileName = "InputReader", menuName = "Platformer/InputReader")]
    public class InputReader : ScriptableObject, IPlayerActions, IUIActions
    {
        public event UnityAction<Vector2> Move = delegate { };
        public event UnityAction<Vector2> Look = delegate { };
        public event UnityAction<bool> Jump = delegate { };
        public event UnityAction<bool> Run = delegate { };
        public event UnityAction<bool> Fire = delegate { };
        public event UnityAction<bool> Lock = delegate { };

        public event UnityAction MenuOpenClose = delegate { };

        private PlayerInputActions inputActions;

        public Vector3 Direction => inputActions?.Player.Move.ReadValue<Vector2>() ?? Vector2.zero;

        void OnEnable()
        {
            if (inputActions == null)
            {
                inputActions = new PlayerInputActions();

                inputActions.Player.SetCallbacks(this);
                inputActions.UI.SetCallbacks(this);
            }

            inputActions.Player.Enable(); // Default to gameplay
            inputActions.UI.Disable();
        }

        void OnDisable()
        {
            if (inputActions != null)
            {
                inputActions.Disable(); // Disables all maps
            }
        }

        public void EnablePlayerInput()
        {
            inputActions.UI.Disable();
            inputActions.Player.Enable();
        }

        public void EnableUIInput()
        {
            inputActions.Player.Disable();
            inputActions.UI.Enable();
        }

        void OnDestroy()
        {
            if (inputActions != null)
            {
                inputActions.Dispose(); // Fully releases unmanaged resources
                inputActions = null;
            }
        }

        public void Disable()
        {
            inputActions?.Disable();
        }

        public void OnMove(InputAction.CallbackContext context)
        {
            Move.Invoke(context.ReadValue<Vector2>());
        }

        public void OnJump(InputAction.CallbackContext context) 
        {
            switch (context.phase)
            {
                case InputActionPhase.Started:
                    Jump.Invoke(true);
                    break;
                case InputActionPhase.Canceled:
                    Jump.Invoke(false);
                    break;
            }
        }

        public void OnRun(InputAction.CallbackContext context) 
        {
            switch (context.phase)
            {
                case InputActionPhase.Started:
                    Run.Invoke(true);
                    break;
                case InputActionPhase.Canceled:
                    Run.Invoke(false);
                    break;
            }
        }

        public void OnFire(InputAction.CallbackContext context)
        {
            switch (context.phase)
            {
                case InputActionPhase.Started:
                    Fire.Invoke(true);
                    break;
                case InputActionPhase.Canceled:
                    Fire.Invoke(false);
                    break;
            }
        }

        public void OnLockOn(InputAction.CallbackContext context)
        {
            switch (context.phase)
            {
                case InputActionPhase.Started:
                    Lock.Invoke(true);
                    break;
                case InputActionPhase.Canceled:
                    Lock.Invoke(false);
                    break;
            }
        }

        public void OnLookAround(InputAction.CallbackContext context)
        {
            Look.Invoke(context.ReadValue<Vector2>());
        }

        // UI Actions

        public void OnNavigate(InputAction.CallbackContext context)
        {
            // noop
        }

        public void OnSubmit(InputAction.CallbackContext context)
        {
            // noop
        }

        public void OnCancel(InputAction.CallbackContext context)
        {
            // noop
        }

        public void OnPoint(InputAction.CallbackContext context)
        {
            // noop
        }

        public void OnClick(InputAction.CallbackContext context)
        {
            // noop
        }

        public void OnScrollWheel(InputAction.CallbackContext context)
        {
            // noop
        }

        public void OnMiddleClick(InputAction.CallbackContext context)
        {
            // noop
        }

        public void OnRightClick(InputAction.CallbackContext context)
        {
            // noop
        }

        public void OnTrackedDevicePosition(InputAction.CallbackContext context)
        {
            // noop
        }

        public void OnTrackedDeviceOrientation(InputAction.CallbackContext context)
        {
            // noop
        }

        public void OnMenuOpenClose(InputAction.CallbackContext context)
        {
            if (context.phase == InputActionPhase.Performed)
                MenuOpenClose.Invoke();
        }
    }
}
