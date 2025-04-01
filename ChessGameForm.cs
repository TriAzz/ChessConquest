using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using ChessConquest;
using ChessConquest.Pieces;

namespace ChessConquestGUI
{
    public partial class ChessGameForm : Form
    {
        private Game game;
        private readonly Button[,] boardButtons;
        private readonly int squareSize = 80;
        private Position? selectedPosition;
        private List<Position> highlightedMoves;
        private readonly Color lightSquareColor = Color.AntiqueWhite;
        private readonly Color darkSquareColor = Color.SaddleBrown;
        private readonly Color highlightColor = Color.LightGreen;
        private readonly Color selectedColor = Color.Yellow;
        private readonly string battleTitle;
        
        // Game over event
        public event EventHandler<GameOverEventArgs>? GameOver;
        
        public ChessGameForm(Game game, string title = "Chess Battle")
        {
            InitializeComponent();
            
            this.game = game;
            this.battleTitle = title;
            
            // Set up the form
            this.Text = battleTitle;
            this.ClientSize = new Size(squareSize * 8 + 300, squareSize * 8 + 50);
            this.FormBorderStyle = FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.StartPosition = FormStartPosition.CenterScreen;
            
            // Initialize highlighted moves list
            highlightedMoves = new List<Position>();
            
            // Create the chess board
            boardButtons = new Button[8, 8];
            for (int row = 0; row < 8; row++)
            {
                for (int col = 0; col < 8; col++)
                {
                    Button button = new Button
                    {
                        Size = new Size(squareSize, squareSize),
                        Location = new Point(col * squareSize, row * squareSize),
                        FlatStyle = FlatStyle.Flat,
                        BackColor = (row + col) % 2 == 0 ? lightSquareColor : darkSquareColor,
                        Font = new Font("Segoe UI Symbol", 24, FontStyle.Bold),
                        Tag = new Position(row, col),
                        TabStop = false
                    };
                    
                    button.Click += BoardButton_Click;
                    this.Controls.Add(button);
                    boardButtons[row, col] = button;
                }
            }
            
            // Create game control panel
            CreateGameControlPanel();
            
            // Update the board display
            UpdateBoardDisplay();
            
            // If AI goes first, make its move
            if (game.CurrentTurn == game.AIColor)
            {
                MakeAIMove();
            }
        }
        
        private void CreateGameControlPanel()
        {
            // Create panel for game controls
            Panel controlPanel = new Panel
            {
                Location = new Point(squareSize * 8 + 10, 10),
                Size = new Size(280, squareSize * 8),
                BorderStyle = BorderStyle.FixedSingle
            };
            this.Controls.Add(controlPanel);
            
            // Battle title
            Label titleLabel = new Label
            {
                Text = battleTitle,
                Font = new Font("Arial", 16, FontStyle.Bold),
                Location = new Point(10, 10),
                Size = new Size(260, 40),
                TextAlign = ContentAlignment.MiddleCenter
            };
            controlPanel.Controls.Add(titleLabel);
            
            // Current turn display
            Label turnLabel = new Label
            {
                Text = "Current Turn:",
                Font = new Font("Arial", 12),
                Location = new Point(10, 60),
                Size = new Size(120, 30),
                TextAlign = ContentAlignment.MiddleLeft
            };
            controlPanel.Controls.Add(turnLabel);
            
            Label currentTurnLabel = new Label
            {
                Name = "currentTurnLabel",
                Text = "White",
                Font = new Font("Arial", 12, FontStyle.Bold),
                Location = new Point(130, 60),
                Size = new Size(140, 30),
                TextAlign = ContentAlignment.MiddleLeft
            };
            controlPanel.Controls.Add(currentTurnLabel);
            
            // Game status display
            Label statusLabel = new Label
            {
                Name = "statusLabel",
                Text = "Game in progress",
                Font = new Font("Arial", 12),
                Location = new Point(10, 100),
                Size = new Size(260, 30),
                TextAlign = ContentAlignment.MiddleCenter
            };
            controlPanel.Controls.Add(statusLabel);
            
            // Instructions
            GroupBox instructionsBox = new GroupBox
            {
                Text = "Instructions",
                Font = new Font("Arial", 12),
                Location = new Point(10, 150),
                Size = new Size(260, 150)
            };
            controlPanel.Controls.Add(instructionsBox);
            
            Label instructionsLabel = new Label
            {
                Text = "1. Click on a piece to select it\n" +
                      "2. Valid moves will be highlighted in green\n" +
                      "3. Click on a highlighted square to move\n" +
                      "4. Capture all enemy pieces to win",
                Font = new Font("Arial", 10),
                Location = new Point(10, 25),
                Size = new Size(240, 115),
                TextAlign = ContentAlignment.TopLeft
            };
            instructionsBox.Controls.Add(instructionsLabel);
            
            // Resign button
            Button resignButton = new Button
            {
                Text = "Resign",
                Font = new Font("Arial", 12),
                Location = new Point(10, squareSize * 8 - 60),
                Size = new Size(260, 40)
            };
            resignButton.Click += (sender, e) => 
            {
                if (MessageBox.Show("Are you sure you want to resign?", "Resign", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                {
                    // Trigger game over event with AI as winner
                    GameOver?.Invoke(this, new GameOverEventArgs(game.AIColor));
                    this.Close();
                }
            };
            controlPanel.Controls.Add(resignButton);
        }
        
        private void BoardButton_Click(object? sender, EventArgs e)
        {
            if (game.IsGameOver || game.CurrentTurn != game.PlayerColor || sender == null)
            {
                return;
            }
            
            Button clickedButton = (Button)sender;
            if (clickedButton.Tag is not Position clickedPosition)
            {
                return;
            }
            
            // If no piece is selected yet
            if (selectedPosition == null)
            {
                Piece? piece = game.Board.GetPieceAt(clickedPosition);
                
                // Check if there's a piece at the clicked position and it's the player's piece
                if (piece != null && piece.Color == game.PlayerColor)
                {
                    // Select the piece
                    selectedPosition = clickedPosition;
                    
                    // Get valid moves for the selected piece
                    highlightedMoves = game.GetValidMovesForPiece(clickedPosition);
                    
                    // Update the board to show the selection and valid moves
                    UpdateBoardDisplay();
                }
            }
            // If a piece is already selected
            else
            {
                // If the clicked position is a valid move
                if (highlightedMoves.Contains(clickedPosition))
                {
                    // Make the move
                    game.MakeMove(selectedPosition.Value, clickedPosition);
                    
                    // Clear selection
                    selectedPosition = null;
                    highlightedMoves.Clear();
                    
                    // Update the board
                    UpdateBoardDisplay();
                    
                    // Check if the game is over
                    if (game.IsGameOver)
                    {
                        HandleGameOver();
                    }
                    else
                    {
                        // Let AI make a move if it's its turn
                        if (game.CurrentTurn == game.AIColor)
                        {
                            MakeAIMove();
                        }
                    }
                }
                // If the clicked position is not a valid move
                else
                {
                    // Deselect the current piece
                    selectedPosition = null;
                    highlightedMoves.Clear();
                    
                    // Try selecting a new piece
                    BoardButton_Click(sender, e);
                }
            }
        }
        
        private void UpdateBoardDisplay()
        {
            // Update the board
            for (int row = 0; row < 8; row++)
            {
                for (int col = 0; col < 8; col++)
                {
                    Position position = new Position(row, col);
                    Button button = boardButtons[row, col];
                    
                    // Set the background color
                    if (selectedPosition != null && position.Equals(selectedPosition.Value))
                    {
                        button.BackColor = selectedColor;
                    }
                    else if (highlightedMoves.Contains(position))
                    {
                        button.BackColor = highlightColor;
                    }
                    else
                    {
                        button.BackColor = (row + col) % 2 == 0 ? lightSquareColor : darkSquareColor;
                    }
                    
                    // Get the piece at this position
                    Piece? piece = game.Board.GetPieceAt(position);
                    
                    // Set the button text to the piece symbol
                    if (piece != null)
                    {
                        button.Text = piece.Symbol.ToString();
                        button.ForeColor = piece.Color == PieceColor.White ? Color.White : Color.Black;
                    }
                    else
                    {
                        button.Text = "";
                    }
                }
            }
            
            // Update the current turn label
            Label? currentTurnLabel = this.Controls.Find("currentTurnLabel", true).FirstOrDefault() as Label;
            if (currentTurnLabel != null)
            {
                currentTurnLabel.Text = game.CurrentTurn == PieceColor.White ? "White" : "Black";
            }
            
            // Update the status label
            if (game.IsGameOver)
            {
                HandleGameOver();
            }
            else
            {
                UpdateStatusLabel("Game in progress");
            }
        }
        
        private void MakeAIMove()
        {
            // Show thinking message
            UpdateStatusLabel("AI is thinking...");
            Application.DoEvents();
            
            // Use a background worker to prevent UI freezing
            System.ComponentModel.BackgroundWorker worker = new System.ComponentModel.BackgroundWorker();
            worker.DoWork += (sender, e) =>
            {
                // Make the AI move
                game.MakeAIMove();
            };
            
            worker.RunWorkerCompleted += (sender, e) =>
            {
                // Update the board
                UpdateBoardDisplay();
                
                // Check if the game is over
                if (game.IsGameOver)
                {
                    HandleGameOver();
                }
            };
            
            worker.RunWorkerAsync();
        }
        
        private void HandleGameOver()
        {
            // Update the board display one last time
            UpdateBoardDisplay();
            
            // Determine the winner
            string statusMessage = "Game Over! ";
            var winner = game.GetWinner();
            
            if (winner == PieceColor.White)
            {
                statusMessage += "White wins!";
            }
            else if (winner == PieceColor.Black)
            {
                statusMessage += "Black wins!";
            }
            else
            {
                statusMessage += "Draw!";
            }
            
            UpdateStatusLabel(statusMessage);
            
            // Show game over message
            MessageBox.Show(statusMessage, "Game Over", MessageBoxButtons.OK, MessageBoxIcon.Information);
            
            // Trigger game over event
            if (winner.HasValue)
            {
                GameOver?.Invoke(this, new GameOverEventArgs(winner.Value));
            }
            else
            {
                // In case of a draw, we'll consider it a loss for the player
                GameOver?.Invoke(this, new GameOverEventArgs(game.PlayerColor == PieceColor.White ? PieceColor.Black : PieceColor.White));
            }
            
            // Close the form
            this.Close();
        }
        
        private void UpdateStatusLabel(string message)
        {
            Label? statusLabel = this.Controls.Find("statusLabel", true).FirstOrDefault() as Label;
            if (statusLabel != null)
            {
                statusLabel.Text = message;
            }
        }
    }
}
