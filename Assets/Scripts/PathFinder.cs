using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// A simple DFS pathfinder class. Returns the first path find, not the one with the best score.
/// </summary>
public class PathFinder
{
    /// <summary>
    /// Givend a mapGrid, returns its path as a Stack using DeepFirstSearch
    /// </summary>
    /// <returns> May return null if can't find a path </returns>
   public Stack<Vector2Int> GetMapGrid_Path(int[,] mapGrid)
    {
        // Find the start and end points, and track the amount of them
        int amountOfStartPos = 0;
        int amountOfEndPos = 0;
        Vector2Int startPos = new(-1,-1);
        Vector2Int endPos = new(-1,-1);
        for (int i = 0; i < mapGrid.GetLength(0); i++)
        {
            for (int j = 0; j < mapGrid.GetLength(1); j++)
            {
                if (mapGrid[i, j] == Map_Generator.CELLVALUE_ENEMYSPAWN)
                {
                    startPos = new(i, j);
                    ++amountOfStartPos;
                }
                else if (mapGrid[i, j] == Map_Generator.CELLVALUE_PLAYERTOWER)
                {
                    endPos = new(i, j);
                    ++amountOfEndPos;
                }
            }
        }

        // If more than one start or end position, return null
        if (amountOfStartPos != 1 || amountOfEndPos != 1)
        {
            LogHandler.LogError("Path cannot be found, grid have wrong amount of start and/or end positions.");
            return null;
        }

        Stack<Vector2Int> path = new();

        // Solve the maze using depth-first search
        bool[,] visited = new bool[mapGrid.GetLength(0), mapGrid.GetLength(1)];
        if (DFS_Pathfind(mapGrid, visited, startPos, endPos, path))
        {
            return path;
        }
        else
        {
            LogHandler.LogError("Path does not exist in this layout.");
            return null;
        }
    }

    /// <summary>
    /// DFS recursive method, returns true if have unvisited valid cells surrounding. Order L-R-U-D
    /// </summary>
    bool DFS_Pathfind(int[,] mapGrid, bool[,] visited, Vector2Int currPos, Vector2Int endPos, Stack<Vector2Int> path)
    {
        // Check if end is reached
        if (currPos.Equals(endPos))
        {
            path.Push(currPos);
            return true;
        }

        // Check if the current cell is a not a path, start, or has already been visited
        if (  ( mapGrid[currPos.x, currPos.y] != Map_Generator.CELLVALUE_EMPTY && 
                mapGrid[currPos.x, currPos.y] != Map_Generator.CELLVALUE_ENEMYSPAWN ) || 
                visited[currPos.x, currPos.y] )
            return false;

        // Mark the current cell as visited
        visited[currPos.x, currPos.y] = true;

        // Check surrounding cells        
        if ( currPos.x > 0 && DFS_Pathfind(mapGrid, visited, new(currPos.x - 1, currPos.y), endPos, path) ||                           // Left cell
             currPos.x < mapGrid.GetLength(0) - 1 && DFS_Pathfind(mapGrid, visited, new(currPos.x + 1, currPos.y), endPos, path) ||   // Right cell
             currPos.y > 0 && DFS_Pathfind(mapGrid, visited, new(currPos.x, currPos.y - 1), endPos, path) ||                           // Top cell
             currPos.y < mapGrid.GetLength(1) - 1 && DFS_Pathfind(mapGrid, visited, new(currPos.x, currPos.y + 1), endPos, path) )    // Bot cell
        {
            path.Push(currPos);
            return true;
        }

        return false;
    }
}


