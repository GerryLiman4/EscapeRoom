using UnityEngine;

public class MinigamePopup : MonoBehaviour
{
    public Animator animator;

    public virtual void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            ClosePopup();
        }
    }

    public virtual void OpenPopup()
    {
        gameObject.SetActive(true);
        animator.PlayInFixedTime("Open");
    }

    public virtual void ClosePopup()
    {
        SignalManager.ClosePopup(gameObject);
        Destroy(gameObject);
    }
}
