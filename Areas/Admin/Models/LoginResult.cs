namespace Corno.Web.Areas.Admin.Models;

public enum LoginResult
{
    Success,
    LockedOut,
    RequiresVerification,
    Failure
}