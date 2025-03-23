using UnityEngine;

public class Spawner : MonoBehaviour
{
    public GameObject[] spawnMarkers;
    private Transform[] _blueMarkers;
    private Transform[] _pinkMarkers;
    
    public GameObject[] bluePrefabs;
    public GameObject[] pinkPrefabs;

    public float minSpawnTime;
    public float maxSpawnTime;
    
    private float[] _blueTimes;
    private float[] _pinkTimes;
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        if (spawnMarkers[0] != null && spawnMarkers[1] != null)
        {
            _blueMarkers = new Transform[] { spawnMarkers[0].transform, spawnMarkers[1].transform };
        }

        if (spawnMarkers[2] != null && spawnMarkers[3] != null)
        {
            _pinkMarkers = new Transform[] { spawnMarkers[2].transform, spawnMarkers[3].transform }; 
        }
        
        _blueTimes =  new float[_blueMarkers.Length];
        _pinkTimes =  new float[_pinkMarkers.Length];
        
        for (int i = 0; i < _blueTimes.Length; i++)
        {
            _blueTimes[i] = CreateTimeTillNextSpawn();
        }

        for (int i = 0; i < _pinkTimes.Length; i++)
        {
            _pinkTimes[i] = CreateTimeTillNextSpawn();
        }
    }

    // Update is called once per frame
    void Update()
    {
        // Blue Timers
        CheckTimers(_blueTimes,_blueMarkers, bluePrefabs);
        
        // Pink Side
        CheckTimers(_pinkTimes,_pinkMarkers, pinkPrefabs);
    }

    private void CheckTimers(float[] times, Transform[] markers, GameObject[] prefabs)
    {
        for (int i = 0; i < times.Length; i++)
        {
            times[i] -= Time.deltaTime;
            
            if (times[i] <= 0)
            {
                Instantiate(ChooseRandomPrefab(prefabs), ChooseRandomSpawnPosition(markers), Quaternion.identity);
                times[i] = CreateTimeTillNextSpawn();
            }
        }
    }
    private float CreateTimeTillNextSpawn()
    {
        return Random.Range(minSpawnTime, maxSpawnTime);
    }

    private static GameObject ChooseRandomPrefab(GameObject[] prefabs)
    {
        return prefabs[Random.Range(0, prefabs.Length)];
    }

    private static Vector3 ChooseRandomSpawnPosition(Transform[] markers)
    {
        return markers[Random.Range(0, markers.Length)].position;
    }
}
