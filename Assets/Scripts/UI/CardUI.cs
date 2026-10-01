using DG.Tweening;
using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class CardUI : MonoBehaviour, IPointerUpHandler,IPointerDownHandler, IPointerEnterHandler, IPointerExitHandler
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
        GetComponent<RectTransform>().localScale = Vector3.one;
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

        GetComponent<RectTransform>().localScale = Vector3.one;
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

    public void OnPointerExit(PointerEventData eventData)
    {
        if (isFlipping) return;
        transform.DOKill();
        transform.DOScale(1f, 0.15f)
            .SetEase(Ease.OutBack);
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (isFlipping) return;
        transform.DOKill();
        transform.DOScale(1.15f, 0.1f)
        .SetEase(Ease.OutBack);
    }

    public Sequence PopAndDisappear()
    {
        transform.DOKill();

        Sequence seq = DOTween.Sequence();

        seq.Append(transform.DOScale(1.3f, 0.12f).SetEase(Ease.OutBack));
        seq.Append(transform.DOScale(1.1f, 0.08f).SetEase(Ease.OutQuad));
        seq.Append(transform.DOScale(0f, 0.15f).SetEase(Ease.InBack));

        seq.OnComplete(() =>
        {
            gameObject.SetActive(false);
        });

        return seq;
    }
}
