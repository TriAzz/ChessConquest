using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;

namespace ChessConquestGUI
{
    public partial class FactionSelectionForm : Form
    {
        private List<FactionInfo> factions;
        private int hoveredFactionIndex = -1;
        private int selectedFactionIndex = -1;
        private Action<string> onFactionSelected;
        private Label descLabel;
        private Button confirmBtn;

        public FactionSelectionForm(Action<string> onFactionSelected)
        {
            this.onFactionSelected = onFactionSelected;
            InitializeFactions();
            BuildUI();
        }

        private void InitializeFactions()
        {
            factions = new List<FactionInfo>
            {
                new FactionInfo(
                    "Iron Legion", 
                    "A disciplined army known for its resilience and heavy armor. Masters of defense and attrition.",
                    "Uses two Vanguards in the center of their front line (replacing the middle pawns)",
                    new Dictionary<string, string> { 
                        { "Vanguard", "Moves diagonally 1 square or horizontally/vertically up to 2 squares. Can capture in any of these directions." } 
                    }
                ),
                new FactionInfo(
                    "Crimson Order", 
                    "A zealous force that excels in aggressive tactics and rapid strikes. Unyielding in battle.",
                    "Uses four Cavalry units - replacing knights and the pawns in front of bishops",
                    new Dictionary<string, string> { 
                        { "Cavalry", "Moves like a knight (L-shape) or can move 1-2 spaces Horizontally or Vertically. Can only jump pieces when moving like a knight." } 
                    }
                ),
                new FactionInfo(
                    "Emerald Covenant", 
                    "Nature-bound warriors with cunning strategies and adaptability. Masters of mobility and surprise.",
                    "Uses the standard chess piece arrangement",
                    new Dictionary<string, string>()
                ),
                new FactionInfo(
                    "Golden Dynasty", 
                    "A noble faction with a focus on wealth and influence. Balanced armies with versatile abilities.",
                    "Uses the standard chess piece arrangement",
                    new Dictionary<string, string>()
                ),
                new FactionInfo(
                    "Shadow Collective", 
                    "Masters of subterfuge and deception. Specialized in ambushes and unconventional warfare.",
                    "Uses the standard chess piece arrangement",
                    new Dictionary<string, string>()
                )
            };
        }

        private void BuildUI()
        {
            this.Text = "Select Your Faction";
            this.Size = new Size(700, 400);
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;

            // Left panel - Faction buttons
            Panel leftPanel = new Panel
            {
                Location = new Point(0, 0),
                Size = new Size(220, 400),
                BackColor = Color.FromArgb(240, 240, 240)
            };
            this.Controls.Add(leftPanel);

            int buttonY = 30;
            for (int i = 0; i < factions.Count; i++)
            {
                int idx = i;
                Button btn = new Button
                {
                    Text = factions[i].Name,
                    Location = new Point(20, buttonY),
                    Size = new Size(180, 40),
                    Tag = idx,
                    BackColor = Color.White,
                    FlatStyle = FlatStyle.Flat,
                    Font = new Font("Segoe UI", 12F, FontStyle.Bold)
                };
                btn.MouseEnter += (s, e) => { hoveredFactionIndex = idx; UpdateDescription(); };
                btn.MouseLeave += (s, e) => { hoveredFactionIndex = -1; UpdateDescription(); };
                btn.Click += (s, e) => { selectedFactionIndex = idx; UpdateSelection(leftPanel); };
                leftPanel.Controls.Add(btn);
                buttonY += 50;
            }

            // Right panel - Faction description
            Panel rightPanel = new Panel
            {
                Location = new Point(220, 0),
                Size = new Size(480, 400),
                BackColor = Color.White
            };
            this.Controls.Add(rightPanel);

            descLabel = new Label
            {
                Name = "descLabel",
                Location = new Point(30, 40),
                Size = new Size(410, 220),
                Font = new Font("Segoe UI", 11F),
                ForeColor = Color.Black,
                AutoSize = false
            };
            rightPanel.Controls.Add(descLabel);

            // Confirm button
            confirmBtn = new Button
            {
                Text = "Confirm",
                Location = new Point(320, 320),
                Size = new Size(120, 40),
                Font = new Font("Segoe UI", 12F, FontStyle.Bold),
                Enabled = false
            };
            confirmBtn.Click += (s, e) => ConfirmSelection();
            rightPanel.Controls.Add(confirmBtn);

            void UpdateDescription()
            {
                if (hoveredFactionIndex >= 0)
                {
                    var faction = factions[hoveredFactionIndex];
                    string description = $"{faction.Name}\n\n{faction.Description}\n\nArmy Setup: {faction.ArmySetup}";
                    
                    // Add custom pieces information if there are any
                    if (faction.CustomPieces != null && faction.CustomPieces.Count > 0)
                    {
                        description += "\n\nSpecial Units:";
                        foreach (var piece in faction.CustomPieces)
                        {
                            description += $"\n• {piece.Key}: {piece.Value}";
                        }
                    }
                    
                    descLabel.Text = description;
                }
                else if (selectedFactionIndex >= 0)
                {
                    var faction = factions[selectedFactionIndex];
                    string description = $"{faction.Name}\n\n{faction.Description}\n\nArmy Setup: {faction.ArmySetup}";
                    
                    // Add custom pieces information if there are any
                    if (faction.CustomPieces != null && faction.CustomPieces.Count > 0)
                    {
                        description += "\n\nSpecial Units:";
                        foreach (var piece in faction.CustomPieces)
                        {
                            description += $"\n• {piece.Key}: {piece.Value}";
                        }
                    }
                    
                    descLabel.Text = description;
                }
                else
                {
                    descLabel.Text = "Hover over a faction to see its description.";
                }
            }

            void UpdateSelection(Panel panel)
            {
                foreach (Control c in panel.Controls)
                {
                    if (c is Button b)
                    {
                        int idx = (int)b.Tag;
                        b.BackColor = idx == selectedFactionIndex ? Color.LightBlue : Color.White;
                    }
                }
                confirmBtn.Enabled = selectedFactionIndex >= 0;
                UpdateDescription();
            }

            UpdateDescription();
        }

        private void ConfirmSelection()
        {
            if (selectedFactionIndex >= 0)
            {
                onFactionSelected?.Invoke(factions[selectedFactionIndex].Name);
                this.DialogResult = DialogResult.OK;
                this.Close();
            }
        }
    }

    public class FactionInfo
    {
        public string Name { get; }
        public string Description { get; }
        public string ArmySetup { get; }
        public Dictionary<string, string> CustomPieces { get; }

        public FactionInfo(string name, string description, string armySetup = "", Dictionary<string, string> customPieces = null)
        {
            Name = name;
            Description = description;
            ArmySetup = armySetup;
            CustomPieces = customPieces ?? new Dictionary<string, string>();
        }
    }
}
