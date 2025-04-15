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
        private Faction? playerFaction;
        private readonly Random random = new();
        private Territory? selectedTerritory;
        private bool hasAttackedThisTurn = false;
        private bool devModeEnabled = false;
        private readonly FactionArmy factionArmy = new FactionArmy();
        private int aiDifficultyLevel = 3; // Default AI difficulty level
        
        // Bank of territory names
        private readonly List<string> territoryNameBank = new List<string>
        {
            // Medieval/Fantasy settlement names
            "Ironhold", "Stormwatch", "Shadowfen", "Oakvale", "Ravencrest",
            "Frostpeak", "Silverkeep", "Dragonspire", "Blackwater", "Highgarden",
            "Westmarch", "Duskwood", "Sunhaven", "Moonbrook", "Windhelm",
            "Eaglecrest", "Wolfden", "Bearhollow", "Foxburrow", "Hawkridge",
            "Liongate", "Serpentcoil", "Griffonreach", "Phoenixrest", "Unicornvale",
            
            // Geographic feature names
            "Misty Vale", "Amber Hills", "Crystal Lake", "Emerald Forest", "Ruby Canyon",
            "Sapphire Bay", "Diamond Peak", "Jade Plains", "Obsidian Cliffs", "Pearl Harbor",
            "Golden Fields", "Silver Springs", "Bronze Plateau", "Copper Marsh", "Iron Mountains",
            
            // Strategic location names
            "Northern Outpost", "Eastern Garrison", "Western Stronghold", "Southern Bastion",
            "Central Citadel", "Coastal Watchtower", "Mountain Fortress", "Forest Hideout",
            "Desert Sanctuary", "River Crossing"
        };

        private readonly Dictionary<Territory, Button> territoryButtons = new Dictionary<Territory, Button>();
        private readonly int territoryButtonSize = 80;
        private readonly int gridSize = 5;
        private readonly int mapPadding = 50;

        private string selectedFactionName;

        public OverworldMapForm(string selectedFactionName)
        {
            this.selectedFactionName = selectedFactionName;
            InitializeComponent();

            // Set up the form
            this.Text = "Chess Conquest - Overworld Map";
            this.ClientSize = new Size(1000, 700);
            this.FormBorderStyle = FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.StartPosition = FormStartPosition.CenterScreen;

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
                // Create overworld
                overworld = new Overworld();

                // Create factions with thematic names not tied to directions
                Faction ironLegion = new Faction("Iron Legion", ConsoleColor.Blue);
                Faction crimsonOrder = new Faction("Crimson Order", ConsoleColor.Red);
                Faction emeraldCovenant = new Faction("Emerald Covenant", ConsoleColor.Green);
                Faction goldenDynasty = new Faction("Golden Dynasty", ConsoleColor.Yellow);
                Faction shadowCollective = new Faction("Shadow Collective", ConsoleColor.Gray);

                // Add factions to overworld
                overworld.AddFaction(ironLegion);
                overworld.AddFaction(crimsonOrder);
                overworld.AddFaction(emeraldCovenant);
                overworld.AddFaction(goldenDynasty);
                overworld.AddFaction(shadowCollective);

                // Set player faction based on selection
                var factionsDict = new Dictionary<string, Faction>
                {
                    { "Iron Legion", ironLegion },
                    { "Crimson Order", crimsonOrder },
                    { "Emerald Covenant", emeraldCovenant },
                    { "Golden Dynasty", goldenDynasty },
                    { "Shadow Collective", shadowCollective }
                };
                if (selectedFactionName != null && factionsDict.ContainsKey(selectedFactionName))
                {
                    playerFaction = factionsDict[selectedFactionName];
                }
                else
                {
                    playerFaction = ironLegion; // fallback
                }
                overworld.PlayerFaction = playerFaction;

                // Shuffle the territory name bank to ensure random selection
                List<string> shuffledNames = new List<string>(territoryNameBank);
                shuffledNames = shuffledNames.OrderBy(n => random.Next()).ToList();
                
                // Create territories with unique names
                List<Territory> territories = new List<Territory>();
                int territoryCount = Math.Min(15, shuffledNames.Count);
                
                for (int i = 0; i < territoryCount; i++)
                {
                    Territory territory = new Territory(shuffledNames[i], new MapPosition(0, 0));
                    territories.Add(territory);
                }

                // Assign territories to factions (3 each)
                for (int i = 0; i < 3; i++)
                {
                    ironLegion.AddTerritory(territories[i]);
                    crimsonOrder.AddTerritory(territories[i + 3]);
                    emeraldCovenant.AddTerritory(territories[i + 6]);
                    goldenDynasty.AddTerritory(territories[i + 9]);
                    
                    if (i < territories.Count - 12)
                    {
                        shadowCollective.AddTerritory(territories[i + 12]);
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
                Size = new Size(300, 620), 
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

            // Create Dev Mode checkbox
            CheckBox devModeCheckBox = new CheckBox
            {
                Text = "Dev Mode",
                Name = "devModeCheckBox",
                Font = new Font("Arial", 10),
                Location = new Point(200, 40),
                Size = new Size(90, 20),
                Checked = devModeEnabled
            };
            devModeCheckBox.CheckedChanged += (sender, e) => {
                devModeEnabled = devModeCheckBox.Checked;
            };
            controlPanel.Controls.Add(devModeCheckBox);
            
            // Create turn status label
            Label turnStatusLabel = new Label
            {
                Name = "turnStatusLabel",
                Text = "You can attack one territory this turn.",
                Font = new Font("Arial", 10),
                Location = new Point(10, 40),
                Size = new Size(180, 20),
                TextAlign = ContentAlignment.MiddleLeft
            };
            controlPanel.Controls.Add(turnStatusLabel);

            // Add AI Difficulty selector
            GroupBox difficultyBox = new GroupBox
            {
                Text = "AI Difficulty",
                Font = new Font("Arial", 10),
                Location = new Point(10, 70),
                Size = new Size(280, 60)
            };
            controlPanel.Controls.Add(difficultyBox);

            // Create radio buttons for difficulty levels
            RadioButton easyButton = new RadioButton
            {
                Text = "Easy",
                Location = new Point(20, 25),
                Size = new Size(70, 20),
                Checked = aiDifficultyLevel == 1
            };
            easyButton.CheckedChanged += (sender, e) => {
                if (easyButton.Checked) aiDifficultyLevel = 1;
            };
            difficultyBox.Controls.Add(easyButton);

            RadioButton mediumButton = new RadioButton
            {
                Text = "Medium",
                Location = new Point(100, 25),
                Size = new Size(80, 20),
                Checked = aiDifficultyLevel == 2
            };
            mediumButton.CheckedChanged += (sender, e) => {
                if (mediumButton.Checked) aiDifficultyLevel = 2;
            };
            difficultyBox.Controls.Add(mediumButton);

            RadioButton hardButton = new RadioButton
            {
                Text = "Hard",
                Location = new Point(190, 25),
                Size = new Size(70, 20),
                Checked = aiDifficultyLevel == 3
            };
            hardButton.CheckedChanged += (sender, e) => {
                if (hardButton.Checked) aiDifficultyLevel = 3;
            };
            difficultyBox.Controls.Add(hardButton);

            // Create selected territory info
            GroupBox territoryInfoBox = new GroupBox
            {
                Text = "Selected Territory",
                Font = new Font("Arial", 12),
                Location = new Point(10, 140),
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
                Location = new Point(10, 300),
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
                Location = new Point(10, 510),
                Size = new Size(280, 40)
            };
            endTurnButton.Click += EndTurnButton_Click;
            controlPanel.Controls.Add(endTurnButton);

            Button returnToMenuButton = new Button
            {
                Text = "Return to Main Menu",
                Font = new Font("Arial", 12),
                Location = new Point(10, 560),
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

            // Check if the player has already attacked this turn
            if (hasAttackedThisTurn)
            {
                MessageBox.Show("You can only attack once per turn. End your turn to attack again.",
                               "Turn Limit Reached", MessageBoxButtons.OK, MessageBoxIcon.Information);
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
            // Reset the attack flag for the next turn
            hasAttackedThisTurn = false;
            
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
                    // Also check if the player has already attacked this turn
                    bool canAttack = selectedTerritory.Owner != playerFaction &&
                                    selectedTerritory.AdjacentTerritories.Any(t => t.Owner == playerFaction) &&
                                    !hasAttackedThisTurn;
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

            // Update turn status
            Label? turnStatusLabel = this.Controls.Find("turnStatusLabel", true).FirstOrDefault() as Label;
            if (turnStatusLabel != null)
            {
                turnStatusLabel.Text = hasAttackedThisTurn ? "You have already attacked this turn." : "You can attack one territory this turn.";
            }

            // Refresh the form to update the connections
            this.Invalidate();
        }

        private void StartBattle(Territory territory, Faction? attackingFaction = null)
        {
            // Determine attacker and defender
            bool playerIsAttacker = attackingFaction == null;
            Faction? attackerFaction = playerIsAttacker ? playerFaction : (attackingFaction ?? territory.Owner);
            Faction? defenderFaction = playerIsAttacker ? territory.Owner : playerFaction;
            
            // Check for null factions
            if (attackerFaction == null || defenderFaction == null)
            {
                MessageBox.Show("Error: Missing faction information", "Battle Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            
            // Check for Dev Mode
            if (devModeEnabled)
            {
                // Create a simple form to choose the outcome
                Form devModeForm = new Form
                {
                    Text = "Dev Mode - Choose Battle Outcome",
                    Size = new Size(300, 150),
                    FormBorderStyle = FormBorderStyle.FixedDialog,
                    StartPosition = FormStartPosition.CenterParent,
                    MaximizeBox = false,
                    MinimizeBox = false
                };
                
                Label infoLabel = new Label
                {
                    Text = $"Battle for {territory.Name}\nAttacker: {attackerFaction.Name} (Black)\nDefender: {defenderFaction.Name} (White)",
                    Location = new Point(10, 10),
                    Size = new Size(280, 50),
                    TextAlign = ContentAlignment.MiddleCenter
                };
                devModeForm.Controls.Add(infoLabel);
                
                Button attackerWinsButton = new Button
                {
                    Text = $"Attacker Wins ({attackerFaction.Name})",
                    Location = new Point(10, 70),
                    Size = new Size(130, 30)
                };
                attackerWinsButton.Click += (sender, e) =>
                {
                    if (playerIsAttacker)
                    {
                        // Player conquered the territory
                        defenderFaction.RemoveTerritory(territory);
                        attackerFaction.AddTerritory(territory);
                        territory.Owner = attackerFaction;
                        
                        MessageBox.Show($"You have conquered {territory.Name}!",
                                       "Territory Conquered", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        
                        // Mark that the player has attacked this turn
                        hasAttackedThisTurn = true;
                    }
                    else
                    {
                        // AI conquered the player's territory
                        playerFaction?.RemoveTerritory(territory);
                        attackerFaction.AddTerritory(territory);
                        territory.Owner = attackerFaction;
                        
                        MessageBox.Show($"You lost {territory.Name} to {attackerFaction.Name}!",
                                       "Territory Lost", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    }
                    
                    // Mark that the player has attacked this turn if they were the attacker
                    if (playerIsAttacker)
                    {
                        hasAttackedThisTurn = true;
                    }
                    
                    // Update the UI
                    UpdateUI();
                    
                    // Check for game over
                    CheckGameOver();
                    
                    devModeForm.Close();
                };
                devModeForm.Controls.Add(attackerWinsButton);
                
                Button defenderWinsButton = new Button
                {
                    Text = $"Defender Wins ({defenderFaction.Name})",
                    Location = new Point(150, 70),
                    Size = new Size(130, 30)
                };
                defenderWinsButton.Click += (sender, e) =>
                {
                    if (playerIsAttacker)
                    {
                        // Player failed to conquer
                        MessageBox.Show($"You failed to conquer {territory.Name}.",
                                       "Battle Lost", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        
                        // Mark that the player has attacked this turn
                        hasAttackedThisTurn = true;
                    }
                    else
                    {
                        // Player successfully defended
                        MessageBox.Show($"You successfully defended {territory.Name}!",
                                       "Territory Defended", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                    
                    // Mark that the player has attacked this turn if they were the attacker
                    if (playerIsAttacker)
                    {
                        hasAttackedThisTurn = true;
                    }
                    
                    // Update the UI
                    UpdateUI();
                    
                    // Check for game over
                    CheckGameOver();
                    
                    devModeForm.Close();
                };
                devModeForm.Controls.Add(defenderWinsButton);

                devModeForm.ShowDialog();
                return;
            }

            // Normal battle mode - chess game
            // Defender always plays as white
            PieceColor playerColor = playerIsAttacker ? PieceColor.Black : PieceColor.White;
            
            // Create a game with no initial setup
            Game game = new Game(playerColor, false);
            
            // Set the AI difficulty level
            game.Difficulty = aiDifficultyLevel;
            
            // Get the faction armies for the battle
            List<Piece> attackerArmy = factionArmy.GetFactionArmy(attackerFaction.Name, PieceColor.Black);
            List<Piece> defenderArmy = factionArmy.GetFactionArmy(defenderFaction.Name, PieceColor.White);
            
            // Combine both armies and set up the custom game
            List<Piece> allPieces = new List<Piece>();
            allPieces.AddRange(attackerArmy);
            allPieces.AddRange(defenderArmy);
            game.SetupCustomGame(allPieces);
            
            // Create battle title with faction information
            string battleTitle = $"Battle for {territory.Name}: {attackerFaction.Name} vs {defenderFaction.Name}";

            // Open the chess game form
            ChessGameForm chessForm = new ChessGameForm(game, battleTitle);
            chessForm.GameOver += (sender, e) =>
            {
                bool playerWon = e.Winner == game.PlayerColor;
                
                if (playerIsAttacker)
                {
                    // Player is attacking
                    if (playerWon)
                    {
                        // Player conquered the territory
                        defenderFaction.RemoveTerritory(territory);
                        attackerFaction.AddTerritory(territory);
                        territory.Owner = attackerFaction;

                        MessageBox.Show($"You have conquered {territory.Name}!",
                                       "Territory Conquered", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                    else
                    {
                        // Player failed to conquer
                        MessageBox.Show($"You failed to conquer {territory.Name}.",
                                       "Battle Lost", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                    
                    // Mark that the player has attacked this turn
                    hasAttackedThisTurn = true;
                }
                else
                {
                    // Player is defending
                    if (playerWon)
                    {
                        // Player successfully defended
                        MessageBox.Show($"You successfully defended {territory.Name}!",
                                       "Territory Defended", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                    else
                    {
                        // Player lost the territory
                        playerFaction.RemoveTerritory(territory);
                        attackerFaction.AddTerritory(territory);
                        territory.Owner = attackerFaction;
                        
                        MessageBox.Show($"You lost {territory.Name} to {attackerFaction.Name}!",
                                       "Territory Lost", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    }
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
            // Each AI faction gets one attack per turn
            if (overworld?.Factions != null)
            {
                foreach (Faction faction in overworld.Factions)
                {
                    // Skip player faction
                    if (faction == playerFaction)
                    {
                        continue;
                    }

                    // Announce the faction's turn
                    MessageBox.Show($"{faction.Name}'s Turn", 
                                   "AI Turn", MessageBoxButtons.OK, MessageBoxIcon.Information);

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
                        string ownerName = targetTerritory.Owner?.Name ?? "Unowned";
                        
                        // Announce the attack
                        MessageBox.Show($"{faction.Name} is attacking {targetTerritory.Name} (owned by {ownerName})",
                                       "Territory Attack", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        
                        // If the target territory belongs to the player, initiate a player vs AI battle
                        if (targetTerritory.Owner == playerFaction)
                        {
                            // Notify the player they are being attacked
                            MessageBox.Show($"{faction.Name} is attacking your territory {targetTerritory.Name}!",
                                           "Territory Under Attack", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                            
                            // AI is attacking player territory
                            StartBattle(targetTerritory, faction);
                        }
                        else
                        {
                            // AI vs AI battle - resolve with RNG
                            // 50% chance for attacker to win
                            bool attackerWins = random.Next(2) == 0;
                            
                            if (attackerWins)
                            {
                                // Attacker wins
                                Faction? defenderFaction = targetTerritory.Owner;
                                if (defenderFaction != null)
                                {
                                    defenderFaction.RemoveTerritory(targetTerritory);
                                    
                                    // Notify the player if an AI faction was eliminated
                                    if (defenderFaction.Territories.Count == 0)
                                    {
                                        MessageBox.Show($"{defenderFaction.Name} has been eliminated by {faction.Name}!",
                                                      "Faction Eliminated", MessageBoxButtons.OK, MessageBoxIcon.Information);
                                    }
                                }
                                
                                faction.AddTerritory(targetTerritory);
                                targetTerritory.Owner = faction;
                                
                                // Announce the battle outcome
                                MessageBox.Show($"{faction.Name} has conquered {targetTerritory.Name}!",
                                              "Battle Outcome", MessageBoxButtons.OK, MessageBoxIcon.Information);
                            }
                            else
                            {
                                // Defender wins
                                MessageBox.Show($"{ownerName} successfully defended {targetTerritory.Name} against {faction.Name}!",
                                              "Battle Outcome", MessageBoxButtons.OK, MessageBoxIcon.Information);
                            }
                        }
                    }
                    else
                    {
                        // No valid targets to attack
                        MessageBox.Show($"{faction.Name} has no valid territories to attack this turn.",
                                       "No Attack", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                }
            }
            
            // Update the UI after all AI turns
            UpdateUI();
            
            // Check for game over conditions
            CheckGameOver();
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
