using UnityEngine;

namespace ThirdPersonInventoryDemo.Core
{
    public class CursorController : MonoBehaviour
    {
        public void SetGameplayMode()
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }

        public void SetUIMode()
        {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }
    }
}
