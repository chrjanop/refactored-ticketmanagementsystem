using System;
namespace TicketManagementSystem
{
    public class UnknownUserException : Exception
    {
        public UnknownUserException(string message) : base(message)
        {
        }
    }
}
