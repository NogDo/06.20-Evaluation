using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CEnemyPool : MonoBehaviour
{
    #region private ����
    Queue<GameObject> enemyPool;
    #endregion

    #region public ����
    public GameObject oEnemyPrefab;
    #endregion

    void Awake()
    {
        enemyPool = new Queue<GameObject>();

        for (int i = 0; i < 10; i++)
        {
            GameObject enemy = Instantiate(oEnemyPrefab, transform);
            enemy.GetComponent<CEnemy>().enemyPool = enemyPool;
            enemy.SetActive(false);
        }

        StartCoroutine(SpawnEnemy());
    }

    /// <summary>
    /// �� ��ȯ �ڷ�ƾ
    /// </summary>
    /// <returns></returns>
    IEnumerator SpawnEnemy()
    {
        while (true)
        {
            if (enemyPool.Count != 0)
            {
                enemyPool.Dequeue().SetActive(true);
            }

            else
            {
                GameObject enemy = Instantiate(oEnemyPrefab, transform);
                enemy.GetComponent<CEnemy>().enemyPool = enemyPool;
            }

            yield return new WaitForSeconds(5.0f);
        }
    }
}