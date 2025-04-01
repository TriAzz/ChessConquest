using System;
using System.Collections.Generic;
using ChessConquest.Pieces;

namespace ChessConquest
{
    public class Board
    {
        public const int BoardSize = 8;
        private readonly Piece?[,] pieces;
        
        public Board()
        {
            pieces = new Piece?[BoardSize, BoardSize];
        }
        
        /// <summary>
        /// Create a deep copy of the board
        /// </summary>
        public Board Clone()
        {
            Board clone = new();
            
            for (int row = 0; row < BoardSize; row++)
            {
                for (int col = 0; col < BoardSize; col++)
                {
                    Piece? piece = pieces[row, col];
                    if (piece != null)
                    {
                        clone.pieces[row, col] = piece.Clone();
                    }
                }
            }
            
            return clone;
        }
        
        /// <summary>
        /// Set up the board with the standard chess piece arrangement
        /// </summary>
        public void SetupStandardBoard()
        {
            // Clear the board
            for (int row = 0; row < BoardSize; row++)
            {
                for (int col = 0; col < BoardSize; col++)
                {
                    pieces[row, col] = null;
                }
            }
            
            // Set up pawns
            for (int col = 0; col < BoardSize; col++)
            {
                // Replace the middle pawns (columns 3 and 4) with Vanguards
                if (col == 3 || col == 4)
                {
                    pieces[1, col] = new Vanguard(PieceColor.Black, new Position(1, col));
                    pieces[6, col] = new Vanguard(PieceColor.White, new Position(6, col));
                }
                else
                {
                    pieces[1, col] = new Pawn(PieceColor.Black, new Position(1, col));
                    pieces[6, col] = new Pawn(PieceColor.White, new Position(6, col));
                }
            }
            
            // Set up rooks
            pieces[0, 0] = new Rook(PieceColor.Black, new Position(0, 0));
            pieces[0, 7] = new Rook(PieceColor.Black, new Position(0, 7));
            pieces[7, 0] = new Rook(PieceColor.White, new Position(7, 0));
            pieces[7, 7] = new Rook(PieceColor.White, new Position(7, 7));
            
            // Set up knights
            pieces[0, 1] = new Knight(PieceColor.Black, new Position(0, 1));
            pieces[0, 6] = new Knight(PieceColor.Black, new Position(0, 6));
            pieces[7, 1] = new Knight(PieceColor.White, new Position(7, 1));
            pieces[7, 6] = new Knight(PieceColor.White, new Position(7, 6));
            
            // Set up bishops
            pieces[0, 2] = new Bishop(PieceColor.Black, new Position(0, 2));
            pieces[0, 5] = new Bishop(PieceColor.Black, new Position(0, 5));
            pieces[7, 2] = new Bishop(PieceColor.White, new Position(7, 2));
            pieces[7, 5] = new Bishop(PieceColor.White, new Position(7, 5));
            
            // Set up queens
            pieces[0, 3] = new Queen(PieceColor.Black, new Position(0, 3));
            pieces[7, 3] = new Queen(PieceColor.White, new Position(7, 3));
            
            // Set up kings
            pieces[0, 4] = new King(PieceColor.Black, new Position(0, 4));
            pieces[7, 4] = new King(PieceColor.White, new Position(7, 4));
        }
        
        /// <summary>
        /// Set up a custom board arrangement
        /// </summary>
        public void SetupCustomGame(List<Piece> customPieces)
        {
            if (customPieces == null)
            {
                throw new ArgumentNullException(nameof(customPieces), "Custom pieces list cannot be null");
            }
            
            // Clear the board
            for (int row = 0; row < BoardSize; row++)
            {
                for (int col = 0; col < BoardSize; col++)
                {
                    pieces[row, col] = null;
                }
            }
            
            // Place the custom pieces
            foreach (var piece in customPieces)
            {
                if (piece != null)
                {
                    pieces[piece.Position.Row, piece.Position.Column] = piece;
                }
            }
        }
        
        /// <summary>
        /// Get the piece at the specified position
        /// </summary>
        public Piece? GetPieceAt(Position position)
        {
            if (!IsPositionInBounds(position))
            {
                return null;
            }
            
            return pieces[position.Row, position.Column];
        }
        
        /// <summary>
        /// Move a piece from its current position to a new position
        /// </summary>
        public void MovePiece(Position from, Position to)
        {
            Piece? piece = GetPieceAt(from);
            
            if (piece == null)
            {
                throw new InvalidOperationException("No piece at the specified position");
            }
            
            // Special handling for castling
            if (piece is King && Math.Abs(from.Column - to.Column) == 2)
            {
                // Kingside castling
                if (to.Column > from.Column)
                {
                    Position rookFrom = new(from.Row, 7);
                    Position rookTo = new(from.Row, from.Column + 1);
                    MovePiece(rookFrom, rookTo);
                }
                // Queenside castling
                else
                {
                    Position rookFrom = new(from.Row, 0);
                    Position rookTo = new(from.Row, from.Column - 1);
                    MovePiece(rookFrom, rookTo);
                }
            }
            
            // Update the piece's position
            piece.MoveTo(to);
            
            // Update the board
            pieces[from.Row, from.Column] = null;
            pieces[to.Row, to.Column] = piece;
        }
        
        /// <summary>
        /// Check if the given position is within the bounds of the board
        /// </summary>
        public bool IsPositionInBounds(Position position)
        {
            return position.Row >= 0 && position.Row < BoardSize && 
                   position.Column >= 0 && position.Column < BoardSize;
        }
        
        /// <summary>
        /// Get all pieces of a specific color
        /// </summary>
        public List<Piece> GetPiecesByColor(PieceColor color)
        {
            List<Piece> result = new();
            
            for (int row = 0; row < BoardSize; row++)
            {
                for (int col = 0; col < BoardSize; col++)
                {
                    Piece? piece = pieces[row, col];
                    if (piece != null && piece.Color == color)
                    {
                        result.Add(piece);
                    }
                }
            }
            
            return result;
        }
        
        /// <summary>
        /// Find the king of the specified color
        /// </summary>
        public King FindKing(PieceColor color)
        {
            for (int row = 0; row < BoardSize; row++)
            {
                for (int col = 0; col < BoardSize; col++)
                {
                    if (pieces[row, col] is King king && king.Color == color)
                    {
                        return king;
                    }
                }
            }
            
            throw new InvalidOperationException($"No {color} king found on the board");
        }
        
        /// <summary>
        /// Check if a position is under attack by any piece of the specified color
        /// </summary>
        public bool IsPositionUnderAttack(Position position, PieceColor attackingColor)
        {
            List<Piece> attackingPieces = GetPiecesByColor(attackingColor);
            
            foreach (var piece in attackingPieces)
            {
                // Special case for King to avoid recursion
                if (piece is King king)
                {
                    // Check if the king is adjacent to the position
                    int rowDiff = Math.Abs(king.Position.Row - position.Row);
                    int colDiff = Math.Abs(king.Position.Column - position.Column);
                    if (rowDiff <= 1 && colDiff <= 1 && (rowDiff > 0 || colDiff > 0))
                    {
                        return true;
                    }
                    continue;
                }
                
                // For pawns, we need special logic since their attack pattern is different from their move pattern
                if (piece is Pawn pawn)
                {
                    int direction = attackingColor == PieceColor.White ? -1 : 1;
                    Position[] attackPositions = new Position[]
                    {
                        new(position.Row - direction, position.Column - 1),
                        new(position.Row - direction, position.Column + 1)
                    };
                    
                    foreach (var attackPos in attackPositions)
                    {
                        if (attackPos.Equals(pawn.Position))
                        {
                            return true;
                        }
                    }
                }
                else
                {
                    // For other pieces, check if the position is in their valid moves
                    List<Position> validMoves = piece.GetValidMoves(this);
                    if (validMoves.Contains(position))
                    {
                        return true;
                    }
                }
            }
            
            return false;
        }
        
        /// <summary>
        /// Check if the king of the specified color is in check
        /// </summary>
        public bool IsKingInCheck(PieceColor kingColor)
        {
            King king = FindKing(kingColor);
            return IsPositionUnderAttack(king.Position, kingColor.GetOppositeColor());
        }
        
        /// <summary>
        /// Check if moving a piece to a target position would result in the king being in check
        /// </summary>
        public bool WouldMoveResultInCheck(Piece piece, Position targetPosition)
        {
            if (piece == null)
            {
                throw new ArgumentNullException(nameof(piece), "Piece cannot be null");
            }
            
            // Save the current state
            Position originalPosition = piece.Position;
            Piece? capturedPiece = GetPieceAt(targetPosition);
            
            // Temporarily update the board state
            pieces[originalPosition.Row, originalPosition.Column] = null;
            pieces[targetPosition.Row, targetPosition.Column] = piece;
            
            // Find the king
            King king = FindKing(piece.Color);
            Position kingPosition = king.Position;
            
            // If we're moving the king, update the king position
            if (piece is King)
            {
                kingPosition = targetPosition;
            }
            
            // Check if the king would be under attack
            bool wouldBeInCheck = false;
            List<Piece> opponentPieces = GetPiecesByColor(piece.Color.GetOppositeColor());
            
            foreach (var opponentPiece in opponentPieces)
            {
                // Skip the captured piece
                if (opponentPiece == capturedPiece)
                {
                    continue;
                }
                
                // Special case for King to avoid recursion
                if (opponentPiece is King opponentKing)
                {
                    int rowDiff = Math.Abs(opponentKing.Position.Row - kingPosition.Row);
                    int colDiff = Math.Abs(opponentKing.Position.Column - kingPosition.Column);
                    if (rowDiff <= 1 && colDiff <= 1 && (rowDiff > 0 || colDiff > 0))
                    {
                        wouldBeInCheck = true;
                        break;
                    }
                    continue;
                }
                
                // For pawns, we need special logic
                if (opponentPiece is Pawn pawn)
                {
                    int direction = pawn.Color == PieceColor.White ? -1 : 1;
                    Position[] attackPositions = new Position[]
                    {
                        new(kingPosition.Row - direction, kingPosition.Column - 1),
                        new(kingPosition.Row - direction, kingPosition.Column + 1)
                    };
                    
                    foreach (var attackPos in attackPositions)
                    {
                        if (attackPos.Equals(pawn.Position))
                        {
                            wouldBeInCheck = true;
                            break;
                        }
                    }
                    
                    if (wouldBeInCheck)
                    {
                        break;
                    }
                }
                else
                {
                    // For other pieces, check if they can attack the king
                    List<Position> validMoves = opponentPiece.GetValidMoves(this);
                    if (validMoves.Contains(kingPosition))
                    {
                        wouldBeInCheck = true;
                        break;
                    }
                }
            }
            
            // Restore the original state
            pieces[originalPosition.Row, originalPosition.Column] = piece;
            // If capturedPiece is null, we're restoring to an empty square
            if (capturedPiece == null)
            {
                pieces[targetPosition.Row, targetPosition.Column] = null;
            }
            else
            {
                pieces[targetPosition.Row, targetPosition.Column] = capturedPiece;
            }
            
            return wouldBeInCheck;
        }
        
        /// <summary>
        /// Check if the game is in checkmate for the specified color
        /// </summary>
        public bool IsCheckmate(PieceColor color)
        {
            // If the king is not in check, it's not checkmate
            if (!IsKingInCheck(color))
            {
                return false;
            }
            
            // Check if any piece can make a move that gets out of check
            List<Piece> colorPieces = GetPiecesByColor(color);
            
            foreach (var piece in colorPieces)
            {
                if (piece != null)
                {
                    List<Position> validMoves = piece.GetValidMoves(this);
                    
                    foreach (var move in validMoves)
                    {
                        if (!WouldMoveResultInCheck(piece, move))
                        {
                            return false; // Found a legal move, not checkmate
                        }
                    }
                }
            }
            
            return true; // No legal moves found, it's checkmate
        }
        
        /// <summary>
        /// Check if the game is in stalemate for the specified color
        /// </summary>
        public bool IsStalemate(PieceColor color)
        {
            // If the king is in check, it's not stalemate
            if (IsKingInCheck(color))
            {
                return false;
            }
            
            // Check if any piece can make a legal move
            List<Piece> colorPieces = GetPiecesByColor(color);
            
            foreach (var piece in colorPieces)
            {
                if (piece != null)
                {
                    List<Position> validMoves = piece.GetValidMoves(this);
                    
                    foreach (var move in validMoves)
                    {
                        if (!WouldMoveResultInCheck(piece, move))
                        {
                            return false; // Found a legal move, not stalemate
                        }
                    }
                }
            }
            
            return true; // No legal moves found, it's stalemate
        }
        
        /// <summary>
        /// Display the board in the console
        /// </summary>
        public void Display()
        {
            Console.WriteLine("  a b c d e f g h");
            Console.WriteLine(" +-----------------+");
            
            for (int row = 0; row < BoardSize; row++)
            {
                Console.Write($"{8 - row}|");
                
                for (int col = 0; col < BoardSize; col++)
                {
                    Piece? piece = pieces[row, col];
                    char symbol = piece != null ? piece.Symbol : ' ';
                    Console.Write($"{symbol}|");
                }
                
                Console.WriteLine($" {8 - row}"); 
            }
            
            Console.WriteLine(" +-----------------+");
            Console.WriteLine("  a b c d e f g h");
        }
    }
}
