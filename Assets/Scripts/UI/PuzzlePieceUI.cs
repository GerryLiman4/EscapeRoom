using DG.Tweening;
using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class PuzzlePieceUI : MonoBehaviour, IPointerDownHandler
{
    [SerializeField] public AudioSource slideAudio;
    [SerializeField] public Image icon;

    public int id;
    public int indexPosition;
    public bool isSliding = false;
    public bool canBeSlided = true;

    public event Action<PuzzlePieceUI> OnPuzzleSelected;

    public void InitializePuzzleData(int id, Sprite iconSprite)
    {
        GetComponent<RectTransform>().localScale = Vector3.one;
        this.id = id;
        icon.sprite = iconSprite;
        indexPosition = -1;
    }

    public void Slide(Vector2 designatedPosition, int indexPosition)
    {
        if (isSliding) return;

        isSliding = true;
        slideAudio.Play();
        this.indexPosition = indexPosition;

        RectTransform rect = GetComponent<RectTransform>();

        rect.DOAnchorPos(designatedPosition, 0.25f)
            .SetEase(Ease.OutQuad)
            .OnComplete(() =>
            {
                isSliding = false;
            });
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        if (isSliding) return;
        OnPuzzleSelected?.Invoke(this);
    }
    public void Shake()
    {
        RectTransform rect = GetComponent<RectTransform>();
        rect.anchoredPosition = Vector3.zero;
        rect.DOKill();

        rect.DOShakeAnchorPos(
            0.25f,              // duration
            new Vector2(15f, 0f), // strength X/Y
            15,                 // vibrato
            90f,                // randomness
            false,              // snapping
            true                // fade out
        );
    }
}
