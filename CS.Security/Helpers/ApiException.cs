namespace CS.Security.Helpers;

public class ApiException : Exception
{
    public int Status { get; set; }
    public string Detail { get; set; }

    public ApiException(int status, string detail)
    {
        Status = status;
        Detail = detail;
    }
    
    public ApiException(int status, string detail, string msg) : base(msg)
    {
        Status = status;
        Detail = detail;
    }
}