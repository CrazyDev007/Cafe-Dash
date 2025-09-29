using UnityEngine;

public class CoffeeMachine : MonoBehaviour
{
    public GameObject cupPrefab;
    public float processTime = 2f;
    
    private ICoffeeBrewingService _brewingService;
    
    public void SetBrewingService(ICoffeeBrewingService service)
    {
        _brewingService = service;
    }
    
    private void Awake()
    {
        // Service will be injected by composition root
    }
    
    public void InsertBean(Transform cupHolder = null)
    {
        if (_brewingService != null)
        {
            _brewingService.StartBrewingProcess(cupHolder);
        }
        else
        {
            Debug.LogWarning("CoffeeMachine: Brewing service not injected!");
        }
    }
    
    public bool HasCup()
    {
        return _brewingService != null && _brewingService.HasReadyCup;
    }
    
    public GameObject TakeCup()
    {
        return _brewingService != null ? _brewingService.TakeReadyCup() : null;
    }
}
