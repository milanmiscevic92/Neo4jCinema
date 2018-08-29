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
    public class NeoCinemaRepository : ICinemaRepository
    {
        private readonly IGraphClient _graphClient;

        public NeoCinemaRepository(IGraphClient graphClient)
        {
            _graphClient = graphClient;
        }

        public IEnumerable<Cinema> GetCinemas()
        {
            return _graphClient.Cypher
                .Match("(c:Cinema)")
                .Return(c => c.As<Cinema>())
                .Results.ToList<Cinema>();
        }

        public Cinema GetCinemaById(string cinemaId)
        {
            return _graphClient.Cypher
                .Match(" (c:Cinema {CinemaId:{cinemaId}} ) ")
                .WithParam("cinemaId", cinemaId)
                .Return(c => c.As<Cinema>())
                .Results.Single();
        }

        public void InsertCinema(Cinema cinema)
        {
            cinema.CinemaId = Guid.NewGuid().ToString();

            Cinema c = _graphClient.Cypher
                .Create(" (ci:Cinema {cinema}) ")
                .WithParam("cinema", cinema)
                .Return(ci => ci.As<Cinema>())
                .Results.Single();
        }

        public void UpdateCinema(Cinema cinema)
        {
            Dictionary<string, object> queryDict = new Dictionary<string, object>();
            queryDict.Add("name", cinema.CinemaName);
            queryDict.Add("location", cinema.CinemaLocation);
            queryDict.Add("numberOfSeats", cinema.NumberOfSeats);
            queryDict.Add("dateBuilt", cinema.DateBuilt);

            _graphClient.Cypher
                .Match("(ci:Cinema)")
                .Where((Cinema ci) => ci.CinemaId == cinema.CinemaId)
                .Set("ci.CinemaName = {name}, ci.CinemaLocation = {location}, ci.NumberOfSeats = {numberOfSeats}, ci.DateBuilt = {dateBuilt}")
                .WithParams(queryDict)
                .ExecuteWithoutResults();
        }

        public void DeleteCinema(string cinemaId)
        {
            _graphClient.Cypher
                .Match(" (ci:Cinema {CinemaId:{cinemaId}} ) ")
                .WithParam("cinemaId", cinemaId)
                .DetachDelete("ci")
                .ExecuteWithoutResults();
        }
    }
}
