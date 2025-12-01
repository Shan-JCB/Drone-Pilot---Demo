using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class GameController : MonoBehaviour
{
    // config params
    [SerializeField] TextMeshProUGUI scoreText;
    [SerializeField] GameObject retryButton;
    [SerializeField] GameObject quitButton;
    [SerializeField] Player player;
    [Header("Player UI")]
    [SerializeField] PlayerHealthUI playerHealthUI;
    [SerializeField] CameraScroller mainCamera;
    [SerializeField] BackgroundScroller background;

    EnemySpawner enemySpawner;

    // state variables
    [SerializeField] int score = 0;

    private void Awake()
    {
        //Esse bloco de codigo tem como funcao fazer prevalecer o status do jogo que o jogador alcançou, conforme o mesmo vai avancando 
        //as fases do jogo
        int gameStatusCount = FindObjectsOfType<GameController>().Length;
        if (gameStatusCount > 1)
        {
            Destroy(gameObject);
        }
        else
        {
            DontDestroyOnLoad(gameObject);
        }

        retryButton.SetActive(false);
        quitButton.SetActive(false);
    }

    private void Start()
    {
        enemySpawner = FindObjectOfType<EnemySpawner>();
        scoreText.SetText(score.ToString());
        // If a Player instance is assigned in inspector and the player HUD is available,
        // update the hud with the player's current health.
        if (player != null && playerHealthUI != null)
        {
            playerHealthUI.UpdateHealth(player.GetHealth(), player.GetMaxHealth());
        }
    }

    public void AddToScore(int enemyPoints)
    {
        score += enemyPoints;
        scoreText.SetText(score.ToString());
    }

    public void Reset()
    {
        Destroy(gameObject);
    }

    public void GameOver()
    {
        retryButton.SetActive(true);
        quitButton.SetActive(true);

        enemySpawner.StopAllCoroutines();

        mainCamera.Stop(true);
        background.Stop();
    }

    public void RestartGame()
    {
        var enemies = FindObjectsOfType<Enemy>();
        var projectiles = GameObject.FindGameObjectsWithTag("EnemyProjectile");
        foreach(Enemy enemy in enemies)
        {
            Destroy(enemy.gameObject);
        }
        foreach (GameObject projectile in projectiles)
        {
            Destroy(projectile.gameObject);
        }
        score = 0;
        scoreText.SetText(score.ToString());

        mainCamera.Reset();
        background.Reset();

        retryButton.SetActive(false);
        quitButton.SetActive(false);

        Instantiate(player);
  
    }

    public void QuitGame()
    {
        Application.Quit();
    }

    public void PlayerLifeController()
    {

    }

    // Called by Player instances to update the HUD
    public void UpdatePlayerHealth(float current, float max)
    {
        if (playerHealthUI != null)
        {
            playerHealthUI.UpdateHealth(current, max);
        }
    }
}
