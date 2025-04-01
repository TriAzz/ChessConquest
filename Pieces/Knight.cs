using System.Collections.Generic;

namespace ChessConquest.Pieces
{
    public class Knight : Piece
    {
        public override char Symbol => Color == PieceColor.White ? '\u2658' : '\u265E';
        
        public Knight(PieceColor color, Position position) : base(color, position)
        {
        }

        public override List<Position> GetValidMoves(Board board)
        {
            List<Position> validMoves = new List<Position>();
            
            // All possible knight moves
            int[,] knightMoves = {
                {-2, -1}, {-2, 1}, {-1, -2}, {-1, 2},
                {1, -2}, {1, 2}, {2, -1}, {2, 1}
            };
            
            for (int i = 0; i < 8; i++)
            {
                int rowOffset = knightMoves[i, 0];
                int colOffset = knightMoves[i, 1];
                
                Position targetPos = new Position(Position.Row + rowOffset, Position.Column + colOffset);
                
                if (board.IsPositionInBounds(targetPos))
                {
                    Piece? pieceAtTarget = board.GetPieceAt(targetPos);
                    
                    if (pieceAtTarget == null || pieceAtTarget.Color != Color)
                    {
                        validMoves.Add(targetPos);
                    }
                }
            }
            
            return validMoves;
        }

        public override Piece Clone()
        {
            Knight clone = new Knight(Color, Position);
            if (HasMoved) clone.MoveTo(Position);
            return clone;
        }
    }
}
