namespace PsqlSharp
{
    public record ConnectionData
    {
        public string Username { get; set; }

        public string Password { get; set; }

        public string Database { get; set; }

        public string Host { get; set; } = "localhost";

        public string Port { get; set; } = "5432";

        public override string ToString()
        {
            return $"Host={Host};Username={Username};Password={Password};Database={Database.ToLower()};Port={Port};Include Error Detail=true";
        }

    }
}
