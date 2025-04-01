using System;
using System.Collections.Generic;
using ChessConquest.Pieces;

namespace ChessConquest
{
    public class Game
    {
        public Board Board { get; set; } 
        public PieceColor CurrentTurn { get; private set; }
        public ChessAI AI { get; private set; }
        public PieceColor PlayerColor { get; private set; }
        public PieceColor AIColor { get; private set; }
        public bool IsGameOver { get; private set; }
        public GameResult? Result { get; private set; }
        public int Difficulty { get; set; } = 3;
        public int TurnCount { get; private set; } = 1;
        
        public Game(PieceColor playerColor = PieceColor.White, bool standardSetup = true)
        {
            Board = new();
            PlayerColor = playerColor;
            AIColor = playerColor == PieceColor.White ? PieceColor.Black : PieceColor.White;
            AI = new ChessAI(AIColor);
            CurrentTurn = PieceColor.White; // White always starts
            
            if (standardSetup)
            {
                Board.SetupStandardBoard();
            }
        }
        
        /// <summary>
        /// Set up a custom game with specified pieces
        /// </summary>
        public void SetupCustomGame(List<Piece> pieces)
        {
            Board.SetupCustomGame(pieces);
        }
        
        /// <summary>
        /// Make a move from one position to another
        /// </summary>
        public bool MakeMove(Position from, Position to)
        {
            // Check if the game is over
            if (IsGameOver)
            {
                return false;
            }
            
            // Check if it's the correct player's turn
            Piece? piece = Board.GetPieceAt(from);
            if (piece == null || piece.Color != CurrentTurn)
            {
                return false;
            }
            
            // Check if the move is valid
            if (!piece.IsValidMove(Board, to))
            {
                return false;
            }
            
            // Check if the move would result in check
            if (Board.WouldMoveResultInCheck(piece, to))
            {
                return false;
            }
            
            // Make the move
            Board.MovePiece(from, to);
            
            // Switch turns
            CurrentTurn = CurrentTurn == PieceColor.White ? PieceColor.Black : PieceColor.White;
            TurnCount++;
            
            // Check for game over conditions
            CheckGameOverConditions();
            
            return true;
        }
        
        /// <summary>
        /// Make a move using algebraic notation (e.g., "e2e4")
        /// </summary>
        public bool MakeMove(string moveNotation)
        {
            if (moveNotation.Length != 4)
            {
                return false;
            }
            
            try
            {
                Position from = Position.FromAlgebraic(moveNotation.Substring(0, 2));
                Position to = Position.FromAlgebraic(moveNotation.Substring(2, 2));
                return MakeMove(from, to);
            }
            catch (ArgumentException)
            {
                return false;
            }
        }
        
        /// <summary>
        /// Let the AI make a move
        /// </summary>
        public bool MakeAIMove()
        {
            // Check if it's the AI's turn
            if (CurrentTurn != AIColor || IsGameOver)
            {
                return false;
            }
            
            try
            {
                // Get the AI's move
                (Position from, Position to) = AI.MakeMove(Board, Difficulty);
                
                // Make the move
                Board.MovePiece(from, to);
                
                // Switch turns
                CurrentTurn = CurrentTurn == PieceColor.White ? PieceColor.Black : PieceColor.White;
                TurnCount++;
                
                // Check for game over conditions
                CheckGameOverConditions();
                
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }
        
        /// <summary>
        /// Check if the game is over (checkmate or stalemate)
        /// </summary>
        private void CheckGameOverConditions()
        {
            // Check for checkmate
            if (Board.IsCheckmate(CurrentTurn))
            {
                IsGameOver = true;
                Result = CurrentTurn == PlayerColor ? GameResult.AIWin : GameResult.PlayerWin;
            }
            // Check for stalemate
            else if (Board.IsStalemate(CurrentTurn))
            {
                IsGameOver = true;
                Result = GameResult.Draw;
            }
        }
        
        /// <summary>
        /// Get all valid moves for a piece at the specified position
        /// </summary>
        public List<Position> GetValidMovesForPiece(Position position)
        {
            Piece? piece = Board.GetPieceAt(position);
            if (piece == null || piece.Color != CurrentTurn)
            {
                return new List<Position>();
            }
            
            List<Position> validMoves = piece.GetValidMoves(Board);
            validMoves.RemoveAll(move => Board.WouldMoveResultInCheck(piece, move));
            
            return validMoves;
        }
        
        /// <summary>
        /// Get the winner of the game
        /// </summary>
        public PieceColor? GetWinner()
        {
            if (!IsGameOver)
            {
                return null;
            }
            
            if (Result == GameResult.PlayerWin)
            {
                return PlayerColor;
            }
            else if (Result == GameResult.AIWin)
            {
                return AIColor;
            }
            
            return null; // Draw
        }
        
        /// <summary>
        /// Display the current state of the game
        /// </summary>
        public void DisplayGame()
        {
            // Console.Clear(); // Removed to avoid issues in some environments
            Console.WriteLine("Chess Conquest");
            Console.WriteLine("==============\n");
            
            Board.Display();
            
            Console.WriteLine();
            Console.WriteLine($"Current turn: {CurrentTurn}");
            Console.WriteLine($"Turn count: {TurnCount}");
            
            if (Board.IsKingInCheck(CurrentTurn))
            {
                Console.WriteLine($"{CurrentTurn} is in check!");
            }
            
            if (IsGameOver)
            {
                Console.WriteLine("\nGame Over!");
                
                switch (Result)
                {
                    case GameResult.PlayerWin:
                        Console.WriteLine("You win!");
                        break;
                    case GameResult.AIWin:
                        Console.WriteLine("AI wins!");
                        break;
                    case GameResult.Draw:
                        Console.WriteLine("Draw!");
                        break;
                }
            }
        }
    }
    
    public enum GameResult
    {
        PlayerWin,
        AIWin,
        Draw
    }
}
