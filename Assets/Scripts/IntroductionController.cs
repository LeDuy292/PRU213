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
        Debug.Log("Nút START đã được bấm! Đang chuyển sang video trailer...");

        // Chuyển sang VideoTrailerScene thay vì GameScene
        string videoSceneName = "VideoTrailerScene";

        if (Application.CanStreamedLevelBeLoaded(videoSceneName))
        {
            SceneManager.LoadScene(videoSceneName);
        }
        else
        {
            Debug.LogWarning($"Không tìm thấy scene: {videoSceneName}. Chuyển thẳng sang GameScene...");
            // Fallback: Nếu không có video scene, chuyển thẳng sang game
            SceneManager.LoadScene(gameSceneName);
        }
    }

    // ---------------------------------------------------------
    // SETTINGS PANEL
    // ---------------------------------------------------------
    [Header("Settings Panel")]
    public GameObject settingsPanel;              // Kéo Settings Panel vào đây

    // ---------------------------------------------------------
    // Hàm được gọi khi người chơi bấm nút "SETTINGS"
    // ---------------------------------------------------------
    public void OnSettingsButtonClicked()
    {
        Debug.Log("Nút SETTINGS đã được bấm!");
        
        if (settingsPanel != null)
        {
            // Bật/tắt panel Settings (nếu đang bật thì tắt, đang tắt thì bật)
            settingsPanel.SetActive(!settingsPanel.activeSelf);
        }
        else
        {
            Debug.LogWarning("Chưa gán Settings Panel trong Unity Inspector!");
        }
    }

    // Đối tượng dữ liệu mẫu để lưu (bạn có thể tuỳ chỉnh theo dữ liệu thực tế của game)
    [System.Serializable]
    public class GameData
    {
        public int level = 1;
        public float health = 100f;
        public string playerName = "Hero";
        // Thêm các biến khác bạn muốn lưu ở đây
    }

    // ---------------------------------------------------------
    // Hàm được gọi khi người chơi bấm nút "SAVE GAME"
    // ---------------------------------------------------------
    public void OnSaveGameButtonClicked()
    {
        Debug.Log("Nút SAVE GAME đang thực thi...");
        
        // 1. Tạo và gán dữ liệu cần lưu
        GameData dataToSave = new GameData();
        // Ví dụ thực tế: dataToSave.level = GameManager.Instance.currentLevel;

        // 2. Chuyển đổi đối tượng GameData thành chuỗi JSON (để dễ đọc, true = pretty print)
        string json = JsonUtility.ToJson(dataToSave, true);

        // 3. Tạo đường dẫn an toàn bằng persistentDataPath (hoạt động tốt trên Windows, Android, iOS...)
        string savePath = System.IO.Path.Combine(Application.persistentDataPath, "savegame.json");

        // 4. Ghi chuỗi JSON đó vào file
        System.IO.File.WriteAllText(savePath, json);

        Debug.Log("<color=green>Game đã được lưu thành công tại:</color> " + savePath);
    }

    // ---------------------------------------------------------
    // Bổ sung: Hàm LOAD GAME để bạn có thể gọi khi cần load lại dữ liệu
    // ---------------------------------------------------------
    public void LoadGameData()
    {
        string savePath = System.IO.Path.Combine(Application.persistentDataPath, "savegame.json");

        if (System.IO.File.Exists(savePath))
        {
            // Nếu có file save, đọc nội dung file
            string json = System.IO.File.ReadAllText(savePath);

            // Chuyển đổi JSON text ngược lại thành object GameData
            GameData loadedData = JsonUtility.FromJson<GameData>(json);
            
            Debug.Log("<color=cyan>Đã load game thành công!</color> Người chơi: " + loadedData.playerName + ", Level: " + loadedData.level);
            
            // TODO: Áp dụng loadedData vào game của bạn
            // Ví dụ: PlayerController.Instance.health = loadedData.health;
        }
        else
        {
            Debug.LogWarning("Chưa có file save game nào được tạo trước đó!");
        }
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
