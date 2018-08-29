using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Domain.Entities;

namespace Domain.Abstract
{
    public interface ICinemaRepository
    {
        IEnumerable<Cinema> GetCinemas();
        Cinema GetCinemaById(string cinemaId);
        void InsertCinema(Cinema cinema);
        void DeleteCinema(string cinemaId);
        void UpdateCinema(Cinema cinema);
    }
}
