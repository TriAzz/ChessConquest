# Chess Conquest GUI

This is a Windows Forms-based GUI version of the Chess Conquest game. The application allows players to interact with the chess board through a graphical interface, clicking and dragging pieces to make moves. The game also features an overworld campaign map where players can conquer territories through chess battles.

## Features

- Graphical chess board with interactive pieces
- Play against an AI opponent with adjustable difficulty levels
- Visual highlighting of selected pieces and valid moves
- Game status display showing current turn and game state
- Option to play as either White or Black
- Support for all standard chess pieces plus the custom Vanguard piece
- Strategic overworld campaign map with territory conquest
- Multiple factions competing for control of the world
- Detailed turn-by-turn battle notifications

## Overworld Campaign

The overworld campaign adds a strategic layer to Chess Conquest:

- **Territory Control**: Conquer territories by winning chess matches against opposing factions
- **Faction Warfare**: Lead your faction (Iron Legion by default) to dominate the map
- **Turn-Based Strategy**: Plan your conquests carefully as you can only attack once per turn
- **Connected World**: The map generates as one connected landmass with unique territory names
- **Faction Regions**: Each faction's territories are grouped together in contiguous regions
- **Victory Conditions**: Achieve victory by conquering all territories on the map

### Factions

The world is divided between five competing factions:

- **Iron Legion** (Blue) - The player's faction
- **Crimson Order** (Red)
- **Emerald Covenant** (Green)
- **Golden Dynasty** (Yellow)
- **Shadow Collective** (Gray)

### Battle System

The game features a comprehensive battle system:

- **Defender Advantage**: In all battles, the defending faction plays as White in chess games
- **One Attack Per Turn**: Each faction (including AI factions) can only make one attack per turn
- **AI vs AI Battles**: When AI factions battle each other, the outcome is determined by a 50/50 chance
- **Player Battles**: When the player is involved, battles are resolved through chess games
- **Battle Notifications**: Detailed messages inform you about each battle's participants and outcome
- **Territory Conquest**: Winning a battle as the attacker lets you take control of the territory

### AI Turn Mechanics

The AI factions follow a structured turn sequence:

- **Turn Announcement**: Each AI faction's turn begins with an announcement
- **Target Selection**: The AI selects a random adjacent enemy territory to attack
- **Attack Notification**: You're informed which territory is being attacked and who owns it
- **Battle Resolution**: AI vs AI battles are resolved with RNG, while AI vs Player battles use chess games
- **Outcome Notification**: After each battle, you're informed of the result and any territory changes

### Dev Mode

For testing and quick play, the game includes a Dev Mode option:
- Enable Dev Mode from the overworld control panel
- When attacking or defending in Dev Mode, you can choose to automatically win or lose battles
- This allows for faster exploration of the campaign mechanics without playing full chess matches

## How to Build and Run

1. Open the solution in Visual Studio
2. Ensure you have .NET 8.0 SDK installed
3. Build the solution (Ctrl+Shift+B)
4. Run the application (F5)

## Game Controls

### Chess Board
- Click on a piece to select it
- Valid moves will be highlighted in green
- Click on a highlighted square to move the selected piece
- Use the control panel to:
  - Start a new game
  - Choose your color (White or Black)
  - Set the AI difficulty level
  - Quit the application

### Overworld Map
- Click on territories to select them
- Attack adjacent enemy territories to expand your faction's control
- End your turn to allow AI factions to make their moves
- Monitor faction status in the control panel
- Enable Dev Mode for quick testing and gameplay

## Implementation Details

The GUI application uses the same core chess logic as the console version, but wraps it in a Windows Forms interface for better user interaction. The main components are:

- **MainForm**: The primary UI component that displays the menu options
- **ChessGameForm**: Handles the chess board display and gameplay
- **OverworldMapForm**: Manages the campaign map and faction interactions
- **Game**: The core game logic, handling moves, turns, and game state
- **Board**: Manages the chess board state and piece positions
- **Piece classes**: Define the behavior of each chess piece type
- **Overworld**: Manages the campaign map state and faction territories
- **Territory**: Represents a conquerable region on the map
- **Faction**: Represents a competing power in the campaign

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
