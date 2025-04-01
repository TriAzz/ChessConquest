# Chess Conquest GUI

This is a Windows Forms-based GUI version of the Chess Conquest game. The application allows players to interact with the chess board through a graphical interface, clicking and dragging pieces to make moves.

## Features

- Graphical chess board with interactive pieces
- Play against an AI opponent with adjustable difficulty levels
- Visual highlighting of selected pieces and valid moves
- Game status display showing current turn and game state
- Option to play as either White or Black
- Support for all standard chess pieces plus the custom Vanguard piece

## How to Build and Run

1. Open the solution in Visual Studio
2. Ensure you have .NET 8.0 SDK installed
3. Build the solution (Ctrl+Shift+B)
4. Run the application (F5)

## Game Controls

- Click on a piece to select it
- Valid moves will be highlighted in green
- Click on a highlighted square to move the selected piece
- Use the control panel to:
  - Start a new game
  - Choose your color (White or Black)
  - Set the AI difficulty level
  - Quit the application

## Implementation Details

The GUI application uses the same core chess logic as the console version, but wraps it in a Windows Forms interface for better user interaction. The main components are:

- **MainForm**: The primary UI component that displays the chess board and game controls
- **Game**: The core game logic, handling moves, turns, and game state
- **Board**: Manages the chess board state and piece positions
- **Piece classes**: Define the behavior of each chess piece type

## Custom Pieces

The game includes the standard chess pieces plus a custom "Vanguard" piece that:
- Can move diagonally 1 square
- Can move horizontally up to 2 spaces
- Can move vertically up to 2 spaces
- Replaces the middle pawns in the standard setup
- Is represented by diamond symbols (♢ for white and ♦ for black)

## System Requirements

- Windows operating system
- .NET 8.0 Runtime
- Minimum 4GB RAM recommended
- 100MB free disk space
