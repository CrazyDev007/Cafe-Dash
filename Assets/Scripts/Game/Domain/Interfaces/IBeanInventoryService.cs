public interface IBeanInventoryService
{
    int CurrentBeanCount { get; }
    int MaxBeans { get; }
    
    bool CanAddBean();
    void AddBean();
    void RemoveBean();
    
    event System.Action<int> OnBeanCountChanged;
}