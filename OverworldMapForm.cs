using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using ChessConquest;
using ChessConquest.Overworld;
using ChessConquest.Pieces;

namespace ChessConquestGUI
{
    public partial class OverworldMapForm : Form
    {
        private Overworld? overworld;
        private Dictionary<Territory, Button> territoryButtons = new Dictionary<Territory, Button>();
        private Territory? selectedTerritory;
        private Faction? playerFaction;
        private Random random;
        private readonly int territoryButtonSize = 80;
        private readonly int gridSize = 5;
        private readonly int mapPadding = 50;

        public OverworldMapForm()
        {
            InitializeComponent();

            // Set up the form
            this.Text = "Chess Conquest - Overworld Map";
            this.ClientSize = new Size(1000, 700);
            this.FormBorderStyle = FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.StartPosition = FormStartPosition.CenterScreen;

            // Initialize random
            random = new Random();

            // Initialize the overworld
            InitializeOverworld();

            // Create the overworld map
            CreateOverworldMap();

            // Create control panel
            CreateControlPanel();

            // Update the UI
            UpdateUI();
        }

        private void InitializeOverworld()
        {
            try
            {
                // Initialize random
                random = new Random();

                // Create overworld
                overworld = new Overworld();

                // Create factions
                Faction northernKingdom = new Faction("Northern Kingdom", ConsoleColor.Blue);
                Faction easternEmpire = new Faction("Eastern Empire", ConsoleColor.Red);
                Faction westernAlliance = new Faction("Western Alliance", ConsoleColor.Green);
                Faction southernDominion = new Faction("Southern Dominion", ConsoleColor.Yellow);
                Faction neutralFaction = new Faction("Neutral Territories", ConsoleColor.Gray);

                // Add factions to overworld
                overworld.AddFaction(northernKingdom);
                overworld.AddFaction(easternEmpire);
                overworld.AddFaction(westernAlliance);
                overworld.AddFaction(southernDominion);
                overworld.AddFaction(neutralFaction);

                // Set player faction
                playerFaction = northernKingdom;
                overworld.PlayerFaction = playerFaction;

                // Create territories
                List<Territory> territories = new List<Territory>();
                for (int i = 0; i < 15; i++)
                {
                    Territory territory = new Territory($"Territory {i + 1}", new MapPosition(0, 0));
                    territories.Add(territory);
                }

                // Assign territories to factions (3 each)
                for (int i = 0; i < 3; i++)
                {
                    northernKingdom.AddTerritory(territories[i]);
                    easternEmpire.AddTerritory(territories[i + 3]);
                    westernAlliance.AddTerritory(territories[i + 6]);
                    southernDominion.AddTerritory(territories[i + 9]);
                    
                    if (i < territories.Count - 12)
                    {
                        neutralFaction.AddTerritory(territories[i + 12]);
                    }
                }

                // Generate a connected map using a modified approach
                GenerateConnectedMap(territories);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error initializing overworld: {ex.Message}", "Initialization Error", 
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void GenerateConnectedMap(List<Territory> territories)
        {
            // Create a grid of positions
            List<MapPosition> allPositions = new List<MapPosition>();
            for (int x = 0; x < gridSize; x++)
            {
                for (int y = 0; y < gridSize; y++)
                {
                    allPositions.Add(new MapPosition(x, y));
                }
            }

            // Shuffle the positions
            allPositions = allPositions.OrderBy(p => random.Next()).ToList();

            // Group territories by faction
            var factionTerritories = territories
                .Where(t => t.Owner != null)
                .GroupBy(t => t.Owner)
                .ToDictionary(g => g.Key!, g => g.ToList());

            // Select positions for our territories
            Dictionary<Faction, List<MapPosition>> factionPositions = new Dictionary<Faction, List<MapPosition>>();
            List<MapPosition> selectedPositions = new List<MapPosition>();
            List<Territory> processedTerritories = new List<Territory>();

            // Process each faction one by one to ensure their territories are adjacent
            foreach (var faction in factionTerritories.Keys)
            {
                if (faction == null) continue;
                
                List<Territory> factionsTerritoriesList = factionTerritories[faction];
                List<MapPosition> factionsPositions = new List<MapPosition>();
                factionPositions[faction] = factionsPositions;

                // If this is the first faction, start at a random position
                if (selectedPositions.Count == 0)
                {
                    MapPosition firstPosition = allPositions[0];
                    selectedPositions.Add(firstPosition);
                    factionsPositions.Add(firstPosition);
                    allPositions.RemoveAt(0);
                }
                else
                {
                    // For subsequent factions, find a position adjacent to existing territories
                    List<MapPosition> adjacentToExisting = new List<MapPosition>();
                    
                    foreach (MapPosition existingPos in selectedPositions)
                    {
                        foreach (MapPosition candidatePos in allPositions)
                        {
                            if (IsAdjacent(existingPos, candidatePos) && !adjacentToExisting.Contains(candidatePos))
                            {
                                adjacentToExisting.Add(candidatePos);
                            }
                        }
                    }
                    
                    if (adjacentToExisting.Count > 0)
                    {
                        // Choose a random adjacent position for the first territory of this faction
                        MapPosition firstFactionPos = adjacentToExisting[random.Next(adjacentToExisting.Count)];
                        selectedPositions.Add(firstFactionPos);
                        factionsPositions.Add(firstFactionPos);
                        allPositions.Remove(firstFactionPos);
                    }
                    else if (allPositions.Count > 0)
                    {
                        // Fallback if no adjacent positions
                        MapPosition firstFactionPos = allPositions[0];
                        selectedPositions.Add(firstFactionPos);
                        factionsPositions.Add(firstFactionPos);
                        allPositions.RemoveAt(0);
                    }
                }

                // Add remaining territories for this faction, ensuring they're adjacent to other territories of the same faction
                for (int i = 1; i < factionsTerritoriesList.Count && allPositions.Count > 0; i++)
                {
                    // Find positions adjacent to this faction's existing territories
                    List<MapPosition> adjacentToFaction = new List<MapPosition>();
                    
                    foreach (MapPosition factionPos in factionsPositions)
                    {
                        foreach (MapPosition candidatePos in allPositions)
                        {
                            if (IsAdjacent(factionPos, candidatePos) && !adjacentToFaction.Contains(candidatePos))
                            {
                                adjacentToFaction.Add(candidatePos);
                            }
                        }
                    }
                    
                    if (adjacentToFaction.Count > 0)
                    {
                        // Choose a random adjacent position
                        MapPosition nextPosition = adjacentToFaction[random.Next(adjacentToFaction.Count)];
                        selectedPositions.Add(nextPosition);
                        factionsPositions.Add(nextPosition);
                        allPositions.Remove(nextPosition);
                    }
                    else if (allPositions.Count > 0)
                    {
                        // If no adjacent positions to faction territories, find positions adjacent to any territory
                        List<MapPosition> adjacentToAny = new List<MapPosition>();
                        
                        foreach (MapPosition existingPos in selectedPositions)
                        {
                            foreach (MapPosition candidatePos in allPositions)
                            {
                                if (IsAdjacent(existingPos, candidatePos) && !adjacentToAny.Contains(candidatePos))
                                {
                                    adjacentToAny.Add(candidatePos);
                                }
                            }
                        }
                        
                        if (adjacentToAny.Count > 0)
                        {
                            MapPosition nextPosition = adjacentToAny[random.Next(adjacentToAny.Count)];
                            selectedPositions.Add(nextPosition);
                            factionsPositions.Add(nextPosition);
                            allPositions.Remove(nextPosition);
                        }
                        else if (allPositions.Count > 0)
                        {
                            // Last resort
                            MapPosition nextPosition = allPositions[0];
                            selectedPositions.Add(nextPosition);
                            factionsPositions.Add(nextPosition);
                            allPositions.RemoveAt(0);
                        }
                    }
                }
                
                // Add the faction's territories to processed list
                processedTerritories.AddRange(factionsTerritoriesList);
            }
            
            // Handle any remaining territories (like neutral ones)
            var remainingTerritories = territories.Except(processedTerritories).ToList();
            foreach (var territory in remainingTerritories)
            {
                if (allPositions.Count == 0) break;
                
                // Find positions adjacent to any existing territory
                List<MapPosition> adjacentPositions = new List<MapPosition>();
                
                foreach (MapPosition existingPos in selectedPositions)
                {
                    foreach (MapPosition candidatePos in allPositions)
                    {
                        if (IsAdjacent(existingPos, candidatePos) && !adjacentPositions.Contains(candidatePos))
                        {
                            adjacentPositions.Add(candidatePos);
                        }
                    }
                }
                
                if (adjacentPositions.Count > 0)
                {
                    MapPosition nextPosition = adjacentPositions[random.Next(adjacentPositions.Count)];
                    selectedPositions.Add(nextPosition);
                    allPositions.Remove(nextPosition);
                }
                else if (allPositions.Count > 0)
                {
                    MapPosition nextPosition = allPositions[0];
                    selectedPositions.Add(nextPosition);
                    allPositions.RemoveAt(0);
                }
            }

            // Create new territories with the connected positions
            Dictionary<Territory, MapPosition> territoryPositions = new Dictionary<Territory, MapPosition>();
            
            // First assign positions to faction territories
            foreach (var faction in factionTerritories.Keys)
            {
                if (faction == null || !factionPositions.ContainsKey(faction)) continue;
                
                var factionsTerritoriesList = factionTerritories[faction];
                var factionsPositions = factionPositions[faction];
                
                for (int i = 0; i < factionsTerritoriesList.Count && i < factionsPositions.Count; i++)
                {
                    territoryPositions[factionsTerritoriesList[i]] = factionsPositions[i];
                }
            }
            
            // Then assign positions to any remaining territories
            var unassignedTerritories = territories.Where(t => !territoryPositions.ContainsKey(t)).ToList();
            var unassignedPositions = selectedPositions.Where(p => !territoryPositions.Values.Contains(p)).ToList();
            
            for (int i = 0; i < unassignedTerritories.Count && i < unassignedPositions.Count; i++)
            {
                territoryPositions[unassignedTerritories[i]] = unassignedPositions[i];
            }
            
            // Create new territories with the assigned positions
            List<Territory> newTerritories = new List<Territory>();
            foreach (var territory in territories)
            {
                if (!territoryPositions.ContainsKey(territory)) continue;
                
                MapPosition position = territoryPositions[territory];
                Territory newTerritory = new Territory(territory.Name, position);
                Faction? owner = territory.Owner;
                
                // Update the faction's territories
                if (owner != null)
                {
                    owner.RemoveTerritory(territory);
                    owner.AddTerritory(newTerritory);
                }

                newTerritories.Add(newTerritory);
                overworld?.AddTerritory(newTerritory);
            }

            // Set adjacencies based on grid positions
            foreach (Territory territory in newTerritories)
            {
                if (territory == null) continue;
                
                foreach (Territory otherTerritory in newTerritories)
                {
                    if (otherTerritory == null || territory == otherTerritory) continue;
                    
                    if (IsAdjacent(territory.Position, otherTerritory.Position))
                    {
                        territory.AddAdjacentTerritory(otherTerritory);
                    }
                }
            }
        }

        private bool IsAdjacent(MapPosition a, MapPosition b)
        {
            // Check if two positions are adjacent (horizontally or vertically, but not diagonally)
            int dx = Math.Abs(a.X - b.X);
            int dy = Math.Abs(a.Y - b.Y);
            
            // Only consider horizontal or vertical adjacency (not diagonal)
            return (dx == 1 && dy == 0) || (dx == 0 && dy == 1);
        }

        private void CreateOverworldMap()
        {
            // Create a panel for the map
            Panel mapPanel = new Panel
            {
                Location = new Point(mapPadding, mapPadding),
                Size = new Size(gridSize * territoryButtonSize + mapPadding * 2,
                               gridSize * territoryButtonSize + mapPadding * 2),
                BorderStyle = BorderStyle.FixedSingle
            };
            this.Controls.Add(mapPanel);

            // Create territory buttons
            territoryButtons = new Dictionary<Territory, Button>();
            if (overworld?.Territories != null)
            {
                foreach (Territory territory in overworld.Territories)
                {
                    Button button = new Button
                    {
                        Size = new Size(territoryButtonSize, territoryButtonSize),
                        Location = new Point(
                            territory.Position.X * territoryButtonSize + mapPadding,
                            territory.Position.Y * territoryButtonSize + mapPadding),
                        FlatStyle = FlatStyle.Flat,
                        Text = territory.Name,
                        Font = new Font("Arial", 8),
                        Tag = territory
                    };

                    // Set the button color based on the faction
                    if (territory.Owner != null)
                    {
                        button.BackColor = GetColorFromConsoleColor(territory.Owner.Color);
                        button.ForeColor = IsDarkColor(button.BackColor) ? Color.White : Color.Black;
                    }

                    button.Click += TerritoryButton_Click;
                    mapPanel.Controls.Add(button);
                    territoryButtons[territory] = button;
                }
            }

            // Draw connections between adjacent territories
            mapPanel.Paint += (sender, e) =>
            {
                Graphics g = e.Graphics;
                Pen pen = new Pen(Color.Black, 2);

                if (overworld?.Territories != null)
                {
                    foreach (Territory territory in overworld.Territories)
                    {
                        foreach (Territory adjacent in territory.AdjacentTerritories)
                        {
                            // Get the center points of the territory buttons
                            Point p1 = new Point(
                                territory.Position.X * territoryButtonSize + territoryButtonSize / 2 + mapPadding,
                                territory.Position.Y * territoryButtonSize + territoryButtonSize / 2 + mapPadding);

                            Point p2 = new Point(
                                adjacent.Position.X * territoryButtonSize + territoryButtonSize / 2 + mapPadding,
                                adjacent.Position.Y * territoryButtonSize + territoryButtonSize / 2 + mapPadding);

                            // Draw a line between them
                            g.DrawLine(pen, p1, p2);
                        }
                    }
                }
            };
        }

        private void CreateControlPanel()
        {
            // Create a panel for controls
            Panel controlPanel = new Panel
            {
                Location = new Point(gridSize * territoryButtonSize + mapPadding * 3, mapPadding),
                Size = new Size(300, gridSize * territoryButtonSize + mapPadding * 2),
                BorderStyle = BorderStyle.FixedSingle
            };
            this.Controls.Add(controlPanel);

            // Create title label
            Label titleLabel = new Label
            {
                Text = "Overworld Control",
                Font = new Font("Arial", 16, FontStyle.Bold),
                Location = new Point(10, 10),
                Size = new Size(280, 30),
                TextAlign = ContentAlignment.MiddleCenter
            };
            controlPanel.Controls.Add(titleLabel);

            // Create selected territory info
            GroupBox territoryInfoBox = new GroupBox
            {
                Text = "Selected Territory",
                Font = new Font("Arial", 12),
                Location = new Point(10, 50),
                Size = new Size(280, 150)
            };
            controlPanel.Controls.Add(territoryInfoBox);

            Label territoryNameLabel = new Label
            {
                Name = "territoryNameLabel",
                Text = "No territory selected",
                Font = new Font("Arial", 10, FontStyle.Bold),
                Location = new Point(10, 25),
                Size = new Size(260, 20),
                TextAlign = ContentAlignment.MiddleLeft
            };
            territoryInfoBox.Controls.Add(territoryNameLabel);

            Label territoryOwnerLabel = new Label
            {
                Name = "territoryOwnerLabel",
                Text = "Owner: None",
                Font = new Font("Arial", 10),
                Location = new Point(10, 50),
                Size = new Size(260, 20),
                TextAlign = ContentAlignment.MiddleLeft
            };
            territoryInfoBox.Controls.Add(territoryOwnerLabel);

            Label adjacentTerritoriesLabel = new Label
            {
                Name = "adjacentTerritoriesLabel",
                Text = "Adjacent Territories: 0",
                Font = new Font("Arial", 10),
                Location = new Point(10, 75),
                Size = new Size(260, 20),
                TextAlign = ContentAlignment.MiddleLeft
            };
            territoryInfoBox.Controls.Add(adjacentTerritoriesLabel);

            Button attackButton = new Button
            {
                Name = "attackButton",
                Text = "Attack Territory",
                Font = new Font("Arial", 10),
                Location = new Point(10, 105),
                Size = new Size(260, 30),
                Enabled = false
            };
            attackButton.Click += AttackButton_Click;
            territoryInfoBox.Controls.Add(attackButton);

            // Create faction info
            GroupBox factionInfoBox = new GroupBox
            {
                Text = "Faction Information",
                Font = new Font("Arial", 12),
                Location = new Point(10, 210),
                Size = new Size(280, 200)
            };
            controlPanel.Controls.Add(factionInfoBox);

            // Create a list view for factions
            ListView factionListView = new ListView
            {
                Name = "factionListView",
                View = View.Details,
                FullRowSelect = true,
                Location = new Point(10, 25),
                Size = new Size(260, 165)
            };
            factionListView.Columns.Add("Faction", 130);
            factionListView.Columns.Add("Territories", 80);
            factionListView.Columns.Add("Status", 80);

            factionInfoBox.Controls.Add(factionListView);

            // Create action buttons
            Button endTurnButton = new Button
            {
                Text = "End Turn",
                Font = new Font("Arial", 12),
                Location = new Point(10, 420),
                Size = new Size(280, 40)
            };
            endTurnButton.Click += EndTurnButton_Click;
            controlPanel.Controls.Add(endTurnButton);

            Button returnToMenuButton = new Button
            {
                Text = "Return to Main Menu",
                Font = new Font("Arial", 12),
                Location = new Point(10, 470),
                Size = new Size(280, 40)
            };
            returnToMenuButton.Click += ReturnToMenuButton_Click;
            controlPanel.Controls.Add(returnToMenuButton);
        }

        private void TerritoryButton_Click(object? sender, EventArgs e)
        {
            if (sender == null) return;

            Button clickedButton = (Button)sender;
            if (clickedButton.Tag is not Territory clickedTerritory)
            {
                return;
            }

            // Select the territory
            selectedTerritory = clickedTerritory;

            // Update the UI
            UpdateUI();
        }

        private void AttackButton_Click(object? sender, EventArgs e)
        {
            if (selectedTerritory == null || selectedTerritory.Owner == playerFaction)
            {
                return;
            }

            // Check if the selected territory is adjacent to any player territory
            bool canAttack = selectedTerritory.AdjacentTerritories.Any(t => t.Owner == playerFaction);
            if (!canAttack)
            {
                MessageBox.Show("You can only attack territories adjacent to your own territories.",
                               "Invalid Attack", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            // Start a battle for the territory
            StartBattle(selectedTerritory);
        }

        private void EndTurnButton_Click(object? sender, EventArgs e)
        {
            // Process AI turns
            ProcessAITurns();

            // Update the UI
            UpdateUI();

            // Check for game over
            CheckGameOver();
        }

        private void ReturnToMenuButton_Click(object? sender, EventArgs e)
        {
            this.Close();
        }

        private void UpdateUI()
        {
            // Update territory buttons
            if (overworld?.Territories != null)
            {
                foreach (Territory territory in overworld.Territories)
                {
                    Button button = territoryButtons[territory];

                    // Update button color based on faction
                    if (territory.Owner != null)
                    {
                        button.BackColor = GetColorFromConsoleColor(territory.Owner.Color);
                        button.ForeColor = IsDarkColor(button.BackColor) ? Color.White : Color.Black;
                    }

                    // Highlight selected territory
                    if (territory == selectedTerritory)
                    {
                        button.FlatAppearance.BorderSize = 3;
                        button.FlatAppearance.BorderColor = Color.Yellow;
                    }
                    else
                    {
                        button.FlatAppearance.BorderSize = 1;
                        button.FlatAppearance.BorderColor = Color.Black;
                    }
                }
            }

            // Update territory info
            Label? territoryNameLabel = this.Controls.Find("territoryNameLabel", true).FirstOrDefault() as Label;
            Label? territoryOwnerLabel = this.Controls.Find("territoryOwnerLabel", true).FirstOrDefault() as Label;
            Label? adjacentTerritoriesLabel = this.Controls.Find("adjacentTerritoriesLabel", true).FirstOrDefault() as Label;
            Button? attackButton = this.Controls.Find("attackButton", true).FirstOrDefault() as Button;

            if (territoryNameLabel != null && territoryOwnerLabel != null &&
                adjacentTerritoriesLabel != null && attackButton != null)
            {
                if (selectedTerritory != null)
                {
                    territoryNameLabel.Text = selectedTerritory.Name;
                    territoryOwnerLabel.Text = $"Owner: {(selectedTerritory.Owner != null ? selectedTerritory.Owner.Name : "None")}";
                    adjacentTerritoriesLabel.Text = $"Adjacent Territories: {selectedTerritory.AdjacentTerritories.Count}";

                    // Enable attack button if the territory is not owned by the player and is adjacent to a player territory
                    bool canAttack = selectedTerritory.Owner != playerFaction &&
                                    selectedTerritory.AdjacentTerritories.Any(t => t.Owner == playerFaction);
                    attackButton.Enabled = canAttack;
                }
                else
                {
                    territoryNameLabel.Text = "No territory selected";
                    territoryOwnerLabel.Text = "Owner: None";
                    adjacentTerritoriesLabel.Text = "Adjacent Territories: 0";
                    attackButton.Enabled = false;
                }
            }

            // Update faction list
            ListView? factionListView = this.Controls.Find("factionListView", true).FirstOrDefault() as ListView;
            if (factionListView != null)
            {
                factionListView.Items.Clear();
                if (overworld?.Factions != null)
                {
                    foreach (Faction faction in overworld.Factions)
                    {
                        ListViewItem item = new ListViewItem(faction.Name);
                        item.SubItems.Add(faction.Territories.Count.ToString());
                        item.SubItems.Add(faction == playerFaction ? "Player" : "AI");
                        item.BackColor = GetColorFromConsoleColor(faction.Color);
                        item.ForeColor = IsDarkColor(item.BackColor) ? Color.White : Color.Black;
                        factionListView.Items.Add(item);
                    }
                }
            }

            // Refresh the form to update the connections
            this.Invalidate();
        }

        private void StartBattle(Territory territory)
        {
            // Create a new game for the battle
            Game game = new Game(PieceColor.White);

            // Open the chess game form
            ChessGameForm chessForm = new ChessGameForm(game, $"Battle for {territory.Name}");
            chessForm.GameOver += (sender, e) =>
            {
                // If the player won, take control of the territory
                if (e.Winner == game.PlayerColor)
                {
                    if (territory.Owner != null)
                    {
                        territory.Owner.RemoveTerritory(territory);
                    }

                    if (playerFaction != null)
                    {
                        playerFaction.AddTerritory(territory);
                        territory.Owner = playerFaction;

                        MessageBox.Show($"You have conquered {territory.Name}!",
                                       "Territory Conquered", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                }
                else
                {
                    MessageBox.Show($"You failed to conquer {territory.Name}.",
                                   "Battle Lost", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }

                // Update the UI
                UpdateUI();

                // Check for game over
                CheckGameOver();
            };

            chessForm.ShowDialog();
        }

        private void ProcessAITurns()
        {
            // Simple AI: Each AI faction tries to attack one adjacent territory
            if (overworld?.Factions != null)
            {
                foreach (Faction faction in overworld.Factions)
                {
                    // Skip player faction
                    if (faction == playerFaction)
                    {
                        continue;
                    }

                    // Get all territories adjacent to this faction's territories
                    HashSet<Territory> adjacentTerritories = new HashSet<Territory>();
                    foreach (Territory territory in faction.Territories)
                    {
                        foreach (Territory adjacent in territory.AdjacentTerritories)
                        {
                            if (adjacent.Owner != faction)
                            {
                                adjacentTerritories.Add(adjacent);
                            }
                        }
                    }

                    // If there are adjacent territories to attack, pick one randomly
                    if (adjacentTerritories.Count > 0)
                    {
                        Territory targetTerritory = adjacentTerritories.ElementAt(random.Next(adjacentTerritories.Count));

                        // 50% chance to successfully attack
                        bool success = random.Next(2) == 0;
                        if (success)
                        {
                            // Take control of the territory
                            if (targetTerritory.Owner != null)
                            {
                                targetTerritory.Owner.RemoveTerritory(targetTerritory);
                            }

                            faction.AddTerritory(targetTerritory);
                            targetTerritory.Owner = faction;

                            // Notify the player if their territory was taken
                            if (targetTerritory.Owner == playerFaction)
                            {
                                MessageBox.Show($"{faction.Name} has conquered your territory {targetTerritory.Name}!",
                                               "Territory Lost", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                            }
                        }
                    }
                }
            }
        }

        private void CheckGameOver()
        {
            // Check if the player has been eliminated
            if (playerFaction?.Territories.Count == 0)
            {
                MessageBox.Show("You have been defeated! All your territories have been conquered.",
                               "Game Over", MessageBoxButtons.OK, MessageBoxIcon.Information);
                this.Close();
                return;
            }

            // Check if the player has conquered all territories
            if (overworld?.Territories != null && playerFaction?.Territories.Count == overworld.Territories.Count)
            {
                MessageBox.Show("Congratulations! You have conquered the entire overworld!",
                               "Victory", MessageBoxButtons.OK, MessageBoxIcon.Information);
                this.Close();
                return;
            }

            // Check if all AI factions have been eliminated
            bool allAIEliminated = true;
            if (overworld?.Factions != null)
            {
                foreach (Faction faction in overworld.Factions)
                {
                    if (faction != playerFaction && faction.Territories.Count > 0)
                    {
                        allAIEliminated = false;
                        break;
                    }
                }
            }

            if (allAIEliminated)
            {
                MessageBox.Show("Congratulations! You have eliminated all enemy factions!",
                               "Victory", MessageBoxButtons.OK, MessageBoxIcon.Information);
                this.Close();
            }
        }

        private Color GetColorFromConsoleColor(ConsoleColor color)
        {
            switch (color)
            {
                case ConsoleColor.Black: return Color.Black;
                case ConsoleColor.DarkBlue: return Color.DarkBlue;
                case ConsoleColor.DarkGreen: return Color.DarkGreen;
                case ConsoleColor.DarkCyan: return Color.DarkCyan;
                case ConsoleColor.DarkRed: return Color.DarkRed;
                case ConsoleColor.DarkMagenta: return Color.DarkMagenta;
                case ConsoleColor.DarkYellow: return Color.Olive;
                case ConsoleColor.Gray: return Color.Gray;
                case ConsoleColor.DarkGray: return Color.DarkGray;
                case ConsoleColor.Blue: return Color.Blue;
                case ConsoleColor.Green: return Color.Green;
                case ConsoleColor.Cyan: return Color.Cyan;
                case ConsoleColor.Red: return Color.Red;
                case ConsoleColor.Magenta: return Color.Magenta;
                case ConsoleColor.Yellow: return Color.Yellow;
                case ConsoleColor.White: return Color.White;
                default: return Color.Gray;
            }
        }

        private bool IsDarkColor(Color color)
        {
            return (color.R * 0.299 + color.G * 0.587 + color.B * 0.114) < 186;
        }
    }
}
