using UnityEngine;
using System.Collections.Generic;

[System.Serializable]
public class PieceSetup
{
    public int X;
    public int Y;
    public PieceType Piece;

    [Tooltip("체크하면 게임 시작 시 첫 기물로 선택할 수 없습니다. (꼼수 방지용)")]
    public bool DisableStart;
}

[CreateAssetMenu(fileName = "NewStage", menuName = "ChessPuzzle/StageData")]
public class StageDataSO : ScriptableObject
{
    [Header("스테이지에 배치할 기물들")]
    public List<PieceSetup> initialPieces = new List<PieceSetup>();

    [Header("힌트 시스템용 (광고 시청 시 제공)")]
    public PieceSetup CorrectStartingPiece; // 나중에 광고를 보면 알려줄 '진짜 정답' 기물
}