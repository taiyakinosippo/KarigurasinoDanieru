using System.Collections;
using UnityEngine;

/// <summary>
/// ロケットの動きを制御するクラス。プレイヤーの状態に応じて、ロケットの動きを変える。
/// </summary>

public class Rocket_Mover : MonoBehaviour
{
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
    public void SpaceRocketMove()
    {
        StartCoroutine(SpaceMoveRocketCoroutine());
    }

    //0～500メートルのの時のロケットの動き
    private IEnumerator MissRocketMoveCoroutine()
    {
        Vector2 uPtarget = new Vector2(transform.position.x, transform.position.y + 3f);
        Vector2 downTarget = new Vector2(transform.position.x, transform.position.y - 3f);
        // 上に飛ぶ
        while ((Vector2)transform.position != uPtarget)
        {
            transform.position = Vector2.MoveTowards(
                    transform.position,
                    uPtarget,
                    2f * Time.deltaTime);
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
                    4f * Time.deltaTime);

            yield return null;
        }
    }

    //
    private IEnumerator SkyRocketMoveCoroutine()
    {
        Vector2 target = new Vector2(transform.position.x + 5f, transform.position.y);
        while ((Vector2)transform.position != target)
        {
            transform.position = Vector3.MoveTowards(
                    transform.position,
                    target,
                    2f * Time.deltaTime);
            yield return null;
        }
    }


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


    private IEnumerator SpaceMoveRocketCoroutine()
    {

        Vector2 target = new Vector2(transform.position.x, -3f);
        while ((Vector2)transform.position != target)
        {
            transform.position = Vector3.MoveTowards(
                    transform.position,
                    target,
                    2f * Time.deltaTime);
            yield return null;
        }
    }
}
