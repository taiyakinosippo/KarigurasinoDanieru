using UnityEngine;

public class PlayerJump : MonoBehaviour
{
    private Rigidbody2D rb;
    [SerializeField]private float basejumpForce = 5f;
    [SerializeField]private float jumpForceIncreaseRate = 0.01f; // ジャンプ力の増加率
    public void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }
    public void Jump()
   {
        int score = ScoreManager.instance.GetScore();
        float jumpForce = basejumpForce + (score * jumpForceIncreaseRate); // Scoreに応じてジャンプ力を増加
        rb.AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse);
    }
}
