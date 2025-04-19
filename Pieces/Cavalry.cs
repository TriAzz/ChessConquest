using System.Collections.Generic;

namespace ChessConquest.Pieces
{
    public class Cavalry : Piece
    {
        // Use the knight emoji for the Cavalry piece (Placeholder)
        public override char Symbol => Color == PieceColor.White ? '\u2658' : '\u265E';
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

            // Horizontal/Vertical moves (1-2 squares, cannot jump)
            int[,] directions = {
                {-1, 0}, {1, 0}, // vertical (up, down)
                {0, -1}, {0, 1}  // horizontal (left, right)
            };

            for (int dir = 0; dir < 4; dir++)
            {
                int rowDir = directions[dir, 0];
                int colDir = directions[dir, 1];

                // Try moving 1 square
                Position oneStep = new Position(Position.Row + rowDir, Position.Column + colDir);
                if (board.IsPositionInBounds(oneStep))
                {
                    Piece? pieceAtOneStep = board.GetPieceAt(oneStep);
                    if (pieceAtOneStep == null || pieceAtOneStep.Color != Color)
                    {
                        validMoves.Add(oneStep);
                        
                        // If we can move one step and the square is empty, try two steps
                        if (pieceAtOneStep == null)
                        {
                            Position twoStep = new Position(Position.Row + 2 * rowDir, Position.Column + 2 * colDir);
                            if (board.IsPositionInBounds(twoStep))
                            {
                                Piece? pieceAtTwoStep = board.GetPieceAt(twoStep);
                                if (pieceAtTwoStep == null || pieceAtTwoStep.Color != Color)
                                {
                                    validMoves.Add(twoStep);
                                }
                            }
                        }
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
