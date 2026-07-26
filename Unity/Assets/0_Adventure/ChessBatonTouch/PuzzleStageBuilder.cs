using System.Collections.Generic;
using System.Linq;

public static class PuzzleStageBuilder
{
    public static GameState CreateStage1()
    {
        var initialBoard = new List<ChessSquare>();

        // 8x8 빈 보드 생성
        for (int y = 0; y < 8; y++)
        {
            for (int x = 0; x < 8; x++)
            {
                initialBoard.Add(new ChessSquare(x, y, PieceType.None));
            }
        }

        // 기물 배치 (레코드의 with 식을 사용해 새로운 상태로 매핑 후 List로 반환)
        var updatedBoard = initialBoard.Select(sq =>
        {
            if (sq.X == 0 && sq.Y == 0) return sq with { Piece = PieceType.Knight };
            if (sq.X == 1 && sq.Y == 2) return sq with { Piece = PieceType.Pawn };
            if (sq.X == 2 && sq.Y == 3) return sq with { Piece = PieceType.King };
            return sq;
        }).ToList();

        // 플레이어가 처음에 선택할 수 있는 기물(힌트 대상) 설정
        var startingSquare = updatedBoard.First(s => s.X == 0 && s.Y == 0);
        var allowedStarts = new List<ChessSquare> { startingSquare };

        // IReadOnlyList 인터페이스로 업캐스팅되어 안전하게 저장됨
        return new GameState(updatedBoard, null, allowedStarts);
    }
}