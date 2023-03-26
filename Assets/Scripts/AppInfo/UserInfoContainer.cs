namespace AppInfo
{
    public class UserInfoContainer
    {
        public string Id { get; set; }
        public string DeviceId { get; set; }
        public UserInfoAuth UserInfoAuth;
        public UserInfo UserInfo;

        public UserInfoContainer()
        {
            UserInfo = new UserInfo();

            UserInfoAuth = new UserInfoAuth();
        }
    }

    public class UserInfo
    {
        public string Name { get; set; }
        public string Lang { get; set; }
        public string Login { get; set; }
        public string userId { get; set; }

        // public bool IsFacebook => string.IsNullOrWhiteSpace(FacebookId) == false;
    }

    public class UserInfoAuth
    {
        public string AccessToken { get; set; }
        public string RefreshToken { get; set; }
    }
}