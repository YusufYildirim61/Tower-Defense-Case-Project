using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class Castle : MonoBehaviour
{
    [SerializeField] float health = 50;
    [Header("Health Bar Settings")]
    [SerializeField] Slider healthBarSlider;
    [SerializeField] Gradient gradient;
    [SerializeField] Image fill;
    [Header("Flash Effect Settings")]
    [ColorUsage(true,true)]
    [SerializeField] Color flashColor = Color.white;
    [SerializeField] float flashTime = 0.25f;

    Animator animator;
    SpriteRenderer spriteRenderer;
    Material material;
    void Start()
    {
        animator = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();

        material = spriteRenderer.material;

        SetMaxHealth(health);
    }
    public void TakeDamage(float amount)
    {
        health -= amount;
        StartCoroutine(DamageFlasher());
        SetHealth(health);
        if(health<=0)
        {
            Die();
        }
    }
    IEnumerator DamageFlasher()
    {
        material.SetColor("_FlashColor",flashColor);

        float currentFlashAmount = 0;
        float elapsedTime = 0;

        while(elapsedTime < flashTime)
        {
            elapsedTime += Time.deltaTime;
            currentFlashAmount = Mathf.Lerp(1,0, elapsedTime / flashTime);
            material.SetFloat("_FlashAmount",currentFlashAmount);
            yield return null;
        }

    }
    public void Die()
    {
        if(!GameManager.Instance.GameOver)
        {
            animator.SetTrigger("Death");
            
            healthBarSlider.gameObject.SetActive(false);
            GetComponent<Collider2D>().enabled = false;
            GameEvents.OnLevelFailed?.Invoke();
        }
        
    }
    public void SetMaxHealth(float health)
    {
        healthBarSlider.maxValue = health;
        healthBarSlider.value = health;
        
        fill.color = gradient.Evaluate(1f);
    }
    public void SetHealth(float health)
    {
        healthBarSlider.value = health;

        fill.color = gradient.Evaluate(healthBarSlider.normalizedValue);
    }
}
