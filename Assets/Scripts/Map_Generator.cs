using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Map_Generator : MonoBehaviour
{
    #region Vars

    public const int CELLVALUE_EMPTY = 0;
    const int CELLVALUE_MAPBOUNDARY = -1;
    const int CELLVALUE_OBSTACLE = 1;
    const int CELLVALUE_BOMBTOWER = 2;
    const int CELLVALUE_FREEZETOWER = 3;
    public const int CELLVALUE_ENEMYSPAWN = 8;
    public const int CELLVALUE_PLAYERTOWER = 9;

    float gridSize_unit = 1;

    Vector2 mapSize;

    public Vector2 MapSize => mapSize;
    #endregion


    #region Public Methods

    /// <summary>
    /// Generates a map according to the stablished MapLayout.  Returns the path if exists, or null if cannot be found
    /// </summary>
    public Stack<Vector3> GenerateMapGrid(int[,] mapGrid, MapLayout mapLayout)
    {
        mapGrid = SetMapBoundaries(mapGrid, mapLayout.boundariesSize);
        GenerateMap(mapGrid, mapLayout);

        Stack<Vector2Int> pathCoords = new PathFinder().GetMapGrid_Path(mapGrid);

        return pathCoords == null ? null : ConvertPath_ToMapUnits(pathCoords);
    }

    #endregion


    #region Private Methods

    /// <summary>
    /// Generates the map layout (floor + boundaries + obstacles)
    /// </summary>
    void GenerateMap(int[,] mapGrid, MapLayout mapLayout)
    {
        Vector3 gridSize = Vector3.zero;
        Vector3 currentPos = Vector3.zero;

        for (int i = 0; i < mapGrid.GetLength(0); i++)
        {
            for (int j = 0; j < mapGrid.GetLength(1); j++)
            {
                // Floor and surrounds
                if (mapGrid[i, j] <= 0)
                {
                    GameObject go = Instantiate(GetPrefabAssociatedWithGridValue(   mapGrid[i, j], mapLayout), currentPos,
                                                                                    GetPrefabAssociatedWithGridValue(mapGrid[i, j], mapLayout).transform.rotation, 
                                                                                    this.transform);
                    // Set Grid size for the first time
                    gridSize = gridSize == Vector3.zero ? go.GetComponent<BoxCollider>().bounds.size : gridSize;
                }

                // Obstacles
                if (mapGrid[i,j] > 0)
                {
                    // Instantiate Obstacle Floor
                    GameObject go = Instantiate(mapLayout.prefab_obstacleCell, currentPos, mapLayout.prefab_obstacleCell.transform.rotation, this.transform);
                    // Instantiate Obstacle Object, and move it up to proper position
                    GameObject obstacle = Instantiate(GetPrefabAssociatedWithGridValue( mapGrid[i, j], mapLayout), currentPos, 
                                                                                        GetPrefabAssociatedWithGridValue(mapGrid[i, j], mapLayout).transform.rotation, 
                                                                                        this.transform);
                    Vector3 obsPosition = go.transform.position;
                    obsPosition.y += go.GetComponent<BoxCollider>().bounds.size.y/2;
                    obstacle.transform.position = obsPosition;
                }
                currentPos = new Vector3(currentPos.x, currentPos.y, currentPos.z + gridSize.z);
            }
            currentPos = new Vector3(currentPos.x + gridSize.x, currentPos.y, 0);
        }
        // Set gridSize_unit and MapSize
        gridSize_unit = gridSize.x;
        mapSize = new(gridSize_unit * mapGrid.GetLength(0), gridSize_unit * mapGrid.GetLength(1));
    }

    /// <summary>
    /// Adds a layer of boundaries surrounding the map grid according to the boundaries_size setted
    /// </summary>
    int[,] SetMapBoundaries(int[,] mapGrid, int boundariesSize)
    {
        if (boundariesSize <= 0)
            return mapGrid;

        int[,] surroundedMapGrid = new int[mapGrid.GetLength(0) + (2 * boundariesSize), mapGrid.GetLength(1) + (2 * boundariesSize)];

        for (int i = 0; i < surroundedMapGrid.GetLength(0); i++)
        {
            for (int j = 0; j < surroundedMapGrid.GetLength(1); j++)
            {
                // Set cell value according if it's surrounding map grid
                surroundedMapGrid[i, j] = ( i >= boundariesSize && i <= mapGrid.GetLength(0) - 1 + boundariesSize &&        // Top and bot boundaries
                                            j >= boundariesSize && j <= mapGrid.GetLength(1) - 1 +boundariesSize ) ?        // Side boundaries
                                                mapGrid[i - boundariesSize, j - boundariesSize] : CELLVALUE_MAPBOUNDARY;    // Sets cell as boundary or current mapGrid cell value
            }
        }
        return surroundedMapGrid;
    }


    GameObject GetPrefabAssociatedWithGridValue(int val, MapLayout mapLayout)
    {
        switch (val)
        {
            case CELLVALUE_MAPBOUNDARY:
                return mapLayout.prefab_mapBoundaryCell;
            case CELLVALUE_OBSTACLE:
                System.Random random = new();
                return mapLayout.prefab_obstacles[random.Next(0,mapLayout.prefab_obstacles.Length)];
            case CELLVALUE_BOMBTOWER:
                return mapLayout.prefab_bombTower;
            case CELLVALUE_FREEZETOWER:
                return mapLayout.prefab_freezeTower;
            case CELLVALUE_ENEMYSPAWN:
                return mapLayout.prefab_enemySpawn;
            case CELLVALUE_PLAYERTOWER:
                return mapLayout.prefab_playerTower;
            default:
                return mapLayout.prefab_pathCell;
        }
    }

    /// <summary>
    /// Given a Queue of Vector2 grid coords, returns a Stack of Vector3 map coords.
    /// </summary>
    Stack<Vector3> ConvertPath_ToMapUnits(Stack<Vector2Int> pathCoords)
    {
        Stack<Vector3> mapUnitsPath = new();
        while (pathCoords.Count > 0)
        {
            Vector2Int gridCoords = pathCoords.Pop();
            mapUnitsPath.Push(new(gridCoords.x * gridSize_unit, 1, gridCoords.y * gridSize_unit));
        }
        return mapUnitsPath;
    }

    #endregion
}
