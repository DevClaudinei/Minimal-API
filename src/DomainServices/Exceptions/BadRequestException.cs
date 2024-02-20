using System;

namespace DomainServices.Exceptions;

public class BadRequestException : Exception
{
	public BadRequestException(string errorMessage) : base(errorMessage) { }
}