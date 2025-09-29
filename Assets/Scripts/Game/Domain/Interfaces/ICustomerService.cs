using UnityEngine;

namespace Game.Domain.Interfaces
{
    public interface ICustomerService
    {
        bool IsWaiting { get; }
        void OnCustomerSpawned();
        void OnCustomerServed();
    }
}