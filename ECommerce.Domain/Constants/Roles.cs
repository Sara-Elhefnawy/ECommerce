namespace ECommerce.Domain.Constants;

public static class Roles
{
    public const string Manager = "Mangager";
    public const string SuperAdmin = "SuperAdmin";
    public const string Admin = "Admin";
    public const string User = "User";

    public static readonly IReadOnlyList<string> All =
    [
        Manager,
        SuperAdmin,
        Admin,
        User
    ];
}
