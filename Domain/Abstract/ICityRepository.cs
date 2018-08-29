using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Domain.Entities;

namespace Domain.Abstract
{
    public interface ICityRepository
    {
        IEnumerable<City> GetCities();
        City GetCityById(string cityId);
        void InsertCity(City city);
        void DeleteCity(string cityId);
        void UpdateCity(City city);
    }
}
