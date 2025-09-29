using UnityEngine;

public interface ICoffeeBrewingService
{
    bool IsProcessing { get; }
    bool HasReadyCup { get; }
    
    void StartBrewingProcess(Transform cupHolder = null);
    GameObject TakeReadyCup();
    event System.Action OnBrewingStarted;
    event System.Action OnBrewingCompleted;
}