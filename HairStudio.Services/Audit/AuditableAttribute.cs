namespace HairStudio.Services.Audit
{
    [AttributeUsage(AttributeTargets.Method)]
    public class AuditableAttribute : Attribute
    {
        public string Action { get; }

        public AuditableAttribute(string action)
        {
            Action = action;
        }
    }
}
