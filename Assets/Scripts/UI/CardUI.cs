using DG.Tweening;
using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class CardUI : MonoBehaviour, IPointerUpHandler,IPointerDownHandler
{
    [SerializeField] public Image front;
    [SerializeField] public Image cardIcon;
    [SerializeField] public Image back;
    [SerializeField] public float flipDuration = 0.2f;
    [SerializeField] public AudioSource flipAudio;

    public int id;
    public bool isShowingFront = true;
    public bool isFlipping = false;
    public bool canBeFlipped = true;

    public event Action<CardUI> OnCardSelected;
    void Awake()
    {
        ChangeSide(false);
    }

    //void Start()
    //{
    //    Flip();
    //}

    public void InitializeCardData(int id , Sprite iconSprite)
    {
        this.id = id;
        cardIcon.sprite = iconSprite;
    }

    public void ChangeSide(bool isFront)
    {
        isShowingFront = isFront;
        front.gameObject.SetActive(isFront);
        back.gameObject.SetActive(!isFront);

        transform.localScale = Vector3.one;
    }

    [ContextMenu("Do Flip")]
    public void Flip()
    {
        if (isFlipping) return;

        isFlipping = true;

        // Switch image while card is invisible from the side
        isShowingFront = !isShowingFront;

        flipAudio.Play();

        // Front → edge
        transform.DOScaleX(0f, flipDuration)
            .SetEase(Ease.InQuad)
            .OnComplete(() =>
            {
                front.gameObject.SetActive(isShowingFront);
                back.gameObject.SetActive(!isShowingFront);

                // Edge → back
                transform.DOScaleX(1f, flipDuration)
                    .SetEase(Ease.OutQuad)
                    .OnComplete(() =>
                    {
                        isFlipping = false;
                    });
            });
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        print("Tapped");
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        OnCardSelected?.Invoke(this);
    }
}
