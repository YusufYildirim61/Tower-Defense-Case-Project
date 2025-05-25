using System;
using UnityEngine;

public class GameEvents 
{
    public static Action OnPlayerDied;
    public static Action<float> OnHealthChanged;
    public static Action<float,Sprite> OnSetMaxHealth;
    public static Action OnLevelComplete;
    public static Action OnLevelFailed;
    public static Action<int,int> OnNextWave;
    public static Action OnEnemyKill;

}
