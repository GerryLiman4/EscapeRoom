using UnityEngine;

public class Door : MonoBehaviour,IInteractable
{
    [SerializeField] public GameObject interactInstruction;

    public void Interact(PlayerController player)
    {
        SignalManager.ExitRoom();
    }

    public void ShowInteractInstruction()
    {
        interactInstruction.SetActive(true);
    }
    public void HideInteractInstruction()
    {
        interactInstruction.SetActive(false);
    }

}
