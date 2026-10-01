using UnityEngine;

public class KitchenTable : MonoBehaviour,IInteractable
{
    [SerializeField] public GameObject minigamePrefab;
    [SerializeField] public GameObject interactInstruction;

    public void Interact(PlayerController player)
    {
        SignalManager.OpenPopup(minigamePrefab);
        SignalManager.OnClosePopup += OnClosePopup;
    }

    public void ShowInteractInstruction()
    {
        interactInstruction.SetActive(true);
    }
    public void HideInteractInstruction()
    {
        interactInstruction.SetActive(false);
    }

    private void OnClosePopup(GameObject obj)
    {
        SignalManager.OnClosePopup -= OnClosePopup;
    }

    private void OnDestroy()
    {
        SignalManager.OnClosePopup -= OnClosePopup;
    }
}
