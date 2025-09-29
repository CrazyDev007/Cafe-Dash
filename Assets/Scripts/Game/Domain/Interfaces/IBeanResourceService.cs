using UnityEngine;

namespace Game.Domain.Interfaces
{
    public interface IBeanResourceService
    {
        void SpawnBeanBag();
        int GetBeanCount();
        bool HasBeans();
    }
}