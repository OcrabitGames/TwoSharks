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
    [SerializeField] private float minSpeed = 5f;
    [SerializeField] private float maxSpeed = 7f;
    [SerializeField] private float highLionfishCooldown = 1.5f;
    [SerializeField] private float lowLionfishCooldown = .6f;
    private float _lionfishCooldown;
    [SerializeField] private float highMinnowCooldown = 1f;
    [SerializeField] private float lowMinnowCooldown = .15f;
    private float _minnowCooldown;
    [SerializeField] private float highMinSpawnTime = 1f;
    [SerializeField] private float lowMinSpawnTime = .5f;
    private float _minSpawnTime;
    [SerializeField] private float highMaxSpawnTime = 3f;
    [SerializeField] private float lowMaxSpawnTime = .9f;
    private float _maxSpawnTime;
    [SerializeField] private int scoreTopline = 500;

    private const float BaseSpeed = 5f;
    private float _currentSpeed;

    private float[] _blueTimers;
    private float[] _pinkTimers;
    
    private float _blueLionfishCooldown;
    private float _pinkLionfishCooldown;
    private float _blueMinnowCooldown;
    private float _pinkMinnowCooldown;
    
    public bool fishActive;
    
    private FishPool _blueMinnowPool;
    private FishPool _pinkMinnowPool;
    private FishPool _blueLionfishPool;
    private FishPool _pinkLionfishPool;

    private void UpdateTimes()
    {
        var progressionRate = Mathf.Clamp01((float)GameManager.Instance.GetScore() / scoreTopline);
        
        _currentSpeed = Mathf.Lerp(minSpeed, maxSpeed, progressionRate);
        var adjustmentFactor = BaseSpeed / _currentSpeed;
        
        _lionfishCooldown = Mathf.Lerp(highLionfishCooldown, lowLionfishCooldown, progressionRate) * adjustmentFactor;
        _minnowCooldown = Mathf.Lerp(highMinnowCooldown, lowMinnowCooldown, progressionRate) * adjustmentFactor;
        _maxSpawnTime = Mathf.Lerp(highMaxSpawnTime, lowMaxSpawnTime, progressionRate) * adjustmentFactor;
        _minSpawnTime = Mathf.Lerp(highMinSpawnTime, lowMinSpawnTime, progressionRate) * adjustmentFactor;
    }
    
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

        UpdateTimes();
        
        _blueLionfishCooldown -= Time.deltaTime;
        _pinkLionfishCooldown -= Time.deltaTime;
        _blueMinnowCooldown -= Time.deltaTime;
        _pinkMinnowCooldown -= Time.deltaTime;
        
        UpdateSharedTimers(_blueTimers, blueMarkers, _blueMinnowPool, _blueLionfishPool, ref _blueLionfishCooldown, ref _blueMinnowCooldown);
        UpdateSharedTimers(_pinkTimers, pinkMarkers, _pinkMinnowPool, _pinkLionfishPool, ref _pinkLionfishCooldown, ref _pinkMinnowCooldown);
    }
    
    // ReSharper disable Unity.PerformanceAnalysis
    private void UpdateSharedTimers(float[] timers, Transform[] markers, FishPool minnowPool, FishPool lionfishPool, ref float lionfishCooldown, ref float minnowCooldown)
    {
        for (int i = 0; i < timers.Length; i++)
        {
            timers[i] -= Time.deltaTime;
            if (timers[i] <= 0f)
            {
                bool canSpawnLionfish = lionfishCooldown <= 0f;
                bool canSpawnMinnow = minnowCooldown <= 0f;
                bool spawnMinnow;

                if (canSpawnLionfish && canSpawnMinnow)
                {
                    spawnMinnow = Random.value <= 0.5f;
                }
                else if (canSpawnLionfish)
                {
                    spawnMinnow = false;
                }
                else if (canSpawnMinnow)
                {
                    spawnMinnow = true;
                }
                else
                {
                    // Neither type is ready, so reset timer and skip spawning
                    timers[i] = RandomSpawnTime();
                    continue;
                }

                FishPool pool = spawnMinnow ? minnowPool : lionfishPool;
                var fish = pool.GetFish();
                Vector3 spawnPos = markers[i].position;
                fish.Activate(spawnPos, _currentSpeed);

                if (spawnMinnow)
                {
                    minnowCooldown = _minnowCooldown;
                }
                else
                {
                    lionfishCooldown = _lionfishCooldown;
                }

                timers[i] = RandomSpawnTime();
            }
        }
    }
    
    private float RandomSpawnTime()
    {
        return Random.Range(_minSpawnTime, _maxSpawnTime);
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
