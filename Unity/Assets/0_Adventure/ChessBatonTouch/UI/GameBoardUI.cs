using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Linq;

public class GameBoardUI : MonoBehaviour
{
    [Header("UI References")]
    public Transform boardPanel;
    public GameObject squarePrefab;

    [Header("Stage Data")]
    public StageDataSO currentStageData;

    private GameState currentState;
    private Button[,] uiButtons = new Button[8, 8];
    private Image[,] uiImages = new Image[8, 8];
    private TMP_Text[,] uiTexts = new TMP_Text[8, 8];

    void Start()
    {
        InitializeUI();
        if (GameDataManager.SelectedStageData != null)
            currentState = PuzzleStageBuilder.CreateFromSO(GameDataManager.SelectedStageData);
        else
        {
            Debug.LogError("로드할 스테이지 데이터가 없습니다!");
            return;
        }

        RenderState(currentState);
    }

    private void InitializeUI()
    {
        for (int y = 0; y < 8; y++)
        {
            for (int x = 0; x < 8; x++)
            {
                int clickX = x;
                int clickY = y;

                GameObject obj = Instantiate(squarePrefab, boardPanel);
                uiButtons[x, y] = obj.GetComponent<Button>();
                uiImages[x, y] = obj.GetComponent<Image>();

                uiTexts[x, y] = obj.GetComponentInChildren<TMP_Text>();

                uiButtons[x, y].onClick.AddListener(() => OnSquareClicked(clickX, clickY));
            }
        }
    }

    private void OnSquareClicked(int x, int y)
    {
        var clickedSquare = currentState.Board.FirstOrDefault(sq => sq.X == x && sq.Y == y);
        if (clickedSquare == null) return;

        GameState nextState = currentState;

        if (currentState.ActiveSquare == null)
        {
            nextState = ChessPuzzleLogic.SelectStartingPiece(currentState, clickedSquare);
        }
        else
        {
            nextState = ChessPuzzleLogic.MoveAndTouch(currentState, clickedSquare);

            if (nextState.IsVictory)
            {
                Debug.Log("스테이지 클리어! 킹을 잡았습니다.");
            }
        }

        if (currentState != nextState)
        {
            currentState = nextState;
            RenderState(currentState);
        }
    }

    private void RenderState(GameState state)
    {
        var validMoves = ChessPuzzleLogic.GetValidBatonTouches(state);

        foreach (var square in state.Board)
        {
            Image img = uiImages[square.X, square.Y];
            TMP_Text txt = uiTexts[square.X, square.Y];

            // 1. 기물 텍스트 표시
            if (square.Piece == PieceType.None)
            {
                txt.text = "";
            }
            else
            {
                txt.text = square.Piece.ToString();
                txt.color = Color.black;
            }

            // 2. 타일 배경 색상 처리 조건
            bool isActive = state.ActiveSquare == square;
            bool isValidMove = validMoves.Contains(square);

            // 기물이 아직 선택되지 않았고, 빈 칸이 아니며, 허용된 시작 기물 리스트에 없는 경우 (DisableStart 처리된 기물)
            bool isStartDisabled = state.ActiveSquare == null &&
                                   square.Piece != PieceType.None &&
                                   !state.AllowedStartingSquares.Contains(square);

            // 3. 색상 적용
            if (isActive)
            {
                img.color = Color.green; // 현재 조작 중인 기물
            }
            else if (isValidMove)
            {
                img.color = Color.yellow; // 바톤 터치 이동 가능
            }
            else if (isStartDisabled)
            {
                img.color = new Color(0.7f, 0.7f, 0.7f); // 시작 기물로 선택 불가능 (어두운 회색)
            }
            else
            {
                img.color = Color.white; // 일반 칸 또는 선택 가능한 첫 기물
            }
        }
    }
}