using UnityEngine;

[CreateAssetMenu(fileName = "MapLayout" ,  menuName = "MyAssets/MapLayout")]
public class MapLayout : ScriptableObject
{
    public int boundariesSize = 2;
    public GameObject prefab_pathCell;
    public GameObject prefab_mapBoundaryCell;
    public GameObject prefab_obstacleCell;
    public GameObject[] prefab_obstacles;
    public GameObject prefab_enemySpawn;
    public GameObject prefab_playerTower;
    public GameObject prefab_bombTower;
    public GameObject prefab_freezeTower;

    public Sounds.SoundID music;
}
