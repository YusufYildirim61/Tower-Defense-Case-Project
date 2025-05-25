using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }
    bool gameOver = false;
    int totalEnemies;
    int killedEnemies;
    public bool GameOver { get {return gameOver;} set {gameOver = value;} }
    public int TotalEnemies { get {return totalEnemies;} set {totalEnemies = value;} }
    public int KilledEnemies { get {return killedEnemies;} set {killedEnemies = value;} }

    private void Awake() 
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }
}
