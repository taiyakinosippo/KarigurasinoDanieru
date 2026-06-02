using System.Collections;
using UnityEngine;

/// <summary>
/// ロケットの動きを制御するクラス。プレイヤーの状態に応じて、ロケットの動きを変える。
/// </summary>
using UnityEngine.UI;
using static UnityEngine.GraphicsBuffer;

public class Rocket_Mover : MonoBehaviour
{
    [SerializeField] private BackGroundMover backGroundMover;      //背景を動かすクラス
    [SerializeField] private float missUpMove = 20f;               //0～1000メートルのの時のロケットの動き(上に飛ぶ)
    [SerializeField] private float missDownMove = 50f;             //0～1000メートルのの時のロケットの動き(下に落ちる)
    [SerializeField] private float skyMove = 100f;                 //1000～10000メートルまでの時のロケットの動き(右に飛ぶ) 
    [SerializeField] private float atmosphereRotate = 360f;        //10000～100000メートルのの時のロケットの動き(回転する)
    [SerializeField] private float missMoveSpeed = 10f;            //0～1000メートルのの時のロケットの動きの速さ
    [SerializeField] private float skyMoveSpeed = 10f;             //1000～10000メートルまでの時のロケットの動きの速さ
    [SerializeField] private float spaceSpeed = 100f;              //100000メートル以上の時の背景のスクロールの速さ
    [SerializeField] RectTransform imageRect;                      //100000メートル以上の時の背景画像

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
        StartCoroutine(GalaxyMoveSpaceCoroution());
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
    }

    //1000～10000メートルまでの時のロケットの動き
    private IEnumerator SkyRocketMoveCoroutine()
    {
        Vector2 target = new Vector2(transform.position.x + skyMove, transform.position.y);
        while ((Vector2)transform.position != target)
        {
            transform.position = Vector3.MoveTowards(
                    transform.position,
                    target,
                    skyMoveSpeed * Time.deltaTime);
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
        RectTransform lowest = backGroundMover.GetLowest();

        imageRect.anchoredPosition =
            new Vector2(
                lowest.anchoredPosition.x,
                lowest.anchoredPosition.y - imageRect.rect.height);

        Vector2 target =
            new Vector2(
                imageRect.anchoredPosition.x,
                0f);

        while (imageRect.anchoredPosition != target)
        {
            Vector2 before = imageRect.anchoredPosition;

            imageRect.anchoredPosition =
                Vector2.MoveTowards(
                    imageRect.anchoredPosition,
                    target,
                    spaceSpeed * Time.deltaTime);

            
            Vector2 delta = imageRect.anchoredPosition - before;

           
            foreach (RectTransform image in backGroundMover._images)
            {
                image.anchoredPosition += delta;
            }

            yield return null;
        }
    }
}
