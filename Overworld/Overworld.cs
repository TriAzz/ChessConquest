using System;
using System.Collections.Generic;
using System.Linq;

namespace ChessConquest.Overworld
{
    public class Overworld
    {
        public List<Faction> Factions { get; }
        public List<Territory> Territories { get; }
        public Faction? PlayerFaction { get; set; }
        private Random random = new Random();
        
        public Overworld()
        {
            Factions = new List<Faction>();
            Territories = new List<Territory>();
        }
        
        /// <summary>
        /// Add a faction to the overworld
        /// </summary>
        public void AddFaction(Faction faction)
        {
            if (!Factions.Contains(faction))
            {
                Factions.Add(faction);
            }
        }
        
        /// <summary>
        /// Add a territory to the overworld
        /// </summary>
        public void AddTerritory(Territory territory)
        {
            if (!Territories.Contains(territory))
            {
                Territories.Add(territory);
            }
        }
        
        /// <summary>
        /// Initialize a simple overworld with factions and territories
        /// </summary>
        public void InitializeSimpleOverworld()
        {
            // Create factions
            Faction northernKingdom = new Faction("Northern Kingdom", ConsoleColor.Blue);
            Faction easternEmpire = new Faction("Eastern Empire", ConsoleColor.Red);
            Faction westernAlliance = new Faction("Western Alliance", ConsoleColor.Green);
            Faction southernDominion = new Faction("Southern Dominion", ConsoleColor.Yellow);
            
            // Add factions to the world
            Factions.Add(northernKingdom);
            Factions.Add(easternEmpire);
            Factions.Add(westernAlliance);
            Factions.Add(southernDominion);
            
            // Create territories
            Territory northCapital = new Territory("Frosthold Capital", new MapPosition(5, 1));
            Territory northEast = new Territory("Icevale", new MapPosition(7, 2));
            Territory northWest = new Territory("Snowpeak", new MapPosition(3, 2));
            
            Territory eastCapital = new Territory("Sunridge Capital", new MapPosition(9, 5));
            Territory eastNorth = new Territory("Dawngate", new MapPosition(8, 3));
            Territory eastSouth = new Territory("Emberfall", new MapPosition(8, 7));
            
            Territory westCapital = new Territory("Verdantia Capital", new MapPosition(1, 5));
            Territory westNorth = new Territory("Greenwood", new MapPosition(2, 3));
            Territory westSouth = new Territory("Misthaven", new MapPosition(2, 7));
            
            Territory southCapital = new Territory("Sandspire Capital", new MapPosition(5, 9));
            Territory southEast = new Territory("Dunewatch", new MapPosition(7, 8));
            Territory southWest = new Territory("Saltmarsh", new MapPosition(3, 8));
            
            // Add territories to the world
            Territories.AddRange(new[] { 
                northCapital, northEast, northWest,
                eastCapital, eastNorth, eastSouth,
                westCapital, westNorth, westSouth,
                southCapital, southEast, southWest
            });
            
            // Set up adjacencies
            northCapital.AddAdjacentTerritory(northEast);
            northCapital.AddAdjacentTerritory(northWest);
            northEast.AddAdjacentTerritory(eastNorth);
            northWest.AddAdjacentTerritory(westNorth);
            
            eastCapital.AddAdjacentTerritory(eastNorth);
            eastCapital.AddAdjacentTerritory(eastSouth);
            eastNorth.AddAdjacentTerritory(northEast);
            eastSouth.AddAdjacentTerritory(southEast);
            
            westCapital.AddAdjacentTerritory(westNorth);
            westCapital.AddAdjacentTerritory(westSouth);
            westNorth.AddAdjacentTerritory(northWest);
            westSouth.AddAdjacentTerritory(southWest);
            
            southCapital.AddAdjacentTerritory(southEast);
            southCapital.AddAdjacentTerritory(southWest);
            southEast.AddAdjacentTerritory(eastSouth);
            southWest.AddAdjacentTerritory(westSouth);
            
            // Assign territories to factions
            northernKingdom.AddTerritory(northCapital);
            northernKingdom.AddTerritory(northEast);
            northernKingdom.AddTerritory(northWest);
            
            easternEmpire.AddTerritory(eastCapital);
            easternEmpire.AddTerritory(eastNorth);
            easternEmpire.AddTerritory(eastSouth);
            
            westernAlliance.AddTerritory(westCapital);
            westernAlliance.AddTerritory(westNorth);
            westernAlliance.AddTerritory(westSouth);
            
            southernDominion.AddTerritory(southCapital);
            southernDominion.AddTerritory(southEast);
            southernDominion.AddTerritory(southWest);
        }
        
        /// <summary>
        /// Display the overworld map
        /// </summary>
        public void DisplayMap()
        {
            Console.Clear();
            Console.WriteLine("Chess Conquest - Overworld Map");
            Console.WriteLine("=============================\n");
            
            // Find map dimensions
            int maxX = Territories.Max(t => t.Position.X) + 2;
            int maxY = Territories.Max(t => t.Position.Y) + 2;
            
            // Create a 2D array to represent the map
            char[,] map = new char[maxY, maxX];
            ConsoleColor[,] colors = new ConsoleColor[maxY, maxX];
            
            // Initialize with empty spaces
            for (int y = 0; y < maxY; y++)
            {
                for (int x = 0; x < maxX; x++)
                {
                    map[y, x] = ' ';
                    colors[y, x] = ConsoleColor.Gray;
                }
            }
            
            // Place territories on the map
            foreach (var territory in Territories)
            {
                int x = territory.Position.X;
                int y = territory.Position.Y;
                
                // Use first letter of territory name as the marker
                map[y, x] = territory.Name[0];
                colors[y, x] = territory.GetDisplayColor();
                
                // Draw connections to adjacent territories
                foreach (var adjacent in territory.AdjacentTerritories)
                {
                    // Only draw if this territory has a lower index to avoid duplicates
                    if (Territories.IndexOf(territory) < Territories.IndexOf(adjacent))
                    {
                        int adjX = adjacent.Position.X;
                        int adjY = adjacent.Position.Y;
                        
                        // Calculate midpoint
                        int midX = (x + adjX) / 2;
                        int midY = (y + adjY) / 2;
                        
                        // Place a connection marker
                        map[midY, midX] = '*';
                        colors[midY, midX] = ConsoleColor.Gray;
                    }
                }
            }
            
            // Display the map
            for (int y = 0; y < maxY; y++)
            {
                for (int x = 0; x < maxX; x++)
                {
                    Console.ForegroundColor = colors[y, x];
                    Console.Write(map[y, x]);
                    Console.Write(' '); // Add space for better visibility
                }
                Console.WriteLine();
            }
            
            // Reset color
            Console.ForegroundColor = ConsoleColor.Gray;
            
            // Display faction information
            Console.WriteLine("\nFactions:");
            foreach (var faction in Factions)
            {
                Console.ForegroundColor = faction.Color;
                Console.Write($"{faction.Name} ");
                Console.ForegroundColor = ConsoleColor.Gray;
                Console.WriteLine($"({faction.Territories.Count} territories)");
            }
            
            Console.WriteLine();
        }
        
        /// <summary>
        /// Process a battle between two factions
        /// </summary>
        public Faction? ResolveBattle(Faction attacker, Faction defender, bool isPlayerInvolved)
        {
            if (isPlayerInvolved)
            {
                // For player battles, we'll return null to indicate that a chess game should be started
                return null;
            }
            else
            {
                // For AI battles, randomly determine the winner
                return random.Next(2) == 0 ? attacker : defender;
            }
        }
        
        /// <summary>
        /// Attack a territory
        /// </summary>
        public bool AttackTerritory(Territory attackingTerritory, Territory targetTerritory, out Faction? winner)
        {
            // Check if territories are adjacent
            if (!attackingTerritory.IsAdjacentTo(targetTerritory))
            {
                winner = null;
                return false;
            }
            
            // Check if attacking territory has an owner
            if (attackingTerritory.Owner == null)
            {
                winner = null; 
                return false;
            }
            
            // Check if target territory has an owner
            if (targetTerritory.Owner == null)
            {
                // If no owner, just claim it
                targetTerritory.Owner = attackingTerritory.Owner;
                winner = attackingTerritory.Owner;
                return true;
            }
            
            // Check if same faction (can't attack your own territory)
            if (attackingTerritory.Owner == targetTerritory.Owner)
            {
                winner = null;
                return false;
            }
            
            // Determine if player is involved
            bool isPlayerInvolved = (attackingTerritory.Owner == PlayerFaction || targetTerritory.Owner == PlayerFaction);
            
            // Resolve the battle
            winner = ResolveBattle(attackingTerritory.Owner, targetTerritory.Owner, isPlayerInvolved);
            
            // If there's a winner (not player battle), update territory ownership
            if (winner != null)
            {
                if (winner == attackingTerritory.Owner)
                {
                    // Attacker won
                    targetTerritory.Owner.RemoveTerritory(targetTerritory);
                    attackingTerritory.Owner.AddTerritory(targetTerritory);
                }
                
                return true;
            }
            
            // Return false to indicate a player battle should be started
            return false;
        }
        
        /// <summary>
        /// Process AI faction turns
        /// </summary>
        public void ProcessAITurns()
        {
            foreach (var faction in Factions.Where(f => f != PlayerFaction && PlayerFaction != null))
            {
                // Each AI faction attempts one attack per turn
                if (faction.Territories.Count > 0)
                {
                    // Pick a random territory owned by this faction
                    Territory attackingTerritory = faction.Territories[random.Next(faction.Territories.Count)];
                    
                    // Get adjacent territories not owned by this faction
                    var possibleTargets = attackingTerritory.AdjacentTerritories
                        .Where(t => t.Owner != faction)
                        .ToList();
                    
                    if (possibleTargets.Count > 0)
                    {
                        // Pick a random target
                        Territory targetTerritory = possibleTargets[random.Next(possibleTargets.Count)];
                        
                        // Attack the territory
                        AttackTerritory(attackingTerritory, targetTerritory, out _);
                    }
                }
            }
        }
    }
}
