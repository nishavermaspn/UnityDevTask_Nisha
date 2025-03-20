# UnityDevTask_Nisha
This contains two unity developer tasks -
1- Reconnection in Photon for a Unity Multiplayer Game

Location - In asset folder Task1_Reconnection named folder is present, which includes scene and scripts associated to task-1.

How to Run - In MainMenuScene it contains 3 panels, MainMenuPnl,RoomPnl,ReconnectPnl.When we run the scene at start user can have 2 lobby options to join the specific room.

On clicking any lobbyBtn user will connect to Photon and join the associated lobby and join or create new room (if not available).

Then if user's network will get down, the reconnect pnl will appear and user will try to reconnect the same room in maximumRecoonectAttempts(3) attempts,if he joins successfully in these 3 attempts then roomPnl will appear otherwise player will return to mainMenu again.

2- Turn Based two player Multiplayer board game(tic-tac-toe)

Location- In asset folder Task2_TurnBasedGame named folder is present, which includes scene and scripts associated to task-2.

HowToPlay - In scene folder we have a scene TurnBasedGame, which includes a basic gridUI(3*3) and text for playerTurnIndication or winMessage.

First we have to play the game on one device and same game instance on another device as this is multiplayer game.

When player clicks on any of grid button the gameplay starts and the master player who joins first will be able to make a move first and then turn will switches between both 2 players till any one of them wins.

Its basic tic tac toe gameplay.

If player1 joins and make a move and after making move the second player joins then second player will get the updated state of turn and boardMoves.

This includes basic turn synchronization, validation of moves with win logic.

This doesnot include the handling of disconnection of due to network or reconnections and all,because focus here is on turn sync. 


