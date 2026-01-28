using UnityEngine;
using UnityEngine.SceneManagement; // Dòng này bắt buộc để Unity hiểu lệnh "Chuyển cảnh"

public class MainMenu : MonoBehaviour
{
    // Biến này để lưu tên màn chơi bạn muốn vào (Ví dụ: "GameScene", "Level1")
    // Public để bạn có thể gõ tên trực tiếp từ bên ngoài Unity
    public string sceneToLoad = "GameScene";

    // ---------------------------------------------------------
    // Hàm 1: Chức năng cho nút START GAME
    // ---------------------------------------------------------
    public void PlayGame()
    {
        // Lệnh này bảo Unity: "Hãy tải cái màn chơi có tên là... lên cho tôi"
        SceneManager.LoadScene(sceneToLoad);

        Debug.Log("Đang vào game: " + sceneToLoad); // In thông báo kiểm tra
    }

    // ---------------------------------------------------------
    // Hàm 2: Chức năng cho nút SETTINGS
    // ---------------------------------------------------------
    public void OpenSettings()
    {
        // Tạm thời chưa có bảng cài đặt, mình chỉ in chữ ra để biết nút có hoạt động
        Debug.Log("Đã bấm nút Settings!");
    }

    // ---------------------------------------------------------
    // Hàm 3: Chức năng cho nút QUIT (Thoát game)
    // ---------------------------------------------------------
    public void QuitGame()
    {
        Debug.Log("Đã thoát game!"); // In thông báo (vì Editor không thoát thật được)
        Application.Quit(); // Lệnh thoát game thật (chỉ chạy khi xuất ra file .exe)
    }
}