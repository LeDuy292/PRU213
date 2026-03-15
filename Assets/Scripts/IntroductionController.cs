using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections.Generic;

/// <summary>
/// Controller cho IntroductionScene - Quản lý các nút bấm và chuyển cảnh
/// </summary>
public class IntroductionController : MonoBehaviour
{
    // Tên scene mà bạn muốn chuyển đến khi bấm nút Start
    public string gameSceneName = "GameScene";

    // ---------------------------------------------------------
    // KẾT NỐI NÚT CONTINUE MỚI
    // ---------------------------------------------------------
    [Header("UI Buttons")]
    public GameObject continueButton;             // Kéo nút Continue bồ vừa tạo vào đây

    // ---------------------------------------------------------
    // MUSIC TOGGLE - References cho 2 buttons và AudioSource
    // ---------------------------------------------------------
    [Header("Music Toggle Settings")]
    public AudioSource backgroundMusic;
    public GameObject soundButtonOn;
    public GameObject soundButtonOff;

    private bool isMusicOn = true;
    private const string MUSIC_STATE_KEY = "MusicEnabled";

    void Start()
    {
        Debug.Log("IntroductionController đã được khởi tạo!");

        if (!string.IsNullOrEmpty(gameSceneName))
        {
            Debug.Log($"Scene target: {gameSceneName}");
        }

        // 1. NGAY KHI MỞ GAME: Kiểm tra file save để bật/tắt nút Continue
        CheckSaveFile();

        // 2. Load và áp dụng trạng thái nhạc đã lưu
        LoadMusicState();
        ApplyMusicState();
    }

    // ---------------------------------------------------------
    // HÀM KIỂM TRA FILE SAVE (ẨN/HIỆN NÚT CONTINUE)
    // ---------------------------------------------------------
    private void CheckSaveFile()
    {
        string savePath = System.IO.Path.Combine(Application.persistentDataPath, "savegame.json");

        if (continueButton != null)
        {
            // Nếu có file save -> Bật nút, không có -> Tắt nút
            bool hasSave = System.IO.File.Exists(savePath);
            continueButton.SetActive(hasSave);
            Debug.Log($"Trạng thái nút Continue: {(hasSave ? "BẬT" : "TẮT")}");
        }
    }

    // ---------------------------------------------------------
    // KHI BẤM NÚT START (CHƠI MỚI TỪ ĐẦU)
    // ---------------------------------------------------------
    public void OnStartButtonClicked()
    {
        Debug.Log("Nút START đã được bấm! Đang chuyển sang video trailer...");
        string videoSceneName = "VideoTrailerScene";

        if (Application.CanStreamedLevelBeLoaded(videoSceneName))
        {
            SceneManager.LoadScene(videoSceneName);
        }
        else
        {
            Debug.LogWarning($"Không tìm thấy scene: {videoSceneName}. Chuyển thẳng sang GameScene...");
            SceneManager.LoadScene(gameSceneName);
        }
    }

    // ---------------------------------------------------------
    // KHI BẤM NÚT CONTINUE (CHƠI TIẾP FILE CŨ)
    // ---------------------------------------------------------
    public void OnContinueButtonClicked()
    {
        Debug.Log("Nút CONTINUE đã được bấm! Đang load dữ liệu và vào game...");

        // 1. Đọc lại dữ liệu
        LoadGameData();

        // 2. Chuyển thẳng vào GameScene (bỏ qua video trailer)
        SceneManager.LoadScene(gameSceneName);
    }

    // ---------------------------------------------------------
    // SETTINGS PANEL
    // ---------------------------------------------------------
    [Header("Settings Panel")]
    public GameObject settingsPanel;

    public void OnSettingsButtonClicked()
    {
        if (settingsPanel != null)
        {
            settingsPanel.SetActive(!settingsPanel.activeSelf);
        }
    }

    // ---------------------------------------------------------
    // CẤU TRÚC LƯU GAME PHÂN TẦNG
    // ---------------------------------------------------------
    [System.Serializable]
    public class GameData
    {
        public PlayerStats playerStats = new PlayerStats();
        public GameProgress progress = new GameProgress();
        public GameSettings settings = new GameSettings();
        public float[] playerPosition = new float[3];
    }

    [System.Serializable]
    public class PlayerStats
    {
        public string playerName = "Hero";
        public int level = 1;
        public float maxHealth = 100f;
        public float currentHealth = 100f;
        public int gold = 0;
        public List<string> inventoryItemIDs = new List<string>();
    }

    [System.Serializable]
    public class GameProgress
    {
        public int highestLevelReached = 1;
        public List<string> defeatedBosses = new List<string>();
        public bool hasUnlockedDoubleJump = false;
        public bool hasUnlockedDash = false;
    }

    [System.Serializable]
    public class GameSettings
    {
        public bool isMusicOn = true;
        public float masterVolume = 1f;
    }

    // ---------------------------------------------------------
    // KHI BẤM NÚT SAVE GAME 
    // ---------------------------------------------------------
    public void OnSaveGameButtonClicked()
    {
        Debug.Log("Nút SAVE GAME đang thực thi...");

        GameData dataToSave = new GameData();
        string json = JsonUtility.ToJson(dataToSave, true);
        string savePath = System.IO.Path.Combine(Application.persistentDataPath, "savegame.json");
        System.IO.File.WriteAllText(savePath, json);

        Debug.Log("<color=green>Game đã được lưu thành công tại:</color> " + savePath);

        // QUAN TRỌNG: Gọi lại hàm này để nút Continue mọc ra ngay lập tức
        CheckSaveFile();
    }

    // ---------------------------------------------------------
    // HÀM LOAD GAME
    // ---------------------------------------------------------
    public void LoadGameData()
    {
        string savePath = System.IO.Path.Combine(Application.persistentDataPath, "savegame.json");

        if (System.IO.File.Exists(savePath))
        {
            string json = System.IO.File.ReadAllText(savePath);
            GameData loadedData = JsonUtility.FromJson<GameData>(json);

            Debug.Log("<color=cyan>Đã load game thành công!</color> Người chơi: " + loadedData.playerStats.playerName + ", Level: " + loadedData.playerStats.level);
        }
        else
        {
            Debug.LogWarning("Chưa có file save game nào được tạo trước đó!");
        }
    }

    // ---------------------------------------------------------
    // KHI BẤM NÚT QUIT
    // ---------------------------------------------------------
    public void OnQuitButtonClicked()
    {
        Debug.Log("Nút QUIT đã được bấm! Đang thoát game...");
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }

    // ---------------------------------------------------------
    // CÁC HÀM QUẢN LÝ ÂM THANH
    // ---------------------------------------------------------
    public void OnMusicToggleClicked()
    {
        isMusicOn = !isMusicOn;
        ApplyMusicState();
        SaveMusicState();
    }

    private void ApplyMusicState()
    {
        if (backgroundMusic == null || soundButtonOn == null || soundButtonOff == null) return;

        if (isMusicOn) { if (!backgroundMusic.isPlaying) backgroundMusic.Play(); }
        else { if (backgroundMusic.isPlaying) backgroundMusic.Pause(); }

        soundButtonOn.SetActive(isMusicOn);
        soundButtonOff.SetActive(!isMusicOn);
    }

    private void SaveMusicState()
    {
        PlayerPrefs.SetInt(MUSIC_STATE_KEY, isMusicOn ? 1 : 0);
        PlayerPrefs.Save();
    }

    private void LoadMusicState()
    {
        isMusicOn = PlayerPrefs.GetInt(MUSIC_STATE_KEY, 1) == 1;
    }
}