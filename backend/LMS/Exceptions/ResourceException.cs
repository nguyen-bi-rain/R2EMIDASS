namespace LMS.Exceptions;

public class ResourceNotFoundException(string message) : Exception(message)
{
}

public class ResourceUniqueException(string message) : Exception(message)
{
}

public class DatabaseBadRequestException(string message) : Exception(message)
{
}
public class DatabaseConflictException(string message) : Exception(message)
{
}