using System.Collections.Generic;
using System.Linq;

namespace TicketManagementSystem
{
    public static class TicketRepository
    {
        private static readonly List<Ticket> Tickets = new List<Ticket>();

        public static int CreateTicket(Ticket ticket)
        {
            // Assume that the implementation of this method does not need to change.
            var currentHighestTicket = Tickets.Any() ? Tickets.Max(i => i.Id) : 0;
            var id = currentHighestTicket + 1;
            ticket.Id = id;

            Tickets.Add(ticket);

            return id;
        }

        public static void UpdateTicket(Ticket ticket)
        {
            // Assume that the implementation of this method does not need to change.
            var outdatedTicket = Tickets.FirstOrDefault(t => t.Id == ticket.Id);

            if (outdatedTicket != null)
            {
                Tickets.Remove(outdatedTicket);
                Tickets.Add(ticket);
            }
        }

        public static Ticket GetTicket(int id)
        {
            // Assume that the implementation of this method does not need to change.
            return Tickets.FirstOrDefault(a => a.Id == id);
        }
    }
}
