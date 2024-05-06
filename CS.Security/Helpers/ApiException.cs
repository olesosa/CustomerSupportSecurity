namespace CS.Security.Helpers;

public class ApiException : Exception//not used
{
    public int Status { get; set; }
    public string Detail { get; set; }

    public ApiException(int status, string detail) : base()
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