using Game.Domain.Interfaces;

namespace Game.Application
{
    public class CustomerService : ICustomerService
    {
        private readonly IScoreService _scoreService;
        private bool _waiting = false;
        private readonly int _customerReward = 1; // Changed reward value to 1 for serving a customer

        public CustomerService(IScoreService scoreService)
        {
            _scoreService = scoreService;
        }

        public bool IsWaiting => _waiting;

        public void OnCustomerSpawned()
        {
            _waiting = true;
        }

        public void OnCustomerServed()
        {
            _scoreService.AddScore(_customerReward);
            _waiting = false;
        }
    }
}