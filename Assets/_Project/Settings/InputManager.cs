using UnityEngine;

namespace CroakCreek
{
    public class InputManager : MonoBehaviour
    {
        public static InputManager instance;

        [SerializeField] private InputReader inputReader;

        public bool MenuOpenCloseInput { get; private set; }

        private void Awake()
        {
            if (instance == null)
            {
                instance = this;
                DontDestroyOnLoad(gameObject); // Optional
            }
            else
            {
                Destroy(gameObject);
                return;
            }
        }

        private void OnEnable()
        {
            if (inputReader != null)
                inputReader.MenuOpenClose += OnMenuOpenClose;
        }

        private void OnDisable()
        {
            if (inputReader != null)
                inputReader.MenuOpenClose -= OnMenuOpenClose;
        }

        private void OnMenuOpenClose()
        {
            MenuOpenCloseInput = true;
        }

        private void LateUpdate()
        {
            MenuOpenCloseInput = false; // Reset every frame
        }
    }
}
