using Game.Domain.Interfaces;

namespace Game.Application
{
    public class BeanResourceService : IBeanResourceService
    {
        private int _beanCount = 10; // Initial bean count, can be loaded from persistence

        public void SpawnBeanBag()
        {
            if (HasBeans())
            {
                _beanCount--;
                // Business logic: e.g., notify observers or integrate with score if needed
            }
        }

        public int GetBeanCount()
        {
            return _beanCount;
        }

        public bool HasBeans()
        {
            return _beanCount > 0;
        }

        // Optional: Method to add beans (e.g., from purchases)
        public void AddBeans(int amount)
        {
            _beanCount += amount;
        }
    }
}