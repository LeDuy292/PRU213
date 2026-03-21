using UnityEngine;
using Unity.Cinemachine;
using System.Collections;

public class BossIntroController : MonoBehaviour
{
    [Header("Camera & Player HUD")]
    public CinemachineCamera vcamBoss;
    public GameObject playerHUD;

    [Header("Animators UI")]
    public Animator topBarAnim;
    public Animator bottomBarAnim;
    public Animator bossNameAnim;

    [Header("Thanh máu Boss")]
    public GameObject bossHealthBar;

    [Header("Đối tượng cần dừng")]
    public GameObject playerObject;
    public GameObject bossObject;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            Debug.Log("<color=yellow>==> BẮT ĐẦU INTRO THEO KỊCH BẢN MỚI...</color>");
            StartCoroutine(PlayIntroSequence());
            GetComponent<Collider2D>().enabled = false;
        }
    }

    IEnumerator PlayIntroSequence()
    {
        // --- BƯỚC 1: ĐÓNG BĂNG NGAY LẬP TỨC ---
        FreezeObject(playerObject, true);
        FreezeObject(bossObject, true);
        if (playerHUD) playerHUD.SetActive(false);

        // --- BƯỚC 2: HAI THANH ĐEN KẸP VÔ TRƯỚC ---
        if (topBarAnim) topBarAnim.SetTrigger("Show");
        if (bottomBarAnim) bottomBarAnim.SetTrigger("Show");
        Debug.Log("1. Thanh đen đã kẹp vô.");

        // Đợi thanh đen trượt vào xong (khoảng 1 giây)
        yield return new WaitForSeconds(1.0f);

        // --- BƯỚC 3: BẮT ĐẦU CHUYỂN CAM ĐẾN BOSS ---
        if (vcamBoss != null)
        {
            vcamBoss.gameObject.SetActive(true);
            vcamBoss.Priority = 100;
            Debug.Log("2. Đang lia cam đến Boss.");
        }

        // Đợi cam lia tới nơi và dừng lại 1 tí (khoảng 2 giây)
        yield return new WaitForSeconds(2.0f);

        // --- BƯỚC 4: CHỮ TÊN BOSS MỚI CHẠY RA ---
        if (bossNameAnim)
        {
            bossNameAnim.SetTrigger("Show");
            Debug.Log("3. Hiện tên Boss.");
        }

        // Đợi người chơi đọc tên Boss (4 giây)
        yield return new WaitForSeconds(4.0f);

        // --- BƯỚC 5: CHỮ MẤT ĐI TRƯỚC ---
        if (bossNameAnim) bossNameAnim.SetTrigger("Hide");
        Debug.Log("4. Tên Boss biến mất.");

        // Đợi chữ biến mất hẳn rồi mới làm bước tiếp theo
        yield return new WaitForSeconds(1.0f);

        // --- BƯỚC 6: ZOOM LẠI NGƯỜI CHƠI (TẮT CAM BOSS) ---
        if (vcamBoss) vcamBoss.Priority = 1;
        Debug.Log("5. Đang lia cam về lại người chơi.");

        // Đợi cam về tới nơi hoàn toàn
        yield return new WaitForSeconds(1.5f);

        // --- BƯỚC 7: THU THANH ĐEN LẠI & HIỆN MÁU BOSS ---
        if (topBarAnim) topBarAnim.SetTrigger("Hide");
        if (bottomBarAnim) bottomBarAnim.SetTrigger("Hide");

        // Đợi thanh đen trượt ra hết
        yield return new WaitForSeconds(1.0f);

        // GIẢI PHÓNG & CHIẾN ĐẤU
        vcamBoss.gameObject.SetActive(false);
        FreezeObject(playerObject, false);
        FreezeObject(bossObject, false);

        if (bossHealthBar) bossHealthBar.SetActive(true);
        if (playerHUD) playerHUD.SetActive(true);

        Debug.Log("<color=cyan>==> KỊCH BẢN HOÀN TẤT: CHIẾN ĐI!</color>");
    }

    void FreezeObject(GameObject obj, bool isFreeze)
    {
        if (obj == null) return;
        MonoBehaviour[] scripts = obj.GetComponents<MonoBehaviour>();
        foreach (var s in scripts) if (s != this) s.enabled = !isFreeze;

        Rigidbody2D rb = obj.GetComponent<Rigidbody2D>();
        if (rb != null)
        {
            if (isFreeze)
            {
                rb.linearVelocity = Vector2.zero;
                rb.bodyType = RigidbodyType2D.Kinematic;
            }
            else
            {
                rb.bodyType = RigidbodyType2D.Dynamic;
            }
        }

        Animator anim = obj.GetComponent<Animator>();
        if (anim != null) anim.speed = isFreeze ? 0 : 1;
    }
}