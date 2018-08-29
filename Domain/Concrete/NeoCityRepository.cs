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
    public class NeoCityRepository : ICityRepository
    {
        private readonly IGraphClient _graphClient;

        public NeoCityRepository(IGraphClient graphClient)
        {
            _graphClient = graphClient;
        }

        public IEnumerable<City> GetCities()
        {
            return _graphClient.Cypher
                .Match("(c:City)")
                .Return(c => c.As<City>())
                .Results.ToList<City>();
        }

        public City GetCityById(string cityId)
        {
            return _graphClient.Cypher
                .Match(" (ci:City {CityId:{cityId}} ) ")
                .WithParam("cityId", cityId)
                .Return(ci => ci.As<City>())
                .Results.Single();
        }

        public void InsertCity(City city)
        {
            city.CityId = Guid.NewGuid().ToString();

            City c = _graphClient.Cypher
                .Create(" (ci:City {city}) ")
                .WithParam("city", city)
                .Return(ci => ci.As<City>())
                .Results.Single();
        }

        public void UpdateCity(City city)
        {
            Dictionary<string, object> queryDict = new Dictionary<string, object>();
            queryDict.Add("name", city.Name);
            queryDict.Add("zip", city.Zip);

            _graphClient.Cypher
                .Match("(ci:City)")
                .Where((City ci) => ci.CityId == city.CityId)
                .Set("ci.Name = {name}, ci.Zip = {zip}")
                .WithParams(queryDict)
                .ExecuteWithoutResults();
        }

        public void DeleteCity(string cityId)
        {
            _graphClient.Cypher
                .Match(" (ci:City {CityId:{cityId}} ) ")
                .WithParam("cityId", cityId)
                .DetachDelete("ci")
                .ExecuteWithoutResults();
        }



    }
}