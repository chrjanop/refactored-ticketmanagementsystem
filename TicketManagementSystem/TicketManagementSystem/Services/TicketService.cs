using System;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Text.Json;
using EmailService;

namespace TicketManagementSystem
{
    public class TicketService
    {
        public int CreateTicket(string title, Priority priority, string assignedTo, string description, DateTime created, bool isPayingCustomer)
        {
            // Check if title or description are null or if they are invalid and throw exception
            if (string.IsNullOrWhiteSpace(title) || string.IsNullOrWhiteSpace(description))
            {
                throw new InvalidTicketException("Title or description were null or invalid");
            }

            User user = GetUser(assignedTo);

            if (user == null)
            {
                throw new UnknownUserException("Assigned user " + assignedTo + " not found");
            }

            // We can only elevate priority if it's low or medium
            if (priority != Priority.High)
            {
                var priorityRaised = false;
                if (created < DateTime.UtcNow - TimeSpan.FromHours(1))
                {
                    priority = ElevatePriority(priority);
                    priorityRaised = true;
                }

                var importantTitles = new[] { "Crash", "Important", "Failure" };
                if ((importantTitles.Any(title.Contains) && !priorityRaised))
                {
                    priority = ElevatePriority(priority);
                }
            }

            if (priority == Priority.High)
            {
                var emailService = new EmailServiceProxy();
                emailService.SendEmailToAdministrator(title, assignedTo);
            }

            double price = 0;
            User accountManager = null;
            if (isPayingCustomer)
            {
                // Only paid customers have an account manager.
                accountManager = new UserRepository().GetAccountManager();
                if (priority == Priority.High)
                {
                    price = 100;
                }
                else
                {
                    price = 50;
                }
            }

            var ticket = new Ticket()
            {
                Title = title,
                AssignedUser = user,
                Priority = priority,
                Description = description,
                Created = created,
                PriceDollars = price,
                AccountManager = accountManager
            };

            var id = TicketRepository.CreateTicket(ticket);

            // Return the id
            return id;
        }

        public void AssignTicket(int id, string username)
        {
            User user = GetUser(username);

            if (user == null)
            {
                throw new UnknownUserException("User not found");
            }

            var ticket = TicketRepository.GetTicket(id);

            if (ticket == null)
            {
                throw new ApplicationException("No ticket found for id " + id);
            }

            ticket.AssignedUser = user;

            TicketRepository.UpdateTicket(ticket);
        }

        private User GetUser(string username)
        {
            User user = null;
            using (var ur = new UserRepository())
            {
                if (username != null)
                {
                    user = ur.GetUser(username);
                }
            }

            return user;
        }

        private Priority ElevatePriority(Priority priority)
        {
            if (priority == Priority.Low)
            {
                priority = Priority.Medium;
            }
            else if (priority == Priority.Medium)
            {
                priority = Priority.High;
            }

            return priority;
        }
    }

    public enum Priority
    {
        High,
        Medium,
        Low
    }
}
