using UnityEngine;

public enum GameMode
{
    Gameplay,
    Inventory
}


public class GameStateManager : MonoBehaviour
{
    [SerializeField] private CursorController cursor;

    public GameMode Mode { get; private set; } = GameMode.Gameplay;

    private void Start()
    {
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
