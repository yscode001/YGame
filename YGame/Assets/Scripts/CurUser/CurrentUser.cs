using System;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;
using YUnity;

#region 属性
[Serializable]
public partial class CurrentUser
{
    public int id = 0;
    public string name = "";
    public string avatar = "";
    public int gender = (int)TGender.none;

    public string hair = "";
    public string top = "";
    public string pants = "";
    public string shoes = "";
}
#endregion
#region 辅助属性
public partial class CurrentUser
{
    public bool IsEmpty => id <= 0;
    public bool IsNotEmpty => id > 0;

    public bool IsMale => gender == (int)TGender.male;
    public bool IsNotMale => gender != (int)TGender.male;

    public bool IsFemale => gender == (int)TGender.female;
    public bool IsNotFemale => gender != (int)TGender.female;
}
#endregion
#region 单例及持久化方法
public partial class CurrentUser
{
    private static readonly string LocalPath = Application.persistentDataPath + "/CurrentUser.fun";
    private CurrentUser() { }
    private static readonly Lazy<CurrentUser> _instance = new Lazy<CurrentUser>(() =>
    {
        CurrentUser user = GetFromLocal();
        if (user != null) { return user; }
        else
        {
            user = new CurrentUser();
            user.Save();
            return user;
        }
    });
    public static CurrentUser Instance => _instance.Value;
    private static CurrentUser GetFromLocal()
    {
        try
        {
            if (File.Exists(LocalPath))
            {
                BinaryFormatter formatter = new BinaryFormatter();
                FileStream stream = new FileStream(LocalPath, FileMode.Open);
                CurrentUser user = formatter.Deserialize(stream) as CurrentUser;
                stream.Close();
                return user;
            }
        }
        catch
        {
            FileUtil.DeleteFile(LocalPath);
        }
        return null;
    }

    public void Save()
    {
        try
        {
            BinaryFormatter formatter = new BinaryFormatter();
            FileStream stream = new FileStream(LocalPath, FileMode.OpenOrCreate);
            formatter.Serialize(stream, this);
            stream.Close();
        }
        catch
        {
            FileUtil.DeleteFile(LocalPath);
        }
    }
}
#endregion
#region 清理数据
public partial class CurrentUser
{
    /// 清理数据
    public void ClearData()
    {
        id = 0;
        name = "";
        avatar = "";
        gender = 1;
        hair = "";
        top = "";
        pants = "";
        shoes = "";
        Save();
    }
}
#endregion