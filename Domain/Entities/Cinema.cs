using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Neo4jClient;
using System.ComponentModel.DataAnnotations;



namespace Domain.Entities
{
    public class Cinema
    {
        public string CinemaId { get; set; }
        public string CinemaName { get; set; }
        public string CinemaLocation { get; set; }
        public int NumberOfSeats { get; set; }
        public int DateBuilt { get; set; }

        public void AddCinemaLocation(string cinemaId, string locationId, IGraphClient graphClient)
        {
            IGraphClient _graphClient = graphClient;

            _graphClient.Cypher
                .Match("(c:Cinema)", "(ci:City)")
                .Where((Cinema c) => c.CinemaId == cinemaId)
                .AndWhere((City ci) => ci.CityId == locationId)
                .Create("(c)-[:IS_LOCATED_IN]->(ci)")
                .ExecuteWithoutResults();
        }

        public void RemoveCinemaLocation(string cinemaId, IGraphClient graphClient)
        {
            IGraphClient _graphClient = graphClient;

            _graphClient.Cypher
                .Match("(c:Cinema)-[r:IS_LOCATED_IN]->()")
                .Where((Cinema c) => c.CinemaId == cinemaId)
                .Delete("r")
                .ExecuteWithoutResults();
        }

        public string ReturnCinemaLocationName(string cinemaId, IGraphClient graphClient)
        {
            IGraphClient _graphClient = graphClient;

            string cityName;

            cityName = graphClient.Cypher
                .OptionalMatch("(c:Cinema)-[r:IS_LOCATED_IN]->(ci:City)")
                .Where((Cinema c) => c.CinemaId == CinemaId)
                .Return<string>("ci.Name")
                .Results.Single();

            if(cityName == null)
            {
                return "None";
            }
            else
            {
                return cityName;
            }
        }

        public string ReturnCinemaLocationId(string cinemaId, IGraphClient graphClient)
        {
            IGraphClient _graphClient = graphClient;

            string cityId;

            cityId = _graphClient.Cypher
                .OptionalMatch("(c:Cinema)-[r:IS_LOCATED_IN]->(ci:City)")
                .Where((Cinema c) => c.CinemaId == cinemaId)
                .Return<string>("ci.CityId")
                .Results.Single();

            if (cityId == null)
            {
                return "None";
            }
            else
            {
                return cityId;
            }
        }
    }

    
}
