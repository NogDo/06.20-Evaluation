using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using System.IO;
using Newtonsoft.Json;

[System.Serializable]
public struct STPlayerStats
{
    public float fAttack;
    public int nLevel;
}

public class CPlayerController : MonoBehaviour
{
    #region private ����
    CharacterController characterController;
    SkillData skill;
    GameObject oLevelUpEffect;

    Vector3 v3MoveDirection;
    STPlayerStats playerStats = new STPlayerStats();

    float fMoveSpeed;
    float fRotateSpeed;
    float fJumpPower;
    float fGravity;
    bool isAttackCooltime;
    #endregion

    #region public ����
    public Transform shootTransform;
    public CBulletControl bulletPrefab;
    public UnityEvent<string> changeLevel;
    #endregion

    void Awake()
    {
        characterController = GetComponent<CharacterController>();
        oLevelUpEffect = transform.GetChild(4).gameObject;

        fMoveSpeed = 6.5f;
        fRotateSpeed = 60.0f;
        fJumpPower = 10.0f;
        fGravity = 20.0f;
        isAttackCooltime = false;

        Init();
    }

    void Update()
    {
        if (!characterController.isGrounded)
        {
            Gravity();
        }

        else
        {
            if (Input.GetKeyDown(KeyCode.Space))
            {
                Jump();
            }
        }

        Move();
        Rotate();

        if (Input.GetMouseButtonDown(0) && !isAttackCooltime)
        {
            Attack();
        }
    }

    void OnApplicationQuit()
    {
        Save();
    }

    /// <summary>
    /// �÷��̾� ���� �ʱ�ȭ
    /// </summary>
    void Init()
    {
        if (!Load())
        {
            playerStats.nLevel = 1;
            playerStats.fAttack = 5.0f;
        }

        changeLevel?.Invoke($"LV. {playerStats.nLevel}");
        CSkillManager.Instance.Load();
        foreach (SkillData skill in CSkillManager.Instance.skillData)
        {
            if (playerStats.nLevel >= skill.nUseLevel)
            {
                this.skill = skill;
            }
        }
    }

    /// <summary>
    /// �÷��̾� ���� ����
    /// </summary>
    void Save()
    {
        string path = $"{Application.streamingAssetsPath}/PlayerStats.json";
        string json = JsonConvert.SerializeObject(playerStats);

        File.WriteAllText(path, json);
    }

    /// <summary>
    /// �÷��̾� ���� �ε�
    /// </summary>
    bool Load()
    {
        DirectoryInfo directoryInfo = new DirectoryInfo(Application.streamingAssetsPath);

        FileInfo[] file = directoryInfo.GetFiles("PlayerStats.json");

        if (file.Length != 0)
        {
            string json = File.ReadAllText(file[0].FullName);

            playerStats = JsonConvert.DeserializeObject<STPlayerStats>(json);

            return true;
        }

        else
        {
            return false;
        }
    }

    /// <summary>
    /// �÷��̾� �̵�
    /// </summary>
    void Move()
    {
        float horizontal = Input.GetAxis("Horizontal");
        float vertical = Input.GetAxis("Vertical");

        float y = v3MoveDirection.y;

        v3MoveDirection = new Vector3(horizontal, 0, vertical);
        v3MoveDirection = transform.TransformDirection(v3MoveDirection) * fMoveSpeed;
        v3MoveDirection.y = y;

        characterController.Move(v3MoveDirection * Time.deltaTime);
    }

    /// <summary>
    /// �÷��̾� ȸ��
    /// </summary>
    void Rotate()
    {
        float horizontal = Input.GetAxis("Mouse X");

        Vector3 rotation = new Vector3(0.0f, horizontal * fRotateSpeed * Time.deltaTime, 0.0f);

        transform.Rotate(rotation);
    }

    /// <summary>
    /// �߷�
    /// </summary>
    void Gravity()
    {
        v3MoveDirection.y -= fGravity * Time.deltaTime;
    }

    /// <summary>
    /// �÷��̾� ����
    /// </summary>
    void Jump()
    {
        v3MoveDirection.y = fJumpPower;
    }

    /// <summary>
    /// �÷��̾� ����, ������ �ִ� ��ų�� ����Ѵ�. ������ ���� ��ų�� �޶���
    /// </summary>
    void Attack()
    {
        isAttackCooltime = true;
        StartCoroutine(AttackCooltime());
        StartCoroutine(ShootBullet());
    }

    /// <summary>
    /// �÷��̾� �����̵�
    /// </summary>
    /// <param name="targetPosition">������ Position</param>
    public void Teleport(Vector3 targetPosition)
    {
        characterController.enabled = false;
        transform.position = targetPosition;
        characterController.enabled = true;
    }

    /// <summary>
    /// �÷��̾� ������. Ư�� ������ �޼��ϸ� ��ų�� �ٲ��.
    /// </summary>
    public void LevelUp()
    {
        playerStats.nLevel++;
        playerStats.fAttack += 5.0f;
        changeLevel?.Invoke($"LV. {playerStats.nLevel}");

        oLevelUpEffect.SetActive(false);
        oLevelUpEffect.SetActive(true);

        foreach (SkillData skill in CSkillManager.Instance.skillData)
        {
            if (playerStats.nLevel >= skill.nUseLevel)
            {
                this.skill = skill;
            }
        }
    }

    /// <summary>
    /// ���� ��Ÿ�� (��ų�� ������ �ִ� ��Ÿ���� ������ �ٽ� ������ �� �ִ�.)
    /// </summary>
    /// <returns></returns>
    IEnumerator AttackCooltime()
    {
        yield return new WaitForSeconds(skill.fShootCoolTime);
        isAttackCooltime = false;
    }

    /// <summary>
    /// �Ѿ� �߻�
    /// </summary>
    /// <returns></returns>
    IEnumerator ShootBullet()
    {
        for (int i = 0; i < skill.nBulletCount; i++)
        {
            CBulletControl bullet = Instantiate(bulletPrefab, shootTransform.position, shootTransform.rotation);
            bullet.fDamage = skill.fAttack + playerStats.fAttack;

            Destroy(bullet, 3.0f);
            yield return new WaitForSeconds(0.1f);
        }
    }
}
