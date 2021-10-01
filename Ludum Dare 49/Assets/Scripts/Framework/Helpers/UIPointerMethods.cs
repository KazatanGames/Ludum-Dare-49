namespace KazatanGames.Framework
{
    using UnityEngine;
    using UnityEngine.UI;
    using UnityEngine.EventSystems;

    public static class UIPointerMethods
    {
        public static bool IsPointerOverGameObject()
        {
            // Check mouse
            if (EventSystem.current.IsPointerOverGameObject())
            {
                return true;
            }

            // Check touches
            for (int i = 0; i < Input.touchCount; i++)
            {
                var touch = Input.GetTouch(i);
                if (touch.phase == TouchPhase.Began)
                {
                    if (EventSystem.current.IsPointerOverGameObject(touch.fingerId))
                    {
                        return true;
                    }
                }
            }

            return false;
        }
    }
}