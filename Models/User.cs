namespace TafRestSharpWireMock.Tests.Models
{
    public class User
    {
        public int Id { get; set; }
        public string Email { get; set; }
        public string First_Name { get; set; }
        public string Last_Name { get; set; }
        public string Avatar { get; set; }
    }

    public class UserData
    {
        public User Data { get; set; }
        public Support Support { get; set; }
    }

    public class UserListData
    {
        public int Page { get; set; }
        public int Per_Page { get; set; }
        public int Total { get; set; }
        public int Total_Pages { get; set; }
        public User[] Data { get; set; }
        public Support Support { get; set; }
    }

    public class Support
    {
        public string Url { get; set; }
        public string Text { get; set; }
    }

    public class CreateUserRequest
    {
        public string Name { get; set; }
        public string Job { get; set; }
    }

    public class CreateUserResponse
    {
        public string Name { get; set; }
        public string Job { get; set; }
        public string Id { get; set; }
        public string CreatedAt { get; set; }
    }

    public class UpdateUserResponse
    {
        public string Name { get; set; }
        public string Job { get; set; }
        public string UpdatedAt { get; set; }
    }
}
