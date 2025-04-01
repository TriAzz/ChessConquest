using System.Collections.Generic;

namespace ChessConquest.Pieces
{
    public class Rook : Piece
    {
        public override char Symbol => Color == PieceColor.White ? '\u2656' : '\u265c';
        
        public Rook(PieceColor color, Position position) : base(color, position)
        {
        }

        public override List<Position> GetValidMoves(Board board)
        {
            List<Position> validMoves = new List<Position>();
            
            // Directions: up, right, down, left
            int[,] directions = { {-1, 0}, {0, 1}, {1, 0}, {0, -1} };
            
            for (int i = 0; i < 4; i++)
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
            Rook clone = new Rook(Color, Position);
            if (HasMoved) clone.MoveTo(Position); // This sets HasMoved to true
            return clone;
        }
    }
}
