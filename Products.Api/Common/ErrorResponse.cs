namespace Products.Api.Common;

public class ErrorResponse
{
    public string Type { get; set; }
    public int Status { get; set; }
    public string Code { get; set; }
    public string Title { get; set; }
    public string Detail { get; set; }
    public string Instance { get; set; }
    public string TraceId { get; set; }
}
