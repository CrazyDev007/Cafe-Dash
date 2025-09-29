using UnityEngine;

public interface ICupInventoryService
{
    bool HasCup { get; }
    GameObject CurrentCup { get; }
    
    void AttachCup(GameObject cup);
    void RemoveCup();
    
    event System.Action OnCupAttached;
    event System.Action OnCupRemoved;
}