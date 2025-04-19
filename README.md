# Chess Conquest GUI

This is a Windows Forms-based GUI version of the Chess Conquest game. The application allows players to interact with the chess board through a graphical interface, clicking and dragging pieces to make moves. The game also features an overworld campaign map where players can conquer territories through chess battles.

## Features

- Graphical chess board with interactive pieces
- Play against an AI opponent with adjustable difficulty levels
- Visual highlighting of selected pieces and valid moves
- Game status display showing current turn and game state
- Option to play as either White or Black
- Support for all standard chess pieces plus custom pieces like Cavalry and Vanguard
- Strategic overworld campaign map with territory conquest
- Multiple factions competing for control of the world
- Faction selection screen with descriptions before starting a campaign
- Detailed turn-by-turn battle notifications

## Overworld Campaign

The overworld campaign adds a strategic layer to Chess Conquest:

- **Territory Control**: Conquer territories by winning chess matches against opposing factions
- **Faction Warfare**: Choose your faction at the start of the campaign and lead it to dominate the map
- **Turn-Based Strategy**: Plan your conquests carefully as you can only attack once per turn
- **Connected World**: The map generates as one connected landmass with unique territory names
- **Faction Regions**: Each faction's territories are grouped together in contiguous regions
- **Victory Conditions**: Achieve victory by conquering all territories on the map

### Factions

The world is divided between five competing factions, each with its own theme and description:

- **Iron Legion** (Blue): A disciplined army known for its resilience and heavy armor
- **Crimson Order** (Red): A zealous force that excels in aggressive tactics and rapid strikes with their cavalry
- **Emerald Covenant** (Green): Nature-bound warriors with cunning strategies and adaptability
- **Golden Dynasty** (Yellow): A noble faction with a focus on wealth and influence
- **Shadow Collective** (Gray): Masters of subterfuge and deception

At the start of a campaign, you select which faction you want to play as. A description for each faction is shown in the selection screen.

### Unique Army Compositions

Each faction has its own army setup:

- **Standard Army**: Uses the classic chess piece arrangement with standard pieces
- **Iron Legion**: Uses two Vanguards in the center of their front line (replacing the middle pawns), giving them additional defensive capability
- **Crimson Order**: Replaces Knights with Cavalry pieces and also replaces two pawns in front of bishops with Cavalry, giving them a total of 4 Cavalry units for enhanced mobility

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
- **FactionSelectionForm**: Lets you choose your faction before starting a campaign
- **Game**: The core game logic, handling moves, turns, and game state
- **Board**: Manages the chess board state and piece positions
- **Piece classes**: Define the behavior of each chess piece type
- **Overworld**: Manages the campaign map state and faction territories
- **Territory**: Represents a conquerable region on the map
- **Faction**: Represents a competing power in the campaign

## Custom Pieces

The game includes custom pieces that different factions may use:

### Cavalry
- Moves like a Knight (L-shape)
- Used exclusively by the Crimson Order faction
- Replaces Knights in the back row and two pawns in front of bishops
- Gives the Crimson Order a unique cavalry-focused army composition

### Vanguard
- Moves diagonally 1 square or horizontally/vertically up to 2 squares
- Used by the Iron Legion faction to replace the middle pawns
- Provides enhanced defensive capability on the front line

## System Requirements

- Windows operating system
- .NET 8.0 Runtime
- Minimum 4GB RAM recommended
- 100MB free disk space
