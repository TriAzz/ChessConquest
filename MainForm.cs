using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using ChessConquest;
using ChessConquest.Pieces;

namespace ChessConquestGUI
{
    public partial class MainForm : Form
    {
        public MainForm()
        {
            InitializeComponent();
            
            // Set up the form
            this.Text = "Chess Conquest";
            this.ClientSize = new Size(800, 600);
            this.FormBorderStyle = FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.StartPosition = FormStartPosition.CenterScreen;
            
            // Create main menu
            CreateMainMenu();
        }
        
        private void CreateMainMenu()
        {
            // Create title label
            Label titleLabel = new Label
            {
                Text = "Chess Conquest",
                Font = new Font("Arial", 36, FontStyle.Bold),
                Location = new Point(150, 50),
                Size = new Size(500, 60),
                TextAlign = ContentAlignment.MiddleCenter
            };
            this.Controls.Add(titleLabel);
            
            // Create subtitle label
            Label subtitleLabel = new Label
            {
                Text = "Strategic Chess Warfare",
                Font = new Font("Arial", 18, FontStyle.Italic),
                Location = new Point(150, 110),
                Size = new Size(500, 40),
                TextAlign = ContentAlignment.MiddleCenter
            };
            this.Controls.Add(subtitleLabel);
            
            // Create game mode buttons
            Button overworldButton = new Button
            {
                Text = "Overworld Conquest",
                Font = new Font("Arial", 16),
                Location = new Point(250, 200),
                Size = new Size(300, 60),
                FlatStyle = FlatStyle.Flat
            };
            overworldButton.Click += OverworldButton_Click;
            this.Controls.Add(overworldButton);
            
            Button chessMatchButton = new Button
            {
                Text = "Standard Chess Match",
                Font = new Font("Arial", 16),
                Location = new Point(250, 280),
                Size = new Size(300, 60),
                FlatStyle = FlatStyle.Flat
            };
            chessMatchButton.Click += ChessMatchButton_Click;
            this.Controls.Add(chessMatchButton);
            
            // Create options button
            Button optionsButton = new Button
            {
                Text = "Options",
                Font = new Font("Arial", 16),
                Location = new Point(250, 360),
                Size = new Size(300, 60),
                FlatStyle = FlatStyle.Flat
            };
            optionsButton.Click += OptionsButton_Click;
            this.Controls.Add(optionsButton);
            
            // Create quit button
            Button quitButton = new Button
            {
                Text = "Quit",
                Font = new Font("Arial", 16),
                Location = new Point(250, 440),
                Size = new Size(300, 60),
                FlatStyle = FlatStyle.Flat
            };
            quitButton.Click += QuitButton_Click;
            this.Controls.Add(quitButton);
            
            // Create version label
            Label versionLabel = new Label
            {
                Text = "Version 1.0.0",
                Font = new Font("Arial", 10),
                Location = new Point(10, 570),
                Size = new Size(100, 20),
                TextAlign = ContentAlignment.MiddleLeft
            };
            this.Controls.Add(versionLabel);
        }
        
        private void OverworldButton_Click(object? sender, EventArgs e)
        {
            // Open the overworld map form
            OverworldMapForm overworldForm = new OverworldMapForm();
            this.Hide();
            overworldForm.ShowDialog();
            this.Show();
        }
        
        private void ChessMatchButton_Click(object? sender, EventArgs e)
        {
            // Show color selection dialog
            ColorSelectionForm colorForm = new ColorSelectionForm();
            if (colorForm.ShowDialog() == DialogResult.OK)
            {
                // Create a new chess game
                Game game = new Game(colorForm.SelectedColor);
                game.Difficulty = colorForm.SelectedDifficulty;
                
                // Open the chess game form
                ChessGameForm chessForm = new ChessGameForm(game);
                this.Hide();
                chessForm.ShowDialog();
                this.Show();
            }
        }
        
        private void OptionsButton_Click(object? sender, EventArgs e)
        {
            // Show options dialog
            MessageBox.Show("Options will be available in a future update.", "Options", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }
        
        private void QuitButton_Click(object? sender, EventArgs e)
        {
            // Confirm quit
            if (MessageBox.Show("Are you sure you want to quit?", "Quit", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
            {
                Application.Exit();
            }
        }
    }
}
