using System;

namespace Planora.Web.ViewModels;

public class AuditLogViewModel
{
    public int Id { get; set; }
    public string? UserName { get; set; }
    public string Type { get; set; } = string.Empty;
    public string TableName { get; set; } = string.Empty;
    public DateTime DateTime { get; set; }
    public string? OldValues { get; set; }
    public string? NewValues { get; set; }
    public string? AffectedColumns { get; set; }
    public string PrimaryKey { get; set; } = string.Empty;
}
