using UnityEngine;
using UnityEngine.SceneManagement;

public class GameOverManager : MonoBehaviour
{
    [Header("Kéo GameOverPanel vào đây")]
    public GameObject gameOverPanel;

    // 1. Hàm hiện UI Game Over
    public void ShowGameOver()
    {
        gameOverPanel.SetActive(true);
        Time.timeScale = 0f; // Đóng băng mọi hoạt động trong game
    }

    // 2. Hàm cho nút RESTART (Chơi lại map này) 
    public void RestartGame()
    {
        Time.timeScale = 1f; // Xả đông thời gian trước khi load
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    // 3. Hàm cho nút QUIT (Về màn hình Menu chính)
    public void QuitToMenu()
    {
        Time.timeScale = 1f; // Xả đông thời gian trước khi chuyển Scene
        SceneManager.LoadScene("IntroductionScene");
    }
}