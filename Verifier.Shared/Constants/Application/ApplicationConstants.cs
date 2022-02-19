namespace Verifier.Shared.Constants.Application
{
    public static class ApplicationConstants
    {
        public static class AppSettings
        {
            public const string AppVersion = "AppVersion";
            public const string CachedDuration = "CachedDuration";
            public const string SendGridAccountActivationEmailTemplate = "SendGridAccountActivationEmailTemplate";
            public const string SendGridForgotPasswordEmailTemplate = "SendGridForgotPasswordEmailTemplate";
        }

        public static class Redis
        {
            public const string AppSettings = "AppSettings";
        }

        public static class AppMessages
        {
            public const string DocumentUploadedSuccessfully = "Document uploaded successfully";
        }
        
        public static class User
        {
            public const string AdminUsername = "admin";
            public const string BasicUsername = "basic";
            public const string AdminEmail = "admin@theterminalsample.com";
            public const string AdminFirstName = "Admin";
            public const string AdminLastName = "User";
            public const string BasicUserEmail = "basic@theterminalsample.com";
            public const string BasicUserFirstName = "BasicUser";
            public const string BasicUserLastName = "User";
        }
        
        
        public static class DatabaseConfiguration
        {
            public const string CaseInsensitiveCollation = "TheTerminalColumnCollation";
            public const string Locale = "en-u-ks-primary";
            public const string Provider = "icu";
            public const string DefaultCollation = "default";
        }

        public static class DataSeeder
        {
            public const string ShouldSeedDefaultData = "ShouldSeedDefaultData";
        }

        public static class FileType
        {
            public const string Csv = "text/csv";
            public const string Xlsx = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
        }
    }
}