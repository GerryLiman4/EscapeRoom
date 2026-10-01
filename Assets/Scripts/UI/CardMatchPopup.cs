using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class CardMatchPopup : MinigamePopup
{
    [SerializeField] public Sprite[] spriteList;
    [SerializeField] public CardUI[] cardList;
    [SerializeField] public Vector2 xScatterRange = new Vector2(-200.0f,200f);
    [SerializeField] public Vector2 yScatterRange = new Vector2(-100f,100f);
    [SerializeField] public Vector2 rotationScatterRange = new Vector2(-180f, 180f);

    [SerializeField] public RectTransform gameOverPanel;
    [SerializeField] public TextMeshProUGUI gameOverText;
    [SerializeField] public TextMeshProUGUI timerText;
    [SerializeField] public float maxTime = 20f;
    private float gameTimer;

    const int maxCardPair = 5;

    public CardUI currentSelectedCard;
    bool isChecking = false;
    bool canExit = true;
    bool isPlaying = false;

    public void InitializeMinigame() 
    {
        isChecking = false;
        canExit = false;
        isPlaying = false;
        gameTimer = maxTime;
        timerText.text = "Time : " + gameTimer.ToString();

        List<int> cardUsedIndex = new List<int>();
        for(int index = 0; index < cardList.Length; index +=2)
        {
            int usedIndex;
            do
            {
                usedIndex = Random.Range(0, spriteList.Length);
            } while (cardUsedIndex.Contains(usedIndex));
            cardUsedIndex.Add(usedIndex);

            cardList[index].InitializeCardData(usedIndex,spriteList[usedIndex]);
            cardList[index].gameObject.SetActive(index < maxCardPair * 2);
            cardList[index].OnCardSelected += OnCardSelected;
            SetRandomPositionForCard(cardList[index]);

            cardList[index + 1].InitializeCardData(usedIndex, spriteList[usedIndex]);
            cardList[index + 1].gameObject.SetActive(index < maxCardPair * 2);
            cardList[index + 1].OnCardSelected += OnCardSelected;
            SetRandomPositionForCard(cardList[index + 1]);
        }
    }

    public void SetRandomPositionForCard(CardUI card)
    {
        RectTransform rect = card.GetComponent<RectTransform>();

        float randomX = Random.Range(xScatterRange.x,xScatterRange.y);
        float randomY = Random.Range(yScatterRange.x,yScatterRange.y);

        rect.anchoredPosition = new Vector2(randomX,randomY);

        float randomRotation = Random.Range(rotationScatterRange.x,rotationScatterRange.y);
        rect.localRotation = Quaternion.Euler(0f,0f,randomRotation);
    }

    private void OnCardSelected(CardUI cardUI)
    {
        if (isChecking || !isPlaying) return;

        if(currentSelectedCard == null)
        {
            currentSelectedCard = cardUI;
            currentSelectedCard.Flip();
            return;
        }

        if (currentSelectedCard == cardUI) return;

        StartCoroutine(CheckScore(cardUI));
    }

    public IEnumerator CheckScore(CardUI secondCard)
    {
        isChecking = true;
        secondCard.Flip();

        yield return new WaitForSeconds(1.25f);

        if (currentSelectedCard.id == secondCard.id) 
        {
            currentSelectedCard.PopAndDisappear();
            secondCard.PopAndDisappear();
            yield return new WaitForSeconds(secondCard.flipDuration * 2f);

            bool isGameOver = true;
            foreach (CardUI card in cardList)
            {
                if (card.gameObject.activeInHierarchy)
                {
                    isGameOver = false;
                    break;
                }
            }

            if(isGameOver) GameOver(true);
        }
        else
        {
            currentSelectedCard.Flip();
            secondCard.Flip();
            yield return new WaitForSeconds(secondCard.flipDuration * 2f);
        }

        isChecking = false;
        currentSelectedCard = null;
    }

    public override void OpenPopup()
    {
        InitializeMinigame();
        base.OpenPopup();
        StartCoroutine(StartGame());
    }
    public IEnumerator StartGame()
    {
        yield return new WaitForSeconds(0.5f);

        canExit = true;
        isPlaying = true;
    }

    public override void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape) && canExit)
        {
            StartCoroutine(PlayCloseAnimation());
        }

        if (isPlaying)
        {
            gameTimer -= Time.deltaTime;
            
            if(gameTimer <= 0.0f)
            {
                gameTimer = 0f;
                GameOver(false);
            }

            timerText.text = "Time : " + gameTimer.ToString();
        }
    }

    public IEnumerator PlayCloseAnimation()
    {
        // reset
        for (int index = 0; index < cardList.Length; index++)
        {
            cardList[index].OnCardSelected -= OnCardSelected;
        }
        currentSelectedCard = null;
        //

        animator.PlayInFixedTime("Close");

        yield return null;

        AnimatorStateInfo stateInfo = animator.GetCurrentAnimatorStateInfo(0);

        while (stateInfo.IsName("Close") && stateInfo.normalizedTime < 1.0f && !animator.IsInTransition(0))
        {
            stateInfo = animator.GetCurrentAnimatorStateInfo(0);
            yield return null; 
        }

        ClosePopup();
    }

    public void GameOver(bool isWinning)
    {
        isPlaying = false;
        canExit = false;

        gameOverText.text = isWinning ? "Cleared" : "Try Again";
        if(isWinning) SignalManager.ClearMinigame(0);
        gameOverPanel.gameObject.SetActive(true);

        // Start collapsed vertically
        gameOverPanel.localScale = new Vector3(1f, 0f, 1f);

        Sequence sequence = DOTween.Sequence();

        // Appear from the center vertically
        sequence.Append(
            gameOverPanel.DOScaleY(1f, 0.75f)
                .SetEase(Ease.OutBack)
        );

        // Stay visible for 1 second
        sequence.AppendInterval(1f);

        // Disappear
        sequence.Append(
            gameOverPanel.DOScaleY(0f, 0.3f)
                .SetEase(Ease.InBack)
        );

        sequence.OnComplete(() =>
        {
            gameOverPanel.gameObject.SetActive(false);
            StartCoroutine(PlayCloseAnimation());
        });

        
    }
}
