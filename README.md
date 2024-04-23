# Procedural-Gen
 
## Scene - SampleScene
Places and offsets boxes based on perlin noise

### Features

- Procedural generation using Perlin noise
- Adjustable parameters for terrain generation (amplitude, frequency, world size)

## Scene - Simple WFC
Unfinnished and unoptimized. Just playing around with Wave Function Collaple to generate mazes and patterns.

### Features

- Procedural generation using wave function collapse
- Adjustable prefabs and relations

## Scene - Simple Perlin Noise
This project demonstrates procedural terrain generation with dynamic object placement in Unity. It utilizes Perlin noise to generate heightmaps for terrain and places objects such as plants, rocks, and underwater features across the terrain.

### Features

- Procedural generation using Perlin noise
- Infinet generation based on player position
- Support for various types of objects (big objects, small objects, underwater objects)
- Adjustable parameters for terrain generation and object placement (amplitude, frequency, world size)

## Scene - Perlin Terrain
Unused.

### Features

- none

## Scene - Mershes
Unused.

### Features

- none

## Scene - Perlin Mesh
This scene demonstrates procedural terrain generation with dynamic object placement. It utilizes Perlin noise to generate heightmaps for terrain and places objects such as plants, rocks, and underwater features across the terrain.

### Features

- Procedural terrain generation using Perlin noise
- Heightmap made from multiple noisemaps
- Seeded generation
- Infinet generation based on player position
- Dynamic object placement based on terrain features
- Support for various types of objects (big objects, small objects, underwater objects)
- Adjustable parameters for terrain generation and object placement (amplitude, frequency, density, etc.)
- Adjustable colors based on height
- Automatic removal of out-of-bounds objects to optimize performance

## Scene - Planar graph no crossings
This scene explores procedural room generation. No rooms overlapp.

After the placement of the rooms connections between them are created. Connections are made by triangulating using Boywer Watsons algorithm for delaunary triangulation. Then finding the minimum spanning tree for the triangulation using Prim's algorithm. Lastly loops are added by iterating over all edges in the triangulation but not in the minimum spanning tree.

### Random

1. A starting room is placed.
2. Rooms of random size are placed in the world.
3. If a room would be placed intersecting annother room the room is regenerated with annother size and position.
4. When all rooms have been placed connections between rooms can be showed by pressing space.

### Features

- Procedural room generation
- Random room size and location
- Loops occur

### Expand

1. Rooms of random size are placed in a circle around the starting room.
2. Then rooms are moved away from the starting room until they no longer overlap.
3. When all rooms have been placed connections between rooms can be showed by pressing space.

### Features

- Procedural room generation
- Random room size and location
- Rooms souround the starting room 
- Loops occur
- Adjustable parameters for terrain generation and object placement (World size, world scale, loop chance, etc.)

### Hallways

- Procedural room generation
- Random room size and location
- Rooms forms hallways
- Loops occur
- Adjustable parameters for terrain generation and object placement (World size, world scale, loop chance, etc.)

### Force
Unfinnished

- Force is applyed to some rooms

### Raymarch
Unfinnished

## Dependencies

- Unity Engine (version 2022.3.18f1)
