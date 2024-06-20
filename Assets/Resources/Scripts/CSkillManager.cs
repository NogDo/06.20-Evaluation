using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using Newtonsoft.Json;

public class CSkillManager : MonoBehaviour
{
    #region public 변수
    public List<SkillData> skillData;
    #endregion

    #region private 변수
    static CSkillManager instance;
    #endregion

    public static CSkillManager Instance
    {
        get
        {
            if (instance == null)
            {
                instance = FindObjectOfType<CSkillManager>();
            }

            return instance;
        }
    }

    void Awake()
    {
        instance = this;
    }

    void OnApplicationQuit()
    {
        Save();
    }

    /// <summary>
    /// 스킬 정보 저장
    /// </summary>
    public void Save()
    {
        string path = $"{Application.streamingAssetsPath}/SkillData.json";
        string json = JsonConvert.SerializeObject(skillData);

        File.WriteAllText(path, json);
    }

    /// <summary>
    /// 저장된 스킬 정보 불러오기
    /// </summary>
    public void Load()
    {
        DirectoryInfo directoryInfo = new DirectoryInfo(Application.streamingAssetsPath);

        FileInfo[] file = directoryInfo.GetFiles("SkillData.json");
        string json = File.ReadAllText(file[0].FullName);

        skillData = JsonConvert.DeserializeObject<List<SkillData>>(json);
    }
}

[System.Serializable]
public class SkillData
{
    public float fAttack;
    public float fShootCoolTime;
    public int nUseLevel;
    public int nBulletCount;
}