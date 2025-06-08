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
        public event UnityAction<bool> Jump = delegate { };
        public event UnityAction<bool> Run = delegate { };
        public event UnityAction<bool> Fire = delegate { };

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

        void OnDisable()
        {
            if (inputActions != null)
            {
                inputActions.Disable(); // Disables all maps
            }
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

    }
}
