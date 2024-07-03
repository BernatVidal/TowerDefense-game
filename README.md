# TowerDefense-game
A Unity Tower defense simple game. Develop within a weekend following the Ubisoft Barcelona GamePlayProgrammer test.
Ready to be played on PC and Mobile.

An approach to Tower Defense GPP Ubisoft’s Test by Bernat V.

My strategy for tackling this test was formulated with a keen awareness of the time constrictions. I focused on optimizing the overall game performance without compromising on the feel or details.
The main architecture is planned so it would be really easy to expand it to a big variety of levels, enemies, towers, and projectiles without almost any code.
Everything was designed with the aim of scalability and modularity.

I hope you like it!

How it works

Setup

The scene is composed of a Game Manager, a UI Manager, an Audio Manager, the parent of the Map Grid, and the pool parents and managers of Enemies and Projectiles.

The Game Settings define the Level we want to load, the Map Layout (or Map appearance) we want to see, the enemies that will be used, and the total player Lives.
With this information, the game manager will load the Map, the game's main settings, and their dependencies from Start().

![Game Setup](https://github.com/BernatVidal/TowerDefense-game/blob/main/Screenshots/editor_setup.png)


First, the map's text file is decoded so the Level information is extracted (Map grid and Enemy waves).
During the process, it detects some possible errors/corner cases on the file and, if one of them is detected,  the application will be stopped and will show information about what happened and how to solve it via the custom logger LogHandler class.
After this process, the map is generated. Using the prefabs on the Map_Layout scriptable object we selected, it will use the map grid to generate the desired map and return the Path to solve it.

![MapText](https://github.com/BernatVidal/TowerDefense-game/blob/main/Screenshots/levelText.png)

(Example of a level layout definition. First level grid with the path, walls and towers, and then the spawn sequence (according to test specs)).

The pathfinding algorithm used is a simple Depth First Search algorithm. As the types of maps used in Tower Defense usually are narrow single-unit paths, I implemented a simple DFS algorithm that doesn't keep track of the best path (implementing a score system), as when the path is found it will probably be the only path to the End Tile. A more robust way could be easily implemented, but the complexity and process consumption of it would easily grow.
If any error occurs during the process it will also stop the application and use the custom logger to show what happened.

Once the Map is created, a camera controller will set the camera in the desired position and zoom in/out depending on the map size. If the map is portrait it will also rotate itself counterclockwise to adapt to a landscape set.

Then the main object manager classes are injected with their main dependencies and start to pool their objects, so no performance spikes will happen when the poolable objects are instantiated for the first time.

From here, the Game manager will be listening for some main events from the Game Events class, to keep track of game status and give feedback through UI Manager.


- Game -

![gameplay_example](https://github.com/BernatVidal/TowerDefense-game/blob/main/Screenshots/gameplay1.png)

Enemies Manager

During the game, the Enemies Manager will spawn enemies according to the Waves decoded from the map text file. When the last enemy is despawned, it will spawn the following one. When the last wave ends, the Game Completed event is called.

The enemy dependencies are injected from Enemies manager, and they are pooled or unpooled according to it.
Also, due to performance reasons, the Update method of the manager will call the methods responsible for the enemies' live cycle, taking advantage of the method DoStuff() from the IPoolable interface they implement.


Enemies / Enemy Controller

The enemies are spawned with the path they must follow using the utils static class MoveToTarget and will react to their environment according to the projectiles and the End Point or Player Tower. 

They are defined with Enemy_Data scriptable objects, where their total hp, velocity, prefab, animations, and dead sound are indicated.

They also trigger events when they attack or die, so others can listen to these events without knowing about their existence.

![Enemies](https://github.com/BernatVidal/TowerDefense-game/blob/main/Screenshots/enemyController_ScriptableObj.png)


Towers / Tower Controller

Towers are defined and set while the map is generated. They use Tower_Data scriptable objects to set the detection radius, attack recovery (time between attacks in seconds), their prefab, the scriptable object Projectile_data they shoot, their animations, and their Shoot soundFX.

As soon as their Trigger detects an enemy, it will target and shoot it.
The shoot process involves a ShootRequest method that triggers an event that will spawn and shoot a projectile in the desired position.
So the towers are not responsible for the projectiles they shoot.

![Towers](https://github.com/BernatVidal/TowerDefense-game/blob/main/Screenshots/tower_scriptableObj.png)


Projectiles Manager

Projectiles Manager is pretty similar to Enemies Manager but with Projectiles.
They pool and un-pool projectiles, inject their dependencies and Shoot them when a request shoot event is triggered by a tower.


Projectiles / Projectile Controller

Projectiles are defined by the Projectiles Data scriptable object.

From it, projectiles obtain their damage to units, the attack radius, the attack duration, the projectile velocity, if they have to follow the enemy after the impact (notice the mistype of Player instead Enemy on the script), their prefab, and the sound they produce when explodes.

They follow the targeted enemy using the utils static class MoveToTarget, as Enemies does.
If the target dies in the process, the projectile will despawn.

![Projectiles](https://github.com/BernatVidal/TowerDefense-game/blob/main/Screenshots/projectileBomb_scriptableObj.png)


Audio Manager

Note: The sounds used on this project are a mix of sounds created by myself during my game dev career, and extracted from some arcade classic games.

The Audio Manager is a singleton class pretty easy to implement and use. 
The main sound parameters can be set pretty easily, and it will generate an object with a sound for every sound defined when this is instantiated so objects don’t need to implement their own sound and just need to call for play sound with an enum.

It’s a pretty solid manager for small projects like this one, with not too many sounds or scenes, as sounds are defined with an enum to provide agility on the setup and a straightforward error-proof system on the sound definition and call.

The volumes are also controlled by a Mixer having its master, and Music and SFX channels on it, so it could be easily configurable from an in-game menu.

![Audio Manager](https://github.com/BernatVidal/TowerDefense-game/blob/main/Screenshots/audioManager.png)


Gameplay screenshots

The game uses basic Post Processing processes to improve the overall look and feel without compromising the performance.

![gameplay 2](https://github.com/BernatVidal/TowerDefense-game/blob/main/Screenshots/gameplay2.png)

![Level completed](https://github.com/BernatVidal/TowerDefense-game/blob/main/Screenshots/levelCompleted.png)

[![Game Over](https://github.com/BernatVidal/TowerDefense-game/blob/main/Screenshots/gameOver.png)

