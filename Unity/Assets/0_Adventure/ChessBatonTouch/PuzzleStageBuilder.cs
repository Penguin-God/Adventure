using System.Collections.Generic;

public static class PuzzleStageBuilder
{
    public static GameState CreateFromSO(StageDataSO stageData)
    {
        var board = new List<ChessSquare>();

        for (int y = 0; y < 8; y++)
        {
            for (int x = 0; x < 8; x++)
                board.Add(new ChessSquare(x, y, PieceType.None));
        }

        var allowedStarts = new List<ChessSquare>();

        foreach (var setup in stageData.initialPieces)
        {
            int index = setup.Y * 8 + setup.X;
            board[index] = board[index] with { Piece = setup.Piece };

            if (setup.DisableStart == false)
                allowedStarts.Add(board[index]);
        }

        return new GameState(board, null, allowedStarts);
    }
}