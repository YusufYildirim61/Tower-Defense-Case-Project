using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameUI : MonoBehaviour
{
    [SerializeField] Transform spawnPoint;
    [Header("Level Fail/Complete Settings")]
    [SerializeField] GameObject levelFailedPanel;
    [SerializeField] GameObject levelCompletePanel;
    [Header("Wave Settings")]
    [SerializeField] TextMeshProUGUI wavesText;
    [SerializeField] WaveManager waveManager;
    [Header("Characters Panel Settings")]
    [SerializeField] GameObject characterChoosePanel;
    [SerializeField] GameObject[] characters;
    [SerializeField] CharacterInfos[] characterInfos;
    [Header("Respawn Settings")]
    [SerializeField] GameObject respawnButton;
    [SerializeField] Image cooldownImage;
    [SerializeField] float respawnCooldown;
    [Header("Health Bar Settings")]
    [SerializeField] Image healthBarIcon;
    [SerializeField] Slider playerHealthSlider;
    [SerializeField] TextMeshProUGUI healthText;
    [SerializeField] Gradient gradient;
    [SerializeField] Image fill;

    PlayerMovement currentPlayer;

    bool canSpawn = true;

    private void OnEnable()
    {
        GameEvents.OnPlayerDied += HandleRespawnButton;
        GameEvents.OnHealthChanged += SetHealth;
        GameEvents.OnSetMaxHealth += SetMaxHealth;
        GameEvents.OnLevelFailed += ShowLevelFailedPanel;
        GameEvents.OnLevelComplete += ShowLevelCompletePanel;
        GameEvents.OnNextWave += HandleWaveTexts;
    }

    private void OnDisable()
    {
        GameEvents.OnPlayerDied -= HandleRespawnButton;
        GameEvents.OnHealthChanged -= SetHealth;
        GameEvents.OnSetMaxHealth -= SetMaxHealth;
        GameEvents.OnLevelComplete -= ShowLevelCompletePanel;
        GameEvents.OnNextWave -= HandleWaveTexts;
    }
    void Start()
    {
        SetupCharacterInfos();
    }

    void Update()
    {
        if(!canSpawn)
        {
            cooldownImage.fillAmount -= 1.0f / respawnCooldown * Time.deltaTime;
        }
    }
    
    #region "Health Bar Methods"
    public void SetMaxHealth(float health,Sprite icon)
    {
        playerHealthSlider.gameObject.SetActive(true);

        healthBarIcon.sprite = icon;
        playerHealthSlider.maxValue = health;
        playerHealthSlider.value = health;

        healthText.text = $"{health} / {health}";

        fill.color = gradient.Evaluate(1f);
    }
    public void SetHealth(float health)
    {
        playerHealthSlider.value = health;
        healthText.text = $"{health} / {playerHealthSlider.maxValue}";

        fill.color = gradient.Evaluate(playerHealthSlider.normalizedValue);
    }
    #endregion
    
    #region "Character Methods"
    void HandleRespawnButton()
    {
        cooldownImage.fillAmount = 1f;
        playerHealthSlider.gameObject.SetActive(false);

        canSpawn = false;

        respawnButton.SetActive(true);
        respawnButton.GetComponent<Button>().interactable = false;

        StartCoroutine(UnlockRespawn());
    }
    IEnumerator UnlockRespawn()
    {
        yield return new WaitForSeconds(respawnCooldown);
        respawnButton.GetComponent<Button>().interactable = true;
        canSpawn = true;
    }
    public void RespawnPlayer() // OnClick Method
    {
        currentPlayer.RespawnPlayer(spawnPoint);
        playerHealthSlider.gameObject.SetActive(true);
        respawnButton.SetActive(false);
    }
    public void ChooseCharacter(int characterIndex)
    {
        characterChoosePanel.SetActive(false);
        GameObject chosenPlayer = Instantiate(characters[characterIndex],spawnPoint.position,Quaternion.identity);
        currentPlayer = chosenPlayer.GetComponent<PlayerMovement>();
        waveManager.StartWaves();
    }
    void SetupCharacterInfos()
    {
        for (int i = 0; i < characters.Length; i++)
        {
            var characterSO = characters[i].GetComponent<PlayerMovement>().GetCharacterSO();
            characterInfos[i].SetInfo(characterSO);
        }
    }
    #endregion
    
    #region "Button Methods"
    public void RestartGame()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
    public void ReturnToMainMenu()
    {
        SceneManager.LoadScene(0);
    }
    public void OpenNextLevel()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex+1);
    }
    public void NextWaveButton()
    {
        waveManager.CallNextWave();
    }
    #endregion
    
    void ShowLevelFailedPanel()
    {
        PlayerMovement player = FindFirstObjectByType<PlayerMovement>();
        if(player != null)
        {
            player.StopPlayer();
        }
        GameManager.Instance.GameOver = true;
        levelFailedPanel.SetActive(true);
    }
    void ShowLevelCompletePanel()
    {
        PlayerMovement player = FindFirstObjectByType<PlayerMovement>();
        if(player != null)
        {
            player.StopPlayer();
        }
        GameManager.Instance.GameOver = true;
        levelCompletePanel.SetActive(true);
    }
    void HandleWaveTexts(int waveIndex, int totalWaves)
    {
        wavesText.text = $"Wave {waveIndex} / {totalWaves}"; 
    }
}

[System.Serializable]
public class CharacterInfos
{
    [SerializeField] TextMeshProUGUI health;
    [SerializeField] TextMeshProUGUI damage;
    [SerializeField] TextMeshProUGUI moveSpeed;
    [SerializeField] TextMeshProUGUI characterName;
    [SerializeField] TextMeshProUGUI attackRateInfo;

    public void SetInfo(CharacterSO character)
    {
        health.text         = $"Health: {character.health}";
        damage.text         = $"Damage: {character.damage}";
        moveSpeed.text      = $"Move Speed: {character.moveSpeed}";
        characterName.text  = character.characterName;
        attackRateInfo.text = character.attackRateInfo;
    }
}
