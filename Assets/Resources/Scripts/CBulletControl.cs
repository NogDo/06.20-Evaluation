using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CBulletControl : MonoBehaviour
{
    #region private ����
    Rigidbody rb;

    float fShootPower;
    #endregion

    #region public ����
    public float fDamage;
    #endregion

    void Awake()
    {
        fShootPower = 50.0f;
        rb = GetComponent<Rigidbody>();
    }

    void OnEnable()
    {
        rb.AddForce(transform.forward * fShootPower, ForceMode.Impulse);
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Bullet"))
        {
            return;
        }

        if (other.TryGetComponent<CEnemy>(out CEnemy enemy))
        {
            enemy.Hit(fDamage);
        }

        Destroy(gameObject);
    }
}