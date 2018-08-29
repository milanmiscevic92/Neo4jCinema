using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Domain.Entities;

namespace Domain.Abstract
{
    public interface IEventRepository
    {
        IEnumerable<Event> GetEvents();
        IEnumerable<Event> GetEventsThatContainString(string searchString);
        Dictionary<Event, int> GetTopEvents();
        Event GetEventById(string eventId);
        void InsertEvent(Event ev);
        void DeleteEvent(string eventId);
        void UpdateEvent(Event ev);
    }
}
