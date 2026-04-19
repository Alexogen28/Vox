using UnityEngine;
using UnityEngine.UI;

public class HealthBarScript : MonoBehaviour
{
    [SerializeField] Sprite[] animationSprites;
    [SerializeField] float spriteChangeTimer;

    float currentTimeElapsed;
    Image healthBarImage;
    int currentImage;

    PlayerController playerController;

    private void Start()
    {
        currentTimeElapsed = 0.0f;
        currentImage = 0;

        healthBarImage = GetComponent<Image>();

        playerController = FindFirstObjectByType<PlayerController>();
    }


    // Update is called once per frame
    void Update()
    {
        CycleSprites();
        CheckHealthAndUpdateBar();
    }


    void CycleSprites()
    {
        currentTimeElapsed += Time.deltaTime;

        if(currentTimeElapsed >= spriteChangeTimer)
        {
            currentTimeElapsed = 0.0f;

            if (currentImage == animationSprites.Length - 1)
                currentImage = 0;
            else
                currentImage++;

            healthBarImage.sprite = animationSprites[currentImage];
        }
    }
    void CheckHealthAndUpdateBar()
    {
        float ratio = playerController.health.currentHealth / playerController.health.maxHealth;
        healthBarImage.fillAmount = ratio;
    }
}
