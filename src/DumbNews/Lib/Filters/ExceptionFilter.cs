namespace DumbNews.Lib.Filters
{
    using Microsoft.AspNet.Mvc;
    using Microsoft.AspNet.Mvc.Filters;
    using Microsoft.Extensions.Logging;
    using Response;
    using System;

    public class ExceptionFilter : IExceptionFilter, IDisposable
    {
        private readonly ILogger _logger;
        private bool _disposed;

        public ExceptionFilter(ILoggerFactory logger)
        {
            if (logger == null)
            {
                throw new ArgumentNullException(nameof(logger));
            }

            this._logger = logger.CreateLogger("Exception Filter");
        }

        public void Dispose()
        {
            if(this._disposed)
            {
                return;
            }
            this._disposed = true;
        }

        public void OnException(ExceptionContext context)
        {
            var response = new ErrorResponse()
            {
                Message = context.Exception.Message
            };
#if DEBUG
            response.StackTrace = context.Exception.StackTrace;
#endif
            context.Result = new ObjectResult(response)
            {
                StatusCode = 500,
                DeclaredType = typeof(ErrorResponse)
            };

            this._logger.LogError("ExceptionFilter", context.Exception);
        }
    }
}
