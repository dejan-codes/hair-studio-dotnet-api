using System.Reflection;
using System.Text.Json;
using Castle.DynamicProxy;
using Microsoft.Extensions.Logging;

namespace HairStudio.Services.Audit
{
    public class AuditInterceptor : Castle.DynamicProxy.IInterceptor
    {
        private readonly ILogger<AuditInterceptor> _logger;
        private static readonly string[] SensitiveKeywords = new[]
        {
            "password", "token", "code", "secret", "key"
        };

        public AuditInterceptor(ILogger<AuditInterceptor> logger)
        {
            _logger = logger;
        }

        public void Intercept(IInvocation invocation)
        {
            var method = invocation.MethodInvocationTarget ?? invocation.Method;
            var auditable = method.GetCustomAttributes(typeof(AuditableAttribute), true)
                                  .Cast<AuditableAttribute>()
                                  .FirstOrDefault();

            if (auditable == null)
            {
                invocation.Proceed();
                return;
            }

            try
            {
                invocation.Proceed();

                if (invocation.ReturnValue is Task task)
                {
                    task.ContinueWith(t =>
                    {
                        if (t.IsFaulted)
                        {
                            LogFailure(auditable, method, invocation, t.Exception);
                        }
                        else
                        {
                            LogSuccess(auditable, method, invocation);
                        }
                    });
                }
                else
                {
                    LogSuccess(auditable, method, invocation);
                }
            }
            catch (Exception ex)
            {
                LogFailure(auditable, method, invocation, ex);
                throw;
            }
        }

        private void LogSuccess(AuditableAttribute auditable, MethodInfo method, IInvocation invocation)
        {
            var safeParams = SanitizeArguments(invocation.Arguments);
            _logger.LogInformation("[AUDIT] {Action} executed successfully | Method: {Method} | Params: {Params}",
                auditable.Action, method.Name, safeParams);
        }

        private void LogFailure(AuditableAttribute auditable, MethodInfo method, IInvocation invocation, Exception? ex)
        {
            var safeParams = SanitizeArguments(invocation.Arguments);
            _logger.LogError(ex, "[AUDIT] {Action} failed | Method: {Method} | Params: {Params} | Error: {Message}",
                auditable.Action, method.Name, safeParams, ex?.Message);
        }

        private string SanitizeArguments(object?[] args)
        {
            try
            {
                var serialized = args.Select(arg =>
                {
                    if (arg == null) return "null";

                    var type = arg.GetType();
                    if (type.IsClass && type != typeof(string))
                    {
                        var properties = type.GetProperties(BindingFlags.Public | BindingFlags.Instance)
                            .ToDictionary(
                                p => p.Name,
                                p =>
                                {
                                    try
                                    {
                                        var value = p.GetValue(arg);
                                        if (value == null) return null;

                                        if (SensitiveKeywords.Any(k => p.Name.ToLower().Contains(k)))
                                            return "***MASKED***";

                                        return value;
                                    }
                                    catch
                                    {
                                        return "***ERROR***";
                                    }
                                });

                        return JsonSerializer.Serialize(properties);
                    }

                    return arg.ToString();
                });

                return string.Join(", ", serialized);
            }
            catch (Exception ex)
            {
                return $"[Audit parameter logging failed: {ex.Message}]";
            }
        }
    }
}
