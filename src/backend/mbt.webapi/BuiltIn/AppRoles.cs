namespace mbt.webapi.BuiltIn;

public static class AppRoles
{
    public const string SysAdmins = "SystemAdmins";
    public const string Admins = "Admins";
    public const string Designers = "Designers";
    public const string Contributors = "Contributors";
    public const string GlobalApprovers = "GlobalApprovers";
    public const string PaidMedia = "PaidMedia";

    public const string Viewers = "Viewers";


    //
    public const string ManageGroupedActivities = $"{SysAdmins},{Admins},{Designers},{Contributors}";

    //
    public const string PaidMediaRolePolicy = $"{SysAdmins},{PaidMedia}";

    //
    public const string AdminPolicy = $"{SysAdmins},{Admins}";

    //
    public const string ViewPolicy =
        $"{SysAdmins},{Admins},{Designers},{Contributors},{Viewers},{GlobalApprovers},{PaidMedia}";
}
