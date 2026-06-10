using System;
using System.Collections;
using System.Runtime.CompilerServices;
using UnityEngine;

/// <summary>
/// ロケットの動きを制御するクラス。プレイヤーの状態に応じて、ロケットの動きを変える。
/// </summary>
using UnityEngine.UI;
using static UnityEngine.GraphicsBuffer;

public class Rocket_Mover : MonoBehaviour
{
    [SerializeField] private BackGroundMover soloBackGroundMover;  //ソロ用の背景を動かすクラス
    [SerializeField] private BackGroundMover malutiBackGroundMover;//マルチ用の背景を動かすクラス
    [SerializeField] private float missUpMove = 20f;               //0～1000メートルのの時のロケットの動き(上に飛ぶ)
    [SerializeField] private float missDownMove = 50f;             //0～1000メートルのの時のロケットの動き(下に落ちる)
    [SerializeField] private float skyMove = 100f;                 //1000～10000メートルまでの時のロケットの動き(右に飛ぶ) 
    [SerializeField] private float atmosphereRotate = 360f;        //10000～100000メートルのの時のロケットの動き(回転する)
    [SerializeField] private float missMoveSpeed = 10f;            //0～1000メートルのの時のロケットの動きの速さ
    [SerializeField] private float skyMoveSpeed = 10f;             //1000～10000メートルまでの時のロケットの動きの速さ
    [SerializeField] private float spaceSpeed = 100f;              //100000メートル以上の時の背景のスクロールの速さ
    [SerializeField] private bool isMultiRocket = false;　　　　　 //マルチ用のスクリプトかどうか
    [SerializeField] RectTransform soloimageRect;                  //100000メートル以上の時のソロ用の背景画像
    [SerializeField] RectTransform malutyimageRect;                 //100000メートル以上の時のマルチ用の背景画像
    [Header("Sky (1000～10000m) 演出")]
    [SerializeField] private RectTransform soloWindImage;           //ソロ用の風イラスト
    [SerializeField] private RectTransform multiWindImage;          //マルチ用の風イラスト
    [SerializeField] private float skyRotationSpeed = 360f;         //Sky時のロケット回転速度(度/秒)
    [SerializeField] private float windMoveSpeed = 10f;             //風の移動速度
    [SerializeField] private bool playWindSE = true;                //風のSEを再生するか
    [SerializeField] private SEType windSE = SEType.windSE;         //風のSE
    [Header("Miss (整備不良) 爆発エフェクト")]
    [SerializeField] private GameObject explosionPrefab;
    [SerializeField] private float explosionDelay = 1.0f; // 落下完了後に爆発するまでの遅延秒数
    [SerializeField] private bool destroyRocketOnExplosion = true;
    [SerializeField] private bool playExplosionSE = true;
    [SerializeField] private SEType explosionSE = SEType.explosionSE;
   
    public void MissRocketMove()
    {
       StartCoroutine(MissRocketMoveCoroutine());
    }
    public void SkyRocketMove()
    {
        StartCoroutine(SkyRocketMoveCoroutine());
    }
    public void AtmosphereRocketMove()
    {
        StartCoroutine(AtmosphereRocketMoveCoroutine());
    }
    public void GalaxyRocketMove()
    {
        if (!isMultiRocket)
        {
            StartCoroutine(GalaxyMoveSpaceCoroution());
        }
        else 
        {
            StartCoroutine(MultiGalaxyMoveCoroutine());
        }
    }

    //0～1000メートルのの時のロケットの動き
    private IEnumerator MissRocketMoveCoroutine()
    {
        Vector2 uPtarget = new Vector2(transform.position.x, transform.position.y + missUpMove);
        Vector2 downTarget = new Vector2(transform.position.x, transform.position.y - missDownMove);
        // 上に飛ぶ
        while ((Vector2)transform.position != uPtarget)
        {
            transform.position = Vector2.MoveTowards(
                    transform.position,
                    uPtarget,
                    missMoveSpeed * Time.deltaTime);
            yield return null;
        }
        yield return new WaitForSeconds(0.5f);

        // 下に落ちる
        while ((Vector2)transform.position != downTarget)
        {
            transform.position =
                Vector2.MoveTowards(
                    transform.position,
                    downTarget,
                    missMoveSpeed * Time.deltaTime);

            yield return null;
        }
        // 落下完了後、指定時間経過で爆発エフェクトを生成
        if (explosionDelay > 0f)
        {
            yield return new WaitForSeconds(explosionDelay);
        }

        if (playExplosionSE && AudioSourceManager.instance != null)
        {
            AudioSourceManager.instance.PlaySE(explosionSE);
        }

        if (explosionPrefab != null)
        {
            Instantiate(explosionPrefab, transform.position, Quaternion.identity);
        }

        if (destroyRocketOnExplosion)
        {
            Destroy(gameObject);
        }
    }

    //1000～10000メートルまでの時のロケットの動き
    private IEnumerator SkyRocketMoveCoroutine()
    {
        // 風のSEを再生
        if (playWindSE && AudioSourceManager.instance != null)
        {
            AudioSourceManager.instance.PlaySE(windSE);
        }
        
        // 風イメージを選択（ソロ/マルチ)
        RectTransform windImage = isMultiRocket ? multiWindImage : soloWindImage;
        
        // 初期位置を記録
        Vector2 rocketStartPos = transform.position;
        Vector2 windStartPos = windImage != null ? windImage.anchoredPosition : Vector2.zero;
        
        // 目標位置（画面右外に出るまで）
        float screenWidth = 1920f; // 一般的な値、必要に応じて調整
        Vector2 rocketTarget = new Vector2(rocketStartPos.x + skyMove + screenWidth, rocketStartPos.y);
        Vector2 windTarget = new Vector2(windStartPos.x + skyMove + screenWidth, windStartPos.y);
        
        // ロケットと風が移動中
        while (transform.position.x < rocketTarget.x)
        {
            // ロケットを右に移動
            transform.position = Vector3.MoveTowards(
                    transform.position,
                    rocketTarget,
                    skyMoveSpeed * Time.deltaTime);
            
            // ロケットを回転
            transform.Rotate(0, 0, skyRotationSpeed * Time.deltaTime);
            
            // 風イメージを移動
            if (windImage != null)
            {
                windImage.anchoredPosition = Vector2.MoveTowards(
                    windImage.anchoredPosition,
                    windTarget,
                    windMoveSpeed * Time.deltaTime);
            }
            
            yield return null;
        }
    }

    //10000～100000メートルのの時のロケットの動き
    private IEnumerator AtmosphereRocketMoveCoroutine()
    {
        while (true)
        {
            transform.Rotate(
                0,
                0,
                360f * Time.deltaTime);

            yield return null;
        }
    }


    //100000メートル以上のの時のロケットの動き
    private IEnumerator GalaxyMoveSpaceCoroution()
    {
        RectTransform lowest = soloBackGroundMover.GetLowest();

        soloimageRect.anchoredPosition =
            new Vector2(
                lowest.anchoredPosition.x,
                lowest.anchoredPosition.y - soloimageRect.rect.height);

        Vector2 target =
            new Vector2(
                soloimageRect.anchoredPosition.x,
                0f);

        while (soloimageRect.anchoredPosition != target)
        {
            Vector2 before = soloimageRect.anchoredPosition;

            soloimageRect.anchoredPosition =
                Vector2.MoveTowards(
                    soloimageRect.anchoredPosition,
                    target,
                    spaceSpeed * Time.deltaTime);

            Vector2 delta =
                soloimageRect.anchoredPosition - before;

            foreach (RectTransform image in soloBackGroundMover._images)
            {
                image.anchoredPosition += delta;
            }

            yield return null;
        }
    }
    private IEnumerator MultiGalaxyMoveCoroutine()
    {
        Debug.Log("マルチ背景");
        RectTransform lowest = malutiBackGroundMover.GetLowest();

        malutyimageRect.anchoredPosition =
            new Vector2(
                lowest.anchoredPosition.x,
                lowest.anchoredPosition.y - malutyimageRect.rect.height);

        Vector2 target =
            new Vector2(
                malutyimageRect.anchoredPosition.x,
                0f);

        while (malutyimageRect.anchoredPosition != target)
        {
            Vector2 before = malutyimageRect.anchoredPosition;

            malutyimageRect.anchoredPosition =
                Vector2.MoveTowards(
                    malutyimageRect.anchoredPosition,
                    target,
                    spaceSpeed * Time.deltaTime);

            Vector2 delta =
                malutyimageRect.anchoredPosition - before;

            foreach (RectTransform image in malutiBackGroundMover._images)
            {
                image.anchoredPosition += delta;
            }

            yield return null;
        }
    }
}
