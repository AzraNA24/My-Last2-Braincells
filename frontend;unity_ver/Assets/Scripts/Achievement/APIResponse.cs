[System.Serializable]
public class BaseApiResponse
{
    public bool success;
    public string message;
}

[System.Serializable]
public class AllAchievementsResponse : BaseApiResponse
{
    public Achievement[] payload;
}

[System.Serializable]
public class UserAchievementsResponse : BaseApiResponse
{
    public Achievement[] payload;
}