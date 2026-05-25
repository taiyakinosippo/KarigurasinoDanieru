
using UnityEngine;
using UnityEngine.UI;

//”wŒi‚ğ“®‚©‚·ƒNƒ‰ƒX
public class BackGroundMover : MonoBehaviour
{
    [SerializeField]private ScoreController scoreController;   //ƒXƒRƒA‚Ì•Ï‰»‚ğó‚¯æ‚é‚½‚ß‚ÌƒXƒRƒAƒRƒ“ƒgƒ[ƒ‰[
    [SerializeField]private StageManager stageManager;
    [SerializeField]private RectTransform[] images;
    [SerializeField] private float imageHeight = 540f;
    private float deceleration;
    private float speed;
    private bool isMoving;
    private bool isSlowDown;



    // ========================================
    // ‰Šú‰»
    // ========================================

    private void Start()
    {
        scoreController.OnStartSpeedEnd +=
            HalfSpeed;

        scoreController.OnMiddleSpeedEnd +=
            StartSlowDown;

        scoreController.OnEndSpeedEnd +=
            StopMove;

        //‰Šú‚Ì”wŒi‰æ‘œ‚ğƒ‰ƒ“ƒ_ƒ€‚Éİ’è
        foreach (var image in images)
        {
            Sprite sprite =stageManager.GetRandomBackground(0);
            if (sprite != null)
            {
                image.GetComponent<Image>().sprite = sprite;
            }
        }
    }

    // ========================================
    // Å‚‘¬“x‚Ì“®‚«‚ğŠJn
    // ========================================

    public void StartMoving(float startSpeed, float decelerationRate)
    {
        speed = startSpeed;
        deceleration = decelerationRate;
        isMoving = true;
    }

    // ========================================
    // ”wŒi‚ğ“®‚©‚·‘¬“x‚ÌŒvZ
    // ========================================

    private void Update()
    {
        if (!isMoving)
            return;

        if (isSlowDown)
        {
            speed = Mathf.Lerp(speed,  0,deceleration * Time.deltaTime);
        }
        Move();
    }

    // ========================================
    // ”wŒi‚ğ“®‚©‚·
    // ========================================

    private void Move()
    {
        float moveAmount =speed * Time.deltaTime;
        foreach (var image in images)
        {
            image.anchoredPosition -=new Vector2(0, moveAmount);
        }
        LoopBackground();
    }

    // ========================================
    // ’†‘¬‚ÌƒXƒs[ƒh‚Ì“®‚«‚ğŠJn
    // ========================================
    private void HalfSpeed()
    {
        speed *= 0.5f;
        Debug.Log("”wŒi”¼Œ¸‘¬");
    }

    // ========================================
    // ÅŒã‚Ì‘¬“x‚Ì“®‚«‚ğŠJn
    // ========================================

    private void StartSlowDown()
    {
        isSlowDown = true;
        speed *= 0.5f;
        Debug.Log("”wŒiŒ¸‘¬");
    }

    // ========================================
    // ”wŒi‚Ì“®‚«‚ğ~‚ß‚é
    // ========================================

    private void StopMove()
    {
        isMoving = false;

        Debug.Log("”wŒi’â~");
    }

    // ========================================
    // ”wŒi‚ª‰º‚Ü‚Å‰º‚ª‚Á‚½‚©‚Ì”»’è‚Æ‰º‚É‰º‚ª‚Á‚½ê‡ˆê”Ôã‚É‚ ‚é‰æ‘œ‚ÌˆÊ’u‚Ìæ“¾‚µ‚Ä‚»‚Ìã‚É‰º‚ª‚Á‚½‰æ‘œ‚ğˆÚ“®‚³‚¹‚é
    // ========================================

    private void LoopBackground()
    {
        RectTransform lowest =
            GetLowest();

        if (lowest == null)
            return;

        if (lowest.anchoredPosition.y <= -imageHeight)
        {
            RectTransform highest =GetHighest();
            lowest.anchoredPosition =new Vector2( 0, highest.anchoredPosition.y + imageHeight); 
            Sprite sprite =stageManager.GetRandomBackground(scoreController.GetCurrentScore());
            Image image = lowest.GetComponent<Image>();
            if (sprite != null) image.sprite = sprite;
        }

    }


    // ========================================
    /// Å‚à‰º‚É‚ ‚é”wŒi‰æ‘œ‚ğæ“¾‚·‚é
     // ========================================
    private RectTransform GetLowest()
    {
        RectTransform result = images[0];

        foreach (var image in images)
        {
            if (image.anchoredPosition.y <
                result.anchoredPosition.y)
            {
                result = image;
            }
        }

        return result;
    }

    // ========================================
    /// Å‚àã‚É‚ ‚é”wŒi‰æ‘œ‚ğæ“¾‚·‚é
    // ========================================
    private RectTransform GetHighest()
    {
        RectTransform result =images[0];
        foreach (var image in images)
        {
            if (image.anchoredPosition.y > result.anchoredPosition.y)
            {
                result = image;
            }
        }
        return result;
    }


}
