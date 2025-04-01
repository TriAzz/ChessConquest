using System;
using System.Drawing;
using System.Windows.Forms;
using ChessConquest;
using ChessConquest.Pieces;

namespace ChessConquestGUI
{
    public class ColorSelectionForm : Form
    {
        public PieceColor SelectedColor { get; private set; }
        public int SelectedDifficulty { get; private set; }
        
        private RadioButton whiteRadioButton = new RadioButton();
        private RadioButton blackRadioButton = new RadioButton();
        private RadioButton easyRadioButton = new RadioButton();
        private RadioButton mediumRadioButton = new RadioButton();
        private RadioButton hardRadioButton = new RadioButton();
        
        public ColorSelectionForm()
        {
            InitializeComponent();
        }
        
        private void InitializeComponent()
        {
            // Set up the form
            this.Text = "Game Options";
            this.ClientSize = new Size(400, 350);
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.StartPosition = FormStartPosition.CenterParent;
            this.ShowInTaskbar = false;
            
            // Create title label
            Label titleLabel = new Label
            {
                Text = "Game Options",
                Font = new Font("Arial", 16, FontStyle.Bold),
                Location = new Point(20, 20),
                Size = new Size(360, 30),
                TextAlign = ContentAlignment.MiddleCenter
            };
            this.Controls.Add(titleLabel);
            
            // Create color selection group
            GroupBox colorGroupBox = new GroupBox
            {
                Text = "Play As",
                Font = new Font("Arial", 12),
                Location = new Point(20, 60),
                Size = new Size(360, 80)
            };
            
            whiteRadioButton.Text = "White";
            whiteRadioButton.Font = new Font("Arial", 10);
            whiteRadioButton.Location = new Point(30, 30);
            whiteRadioButton.Size = new Size(140, 30);
            whiteRadioButton.Checked = true;
            colorGroupBox.Controls.Add(whiteRadioButton);
            
            blackRadioButton.Text = "Black";
            blackRadioButton.Font = new Font("Arial", 10);
            blackRadioButton.Location = new Point(190, 30);
            blackRadioButton.Size = new Size(140, 30);
            colorGroupBox.Controls.Add(blackRadioButton);
            
            this.Controls.Add(colorGroupBox);
            
            // Create difficulty selection group
            GroupBox difficultyGroupBox = new GroupBox
            {
                Text = "AI Difficulty",
                Font = new Font("Arial", 12),
                Location = new Point(20, 150),
                Size = new Size(360, 120)
            };
            
            easyRadioButton.Text = "Easy";
            easyRadioButton.Font = new Font("Arial", 10);
            easyRadioButton.Location = new Point(30, 30);
            easyRadioButton.Size = new Size(300, 25);
            difficultyGroupBox.Controls.Add(easyRadioButton);
            
            mediumRadioButton.Text = "Medium";
            mediumRadioButton.Font = new Font("Arial", 10);
            mediumRadioButton.Location = new Point(30, 55);
            mediumRadioButton.Size = new Size(300, 25);
            mediumRadioButton.Checked = true;
            difficultyGroupBox.Controls.Add(mediumRadioButton);
            
            hardRadioButton.Text = "Hard";
            hardRadioButton.Font = new Font("Arial", 10);
            hardRadioButton.Location = new Point(30, 80);
            hardRadioButton.Size = new Size(300, 25);
            difficultyGroupBox.Controls.Add(hardRadioButton);
            
            this.Controls.Add(difficultyGroupBox);
            
            // Create buttons
            Button okButton = new Button
            {
                Text = "OK",
                DialogResult = DialogResult.OK,
                Location = new Point(110, 280),
                Size = new Size(80, 30)
            };
            okButton.Click += OkButton_Click;
            this.Controls.Add(okButton);
            
            Button cancelButton = new Button
            {
                Text = "Cancel",
                DialogResult = DialogResult.Cancel,
                Location = new Point(210, 280),
                Size = new Size(80, 30)
            };
            this.Controls.Add(cancelButton);
            
            this.AcceptButton = okButton;
            this.CancelButton = cancelButton;
        }
        
        private void OkButton_Click(object? sender, EventArgs e)
        {
            // Set the selected color
            SelectedColor = whiteRadioButton.Checked ? PieceColor.White : PieceColor.Black;
            
            // Set the selected difficulty
            if (easyRadioButton.Checked)
                SelectedDifficulty = 1;
            else if (mediumRadioButton.Checked)
                SelectedDifficulty = 3;
            else
                SelectedDifficulty = 5;
        }
    }
    
    // Event args for game over event
    public class GameOverEventArgs : EventArgs
    {
        public PieceColor Winner { get; }
        
        public GameOverEventArgs(PieceColor winner)
        {
            Winner = winner;
        }
    }
}
