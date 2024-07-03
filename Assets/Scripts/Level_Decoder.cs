using UnityEngine;
using System.Text.RegularExpressions;

public struct Level
{
    public int[,] mapGrid;
    public int[,] enemyWaves;
}

/// <summary>
/// Decodes a Level (map grid and enemy waves ) from a text asset
/// </summary>
public class Level_Decoder
{
    char MAP_GRID_DELIMITATOR= '#';
    char MAP_ENEMYWAVE_DELIMITATOR= ' ';

    /// <summary>
    /// Decodes a Map from the text asset  !! IF ERROR PARSING DO SOMETHING!!
    /// </summary>
    public Level DecodeMapFromFile(TextAsset txtLevel)
    {
        string decodedLevelText = txtLevel.text;
        Level decodedLevel = new();

        if (IsMapFileOK(decodedLevelText))
        {
            decodedLevel.mapGrid = GetMapGrid(decodedLevelText);
            decodedLevel.enemyWaves = GetMapEnemyWaves(decodedLevelText);
        }

        return decodedLevel;
    }

    /// <summary>
    /// Returns False if map file is not correct
    /// </summary>
    bool IsMapFileOK(string decodedLevel)
    {
        // General checks
        // Check for any char outside 0-9 a part from spaces, # and breakLines
        if (!Regex.IsMatch(decodedLevel, @"^[\d\n" + MAP_GRID_DELIMITATOR + MAP_ENEMYWAVE_DELIMITATOR + "]+$"))
        {
            LogHandler.LogError("Map file contains a wrong set of characters.");
            return false;
        }

        // Map Grid Checks
        // Check if grid-enemyWaves delimitator exists, and if there's only one
        if (decodedLevel.Split(MAP_GRID_DELIMITATOR).Length != 2)
        {
            LogHandler.LogError($"Map file contains 0 or more than 1 Grid-Waves delimitator {MAP_GRID_DELIMITATOR}.");
            return false;
        }
        // Check if have a grid (at least to fit start and end)
        if (decodedLevel.IndexOf(MAP_GRID_DELIMITATOR) < 2)
        {
            LogHandler.LogError($"Map file contains a Grid too small.");
            return false;
        }

        // Wave Checks
        // Check if have enemy waves (at least more characters after enemywaves delimitator)
        if (decodedLevel.IndexOf(MAP_ENEMYWAVE_DELIMITATOR) >= decodedLevel.Length)
        {
            LogHandler.LogError($"Map file doesn't contains Enemy Waves.");
            return false;
        }

        return true;
    }

    /// <summary>
    /// Decodes Map Grid from decodedMap
    /// </summary>
    int[,] GetMapGrid(string decodedMap)
    {
        // Get Map Grid info
        string mapGrid = decodedMap.Substring(0, decodedMap.IndexOf(MAP_GRID_DELIMITATOR));
        // Set the 2DArray
        int arrayHeight = mapGrid.Split('\n').Length - 1;
        int arrayWidth = mapGrid.IndexOf('\n');
        int[,] int2DArray = new int[arrayHeight, arrayWidth];
        // Fill the array
        int charCount = 0;
        for (int i = arrayHeight - 1; i >= 0; i--)
        {
            for (int j = arrayWidth - 1; j >= 0; j--)
            {
                // Set the value if it's a int
                if (int.TryParse(mapGrid[charCount++].ToString(), out int num))
                    int2DArray[i, j] = num;
                else
                {
                    LogHandler.LogError($"An error ocurred while parsing Level Map Grid, '{mapGrid[charCount-1]}' cannot be parsed to a int, maybe Grid is not simetric.");
                    return null;
                }
            }
            // Jump breakline
            ++charCount;
        }
        return int2DArray;
    }

    /// <summary>
    /// Decodes EnemyWaves from decoded Map
    /// </summary>
    int[,] GetMapEnemyWaves(string decodedMap)
    {
        // Get Map Grid info
        string mapGrid = decodedMap.Substring(decodedMap.IndexOf(MAP_GRID_DELIMITATOR) + 2, decodedMap.Length - (decodedMap.IndexOf(MAP_GRID_DELIMITATOR) + 2));
        string[] rows = mapGrid.Split('\n');
        // Set the 2DArray
        // If only one Wave, needs a special case
        int splitIndexer = mapGrid.Contains('\n') ? mapGrid.IndexOf('\n') : mapGrid.Length - 1;
        int arrayWidth = mapGrid.Substring(0, splitIndexer).Split(MAP_ENEMYWAVE_DELIMITATOR).Length;
        int[,] int2DArray = new int[rows.Length, arrayWidth];
        // Fill the array
        for(int i = 0; i < rows.Length; i++)
        {
            string[] numsThisRow = rows[i].Split(new char[] { MAP_ENEMYWAVE_DELIMITATOR, '\n' }, System.StringSplitOptions.RemoveEmptyEntries);
            // Avoid tracking empty rows (if breakline at the end of the file, etc..)
            if (numsThisRow.Length == 0)
                continue;
            // Error Handling when Waves array is not simetric, or constant on the number of columns per row
            if(numsThisRow.Length != int2DArray.GetLength(1))
            {
                LogHandler.LogError($"An error ocurred while parsing Level Enemy Waves, array is not simetric, please set the same rows of enemies for each wave, or remove spaces at the end of the rows.");
                return null;
            }
            for(int j = 0; j < numsThisRow.Length; j++)
            {
                if (int.TryParse(numsThisRow[j], out int num))
                    int2DArray[i, j] = num;
                else // Error handling
                {
                    LogHandler.LogError($"An error ocurred while parsing Level Enemy Waves, '{numsThisRow[j]}' cannot be parsed to a int.");
                    return null;
                }
            }
        }
        return int2DArray;
    }

}
