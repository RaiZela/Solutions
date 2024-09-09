using System.Reflection;


namespace Solutions.Messages;

public static class MessageKeys
{
    static MessageKeys()
    {
        InitializeProperties();
    }

    static void InitializeProperties()
    {
        PropertyInfo[] properties = typeof(MessageKeys).GetProperties(BindingFlags.Public | BindingFlags.Static);

        foreach (var property in properties)
        {
            if (property.PropertyType == typeof(string))
            {
                property.SetValue(null, property.Name);
            }
        }
    }
    public static string? Empty_Model { get; set; }
    public static string? Nonexistent_User { get; set; }
    public static string? Invalid_Credentials { get; set; }
    public static string? Role_Not_Found { get; set; }
    public static string? Login_Success { get; set; }
    public static string? Something_Went_Wrong { get; set; }
    public static string? User_Exists { get; set; }
    public static string? Account_Created { get; set; }
    public static string? Refresh_Token_Not_Found { get; set; }
    public static string? User_Missing_For_Refresh_Token_Generation { get; set; }
    public static string? The_User_Has_Not_Signed_In { get; set; }
    public static string? Token_Refreshed_Successfully { get; set; }
    public static string? Http_Connection_Error { get; set; }
    public static string? Not_Found { get; set; }
    public static string? Reference_Value_Already_Exists { get; set; }

}