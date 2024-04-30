namespace CS.Security.Helpers;

public class AuthException : Exception
{
    public int Status { get; set; }
    public string Detail { get; set; }

    public AuthException(int status, string detail)
    {
        Status = status;
        Detail = detail;
    }
    
    public AuthException(int status, string detail, string msg) : base(msg)
    {
        Status = status;
        Detail = detail;
    }
}