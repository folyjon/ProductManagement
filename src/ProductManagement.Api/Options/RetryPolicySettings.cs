namespace ProductManagement.Api.Options;

public class RetryPolicySettings
{
    public const string SectionName = "RetryPolicy";
    public int RetryCount { get; set; }
    public int RetryDelayMilliseconds { get; set; }
}