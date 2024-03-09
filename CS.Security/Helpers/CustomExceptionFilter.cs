using System.Net;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace CS.Security.Servises;

public class GlobalExceptionFilter : IExceptionFilter
{
    public void OnException(ExceptionContext context)
    {
        switch (context.Exception)
        {
            case HttpRequestException:
                context.Result = new BadRequestResult();
                break;
            case Exception:
                context.Result = new Exception();
                break;
        }
    }
}