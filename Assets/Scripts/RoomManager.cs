using DG.Tweening;
using System;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class RoomManager : MonoBehaviour
{
    [SerializeField] public Canvas uiCanvas;
    [SerializeField] public RectTransform dialoguePanel;
    [SerializeField] public TextMeshProUGUI dialogueText;
    [SerializeField] public PlayerController player;

    [SerializeField] public TextMeshProUGUI quest1;
    [SerializeField] public TextMeshProUGUI quest2;

    public bool isQuest1Clear;
    public bool isQuest2Clear;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        SignalManager.OnOpenPopup += OnOpenPopup;
        SignalManager.OnClosePopup += OnClosePopup;
        SignalManager.OnClearMinigame += OnClearMinigame;
        SignalManager.OnExitRoom += OnExitRoom;

        player.SwitchState(StateId.Idle, true);
    }

    private void OnExitRoom()
    {
        if (isQuest1Clear && isQuest2Clear) SceneManager.LoadScene("MainMenu");

        ShowDialogue("I need to investigate the room first.");
    }

    private void OnClearMinigame(int minigameId)
    {
        if(minigameId == 0)
        {
            isQuest1Clear = true;
            quest1.text = "Clear Card Minigame - 1/1";
        }
        else if(minigameId == 1)
        {
            isQuest2Clear = true;
            quest2.text = "Clear Puzzle Minigame - 1/1";
        }
    }

    private void OnDestroy()
    {
        SignalManager.OnOpenPopup -= OnOpenPopup;
        SignalManager.OnClosePopup -= OnClosePopup;
        SignalManager.OnClearMinigame -= OnClearMinigame;
        SignalManager.OnExitRoom -= OnExitRoom;
    }

    private void OnOpenPopup(GameObject popupPrefab)
    {
        MinigamePopup popup = Instantiate(popupPrefab, uiCanvas.transform).GetComponent<MinigamePopup>();
        popup.transform.localPosition = Vector3.zero;
        popup.OpenPopup();

        player.SwitchState(StateId.None,true);
    }
    private void OnClosePopup(GameObject popup)
    {
        player.SwitchState(StateId.Idle, true);
    }

    private void ShowDialogue(string text) 
    {
        dialoguePanel.gameObject.SetActive(true);
        dialogueText.text = text;

        // Start collapsed vertically
        dialoguePanel.localScale = new Vector3(1f, 0f, 1f);

        Sequence sequence = DOTween.Sequence();

        // Appear from the center vertically
        sequence.Append(
            dialoguePanel.DOScaleY(1f, 0.75f)
                .SetEase(Ease.OutBack)
        );

        // Stay visible for 1 second
        sequence.AppendInterval(1f);

        // Disappear
        sequence.Append(
            dialoguePanel.DOScaleY(0f, 0.3f)
                .SetEase(Ease.InBack)
        );

        sequence.OnComplete(() =>
        {
            dialoguePanel.gameObject.SetActive(false);
        });
    }
}
