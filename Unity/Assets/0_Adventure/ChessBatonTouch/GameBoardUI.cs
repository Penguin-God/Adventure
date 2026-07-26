using UnityEngine;
using UnityEngine.UI;
using TMPro; // TextMeshPro 네임스페이스 추가
using System.Collections.Generic;
using System.Linq;

public class GameBoardUI : MonoBehaviour
{
    [Header("UI References")]
    public Transform boardPanel;    
    public GameObject squarePrefab; 

    private GameState currentState;
    private Button[,] uiButtons = new Button[8, 8];
    private Image[,] uiImages = new Image[8, 8];
    
    // TextMeshPro용 텍스트 배열로 변경
    private TMP_Text[,] uiTexts = new TMP_Text[8, 8]; 

    void Start()
    {
        InitializeUI();
        currentState = PuzzleStageBuilder.CreateStage1();
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
                
                // 프리팹 자식에 있는 TMP_Text 컴포넌트를 가져와 배열에 저장
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
            TMP_Text txt = uiTexts[square.X, square.Y]; // 캐싱해둔 TMP_Text 컴포넌트

            // 1. 기물이 있으면 텍스트로 이름 표시 (처음부터 모든 기물이 보이게 됨)
            if (square.Piece == PieceType.None)
            {
                txt.text = ""; 
            }
            else
            {
                txt.text = square.Piece.ToString(); 
                txt.color = Color.black; 
            }
            
            // 2. 타일 배경 색상 처리
            bool isActive = state.ActiveSquare == square;
            bool isValidMove = validMoves.Contains(square);
            bool isAllowedStart = state.ActiveSquare == null && state.AllowedStartingSquares.Contains(square);

            if (isActive)
            {
                img.color = Color.green;
            }
            else if (isValidMove)
            {
                img.color = Color.yellow; 
            }
            else if (isAllowedStart)
            {
                img.color = Color.cyan; 
            }
            else
            {
                img.color = Color.white; 
            }
        }
    }
}