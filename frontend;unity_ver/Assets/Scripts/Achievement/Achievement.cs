[System.Serializable]
public class AchievementResponse
{
    public bool success;
    public string message;
    public Achievement[] payload;
}

[System.Serializable]
public class UserAchievementResponse
{
    public bool success;
    public string message;
    public UserAchievement[] payload;
}

[System.Serializable]
public class Achievement
{
    public int id;
    public string name;
    public string imagePath;
    public string description;
}

[System.Serializable]
public class UserAchievement
{
    public int id;
    public User user;
    public Achievement achievement;
    public string unlockedAt;
}

[System.Serializable]
public class User
{
    public int id;
    public string username;
    public string email;
    public string password;
    public string createdAt;
}