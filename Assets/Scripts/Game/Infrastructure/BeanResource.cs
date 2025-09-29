using UnityEngine;
using Game.Domain.Interfaces;

namespace Game.Infrastructure
{
    public class BeanResource : MonoBehaviour
    {
        [SerializeField] private GameObject beanBagPrefab;
        private IBeanResourceService _beanService;

        public void Initialize(IBeanResourceService service)
        {
            _beanService = service;
        }

        public void SpawnBeanBag(Vector3 position)
        {
            _beanService.SpawnBeanBag();
            if (beanBagPrefab != null && _beanService.HasBeans())
            {
                Instantiate(beanBagPrefab, position, Quaternion.identity);
            }
        }

        public int GetBeanCount()
        {
            return _beanService.GetBeanCount();
        }

        public bool HasBeans()
        {
            return _beanService.HasBeans();
        }
    }
}
