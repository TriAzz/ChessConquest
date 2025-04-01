using System;
using System.Collections.Generic;

namespace ChessConquest.Pieces
{
    public class Vanguard : Piece
    {
        public Vanguard(PieceColor color, Position position) : base(color, position)
        {
            HasMoved = false;
        }

        public override char Symbol => Color == PieceColor.White ? '♢' : '♦';

        public override List<Position> GetValidMoves(Board board)
        {
            List<Position> validMoves = new();
            
            // Diagonal movement (1 square only)
            int[,] diagonalDirections = {
                {-1, -1}, {-1, 1}, // Up-left, up-right
                {1, -1}, {1, 1}     // Down-left, down-right
            };
            
            for (int i = 0; i < 4; i++)
            {
                int rowDir = diagonalDirections[i, 0];
                int colDir = diagonalDirections[i, 1];
                
                Position targetPos = new(
                    Position.Row + rowDir,
                    Position.Column + colDir
                );
                
                if (board.IsPositionInBounds(targetPos))
                {
                    Piece? pieceAtTarget = board.GetPieceAt(targetPos);
                    
                    if (pieceAtTarget == null)
                    {
                        validMoves.Add(targetPos); // Empty square
                    }
                    else if (pieceAtTarget.Color != Color)
                    {
                        validMoves.Add(targetPos); // Capture
                    }
                }
            }
            
            // Horizontal movement (2 spaces)
            int[] horizontalDirections = new[] { -1, 1 }; // Left and right
            foreach (int direction in horizontalDirections)
            {
                for (int i = 1; i <= 2; i++)
                {
                    Position targetPos = new(Position.Row, Position.Column + (direction * i));
                    
                    if (!board.IsPositionInBounds(targetPos))
                    {
                        break; // Out of bounds
                    }
                    
                    Piece? pieceAtTarget = board.GetPieceAt(targetPos);
                    
                    if (pieceAtTarget == null)
                    {
                        validMoves.Add(targetPos); // Empty square
                    }
                    else if (pieceAtTarget.Color != Color)
                    {
                        validMoves.Add(targetPos); // Capture
                        break; // Can't move further in this direction
                    }
                    else
                    {
                        break; // Blocked by own piece
                    }
                }
            }
            
            // Vertical movement (2 spaces)
            int[] verticalDirections = new[] { -1, 1 }; // Up and down
            foreach (int direction in verticalDirections)
            {
                for (int i = 1; i <= 2; i++)
                {
                    Position targetPos = new(Position.Row + (direction * i), Position.Column);
                    
                    if (!board.IsPositionInBounds(targetPos))
                    {
                        break; // Out of bounds
                    }
                    
                    Piece? pieceAtTarget = board.GetPieceAt(targetPos);
                    
                    if (pieceAtTarget == null)
                    {
                        validMoves.Add(targetPos); // Empty square
                    }
                    else if (pieceAtTarget.Color != Color)
                    {
                        validMoves.Add(targetPos); // Capture
                        break; // Can't move further in this direction
                    }
                    else
                    {
                        break; // Blocked by own piece
                    }
                }
            }
            
            return validMoves;
        }
        
        public override Piece Clone()
        {
            Vanguard clone = new Vanguard(Color, Position);
            if (HasMoved) clone.MoveTo(Position); // This sets HasMoved to true
            return clone;
        }
    }
}
