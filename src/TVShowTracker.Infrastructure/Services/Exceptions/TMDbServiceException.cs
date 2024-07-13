namespace TVShowTracker.Infrastructure.Services.Exceptions;

public class TMDbServiceException : Exception
{
    public TMDbServiceException(string message, Exception innerException) : base(message, innerException) { }
}