namespace ShangZhu.Dtos
{
    internal class ApplicationSettingDto
    {
        public string Key { get; set; }

        public string Value { get; set; }

        public int ExpiresInSeconds { get; set; }
    }
}