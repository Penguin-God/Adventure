using System;
using System.Collections.Generic;
using System.Linq;

public static class ChessPuzzleLogic
{
    // [1] 처음 기물 선택
    public static GameState SelectStartingPiece(GameState state, ChessSquare startSquare)
    {
        if (state.ActiveSquare != null) return state;
        if (!state.AllowedStartingSquares.Contains(startSquare)) return state;

        return state with { ActiveSquare = startSquare };
    }

    // [2] 바톤 터치가 가능한 칸 목록 반환
    public static IReadOnlyList<ChessSquare> GetValidBatonTouches(GameState state)
    {
        if (state.ActiveSquare == null) return new List<ChessSquare>();

        var active = state.ActiveSquare;

        // 조건을 만족하는 요소들을 찾아 새로운 List로 반환 (IReadOnlyList로 업캐스팅됨)
        return state.Board
            .Where(sq => sq.Piece != PieceType.None && sq != active)
            .Where(sq => IsValidChessMove(state.Board, active, sq))
            .ToList();
    }

    // [3] 바톤 터치 실행
    public static GameState MoveAndTouch(GameState state, ChessSquare targetSquare)
    {
        var validMoves = GetValidBatonTouches(state);

        if (!validMoves.Contains(targetSquare)) return state;

        // LINQ의 Select를 사용해 기존 보드를 기반으로 새로운 List를 생성
        var newBoard = state.Board.Select(sq =>
            sq == state.ActiveSquare ? sq with { Piece = PieceType.None } : sq
        ).ToList();

        var newActiveSquare = newBoard.First(sq => sq.X == targetSquare.X && sq.Y == targetSquare.Y);

        return state with
        {
            Board = newBoard,
            ActiveSquare = newActiveSquare
        };
    }

    // [4] 체스 이동 규칙 검증
    private static bool IsValidChessMove(IReadOnlyList<ChessSquare> board, ChessSquare from, ChessSquare to)
    {
        int dx = Math.Abs(from.X - to.X);
        int dy = Math.Abs(from.Y - to.Y);

        return from.Piece switch
        {
            PieceType.Knight => (dx == 1 && dy == 2) || (dx == 2 && dy == 1),
            PieceType.Rook => (dx == 0 || dy == 0) && IsPathClear(board, from, to),
            PieceType.Bishop => (dx == dy) && IsPathClear(board, from, to),
            PieceType.Queen => (dx == 0 || dy == 0 || dx == dy) && IsPathClear(board, from, to),
            PieceType.Pawn => (dx == 1 && dy == 1) && (to.Y - from.Y == 1),
            _ => false
        };
    }

    // 경로 장애물 확인
    private static bool IsPathClear(IReadOnlyList<ChessSquare> board, ChessSquare from, ChessSquare to)
    {
        int stepX = Math.Sign(to.X - from.X);
        int stepY = Math.Sign(to.Y - from.Y);

        int currX = from.X + stepX;
        int currY = from.Y + stepY;

        while (currX != to.X || currY != to.Y)
        {
            var sq = board.FirstOrDefault(s => s.X == currX && s.Y == currY);
            if (sq != null && sq.Piece != PieceType.None) return false;

            currX += stepX;
            currY += stepY;
        }
        return true;
    }
}