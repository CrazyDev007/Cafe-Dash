using UnityEngine;
using System.Collections;

public class CoffeeBrewingService : ICoffeeBrewingService
{
    public bool IsProcessing { get; private set; }
    public bool HasReadyCup => _readyCup != null;
    
    public event System.Action OnBrewingStarted;
    public event System.Action OnBrewingCompleted;
    
    private GameObject _readyCup;
    private readonly GameObject _cupPrefab;
    private readonly float _processTime;
    private readonly MonoBehaviour _coroutineRunner;
    
    public CoffeeBrewingService(GameObject cupPrefab, float processTime, MonoBehaviour coroutineRunner)
    {
        _cupPrefab = cupPrefab;
        _processTime = processTime;
        _coroutineRunner = coroutineRunner;
    }
    
    public void StartBrewingProcess(Transform cupHolder = null)
    {
        if (!IsProcessing && !HasReadyCup)
        {
            if (_cupPrefab != null)
            {
                _coroutineRunner.StartCoroutine(BrewCoffeeCoroutine(cupHolder));
            }
            else
            {
                Debug.LogWarning("CoffeeBrewingService: Missing cupPrefab reference!");
            }
        }
    }
    
    public GameObject TakeReadyCup()
    {
        if (_readyCup != null)
        {
            GameObject cup = _readyCup;
            _readyCup = null;
            return cup;
        }
        return null;
    }
    
    private IEnumerator BrewCoffeeCoroutine(Transform cupHolder)
    {
        IsProcessing = true;
        OnBrewingStarted?.Invoke();
        
        yield return new WaitForSeconds(_processTime);
        
        if (_cupPrefab != null)
        {
            // If we have a cup holder, instantiate directly there, otherwise use machine's position
            Vector3 spawnPos = cupHolder != null ? cupHolder.position : _coroutineRunner.transform.position;
            Transform parent = cupHolder != null ? cupHolder : _coroutineRunner.transform;
            
            _readyCup = Object.Instantiate(_cupPrefab, spawnPos, Quaternion.identity, parent);
            
            if (cupHolder != null)
            {
                _readyCup.transform.localPosition = Vector3.zero;
                _readyCup.transform.localRotation = Quaternion.identity;
                
                if (_readyCup.TryGetComponent<Rigidbody>(out var rb))
                {
                    rb.isKinematic = true;
                }
            }
        }
        
        IsProcessing = false;
        OnBrewingCompleted?.Invoke();
    }
}