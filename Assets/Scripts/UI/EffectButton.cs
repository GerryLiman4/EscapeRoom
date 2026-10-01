using DG.Tweening;
using UnityEngine;
using UnityEngine.EventSystems;

public class EffectButton : MonoBehaviour,IPointerEnterHandler, IPointerExitHandler, IPointerDownHandler
{
    
    public void OnPointerExit(PointerEventData eventData)
    {
        transform.DOKill();
        transform.DOScale(1f, 0.15f)
            .SetEase(Ease.OutBack);
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        transform.DOKill();
        transform.DOScale(1.15f, 0.1f)
        .SetEase(Ease.OutBack);
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        transform.DOKill();

        transform.DOScale(0.9f, 0.06f)
            .SetEase(Ease.OutQuad)
            .OnComplete(() =>
            {
                transform.DOScale(1f, 0.12f)
                    .SetEase(Ease.OutBack);
            });
    }
}
