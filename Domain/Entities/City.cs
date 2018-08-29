using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Neo4jClient;
using System.ComponentModel.DataAnnotations;

namespace Domain.Entities
{
    public class City
    {
        public string CityId { get; set; }

        // TREBA DA SE URADE REGULAR EXPRESSIONS

        [Required(ErrorMessage = "Please enter city name")]
        public string Name { get; set; }

        [Required(ErrorMessage = "Please enter city ZIP code")]
        public string Zip { get; set; }
    }

}