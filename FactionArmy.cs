using System;
using System.Collections.Generic;
using ChessConquest;
using ChessConquest.Pieces;

namespace ChessConquestGUI
{
    /// <summary>
    /// Manages the chess piece lineup for each faction
    /// </summary>
    public class FactionArmy
    {
        private readonly Dictionary<string, Func<PieceColor, List<Piece>>> armySetups;
        
        public FactionArmy()
        {
            // Initialize the dictionary of army setups with unique functions for each faction
            armySetups = new Dictionary<string, Func<PieceColor, List<Piece>>>
            {
                { "Iron Legion", GetIronLegionArmy },
                { "Crimson Order", GetCrimsonOrderArmy },
                { "Emerald Covenant", GetEmeraldCovenantArmy },
                { "Golden Dynasty", GetGoldenDynastyArmy },
                { "Shadow Collective", GetShadowCollectiveArmy }
            };
        }
        
        /// <summary>
        /// Get the piece lineup for a specific faction
        /// </summary>
        /// <param name="factionName">Name of the faction</param>
        /// <param name="color">Color of the pieces (White or Black)</param>
        /// <returns>List of pieces for the faction's army</returns>
        public List<Piece> GetFactionArmy(string factionName, PieceColor color)
        {
            if (armySetups.TryGetValue(factionName, out var setupFunc))
            {
                return setupFunc(color);
            }
            
            // Default to standard army if faction not found
            return GetStandardArmy(color);
        }
        
        /// <summary>
        /// Standard chess piece lineup used by default
        /// </summary>
        private List<Piece> GetStandardArmy(PieceColor color)
        {
            List<Piece> pieces = new List<Piece>();
            int pawnRow = color == PieceColor.White ? 6 : 1;
            int backRow = color == PieceColor.White ? 7 : 0;
            
            // Add pawns and vanguards
            for (int col = 0; col < 8; col++)
            {
                // Replace the middle pawns (columns 3 and 4) with Vanguards
                if (col == 3 || col == 4)
                {
                    pieces.Add(new Vanguard(color, new Position(pawnRow, col)));
                }
                else
                {
                    pieces.Add(new Pawn(color, new Position(pawnRow, col)));
                }
            }
            
            // Add rooks
            pieces.Add(new Rook(color, new Position(backRow, 0)));
            pieces.Add(new Rook(color, new Position(backRow, 7)));
            
            // Add knights
            pieces.Add(new Knight(color, new Position(backRow, 1)));
            pieces.Add(new Knight(color, new Position(backRow, 6)));
            
            // Add bishops
            pieces.Add(new Bishop(color, new Position(backRow, 2)));
            pieces.Add(new Bishop(color, new Position(backRow, 5)));
            
            // Add queen
            pieces.Add(new Queen(color, new Position(backRow, 3)));
            
            // Add king
            pieces.Add(new King(color, new Position(backRow, 4)));
            
            return pieces;
        }
        
        /// <summary>
        /// Iron Legion army setup - With Vanguards replacing center pawns
        /// </summary>
        private List<Piece> GetIronLegionArmy(PieceColor color)
        {
            List<Piece> pieces = new List<Piece>();
            int pawnRow = color == PieceColor.White ? 6 : 1;
            int backRow = color == PieceColor.White ? 7 : 0;
            
            // Add pawns and vanguards
            for (int col = 0; col < 8; col++)
            {
                // Replace the middle pawns (columns 3 and 4) with Vanguards
                if (col == 3 || col == 4)
                {
                    pieces.Add(new Vanguard(color, new Position(pawnRow, col)));
                }
                else
                {
                    pieces.Add(new Pawn(color, new Position(pawnRow, col)));
                }
            }
            
            // Add rooks
            pieces.Add(new Rook(color, new Position(backRow, 0)));
            pieces.Add(new Rook(color, new Position(backRow, 7)));
            
            // Add knights
            pieces.Add(new Knight(color, new Position(backRow, 1)));
            pieces.Add(new Knight(color, new Position(backRow, 6)));
            
            // Add bishops
            pieces.Add(new Bishop(color, new Position(backRow, 2)));
            pieces.Add(new Bishop(color, new Position(backRow, 5)));
            
            // Add queen
            pieces.Add(new Queen(color, new Position(backRow, 3)));
            
            // Add king
            pieces.Add(new King(color, new Position(backRow, 4)));
            
            return pieces;
        }
        
        /// <summary>
        /// Crimson Order army setup - With Cavalry instead of Knights and pawns in front of bishops
        /// </summary>
        private List<Piece> GetCrimsonOrderArmy(PieceColor color)
        {
            List<Piece> pieces = new List<Piece>();
            int pawnRow = color == PieceColor.White ? 6 : 1;
            int backRow = color == PieceColor.White ? 7 : 0;
            
            // Add pawns and cavalry
            for (int col = 0; col < 8; col++)
            {
                // Replace pawns in front of bishops (columns 2 and 5) with Cavalry
                if (col == 2 || col == 5)
                {
                    pieces.Add(new Cavalry(color, new Position(pawnRow, col)));
                }
                else
                {
                    pieces.Add(new Pawn(color, new Position(pawnRow, col)));
                }
            }
            
            // Add rooks
            pieces.Add(new Rook(color, new Position(backRow, 0)));
            pieces.Add(new Rook(color, new Position(backRow, 7)));
            
            // Add Cavalry instead of knights
            pieces.Add(new Cavalry(color, new Position(backRow, 1)));
            pieces.Add(new Cavalry(color, new Position(backRow, 6)));
            
            // Add bishops
            pieces.Add(new Bishop(color, new Position(backRow, 2)));
            pieces.Add(new Bishop(color, new Position(backRow, 5)));
            
            // Add queen
            pieces.Add(new Queen(color, new Position(backRow, 3)));
            
            // Add king
            pieces.Add(new King(color, new Position(backRow, 4)));
            
            return pieces;
        }
        
        /// <summary>
        /// Emerald Covenant army setup - Standard chess piece arrangement
        /// </summary>
        private List<Piece> GetEmeraldCovenantArmy(PieceColor color)
        {
            List<Piece> pieces = new List<Piece>();
            int pawnRow = color == PieceColor.White ? 6 : 1;
            int backRow = color == PieceColor.White ? 7 : 0;
            
            // Add all standard pawns
            for (int col = 0; col < 8; col++)
            {
                pieces.Add(new Pawn(color, new Position(pawnRow, col)));
            }
            
            // Add rooks
            pieces.Add(new Rook(color, new Position(backRow, 0)));
            pieces.Add(new Rook(color, new Position(backRow, 7)));
            
            // Add knights
            pieces.Add(new Knight(color, new Position(backRow, 1)));
            pieces.Add(new Knight(color, new Position(backRow, 6)));
            
            // Add bishops
            pieces.Add(new Bishop(color, new Position(backRow, 2)));
            pieces.Add(new Bishop(color, new Position(backRow, 5)));
            
            // Add queen
            pieces.Add(new Queen(color, new Position(backRow, 3)));
            
            // Add king
            pieces.Add(new King(color, new Position(backRow, 4)));
            
            return pieces;
        }
        
        /// <summary>
        /// Golden Dynasty army setup - Standard chess piece arrangement
        /// </summary>
        private List<Piece> GetGoldenDynastyArmy(PieceColor color)
        {
            List<Piece> pieces = new List<Piece>();
            int pawnRow = color == PieceColor.White ? 6 : 1;
            int backRow = color == PieceColor.White ? 7 : 0;
            
            // Add all standard pawns
            for (int col = 0; col < 8; col++)
            {
                pieces.Add(new Pawn(color, new Position(pawnRow, col)));
            }
            
            // Add rooks
            pieces.Add(new Rook(color, new Position(backRow, 0)));
            pieces.Add(new Rook(color, new Position(backRow, 7)));
            
            // Add knights
            pieces.Add(new Knight(color, new Position(backRow, 1)));
            pieces.Add(new Knight(color, new Position(backRow, 6)));
            
            // Add bishops
            pieces.Add(new Bishop(color, new Position(backRow, 2)));
            pieces.Add(new Bishop(color, new Position(backRow, 5)));
            
            // Add queen
            pieces.Add(new Queen(color, new Position(backRow, 3)));
            
            // Add king
            pieces.Add(new King(color, new Position(backRow, 4)));
            
            return pieces;
        }
        
        /// <summary>
        /// Shadow Collective army setup - Standard chess piece arrangement
        /// </summary>
        private List<Piece> GetShadowCollectiveArmy(PieceColor color)
        {
            List<Piece> pieces = new List<Piece>();
            int pawnRow = color == PieceColor.White ? 6 : 1;
            int backRow = color == PieceColor.White ? 7 : 0;
            
            // Add all standard pawns
            for (int col = 0; col < 8; col++)
            {
                pieces.Add(new Pawn(color, new Position(pawnRow, col)));
            }
            
            // Add rooks
            pieces.Add(new Rook(color, new Position(backRow, 0)));
            pieces.Add(new Rook(color, new Position(backRow, 7)));
            
            // Add knights
            pieces.Add(new Knight(color, new Position(backRow, 1)));
            pieces.Add(new Knight(color, new Position(backRow, 6)));
            
            // Add bishops
            pieces.Add(new Bishop(color, new Position(backRow, 2)));
            pieces.Add(new Bishop(color, new Position(backRow, 5)));
            
            // Add queen
            pieces.Add(new Queen(color, new Position(backRow, 3)));
            
            // Add king
            pieces.Add(new King(color, new Position(backRow, 4)));
            
            return pieces;
        }
    }
}
