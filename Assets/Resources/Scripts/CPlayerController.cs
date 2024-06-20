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
    #region private 변수
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

    #region public 변수
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
    /// 플레이어 정보 초기화
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
    /// 플레이어 정보 저장
    /// </summary>
    void Save()
    {
        string path = $"{Application.streamingAssetsPath}/PlayerStats.json";
        string json = JsonConvert.SerializeObject(playerStats);

        File.WriteAllText(path, json);
    }

    /// <summary>
    /// 플레이어 정보 로드
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
    /// 플레이어 이동
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
    /// 플레이어 회전
    /// </summary>
    void Rotate()
    {
        float horizontal = Input.GetAxis("Mouse X");

        Vector3 rotation = new Vector3(0.0f, horizontal * fRotateSpeed * Time.deltaTime, 0.0f);

        transform.Rotate(rotation);
    }

    /// <summary>
    /// 중력
    /// </summary>
    void Gravity()
    {
        v3MoveDirection.y -= fGravity * Time.deltaTime;
    }

    /// <summary>
    /// 플레이어 점프
    /// </summary>
    void Jump()
    {
        v3MoveDirection.y = fJumpPower;
    }

    /// <summary>
    /// 플레이어 공격, 가지고 있는 스킬을 사용한다. 레벨에 따라 스킬이 달라짐
    /// </summary>
    void Attack()
    {
        isAttackCooltime = true;
        StartCoroutine(AttackCooltime());
        StartCoroutine(ShootBullet());
    }

    /// <summary>
    /// 플레이어 순간이동
    /// </summary>
    /// <param name="targetPosition">목적지 Position</param>
    public void Teleport(Vector3 targetPosition)
    {
        characterController.enabled = false;
        transform.position = targetPosition;
        characterController.enabled = true;
    }

    /// <summary>
    /// 플레이어 레벨업. 특정 레벨을 달서하면 스킬이 바뀐다.
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
    /// 공격 쿨타임 (스킬이 가지고 있는 쿨타임이 지나야 다시 공격할 수 있다.)
    /// </summary>
    /// <returns></returns>
    IEnumerator AttackCooltime()
    {
        yield return new WaitForSeconds(skill.fShootCoolTime);
        isAttackCooltime = false;
    }

    /// <summary>
    /// 총알 발사
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
