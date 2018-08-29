using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Neo4jClient;
using System.ComponentModel.DataAnnotations;

namespace Domain.Entities
{
    public class Actor
    {
        public string ActorId { get; set; }
        public string ActorFirstName { get; set; }
        public string ActorLastName { get; set; }
        public string ActorFullName { get; set; }
        public string Biography { get; set; }

        public IEnumerable<Movie> ReturnAllMoviesFromActor(string actorId, IGraphClient graphClient)
        {
            IGraphClient _graphClient = graphClient;

            return _graphClient.Cypher
               .Match("(a:Actor)-[r:ACTS_IN]->(m:Movie)")
               .Where((Actor a) => a.ActorId == actorId)
               .Return(m => m.As<Movie>())
               .Results.ToList<Movie>();
        }
    }
}
