using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CEnemy : MonoBehaviour
{
    #region private 변수
    float fHp;
    float fAttack;
    #endregion

    #region public 변수
    public Queue<GameObject> enemyPool;
    #endregion

    void OnEnable()
    {
        float randX = Random.Range(-20.0f, 20.0f);
        float randZ = Random.Range(-20.0f, 20.0f);

        fHp = 30.0f;

        transform.localPosition = new Vector3(randX, 1.0f, randZ);
    }

    void OnDisable()
    {
        enemyPool.Enqueue(gameObject);
    }

    /// <summary>
    /// 적 피격
    /// </summary>
    /// <param name="damage">받은 데미지</param>
    public void Hit(float damage)
    {
        fHp -= damage;

        if (fHp <= 0)
        {
            Die();
        }
    }

    /// <summary>
    /// 적 죽음
    /// </summary>
    public void Die()
    {
        FindObjectOfType<CPlayerController>().LevelUp();
        gameObject.SetActive(false);
    }
}
