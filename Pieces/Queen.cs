using System.Collections.Generic;

namespace ChessConquest.Pieces
{
    public class Queen : Piece
    {
        public override char Symbol => Color == PieceColor.White ? '\u2655' : '\u265B';
        
        public Queen(PieceColor color, Position position) : base(color, position)
        {
        }

        public override List<Position> GetValidMoves(Board board)
        {
            List<Position> validMoves = new List<Position>();
            
            // Directions: all 8 directions (horizontal, vertical, diagonal)
            int[,] directions = {
                {-1, -1}, {-1, 0}, {-1, 1}, // up-left, up, up-right
                {0, -1}, {0, 1},             // left, right
                {1, -1}, {1, 0}, {1, 1}      // down-left, down, down-right
            };
            
            for (int i = 0; i < 8; i++)
            {
                int rowDir = directions[i, 0];
                int colDir = directions[i, 1];
                
                int currentRow = Position.Row + rowDir;
                int currentCol = Position.Column + colDir;
                
                while (board.IsPositionInBounds(new Position(currentRow, currentCol)))
                {
                    Position currentPos = new Position(currentRow, currentCol);
                    Piece? pieceAtPos = board.GetPieceAt(currentPos);
                    
                    if (pieceAtPos == null)
                    {
                        validMoves.Add(currentPos);
                    }
                    else
                    {
                        if (pieceAtPos.Color != Color)
                        {
                            validMoves.Add(currentPos); // Capture
                        }
                        break; // Can't move past a piece
                    }
                    
                    currentRow += rowDir;
                    currentCol += colDir;
                }
            }
            
            return validMoves;
        }

        public override Piece Clone()
        {
            Queen clone = new Queen(Color, Position);
            if (HasMoved) clone.MoveTo(Position);
            return clone;
        }
    }
}
