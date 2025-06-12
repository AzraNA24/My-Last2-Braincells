[System.Serializable]
public class Achievement
{
    public int id;
    public string name;
    public string description;
    public string unlocked_at;
    public bool unlocked;
}

[System.Serializable]
public class AchievementList
{
    public Achievement[] achievements;
}