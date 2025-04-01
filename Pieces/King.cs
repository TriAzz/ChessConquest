using System.Collections.Generic;

namespace ChessConquest.Pieces
{
    public class King : Piece
    {
        public override char Symbol => Color == PieceColor.White ? '\u2654' : '\u265A';
        
        public King(PieceColor color, Position position) : base(color, position)
        {
        }

        public override List<Position> GetValidMoves(Board board)
        {
            List<Position> validMoves = new List<Position>();
            
            // All 8 directions around the king
            int[,] directions = {
                {-1, -1}, {-1, 0}, {-1, 1}, // up-left, up, up-right
                {0, -1}, {0, 1},             // left, right
                {1, -1}, {1, 0}, {1, 1}      // down-left, down, down-right
            };
            
            for (int i = 0; i < 8; i++)
            {
                int rowDir = directions[i, 0];
                int colDir = directions[i, 1];
                
                Position targetPos = new Position(Position.Row + rowDir, Position.Column + colDir);
                
                if (board.IsPositionInBounds(targetPos))
                {
                    Piece? pieceAtTarget = board.GetPieceAt(targetPos);
                    
                    if (pieceAtTarget == null || pieceAtTarget.Color != Color)
                    {
                        // We'll check for check later to avoid recursion
                        validMoves.Add(targetPos);
                    }
                }
            }
            
            // Castling logic (would need to be implemented with additional game state tracking)
            if (!HasMoved && !board.IsKingInCheck(Color))
            {
                // Kingside castling
                if (CanCastle(board, true))
                {
                    validMoves.Add(new Position(Position.Row, Position.Column + 2));
                }
                
                // Queenside castling
                if (CanCastle(board, false))
                {
                    validMoves.Add(new Position(Position.Row, Position.Column - 2));
                }
            }
            
            return validMoves;
        }

        private bool CanCastle(Board board, bool kingSide)
        {
            int direction = kingSide ? 1 : -1;
            int rookColumn = kingSide ? 7 : 0;
            
            // Check if rook is in the correct position and hasn't moved
            Piece? rookPiece = board.GetPieceAt(new Position(Position.Row, rookColumn));
            if (rookPiece == null || !(rookPiece is Rook) || rookPiece.HasMoved)
            {
                return false;
            }
            
            // Check if squares between king and rook are empty
            int endColumn = kingSide ? rookColumn - 1 : rookColumn + 1;
            int startColumn = Position.Column + direction;
            
            for (int col = startColumn; col != endColumn + direction; col += direction)
            {
                if (board.GetPieceAt(new Position(Position.Row, col)) != null)
                {
                    return false;
                }
            }
            
            // Check if king passes through or ends up in check
            for (int offset = 0; offset <= 2; offset++)
            {
                Position checkPos = new Position(Position.Row, Position.Column + offset * direction);
                if (board.IsPositionUnderAttack(checkPos, Color.GetOppositeColor()))
                {
                    return false;
                }
            }
            
            return true;
        }

        public override Piece Clone()
        {
            King clone = new King(Color, Position);
            if (HasMoved) clone.MoveTo(Position);
            return clone;
        }
    }

    public static class PieceColorExtensions
    {
        public static PieceColor GetOppositeColor(this PieceColor color)
        {
            return color == PieceColor.White ? PieceColor.Black : PieceColor.White;
        }
    }
}
