using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// Controller cho IntroductionScene - Quản lý các nút bấm và chuyển cảnh
/// </summary>
public class IntroductionController : MonoBehaviour
{
    // Tên scene mà bạn muốn chuyển đến khi bấm nút Start
    public string gameSceneName = "GameScene";

    // ---------------------------------------------------------
    // MUSIC TOGGLE - References cho 2 buttons và AudioSource
    // ---------------------------------------------------------
    [Header("Music Toggle Settings")]
    public AudioSource backgroundMusic;           // Kéo AudioSource của nhạc nền vào đây
    public GameObject soundButtonOn;              // Kéo Sound Button On vào đây
    public GameObject soundButtonOff;             // Kéo Sound Button Off vào đây
    
    private bool isMusicOn = true;                // Trạng thái nhạc (mặc định bật)
    private const string MUSIC_STATE_KEY = "MusicEnabled"; // Key lưu trong PlayerPrefs

    // ---------------------------------------------------------
    // Hàm được gọi khi người chơi bấm nút "START"
    // ---------------------------------------------------------
    public void OnStartButtonClicked()
    {
        Debug.Log("Nút START đã được bấm! Đang chuyển sang game...");
        
        // Kiểm tra xem scene có tồn tại không
        if (Application.CanStreamedLevelBeLoaded(gameSceneName))
        {
            SceneManager.LoadScene(gameSceneName);
        }
        else
        {
            Debug.LogWarning($"Không tìm thấy scene: {gameSceneName}. Vui lòng kiểm tra Build Settings!");
        }
    }

    // ---------------------------------------------------------
    // Hàm được gọi khi người chơi bấm nút "SETTINGS"
    // ---------------------------------------------------------
    public void OnSettingsButtonClicked()
    {
        Debug.Log("Nút SETTINGS đã được bấm!");
        
        // TODO: Mở panel Settings khi đã có UI Settings
        // Ví dụ: settingsPanel.SetActive(true);
        
        // Tạm thời chỉ hiển thị thông báo
        Debug.Log("Chức năng Settings sẽ được bổ sung sau!");
    }

    // ---------------------------------------------------------
    // Hàm được gọi khi người chơi bấm nút "SAVE GAME"
    // ---------------------------------------------------------
    public void OnSaveGameButtonClicked()
    {
        Debug.Log("Nút SAVE GAME đã được bấm!");
        
        // TODO: Implement save game logic
        // Ví dụ:
        // - Lưu progress của player
        // - Lưu settings
        // - Lưu vào PlayerPrefs hoặc file JSON
        
        // Tạm thời chỉ hiển thị thông báo
        Debug.Log("Chức năng Save Game sẽ được bổ sung sau!");
        Debug.Log("Game đã được lưu thành công! (Demo)");
    }

    // ---------------------------------------------------------
    // Hàm được gọi khi người chơi bấm nút "QUIT" (nếu có)
    // ---------------------------------------------------------
    public void OnQuitButtonClicked()
    {
        Debug.Log("Nút QUIT đã được bấm! Đang thoát game...");
        
        #if UNITY_EDITOR
        // Trong Unity Editor, dừng chế độ Play
        UnityEditor.EditorApplication.isPlaying = false;
        #else
        // Trong bản build, thoát ứng dụng
        Application.Quit();
        #endif
    }

    // ---------------------------------------------------------
    // Optional: Hàm để test xem script có hoạt động không
    // ---------------------------------------------------------
    void Start()
    {
        Debug.Log("IntroductionController đã được khởi tạo!");
        
        // Kiểm tra xem scene target có tồn tại không
        if (!string.IsNullOrEmpty(gameSceneName))
        {
            Debug.Log($"Scene target: {gameSceneName}");
        }

        // Load và áp dụng trạng thái nhạc đã lưu
        LoadMusicState();
        ApplyMusicState();
    }

    // ---------------------------------------------------------
    // Hàm được gọi khi người chơi bấm nút TOGGLE MUSIC
    // ---------------------------------------------------------
    public void OnMusicToggleClicked()
    {
        // Đảo trạng thái nhạc
        isMusicOn = !isMusicOn;
        
        Debug.Log($"Music toggled: {(isMusicOn ? "ON" : "OFF")}");
        
        // Áp dụng trạng thái mới
        ApplyMusicState();
        
        // Lưu trạng thái vào PlayerPrefs
        SaveMusicState();
    }

    // ---------------------------------------------------------
    // Áp dụng trạng thái nhạc (bật/tắt nhạc và swap buttons)
    // ---------------------------------------------------------
    private void ApplyMusicState()
    {
        // Kiểm tra references
        if (backgroundMusic == null)
        {
            Debug.LogWarning("Background Music AudioSource chưa được gán!");
            return;
        }

        if (soundButtonOn == null || soundButtonOff == null)
        {
            Debug.LogWarning("Sound Buttons chưa được gán!");
            return;
        }

        // Bật/tắt nhạc
        if (isMusicOn)
        {
            if (!backgroundMusic.isPlaying)
                backgroundMusic.Play();
        }
        else
        {
            if (backgroundMusic.isPlaying)
                backgroundMusic.Pause();
        }

        // Swap buttons: Hiện button phù hợp
        soundButtonOn.SetActive(isMusicOn);      // Hiện ON khi nhạc BẬT
        soundButtonOff.SetActive(!isMusicOn);    // Hiện OFF khi nhạc TẮT

        Debug.Log($"Music state applied: Playing={backgroundMusic.isPlaying}, ButtonOn={isMusicOn}");
    }

    // ---------------------------------------------------------
    // Lưu trạng thái nhạc vào PlayerPrefs
    // ---------------------------------------------------------
    private void SaveMusicState()
    {
        PlayerPrefs.SetInt(MUSIC_STATE_KEY, isMusicOn ? 1 : 0);
        PlayerPrefs.Save();
        Debug.Log($"Music state saved: {isMusicOn}");
    }

    // ---------------------------------------------------------
    // Load trạng thái nhạc từ PlayerPrefs
    // ---------------------------------------------------------
    private void LoadMusicState()
    {
        // Mặc định là bật (1), lần đầu sẽ lấy giá trị này
        isMusicOn = PlayerPrefs.GetInt(MUSIC_STATE_KEY, 1) == 1;
        Debug.Log($"Music state loaded: {isMusicOn}");
    }

}
