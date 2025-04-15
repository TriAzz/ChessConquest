using System.Collections.Generic;

namespace ChessConquest.Pieces
{
    public class Cavalry : Piece
    {
        public override char Symbol => Color == PieceColor.White ? '\u2659' : '\u265F'; // Placeholder symbol

        public Cavalry(PieceColor color, Position position) : base(color, position) { }

        public override List<Position> GetValidMoves(Board board)
        {
            List<Position> validMoves = new List<Position>();

            // Standard knight moves (can jump)
            int[,] knightMoves = {
                {-2, -1}, {-2, 1}, {-1, -2}, {-1, 2},
                {1, -2}, {1, 2}, {2, -1}, {2, 1}
            };
            for (int i = 0; i < 8; i++)
            {
                int rowOffset = knightMoves[i, 0];
                int colOffset = knightMoves[i, 1];
                Position target = new Position(Position.Row + rowOffset, Position.Column + colOffset);
                if (board.IsPositionInBounds(target))
                {
                    Piece? pieceAtTarget = board.GetPieceAt(target);
                    if (pieceAtTarget == null || pieceAtTarget.Color != Color)
                        validMoves.Add(target);
                }
            }

            // Extended Cavalry moves: (±2,±2), cannot jump
            int[,] cavalryMoves = {
                {2, 2}, {2, -2}, {-2, 2}, {-2, -2}
            };
            for (int i = 0; i < 4; i++)
            {
                int rowOffset = cavalryMoves[i, 0];
                int colOffset = cavalryMoves[i, 1];
                Position target = new Position(Position.Row + rowOffset, Position.Column + colOffset);
                if (board.IsPositionInBounds(target))
                {
                    // Check both intermediate squares must be empty
                    int rowStep = rowOffset / 2;
                    int colStep = colOffset / 2;
                    Position inter1 = new Position(Position.Row + rowStep, Position.Column);
                    Position inter2 = new Position(Position.Row, Position.Column + colStep);
                    if (board.GetPieceAt(inter1) == null && board.GetPieceAt(inter2) == null)
                    {
                        Piece? pieceAtTarget = board.GetPieceAt(target);
                        if (pieceAtTarget == null || pieceAtTarget.Color != Color)
                            validMoves.Add(target);
                    }
                }
            }
            return validMoves;
        }

        public override Piece Clone()
        {
            Cavalry clone = new Cavalry(Color, Position);
            if (HasMoved) clone.MoveTo(Position);
            return clone;
        }
    }
}
