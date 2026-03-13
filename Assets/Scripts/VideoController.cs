using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Video;

public class VideoController : MonoBehaviour
{
    public VideoPlayer myVideoPlayer; // Máy chiếu
    public string nextSceneName = "GameScene"; // Tên màn game

    void Start()
    {
        // Khi hết phim -> Gọi hàm EndReached
        myVideoPlayer.loopPointReached += EndReached;
    }

    void EndReached(UnityEngine.Video.VideoPlayer vp)
    {
        SceneManager.LoadScene(nextSceneName);
    }

    // Bấm chuột để bỏ qua
    void Update()
    {
        if (Input.GetMouseButtonDown(0)) SceneManager.LoadScene(nextSceneName);
    }
}