using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CEnemy : MonoBehaviour
{
    #region private ����
    float fHp;
    float fAttack;
    #endregion

    #region public ����
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
    /// �� �ǰ�
    /// </summary>
    /// <param name="damage">���� ������</param>
    public void Hit(float damage)
    {
        fHp -= damage;

        if (fHp <= 0)
        {
            Die();
        }
    }

    /// <summary>
    /// �� ����
    /// </summary>
    public void Die()
    {
        FindObjectOfType<CPlayerController>().LevelUp();
        gameObject.SetActive(false);
    }
}
