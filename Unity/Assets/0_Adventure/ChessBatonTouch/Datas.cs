using System;
using System.Collections.Generic;
using System.Linq;

namespace System.Runtime.CompilerServices
{
    internal static class IsExternalInit { }
}

public enum PieceType { None, Pawn, Knight, Bishop, Rook, Queen, King }
public record ChessSquare(int X, int Y, PieceType Piece);

public record GameState(IReadOnlyList<ChessSquare> Board, ChessSquare ActiveSquare, IReadOnlyList<ChessSquare> AllowedStartingSquares)
{
    public bool IsVictory => ActiveSquare != null && ActiveSquare.Piece == PieceType.King;
}