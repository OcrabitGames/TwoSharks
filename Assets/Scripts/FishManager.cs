using UnityEditor.Tilemaps;
using UnityEngine;

// Note this script was inspired by ChatGPT, I learned about headers and some more about Instance management in Unity

public class FishManager : MonoBehaviour
{
    public static FishManager Instance { get; private set; }
    
    // Okay just learned headers are a thing like dope
    [Header("Spawn Markers")]
    public Transform[] blueMarkers;
    public Transform[] pinkMarkers;
    
    [Header("Prefabs")]
    public GameObject blueLionfishPrefab;
    public GameObject pinkLionfishPrefab;
    public GameObject blueMinnowPrefab;
    public GameObject pinkMinnowPrefab;
    
    [Header("Pool Settings")]
    public int poolSize = 10;
    public float minSpawnTime = 1f;
    public float maxSpawnTime = 3f;
    public float lionfishCooldownDuration = 2f;
    public float speed = 3f;
    
    private float[] _blueTimers;
    private float[] _pinkTimers;
    
    private float _blueLionfishCooldown = 0f;
    private float _pinkLionfishCooldown = 0f;
    
    public bool fishActive = false;
    
    private FishPool _blueMinnowPool;
    private FishPool _pinkMinnowPool;
    private FishPool _blueLionfishPool;
    private FishPool _pinkLionfishPool; 
    
    
    void Awake()
    {
        Instance = this;
    }
    
    // Here I start to create the pools pun intended. Basically I use this to recycle objects instead of constantly instantiating more.
    void Start()
    {
        _blueMinnowPool = new FishPool(blueMinnowPrefab, poolSize, transform);
        _pinkMinnowPool = new FishPool(pinkMinnowPrefab, poolSize, transform);
        _blueLionfishPool = new FishPool(blueLionfishPrefab, poolSize, transform);
        _pinkLionfishPool = new FishPool(pinkLionfishPrefab, poolSize, transform);
        
        _blueTimers = new float[blueMarkers.Length];
        _pinkTimers = new float[pinkMarkers.Length];
        
        for (int i = 0; i < _blueTimers.Length; i++)
        {
            _blueTimers[i] = RandomSpawnTime() + 0.5f*i;
        }

        for (int i = 0; i < _pinkTimers.Length; i++)
        {
            _pinkTimers[i] = RandomSpawnTime() + 0.5f*i;
        }
    }
    // Update is called once per frame
    private void Update()
    {   
        if (!fishActive) return;
        
        _blueLionfishCooldown -= Time.deltaTime;
        _pinkLionfishCooldown -= Time.deltaTime;
        
        UpdateSharedTimers(_blueTimers, blueMarkers, _blueMinnowPool, _blueLionfishPool, ref _blueLionfishCooldown);
        UpdateSharedTimers(_pinkTimers, pinkMarkers, _pinkMinnowPool, _pinkLionfishPool, ref _pinkLionfishCooldown);
    }
    
    // ReSharper disable Unity.PerformanceAnalysis
    private void UpdateSharedTimers(float[] timers, Transform[] markers, FishPool minnowPool, FishPool lionfishPool, ref float lionFishCooldown)
    {
        for (int i = 0; i < timers.Length; i++)
        {
            timers[i] -= Time.deltaTime;
            if (timers[i] <= 0f)
            {
                bool canSpawnLionfish = lionFishCooldown <= 0f;
                bool spawnMinnow = Random.value <= 0.5f;
                
                bool shouldSpawnMinnow = !canSpawnLionfish || spawnMinnow;
                
                FishPool pool = shouldSpawnMinnow ? minnowPool : lionfishPool;
                var fish = pool.GetFish();
                Vector3 spawnPos = markers[i].position;
                fish.Activate(spawnPos, speed);

                if (!shouldSpawnMinnow)
                {
                    lionFishCooldown = lionfishCooldownDuration;
                }
                timers[i] = RandomSpawnTime();
            }
        }
    }
    
    private float RandomSpawnTime()
    {
        return Random.Range(minSpawnTime, maxSpawnTime);
    }

    public void ReturnFish(PooledFish fish)
    {
        if (fish.isMinnow)
        {
            if (fish.isBlue)
            {
                _blueMinnowPool.ReturnFish(fish);
            }
            else
            {
                _pinkMinnowPool.ReturnFish(fish);
            }
        }
        else
        {
            if (fish.isBlue)
            {
                _blueLionfishPool.ReturnFish(fish);
            }
            else
            {
                _pinkLionfishPool.ReturnFish(fish);
            }
        }
    }

    public void ReturnAllFish()
    {
        _blueMinnowPool.ReturnAllFish();
        _pinkMinnowPool.ReturnAllFish();
        _blueLionfishPool.ReturnAllFish();
        _pinkLionfishPool.ReturnAllFish();
    }

    public void DeactivateFish()
    {
        fishActive = false;
        _blueMinnowPool.FreezeAllFish();
        _pinkMinnowPool.FreezeAllFish();
        _blueLionfishPool.FreezeAllFish();
        _pinkLionfishPool.FreezeAllFish();
    }
    
    public void ActivateFish()
    {
        fishActive = true;
        _blueMinnowPool.UnfreezeAllFish();
        _pinkMinnowPool.UnfreezeAllFish();
        _blueLionfishPool.UnfreezeAllFish();
        _pinkLionfishPool.UnfreezeAllFish();
    }
}
