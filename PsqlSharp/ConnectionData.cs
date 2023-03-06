using System.ComponentModel.DataAnnotations;

namespace PsqlSharp
{
    public record ConnectionData
    {
        public string Username { get; set; } = string.Empty;

        public string Password { get; set; } = string.Empty;

        public string Database { get; set; } = string.Empty;

        public string Host { get; set; } = string.Empty;

        public string Port { get; set; } = string.Empty;

        public override string ToString()
        {
            return $"Host={Host};Username={Username};Password={Password};Database={Database.ToLower()};Port={Port};Include Error Detail=true";
        }

    }
}
