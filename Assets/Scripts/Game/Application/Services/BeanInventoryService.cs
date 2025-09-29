using UnityEngine;

public class BeanInventoryService : IBeanInventoryService
{
    public int CurrentBeanCount { get; private set; }
    public int MaxBeans { get; }
    
    public event System.Action<int> OnBeanCountChanged;
    
    private readonly GameObject _beanBagPrefab;
    private readonly Transform _beanStackParent;
    
    public BeanInventoryService(GameObject beanBagPrefab, Transform beanStackParent, int maxBeans = 3)
    {
        _beanBagPrefab = beanBagPrefab;
        _beanStackParent = beanStackParent;
        MaxBeans = maxBeans;
    }
    
    public bool CanAddBean()
    {
        return CurrentBeanCount < MaxBeans;
    }
    
    public void AddBean()
    {
        if (CanAddBean())
        {
            GameObject bean = Object.Instantiate(_beanBagPrefab, _beanStackParent);
            bean.transform.localPosition = new Vector3(0, 0.3f * CurrentBeanCount, 0);
            CurrentBeanCount++;
            OnBeanCountChanged?.Invoke(CurrentBeanCount);
        }
    }
    
    public void RemoveBean()
    {
        if (CurrentBeanCount > 0)
        {
            if (_beanStackParent != null && _beanStackParent.childCount > 0)
            {
                Transform lastBean = _beanStackParent.GetChild(_beanStackParent.childCount - 1);
                Object.Destroy(lastBean.gameObject);
            }
            CurrentBeanCount--;
            OnBeanCountChanged?.Invoke(CurrentBeanCount);
        }
    }
}