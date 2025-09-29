using UnityEngine;
using Game.Application;
using Game.Domain.Interfaces;
using Game.Infrastructure; // For BeanResource

public class GameCompositionRoot : MonoBehaviour
{
    [SerializeField] private UIManager uiManager;
    [SerializeField] private CoffeeMachine coffeeMachine;
    [SerializeField] private PlayerController playerController;
    [SerializeField] private CustomerManager customerManager;
    [SerializeField] private BeanResource beanResource;

    private IScoreService _scoreService;
    private PlayerPrefsScoreRepository _scoreRepository;
    private ICoffeeBrewingService _brewingService;
    private ICupInventoryService _cupInventoryService;
    private ICustomerService _customerService;
    private IBeanResourceService _beanService;

    private void Awake()
    {
        InitializeServices();
        InitializeUI();
        InjectDependencies();
    }

    private void InitializeServices()
    {
        _scoreRepository = new PlayerPrefsScoreRepository();
        _scoreService = new ScoreService();
        
        // Load initial score from repository
        int savedScore = _scoreRepository.LoadScore();
        // You'd need to modify ScoreService to support initial value

        _customerService = new CustomerService(_scoreService);
        _beanService = new BeanResourceService();
    }

    private void InitializeUI()
    {
        uiManager.Initialize(_scoreService);
    }

    private void InjectDependencies()
    {
        if (coffeeMachine != null)
        {
            _brewingService = new CoffeeBrewingService(coffeeMachine.cupPrefab, coffeeMachine.processTime, coffeeMachine);
            coffeeMachine.SetBrewingService(_brewingService);
        }

        if (playerController != null && playerController.cupHoldPoint != null)
        {
            _cupInventoryService = new CupInventoryService(playerController.cupHoldPoint);
            playerController.SetCupInventoryService(_cupInventoryService);
        }

        if (customerManager != null)
        {
            customerManager.Initialize(_customerService);
        }

        if (beanResource != null)
        {
            beanResource.Initialize(_beanService);
        }
    }

    private void OnApplicationQuit()
    {
        // Save score when application quits
        _scoreRepository.SaveScore(_scoreService.CurrentScore);
    }
}