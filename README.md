# Battleships
A game of Battleships made with c#! 

## Features

-[x] choose the size of the board (1 - 5 grid)
-[x] choose a number of ships (1 - size of the board)
-[x] ships are randomly placed on the board with a random orientation
![image](https://github.com/n0laja00/Battleships/assets/73889850/eb522cea-4644-453a-bd36-32004cdef500)

### Run through
Ships are placed randomly on the board with random orientations. You as the captain can shoot any cell you wish on the grid. 
Upon shooting, you will either hit or miss. After this, the game will print the remaining number of enemy ships, regardless if it was a hit or miss. 
The game will also calculate all the possible placements of the ships on board, excluding the ships whose position is known.
If you hit a ship 2 times, the game will inform you if the ship you shot is vertically or horizontally aligned.

Example of a horizontally aligned.
![image](https://github.com/n0laja00/Battleships/assets/73889850/616cf890-820e-4ac7-af1f-91e1463bad1f)

When there are no more unknown valid placements left, as in the player knows the locations of all the ships, the player will be informed.
![image](https://github.com/n0laja00/Battleships/assets/73889850/4a25e0d1-bb79-4163-8635-cb589b5992ee)


The game will end when there are no more ships left.
