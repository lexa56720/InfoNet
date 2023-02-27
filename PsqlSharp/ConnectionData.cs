using System.ComponentModel.DataAnnotations;

namespace PsqlSharp
{
    public record ConnectionData
    {
        [Required]
        public string Username { get; set; }

        [Required]
        public string Password { get; set; }

        [Required]
        public string Database { get; set; }

        [Required]
        public string Host { get; set; } = "localhost";

        [Required]
        public string Port { get; set; } = "5432";

        public override string ToString()
        {
            return $"Host={Host};Username={Username};Password={Password};Database={Database.ToLower()};Port={Port};Include Error Detail=true";
        }

    }
}
