using System.Collections.Generic;
using UnityEngine;

public class FishPool
{
    private GameObject _prefab;
    private Transform _parent;
    private Queue<PooledFish> _pool = new();
    private List<PooledFish> _activeFish = new();

    public FishPool(GameObject prefab, int initialSize, Transform parent)
    {
        _prefab = prefab;
        _parent = parent;

        for (int i = 0; i < initialSize; i++)
        {
            CreateNewFish();
        }
    }

    private void CreateNewFish()
    {
        GameObject obj = GameObject.Instantiate(_prefab, _parent);
        obj.SetActive(false);
        var fish = obj.GetComponent<PooledFish>();
        _pool.Enqueue(fish);
    }

    public PooledFish GetFish()
    {
        if (_pool.Count == 0)
        {
            CreateNewFish();
        }
        
        var fish = _pool.Dequeue();
        _activeFish.Add(fish);
        return fish;

    }

    public void ReturnFish(PooledFish fish)
    {
        fish.Deactivate();
        _activeFish.Remove(fish);
        _pool.Enqueue(fish);
    }

    public void ReturnAllFish()
    {
        foreach (var fish in new List<PooledFish>(_activeFish))
        {
            ReturnFish(fish);
        }
        _activeFish.Clear();
    }
    
    public void FreezeAllFish()
    {
        foreach (var fish in new List<PooledFish>(_activeFish))
        {
            fish.Freeze();
        }
    }
    
    public void UnfreezeAllFish()
    {
        foreach (var fish in new List<PooledFish>(_activeFish))
        {
            fish.Unfreeze();
        }
    }
}
