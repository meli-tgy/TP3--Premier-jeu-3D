using UnityEngine;
using UnityEngine.SceneManagement;


public class GameOverUI : MonoBehaviour
{
    [Header("Références")]
    [Tooltip("Le Health du Player à surveiller")]
    [SerializeField] private Health playerHealth;
    [Tooltip("Le panneau Game Over, désactivé par défaut dans la scène")]
    [SerializeField] private GameObject gameOverPanel;

    [Header("Options")]
    [Tooltip("Met le jeu en pause (Time.timeScale = 0) pendant l'affichage du Game Over")]
    [SerializeField] private bool pauseOnGameOver = true;

    private void OnEnable()
    {
        if (playerHealth != null)
            playerHealth.OnDeath.AddListener(ShowGameOver);
    }

    private void OnDisable()
    {
        if (playerHealth != null)
            playerHealth.OnDeath.RemoveListener(ShowGameOver);
    }

    private void Start()
    {
        
        if (gameOverPanel != null)
            gameOverPanel.SetActive(false);
    }

    private void ShowGameOver()
    {
        if (gameOverPanel != null)
            gameOverPanel.SetActive(true);

        if (pauseOnGameOver)
            Time.timeScale = 0f;
    }

    public void RestartGame()
    {
        
        Time.timeScale = 1f;

        Scene currentScene = SceneManager.GetActiveScene();
        SceneManager.LoadScene(currentScene.name);
    }
}