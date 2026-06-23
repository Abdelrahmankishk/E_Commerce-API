namespace E_Commerce.Application.Common
{
    public sealed record Error (string Code,string Description, ErrorType ErrorType = ErrorType.Failure)
    {
        public static Error Failure(string code = "General.Failure", string description= "General Failure has Occurred")
        {
            return new Error(code, description, ErrorType.Failure);
        }
        public static Error Validation(string code = "General.Validation", string description= "General Validation Error has Occurred")
        {
            return new Error(code, description, ErrorType.Validation);
        }
        public static Error NotFound(string code = "General.NotFound", string description= "General NotFound Error has Occurred")
        {
            return new Error(code, description, ErrorType.NotFound);
        }
        public static Error Conflict(string code = "General.Conflict", string description= "General Conflict has Occurred")
        {
            return new Error(code, description, ErrorType.Conflict);
        }
        public static Error Unauthorized(string code = "General.Unauthorized", string description= "Access Is Denied")
        {
            return new Error(code, description, ErrorType.Unauthorized);
        }
        public static Error Forbidden(string code = "General.Forbidden", string description= "This Operation is Forbidden")
        {
            return new Error(code, description, ErrorType.Forbidden);
        }
        public static Error InvalidCredentials(string code = "General.InvalidCredentials", string description= "Invalid Credentials")
        {
            return new Error(code, description, ErrorType.InvalidCredentials);
        }
    }
    public enum ErrorType { 
        Failure = 0,
        Validation = 1,
        NotFound = 2,
        Conflict = 3,
        Unauthorized = 4,
        Forbidden = 5,
        InvalidCredentials = 6,
    }
}