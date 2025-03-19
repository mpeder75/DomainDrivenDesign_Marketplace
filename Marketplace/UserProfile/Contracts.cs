namespace Marketplace.UserProfile;

public class Contracts
{
    public static class V1
    {
        public record RegisterUser
        {
            public Guid UserId { get; set; }
            public string FullName { get; set; }
            public string DisplayName { get; set; }
        }

        public record UpdateUserFullName
        {
            public Guid UserId { get; set; }
            public string FullName { get; set; }
        }

        public record UpdateUserDisplayName
        {
            public Guid UserId { get; set; }
            public string DisplayName { get; set; }
        }

        public record UpdateUserProfilePhoto
        {
            public Guid UserId { get; set; }
            public string PhotoUrl { get; set; }
        }

    }
}