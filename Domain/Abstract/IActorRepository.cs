using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Domain.Entities;

namespace Domain.Abstract
{
    public interface IActorRepository
    {
        IEnumerable<Actor> GetActors();
        IEnumerable<Actor> GetActorsThatContainString(string searchString);
        Actor GetActorById(string actorId);
        void InsertActor(Actor actor);
        void DeleteActor(string actorId);
        void UpdateActor(Actor actor);
    }
}
