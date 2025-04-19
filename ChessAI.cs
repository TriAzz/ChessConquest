using System;
using System.Collections.Generic;
using System.Linq;
using ChessConquest.Pieces;

namespace ChessConquest
{
    public class ChessAI
    {
        private readonly PieceColor aiColor;
        private readonly Random random = new Random();
        
        // Piece values for evaluation
        private static readonly Dictionary<Type, int> PieceValues = new Dictionary<Type, int>
        {
            { typeof(Pawn), 100 },
            { typeof(Knight), 320 },
            { typeof(Bishop), 330 },
            { typeof(Rook), 500 },
            { typeof(Queen), 900 },
            { typeof(King), 20000 },
            { typeof(Vanguard), 350 }, // Vanguard is slightly more valuable than a Knight
            { typeof(Cavalry), 380 }  // Cavalry is more valuable than a Knight and slightly more than a Vanguard
        };

        // Position value tables for improved evaluation
        private static readonly int[,] PawnPositionValues = {
            { 0,  0,  0,  0,  0,  0,  0,  0 },
            { 50, 50, 50, 50, 50, 50, 50, 50 },
            { 10, 10, 20, 30, 30, 20, 10, 10 },
            { 5,  5, 10, 25, 25, 10,  5,  5 },
            { 0,  0,  0, 20, 20,  0,  0,  0 },
            { 5, -5,-10,  0,  0,-10, -5,  5 },
            { 5, 10, 10,-20,-20, 10, 10,  5 },
            { 0,  0,  0,  0,  0,  0,  0,  0 }
        };

        private static readonly int[,] KnightPositionValues = {
            {-50,-40,-30,-30,-30,-30,-40,-50 },
            {-40,-20,  0,  0,  0,  0,-20,-40 },
            {-30,  0, 10, 15, 15, 10,  0,-30 },
            {-30,  5, 15, 20, 20, 15,  5,-30 },
            {-30,  0, 15, 20, 20, 15,  0,-30 },
            {-30,  5, 10, 15, 15, 10,  5,-30 },
            {-40,-20,  0,  5,  5,  0,-20,-40 },
            {-50,-40,-30,-30,-30,-30,-40,-50 }
        };

        private static readonly int[,] BishopPositionValues = {
            {-20,-10,-10,-10,-10,-10,-10,-20 },
            {-10,  0,  0,  0,  0,  0,  0,-10 },
            {-10,  0, 10, 10, 10, 10,  0,-10 },
            {-10,  5,  5, 10, 10,  5,  5,-10 },
            {-10,  0,  5, 10, 10,  5,  0,-10 },
            {-10,  5,  5,  5,  5,  5,  5,-10 },
            {-10,  0,  5,  0,  0,  5,  0,-10 },
            {-20,-10,-10,-10,-10,-10,-10,-20 }
        };

        private static readonly int[,] RookPositionValues = {
            { 0,  0,  0,  0,  0,  0,  0,  0 },
            { 5, 10, 10, 10, 10, 10, 10,  5 },
            {-5,  0,  0,  0,  0,  0,  0, -5 },
            {-5,  0,  0,  0,  0,  0,  0, -5 },
            {-5,  0,  0,  0,  0,  0,  0, -5 },
            {-5,  0,  0,  0,  0,  0,  0, -5 },
            {-5,  0,  0,  0,  0,  0,  0, -5 },
            { 0,  0,  0,  5,  5,  0,  0,  0 }
        };

        private static readonly int[,] QueenPositionValues = {
            {-20,-10,-10, -5, -5,-10,-10,-20 },
            {-10,  0,  0,  0,  0,  0,  0,-10 },
            {-10,  0,  5,  5,  5,  5,  0,-10 },
            { -5,  0,  5,  5,  5,  5,  0, -5 },
            {  0,  0,  5,  5,  5,  5,  0, -5 },
            {-10,  5,  5,  5,  5,  5,  0,-10 },
            {-10,  0,  5,  0,  0,  0,  0,-10 },
            {-20,-10,-10, -5, -5,-10,-10,-20 }
        };

        private static readonly int[,] KingMiddleGamePositionValues = {
            {-30,-40,-40,-50,-50,-40,-40,-30 },
            {-30,-40,-40,-50,-50,-40,-40,-30 },
            {-30,-40,-40,-50,-50,-40,-40,-30 },
            {-30,-40,-40,-50,-50,-40,-40,-30 },
            {-20,-30,-30,-40,-40,-30,-30,-20 },
            {-10,-20,-20,-20,-20,-20,-20,-10 },
            { 20, 20,  0,  0,  0,  0, 20, 20 },
            { 20, 30, 10,  0,  0, 10, 30, 20 }
        };

        private static readonly int[,] VanguardPositionValues = {
            {-50,-40,-30,-30,-30,-30,-40,-50 },
            {-40,-20,  0,  0,  0,  0,-20,-40 },
            {-30,  0, 10, 15, 15, 10,  0,-30 },
            {-30,  5, 15, 20, 20, 15,  5,-30 },
            {-30,  0, 15, 20, 20, 15,  0,-30 },
            {-30,  5, 10, 15, 15, 10,  5,-30 },
            {-40,-20,  0,  5,  5,  0,-20,-40 },
            {-50,-40,-30,-30,-30,-30,-40,-50 }
        };

        // Added positional values for Cavalry
        private static readonly int[,] CavalryPositionValues = {
            {-50,-40,-30,-30,-30,-30,-40,-50 },
            {-40,-20,  0,  5,  5,  0,-20,-40 },
            {-30,  0, 15, 20, 20, 15,  0,-30 },
            {-30,  5, 20, 25, 25, 20,  5,-30 },
            {-30,  5, 20, 25, 25, 20,  5,-30 },
            {-30,  0, 15, 20, 20, 15,  0,-30 },
            {-40,-20,  5, 10, 10,  5,-20,-40 },
            {-50,-40,-30,-30,-30,-30,-40,-50 }
        };

        public ChessAI(PieceColor color)
        {
            aiColor = color;
        }
        
        /// <summary>
        /// Make a move based on the current board state
        /// </summary>
        public (Position From, Position To) MakeMove(Board board, int difficulty = 3)
        {
            // Get all possible moves for the AI
            List<(Piece Piece, Position To)> allPossibleMoves = GetAllPossibleMoves(board, aiColor);
            
            if (allPossibleMoves.Count == 0)
            {
                throw new InvalidOperationException("No valid moves available");
            }
            
            // Based on difficulty, choose the move
            if (difficulty <= 1)
            {
                // Easy: Random move
                var randomMove = allPossibleMoves[random.Next(allPossibleMoves.Count)];
                return (randomMove.Piece.Position, randomMove.To);
            }
            else
            {
                // Medium/Hard: Use minimax with alpha-beta pruning
                int depth = difficulty;
                (Piece? Piece, Position To, int Score) bestMove = Minimax(board, depth, int.MinValue, int.MaxValue, true);
                return (bestMove.Piece?.Position ?? new Position(0, 0), bestMove.To);
            }
        }
        
        /// <summary>
        /// Get all possible moves for a specific color
        /// </summary>
        private List<(Piece Piece, Position To)> GetAllPossibleMoves(Board board, PieceColor color)
        {
            List<(Piece Piece, Position To)> allMoves = new List<(Piece Piece, Position To)>();
            List<Piece> pieces = board.GetPiecesByColor(color);
            
            foreach (var piece in pieces)
            {
                List<Position> validMoves = piece.GetValidMoves(board);
                
                foreach (var move in validMoves)
                {
                    // Only add moves that don't result in check
                    if (!board.WouldMoveResultInCheck(piece, move))
                    {
                        allMoves.Add((piece, move));
                    }
                }
            }
            
            return allMoves;
        }
        
        /// <summary>
        /// Minimax algorithm with alpha-beta pruning for move evaluation
        /// </summary>
        private (Piece? Piece, Position To, int Score) Minimax(Board board, int depth, int alpha, int beta, bool isMaximizingPlayer)
        {
            PieceColor currentColor = isMaximizingPlayer ? aiColor : aiColor.GetOppositeColor();
            
            // Base case: reached maximum depth or game over
            if (depth == 0 || board.IsCheckmate(PieceColor.White) || board.IsCheckmate(PieceColor.Black) || 
                board.IsStalemate(PieceColor.White) || board.IsStalemate(PieceColor.Black))
            {
                return (null, new Position(0, 0), EvaluateBoard(board));
            }
            
            List<(Piece Piece, Position To)> allPossibleMoves = GetAllPossibleMoves(board, currentColor);
            
            if (allPossibleMoves.Count == 0)
            {
                // No valid moves, might be checkmate or stalemate
                return (null, new Position(0, 0), EvaluateBoard(board));
            }
            
            Piece? bestPiece = null;
            Position bestMove = new Position(0, 0);
            int bestScore = isMaximizingPlayer ? int.MinValue : int.MaxValue;
            
            foreach (var (piece, move) in allPossibleMoves)
            {
                // Create a copy of the board to simulate the move
                Board simulatedBoard = board.Clone();
                Piece? simulatedPiece = simulatedBoard.GetPieceAt(piece.Position);
                
                if (simulatedPiece != null)
                {
                    simulatedBoard.MovePiece(simulatedPiece.Position, move);
                    
                    // Recursively evaluate the move
                    int score = Minimax(simulatedBoard, depth - 1, alpha, beta, !isMaximizingPlayer).Score;
                    
                    if (isMaximizingPlayer)
                    {
                        if (score > bestScore)
                        {
                            bestScore = score;
                            bestPiece = piece;
                            bestMove = move;
                        }
                        alpha = Math.Max(alpha, bestScore);
                    }
                    else
                    {
                        if (score < bestScore)
                        {
                            bestScore = score;
                            bestPiece = piece;
                            bestMove = move;
                        }
                        beta = Math.Min(beta, bestScore);
                    }
                    
                    // Alpha-beta pruning
                    if (beta <= alpha)
                    {
                        break;
                    }
                }
            }
            
            return (bestPiece, bestMove, bestScore);
        }
        
        /// <summary>
        /// Evaluate the board position and return a score
        /// </summary>
        private int EvaluateBoard(Board board)
        {
            int score = 0;
            
            // Check for checkmate
            if (board.IsCheckmate(aiColor.GetOppositeColor()))
            {
                return 1000000; // AI wins
            }
            else if (board.IsCheckmate(aiColor))
            {
                return -1000000; // AI loses
            }
            
            // Check for stalemate
            if (board.IsStalemate(aiColor.GetOppositeColor()) || board.IsStalemate(aiColor))
            {
                return 0; // Draw
            }
            
            // Count material and position value
            for (int row = 0; row < 8; row++)
            {
                for (int col = 0; col < 8; col++)
                {
                    Position pos = new Position(row, col);
                    Piece? piece = board.GetPieceAt(pos);
                    
                    if (piece != null)
                    {
                        // Material value
                        int pieceValue = PieceValues[piece.GetType()];
                        
                        // Position value
                        int positionValue = GetPositionValue(piece, row, col);
                        
                        // Combine values
                        int totalValue = pieceValue + positionValue;
                        
                        // Add to score (positive for AI, negative for opponent)
                        if (piece.Color == aiColor)
                        {
                            score += totalValue;
                        }
                        else
                        {
                            score -= totalValue;
                        }
                    }
                }
            }
            
            return score;
        }
        
        /// <summary>
        /// Get the position value for a piece
        /// </summary>
        private int GetPositionValue(Piece piece, int row, int col)
        {
            // Flip the row index for black pieces to use the same tables
            if (piece.Color == PieceColor.Black)
            {
                row = 7 - row;
            }
            
            if (piece is Pawn)
            {
                return PawnPositionValues[row, col];
            }
            else if (piece is Knight)
            {
                return KnightPositionValues[row, col];
            }
            else if (piece is Bishop)
            {
                return BishopPositionValues[row, col];
            }
            else if (piece is Rook)
            {
                return RookPositionValues[row, col];
            }
            else if (piece is Queen)
            {
                return QueenPositionValues[row, col];
            }
            else if (piece is King)
            {
                return KingMiddleGamePositionValues[row, col];
            }
            else if (piece is Vanguard)
            {
                return VanguardPositionValues[row, col];
            }
            else if (piece is Cavalry)
            {
                return CavalryPositionValues[row, col];
            }
            
            return 0;
        }
    }
}
