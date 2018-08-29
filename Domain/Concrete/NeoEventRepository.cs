using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Domain.Abstract;
using Domain.Entities;
using Neo4jClient;
using Neo4jClient.Cypher;

namespace Domain.Concrete
{
    public class NeoEventRepository : IEventRepository
    {
        private readonly IGraphClient _graphClient;

        public NeoEventRepository(IGraphClient graphClient)
        {
            _graphClient = graphClient;
        }

        public IEnumerable<Event> GetEvents()
        {
            return _graphClient.Cypher
                .Match("(e:Event)")
                .Return(e => e.As<Event>())
                .Results.ToList<Event>();
        }

        public IEnumerable<Event> GetEventsThatContainString(string searchString)
        {
            return _graphClient.Cypher
               .OptionalMatch("(e:Event)")
               .Where("(e.EventName =~ {searchString} )")
               .WithParam("searchString", "(?i).*" + searchString + ".*")
               .Return(e => e.As<Event>())
               .Results.ToList<Event>();
        }

        public Event GetEventById(string eventId)
        {
            return _graphClient.Cypher
                .Match("(e:Event {EventId:{eventId}})")
                .WithParam("eventId", eventId)
                .Return(e => e.As<Event>())
                .Results.Single();
        }

        public Dictionary<Event, int> GetTopEvents()
        {
            Dictionary<Event, int> topEvents = new Dictionary<Event, int>();

            int numberOfAttendees;

            IEnumerable<Event> allEvents = _graphClient.Cypher
                .OptionalMatch("(e:Event)<-[r:IS_ATTENDING]-(u:User)")
                .ReturnDistinct(e => e.As<Event>()).Results.ToList();

            foreach(Event ev in allEvents)
            {
                if(ev != null)
                {
                    IEnumerable<User> usersAttending = _graphClient.Cypher
                        .OptionalMatch("(e:Event)<-[r:IS_ATTENDING]-(u:User)")
                        .Where((Event e) => e.EventId == ev.EventId)
                        .ReturnDistinct(u => u.As<User>())
                        .Results.ToList();

                    numberOfAttendees = usersAttending.Count();
                    topEvents.Add(ev, numberOfAttendees);
                }
            }

            return topEvents;
        }

        public void InsertEvent(Event ev)
        {
            ev.EventId = Guid.NewGuid().ToString();

            Event evnt = _graphClient.Cypher
                .Create("(e:Event {event})")
                .WithParam("event", ev)
                .Return(e => e.As<Event>())
                .Results.Single();
        }

        public void UpdateEvent(Event ev)
        {
            Dictionary<string, object> queryDict = new Dictionary<string, object>();
            queryDict.Add("name", ev.EventName);
            queryDict.Add("location", ev.EventLocation);
            queryDict.Add("description", ev.EventDescription);
            queryDict.Add("startTime", ev.EventStartTime);

            _graphClient.Cypher
                .Match("(e:Event)")
                .Where((Event e) => e.EventId == ev.EventId)
                .Set("e.EventName = {name}, e.EventLocation = {location}, e.EventDescription = {description}, e.EventStartTime = {startTime}")
                .WithParams(queryDict)
                .ExecuteWithoutResults();
        }

        public void DeleteEvent(string eventId)
        {
            _graphClient.Cypher
                .Match("(e:Event {EventId:{eventId}})")
                .WithParam("eventId", eventId)
                .DetachDelete("e")
                .ExecuteWithoutResults();
        }







    }
}

