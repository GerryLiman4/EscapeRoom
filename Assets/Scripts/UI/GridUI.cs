using UnityEngine;

public class GridUI : MonoBehaviour
{
    [SerializeField] public int id;
    [SerializeField] public PuzzlePieceUI occupiedPuzzlePiece;

    public void SetOccupiedPuzzle(PuzzlePieceUI designatedPuzzle)
    {
        occupiedPuzzlePiece = designatedPuzzle;
    }
}
