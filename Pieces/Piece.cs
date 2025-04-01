using System;
using System.Collections.Generic;

namespace ChessConquest.Pieces
{
    /// <summary>
    /// Base class for all chess pieces
    /// </summary>
    public abstract class Piece
    {
        /// <summary>
        /// The color of the piece (White or Black)
        /// </summary>
        public PieceColor Color { get; protected set; }

        /// <summary>
        /// The current position of the piece on the board
        /// </summary>
        public Position Position { get; protected set; }

        /// <summary>
        /// The symbol used to represent the piece on the board
        /// </summary>
        public virtual char Symbol { get; protected set; }

        /// <summary>
        /// Whether the piece has moved yet (useful for special moves like castling)
        /// </summary>
        public bool HasMoved { get; protected set; } = false;

        /// <summary>
        /// Constructor for the piece
        /// </summary>
        /// <param name="color">The color of the piece</param>
        /// <param name="position">The initial position of the piece</param>
        protected Piece(PieceColor color, Position position)
        {
            Color = color;
            Position = position;
        }

        /// <summary>
        /// Move the piece to a new position
        /// </summary>
        /// <param name="newPosition">The new position to move to</param>
        public virtual void MoveTo(Position newPosition)
        {
            Position = newPosition;
            HasMoved = true;
        }

        /// <summary>
        /// Get all valid moves for this piece on the current board
        /// </summary>
        /// <param name="board">The current game board</param>
        /// <returns>A list of valid positions this piece can move to</returns>
        public abstract List<Position> GetValidMoves(Board board);

        /// <summary>
        /// Check if a move to the target position is valid
        /// </summary>
        /// <param name="board">The current game board</param>
        /// <param name="targetPosition">The position to check</param>
        /// <returns>True if the move is valid, false otherwise</returns>
        public virtual bool IsValidMove(Board board, Position targetPosition)
        {
            return GetValidMoves(board).Contains(targetPosition);
        }

        /// <summary>
        /// Create a deep copy of this piece
        /// </summary>
        /// <returns>A new instance of the piece with the same properties</returns>
        public abstract Piece Clone();

        /// <summary>
        /// Get the display name of the piece (e.g., "White Pawn")
        /// </summary>
        /// <returns>The display name of the piece</returns>
        public virtual string GetDisplayName()
        {
            return $"{Color} {GetType().Name}";
        }
    }

    /// <summary>
    /// Enum representing the color of a chess piece
    /// </summary>
    public enum PieceColor
    {
        White,
        Black
    }

    /// <summary>
    /// Struct representing a position on the chess board
    /// </summary>
    public struct Position
    {
        public int Row { get; }
        public int Column { get; }

        public Position(int row, int column)
        {
            Row = row;
            Column = column;
        }

        public static Position FromAlgebraic(string algebraic)
        {
            if (string.IsNullOrEmpty(algebraic) || algebraic.Length != 2)
                throw new ArgumentException("Invalid algebraic notation", nameof(algebraic));

            char file = algebraic[0];
            char rank = algebraic[1];

            if (file < 'a' || file > 'h' || rank < '1' || rank > '8')
                throw new ArgumentException("Invalid algebraic notation", nameof(algebraic));

            int column = file - 'a';
            int row = 7 - (rank - '1'); // Convert to 0-based index from bottom-left

            return new Position(row, column);
        }

        public string ToAlgebraic()
        {
            char file = (char)('a' + Column);
            char rank = (char)('8' - Row);
            return $"{file}{rank}";
        }

        public override bool Equals(object? obj)
        {
            if (obj is Position other)
            {
                return Row == other.Row && Column == other.Column;
            }
            return false;
        }

        public override int GetHashCode()
        {
            return (Row << 3) | Column;
        }

        public static bool operator ==(Position left, Position right)
        {
            return left.Row == right.Row && left.Column == right.Column;
        }

        public static bool operator !=(Position left, Position right)
        {
            return !(left == right);
        }

        public override string ToString()
        {
            return ToAlgebraic();
        }
    }
}
