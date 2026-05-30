using Domain.Common;

namespace Domain.Users;

public static class UserErrors
{
    public static Error NotFound => Error.Validation("User.NotFound", "Specified user cannot be found."); 
}
