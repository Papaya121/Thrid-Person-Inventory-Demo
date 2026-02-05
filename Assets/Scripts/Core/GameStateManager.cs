using UnityEngine;

namespace ThirdPersonInventoryDemo.Core
{
    public enum GameMode
    {
        Gameplay,
        Inventory
    }


    public class GameStateManager : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] private CursorController cursor;

        public GameMode Mode { get; private set; } = GameMode.Gameplay;

        private void Start()
        {
            if (cursor == null)
            {
                Debug.LogError("[GameStateManager] CursorController is not assigned.");
                enabled = false;
                return;
            }

            SetMode(GameMode.Gameplay);
        }

        public void SetMode(GameMode mode)
        {
            Mode = mode;

            switch (mode)
            {
                case GameMode.Gameplay:
                    cursor.SetGameplayMode();
                    Time.timeScale = 1f;
                    break;

                case GameMode.Inventory:
                    cursor.SetUIMode();
                    Time.timeScale = 0f;
                    break;
            }
        }
    }
}
