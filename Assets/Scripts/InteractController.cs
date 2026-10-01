using UnityEngine;
using UnityEngine.Events;

public class InteractController : MonoBehaviour
{
    public IInteractable currentInteractable;

    public UnityEvent OnInteract;
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.E))
        {
            if (currentInteractable != null) OnInteract?.Invoke();
        }
    }

    private void OnDestroy()
    {
        OnInteract.RemoveAllListeners();
    }

    private void OnTriggerEnter(Collider other)
    {
        IInteractable collidedInteractable = other.GetComponent<IInteractable>();
        if (collidedInteractable == null) return;

        if (currentInteractable != null) currentInteractable.HideInteractInstruction();
        currentInteractable = collidedInteractable;
        currentInteractable.ShowInteractInstruction();

        print("Enter : " + other.name);
    }

    private void OnTriggerExit(Collider other)
    {
        IInteractable collidedInteractable = other.GetComponent<IInteractable>();

        if (collidedInteractable == null) return;

        if (currentInteractable != null && currentInteractable == collidedInteractable) currentInteractable.HideInteractInstruction();
        currentInteractable = null;

        print("Exit : " + other.name);
    }
}
