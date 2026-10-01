using System;
using UnityEngine;

public static class SignalManager 
{
    public static Action<GameObject> OnOpenPopup;
    public static Action<GameObject> OnClosePopup;
    public static Action<int> OnClearMinigame;
    public static Action OnExitRoom;
    public static void OpenPopup(GameObject popupPrefab)
    {
        OnOpenPopup?.Invoke(popupPrefab);
    }

    public static void ClosePopup(GameObject popupObject)
    {
        OnClosePopup?.Invoke(popupObject);
    }

    public static void ClearMinigame(int minigameId)
    {
        OnClearMinigame?.Invoke(minigameId);
    }

    public static void ExitRoom()
    {
        OnExitRoom?.Invoke();
    }
}
