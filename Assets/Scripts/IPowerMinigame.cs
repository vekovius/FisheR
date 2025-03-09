using System;
using UnityEngine;

public interface IPowerMinigame
{
    float GetPowerValue(); // Returns normalized power value (0-1)
    void Activate(); // Activates the minigame
    void Deactivate(); // Deactivates the minigame
    event Action<float> OnPowerChanged; // Event that is invoked when power value changes
}