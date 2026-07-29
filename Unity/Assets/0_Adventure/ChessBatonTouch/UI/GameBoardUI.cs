using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

// 인스펙터에서 기물 타입과 이미지를 연결하기 위한 구조체
[Serializable]
public struct PieceSpriteData
{
    public PieceType pieceType;
    public Sprite pieceSprite;
}

public class GameBoardUI : MonoBehaviour
{
    [Header("UI References")]
    public Transform boardPanel;
    public GameObject squarePrefab;
    public GameResultUI gameResultUI;
    public Button restartBtn;

    [Header("Piece Sprites Configuration")]
    public List<PieceSpriteData> pieceSpritesConfig; // 인스펙터에서 세팅할 리스트
    private Dictionary<PieceType, Sprite> pieceSpriteDict;

    [Header("Stage Data")]
    public StageDataSO currentStageData;

    private GameState currentState;
    private Button[,] uiButtons = new Button[8, 8];
    private Image[,] uiBackgrounds = new Image[8, 8]; // 기존 배경용 Image
    private Image[,] uiPieceImages = new Image[8, 8]; // 새로 추가된 기물용 Image

    void Start()
    {
        // 리스트를 딕셔너리로 변환하여 빠른 검색이 가능하게 세팅
        pieceSpriteDict = new Dictionary<PieceType, Sprite>();
        foreach (var config in pieceSpritesConfig)
        {
            pieceSpriteDict[config.pieceType] = config.pieceSprite;
        }

        InitializeUI();
        currentStageData = LevelManager.Instance.GetStageData(LevelManager.Instance.CurrentAbsoluteLevel);

        if (currentStageData != null)
        {
            currentState = PuzzleStageBuilder.CreateFromSO(currentStageData);
        }
        else
        {
            Debug.LogError("스테이지 데이터를 불러올 수 없습니다!");
            return;
        }

        restartBtn.onClick.AddListener(RestartStage);
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
                uiBackgrounds[x, y] = obj.GetComponent<Image>();

                // 첫 번째 자식인 PieceImage 컴포넌트를 가져옴
                uiPieceImages[x, y] = obj.transform.GetChild(0).GetComponent<Image>();

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
                gameResultUI.gameObject.SetActive(true);
                gameResultUI.OnStageCleared();
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
            Image bgImg = uiBackgrounds[square.X, square.Y];
            Image pieceImg = uiPieceImages[square.X, square.Y];

            // 1. 기물 이미지 렌더링
            if (square.Piece == PieceType.None)
            {
                pieceImg.sprite = null;
                pieceImg.color = Color.clear; // 기물이 없으면 투명하게
            }
            else
            {
                if (pieceSpriteDict.TryGetValue(square.Piece, out Sprite sprite))
                {
                    pieceImg.sprite = sprite;
                    pieceImg.color = Color.white; // 불투명하게 보이도록 설정
                }
                else
                {
                    pieceImg.sprite = null;
                    pieceImg.color = Color.clear;
                }
            }

            // 2. 상태값 판별
            bool isActive = state.ActiveSquare == square;
            bool isValidMove = validMoves.Contains(square);
            bool isStartDisabled = state.ActiveSquare == null &&
                                   square.Piece != PieceType.None &&
                                   !state.AllowedStartingSquares.Contains(square);

            // 3. 배경색 렌더링 (기물 이미지는 영향받지 않음)
            if (isActive)
            {
                bgImg.color = Color.green; // 활성화
            }
            else if (isValidMove)
            {
                bgImg.color = Color.yellow; // 이동 가능
            }
            else if (isStartDisabled)
            {
                bgImg.color = new Color(0.7f, 0.7f, 0.7f); // 비활성화됨
            }
            else
            {
                bgImg.color = Color.white; // 기본
            }
        }
    }

    public void RestartStage() => SceneManager.LoadScene(SceneManager.GetActiveScene().name);
}