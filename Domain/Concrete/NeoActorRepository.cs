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
    public class NeoActorRepository : IActorRepository
    {
        private readonly IGraphClient _graphClient;

        public NeoActorRepository(IGraphClient graphClient)
        {
            _graphClient = graphClient;
        }

        public IEnumerable<Actor> GetActors()
        {
            return _graphClient.Cypher
                .Match("(a:Actor)")
                .Return(a => a.As<Actor>())
                .Results.ToList<Actor>();
        }

        public IEnumerable<Actor> GetActorsThatContainString(string searchString)
        {
            return _graphClient.Cypher
               .OptionalMatch("(a:Actor)")
               .Where("(a.ActorFullName =~ {searchString} )")
               .WithParam("searchString", "(?i).*" + searchString + ".*")
               .Return(a => a.As<Actor>())
               .Results.ToList<Actor>();
        }

        public Actor GetActorById(string actorId)
        {
            return _graphClient.Cypher
                .Match(" (a:Actor {ActorId:{actorId}} ) ")
                .WithParam("actorId", actorId)
                .Return(a => a.As<Actor>())
                .Results.Single();
        }

        public void InsertActor(Actor actor)
        {
            actor.ActorId = Guid.NewGuid().ToString();
            actor.ActorFullName = actor.ActorFirstName + " " + actor.ActorLastName;

            Actor ac = _graphClient.Cypher
                .Create(" (a:Actor {actor}) ")
                .WithParam("actor", actor)
                .Return(a => a.As<Actor>())
                .Results.Single();
        }

        public void UpdateActor(Actor actor)
        {
            Dictionary<string, object> queryDict = new Dictionary<string, object>();
            queryDict.Add("fname", actor.ActorFirstName);
            queryDict.Add("lname", actor.ActorLastName);
            queryDict.Add("fullname", actor.ActorFirstName + " " + actor.ActorLastName);
            queryDict.Add("bio", actor.Biography);

            _graphClient.Cypher
                .Match("(a:Actor)")
                .Where((Actor a) => a.ActorId == actor.ActorId)
                .Set("a.ActorFirstName = {fname}, a.ActorLastName = {lname}, a.ActorFullName = {fullname}, a.Biography = {bio}")
                .WithParams(queryDict)
                .ExecuteWithoutResults();
        }

        public void DeleteActor(string actorId)
        {
            _graphClient.Cypher
                .Match(" (a:Actor {ActorId:{actorId}} ) ")
                .WithParam("actorId", actorId)
                .DetachDelete("a")
                .ExecuteWithoutResults();
        }


    }
}
