namespace Livefront.Referrals.DataAccess.Exceptions;

public class DataPersistenceException : Exception
{
    public DataPersistenceException(string message) : base(message)
    {
    }
    
    public DataPersistenceException(string message, Exception? innerException) : base(message, innerException)
    {
    }
}