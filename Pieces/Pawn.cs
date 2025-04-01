using System;
using System.Collections.Generic;

namespace ChessConquest.Pieces
{
    public class Pawn : Piece
    {
        public override char Symbol => Color == PieceColor.White ? '♙' : '♟';
        
        public Pawn(PieceColor color, Position position) : base(color, position)
        {
        }

        public override List<Position> GetValidMoves(Board board)
        {
            List<Position> validMoves = new List<Position>();
            int direction = Color == PieceColor.White ? -1 : 1; // White moves up, Black moves down
            
            // Forward move
            Position oneForward = new Position(Position.Row + direction, Position.Column);
            if (board.IsPositionInBounds(oneForward) && board.GetPieceAt(oneForward) == null)
            {
                validMoves.Add(oneForward);
                
                // Two squares forward from starting position
                if (!HasMoved)
                {
                    Position twoForward = new Position(Position.Row + 2 * direction, Position.Column);
                    if (board.IsPositionInBounds(twoForward) && board.GetPieceAt(twoForward) == null)
                    {
                        validMoves.Add(twoForward);
                    }
                }
            }
            
            // Captures
            Position[] capturePositions = new Position[]
            {
                new Position(Position.Row + direction, Position.Column - 1), // Left capture
                new Position(Position.Row + direction, Position.Column + 1)  // Right capture
            };
            
            foreach (var capturePos in capturePositions)
            {
                if (board.IsPositionInBounds(capturePos))
                {
                    Piece? pieceAtCapture = board.GetPieceAt(capturePos);
                    if (pieceAtCapture != null && pieceAtCapture.Color != Color)
                    {
                        validMoves.Add(capturePos);
                    }
                    
                    // En passant (would need to be implemented with additional game state tracking)
                }
            }
            
            return validMoves;
        }

        public override Piece Clone()
        {
            Pawn clone = new Pawn(Color, Position);
            if (HasMoved) clone.MoveTo(Position); // This sets HasMoved to true
            return clone;
        }
    }
}
