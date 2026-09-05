namespace MS_Domain.Common
{
    public static class DateTimeHelper
    {
        private static readonly TimeSpan VietnamOffset = TimeSpan.FromHours(7);

        public static DateTime VnNow => DateTime.SpecifyKind(DateTime.UtcNow.Add(VietnamOffset), DateTimeKind.Unspecified);
    }
}
