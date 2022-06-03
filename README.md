# Depth Charts
- This is a project that simulates the depth charts of the NFL with some usecases implemented to manipulate the players on the position of the chart.
- The project is developed on Visual Studio 2017 and .Net Framework 4.6.1

## Structures
The project contains 3 parts.
- Data Models: see details below
- Data Models Unit Tests: all the test cases to cover the interfaces and motheds.
- A WPF Windows applicaiton for the UI to manipulate the players.

## How to build
1. Download the repository
2. Open the solution in Visual Studio
3. Restore Nuget packages (IoC container: Unity)
4. Make sure the startup project set to **DepthChartsApp**
5. Run the solution, the windows application should start

## Data Models
- Player -> **IPlayer**: the class to represent the player model, containing: FirstName, LastName, Number etc properties.
- PositionRankList -> **IPositiaonRankList**: A list to keep the players in order, providing methods to manipulate
  - void AddPlayerToPositionList(IPlayer, positionDepth);
  - IPlayer RemovePlayerFromPositionList(IPlayer);
  - IEnumerable<IPlayer> GetBackups(IPlayer);
  - IEnumerable<IPlayer> GetFullList();
- PositionRankListFactory -> IPositionRankListFactory: A factory to generate the PositionRankList instance. It decouple the dependence of DepthCharts to PositionRankList. The factory is in generic and can provide instance implemented **IPositiaonRankList** and provide it to DepthCharts
- DepthCharts -> **IDepthCharts**: A group of PositionRankLists to keep the data, implemented by a dictionray, with position as key and PositionRankList as value.
- **ISport**: an interface to defined Positions, to feed to the DepthCharts
  - NFL -> **ISport**: provide the positions in NFL
  - Soccer -> **ISport**: provide the positions on the Soccer pitch
- **ITeam**: provides the players list
  - TampaBayBuccaneers -> ITeam: provide the Tampa Bay Buccaneers players
  - RealMadrid -> ITeam: provide the Real Madrid players

## WPF Windows Application
- Assumption:
  - It is only max 5 players showing to rank in the same position. (No restriction in data model, it is only a UI limitation)
  - To development convenience, the window is designed as not resizable.
- Usage:
  - Choose the sport from the dropdown to switch from NFL / Soccer. Once the sport changes, the players in the list and the depth chart (positions) will change coresspondingly.
  - Drag the player name from the players list and drop it on the datagrid cell
    - The player name will show on the dropped cell if the cell is empty. The output window to the below will show the insert log
    - If the cell already contains an player, the dropped player will push the existing player back. The output window will show the insert log
    - Drag the player to the position column has no effect.
    - Drag and drop the same player to the same position list, will pop up and error indication.
  - Select an existing player in the grid:
    - The remove player button text will update with the selected player name. Pressing the button will remove him from the position list. A remove log will show in the output window
    - The Print Backups button will be enabled. Pressing the button will print all the backup players (excluding to the selcted) on the same position, to the output window. Print format is designed as a call back function passed into DepthCharts as an injection. "<NO LIST>" will show if there is no backup players after the selection.
  - Print Full Charts: Pressing the button will print all the position rank list in the output window. Print format is designed as a callback function passed into DepthCharts as an injection.