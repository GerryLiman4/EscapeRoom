using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class SlidingPuzzlePopup : MinigamePopup
{
    [SerializeField] public PuzzlePieceUI[] puzzlePieceList;
    [SerializeField] public GridUI[] gridList;
    [SerializeField] public Sprite[] spriteList;

    [SerializeField] public RectTransform gameOverPanel;
    [SerializeField] public TextMeshProUGUI gameOverText;
    [SerializeField] public TextMeshProUGUI timerText;
    [SerializeField] public float maxTime = 20f;
    [SerializeField] public AudioSource losingSound;
    [SerializeField] public AudioSource winningSound;
    [SerializeField] public AudioSource openSound;
    private float gameTimer;

    const int maxPuzzleCount = 8;

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

        int randomMissingIndex = maxPuzzleCount; // Random.Range(0, gridList.Length);
        List<int> usedGridIndex = new List<int>();
        for (int index = 0; index < puzzlePieceList.Length; index++)
        {
            puzzlePieceList[index].InitializePuzzleData(index,spriteList[index]);
            puzzlePieceList[index].gameObject.SetActive(true);

            // set to grid
            int usedIndex;
            do
            {
                usedIndex = Random.Range(0, gridList.Length);
            } while (usedGridIndex.Contains(usedIndex));

            usedGridIndex.Add(usedIndex);

            if (randomMissingIndex == index)
            {
                puzzlePieceList[index].gameObject.SetActive(false);
                continue;
            }
            gridList[usedIndex].SetOccupiedPuzzle(puzzlePieceList[index]);
            puzzlePieceList[index].indexPosition = usedIndex;

            // reparent to grid
            puzzlePieceList[index].transform.SetParent(gridList[usedIndex].transform);
            puzzlePieceList[index].GetComponent<RectTransform>().anchoredPosition = Vector2.zero;

            // connect the signal
            puzzlePieceList[index].OnPuzzleSelected += OnPuzzlePieceSelected;
        }

        StartCoroutine(StartGame());
    }

    private void OnDestroy()
    {
        for (int index = 0; index < puzzlePieceList.Length; index++)
        {
            puzzlePieceList[index].OnPuzzleSelected -= OnPuzzlePieceSelected;
        }
        
    }

    private void OnPuzzlePieceSelected (PuzzlePieceUI puzzlePiece)
    {
        if (isChecking || !isPlaying) return;

        // find the empty grid
        int gridIndex = puzzlePiece.indexPosition;

        GridUI emptyGrid = null;
        int rightIndex = gridIndex + 1;
        int leftIndex = gridIndex - 1;
        int topIndex = gridIndex - 3;
        int bottomIndex = gridIndex + 3;

        if(rightIndex < gridList.Length && gridList[rightIndex].occupiedPuzzlePiece == null)
        {
            emptyGrid = gridList[rightIndex];
        }
        else if(leftIndex >= 0 && gridList[leftIndex].occupiedPuzzlePiece == null)
        {
            emptyGrid = gridList[leftIndex];
        }
        else if(topIndex >= 0 && gridList[topIndex].occupiedPuzzlePiece == null)
        {
            emptyGrid = gridList[topIndex];
        }
        else if (bottomIndex < gridList.Length && gridList[bottomIndex].occupiedPuzzlePiece == null)
        {
            emptyGrid = gridList[bottomIndex];
        }
        else
        {
            puzzlePiece.Shake();
            // invalid puzzle
            return;
        }

        // slide the puzzle to the empty grid
        gridList[puzzlePiece.indexPosition].SetOccupiedPuzzle(null);
        emptyGrid.SetOccupiedPuzzle(puzzlePiece);
        puzzlePiece.transform.SetParent(emptyGrid.transform);
        puzzlePiece.Slide(Vector2.zero, emptyGrid.id);

        StartCoroutine(CheckScore());
    }

    public IEnumerator CheckScore()
    {
        isChecking = true;
        bool isGameOver = true;
        for (int index = 0; index < gridList.Length; index++)
        {
            if (gridList[index].occupiedPuzzlePiece == null) continue;
            if (gridList[index].occupiedPuzzlePiece.id != gridList[index].id)
            {
                isGameOver = false;
                break;
            }
        }
        yield return new WaitForSeconds(0.15f);
        isChecking = false;

        if (isGameOver) GameOver(true);
    }

    public override void OpenPopup()
    {
        InitializeMinigame();
        openSound.Play();
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

            if (gameTimer <= 0.0f)
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
        for (int index = 0; index < gridList.Length; index++)
        {
            if (gridList[index].occupiedPuzzlePiece != null) gridList[index].occupiedPuzzlePiece.transform.SetParent(gridList[index].transform.parent);
            gridList[index].SetOccupiedPuzzle(null);
        }

        for (int index = 0; index < puzzlePieceList.Length; index++)
        {
            puzzlePieceList[index].OnPuzzleSelected -= OnPuzzlePieceSelected;
        }

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
        gameOverPanel.gameObject.SetActive(true);
        if (isWinning)
        {
            winningSound.Play();
            SignalManager.ClearMinigame(1);
        }
        else
        {
            losingSound.Play();
        }

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
