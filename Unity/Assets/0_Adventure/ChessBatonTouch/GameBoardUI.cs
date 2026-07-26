using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using System.Linq;

public class GameBoardUI : MonoBehaviour
{
    [Header("UI References")]
    public Transform boardPanel;    // 8x8 그리드가 설정된 UI 패널 (GridLayoutGroup 사용 권장)
    public GameObject squarePrefab; // UI 버튼 프리팹

    private GameState currentState;

    private Button[,] uiButtons = new Button[8, 8];
    private Image[,] uiImages = new Image[8, 8];

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
                int clickX = x; // 클로저 이슈 방지를 위한 지역 변수 복사
                int clickY = y;

                GameObject obj = Instantiate(squarePrefab, boardPanel);
                uiButtons[x, y] = obj.GetComponent<Button>();
                uiImages[x, y] = obj.GetComponent<Image>();

                uiButtons[x, y].onClick.AddListener(() => OnSquareClicked(clickX, clickY));
            }
        }
    }

    private void OnSquareClicked(int x, int y)
    {
        // IReadOnlyList에는 Find가 없으므로 LINQ의 FirstOrDefault를 사용합니다.
        var clickedSquare = currentState.Board.FirstOrDefault(sq => sq.X == x && sq.Y == y);
        if (clickedSquare == null) return;

        GameState nextState = currentState;

        // 1. 아직 기물이 선택되지 않은 상태 (처음 시작)
        if (currentState.ActiveSquare == null)
        {
            nextState = ChessPuzzleLogic.SelectStartingPiece(currentState, clickedSquare);
        }
        // 2. 기물이 활성화되어 있는 상태 (바톤 터치 시도)
        else
        {
            nextState = ChessPuzzleLogic.MoveAndTouch(currentState, clickedSquare);

            if (nextState.IsVictory)
            {
                Debug.Log("스테이지 클리어! 킹을 잡았습니다.");
                // TODO: 클리어 UI 호출 또는 다음 스테이지 로드
            }
        }

        // 상태가 변했다면 화면 갱신
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

            // TODO: 게임 내 실제 스프라이트 연결 시 아래 주석 해제
            // img.sprite = GetSpriteForPiece(square.Piece);

            // record 클래스는 값(Value) 비교를 자동으로 해주기 때문에 Contains로 쉽게 체크 가능합니다.
            bool isActive = state.ActiveSquare == square;
            bool isValidMove = validMoves.Contains(square);
            bool isAllowedStart = state.ActiveSquare == null && state.AllowedStartingSquares.Contains(square);

            // 상태에 따른 타일 색상 표현
            if (isActive)
            {
                img.color = Color.green; // 조작 중인 기물
            }
            else if (isValidMove)
            {
                img.color = Color.yellow; // 바톤 터치 가능한 타겟
            }
            else if (isAllowedStart)
            {
                img.color = Color.cyan; // 시작 가능한 기물 (힌트)
            }
            else
            {
                img.color = Color.white; // 일반 상태
            }
        }
    }
}