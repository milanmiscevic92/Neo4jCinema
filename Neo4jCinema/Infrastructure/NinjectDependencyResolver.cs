using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Ninject;
using System.Web.Mvc;
using System.Configuration;
using Domain.Abstract;
using Domain.Concrete;

namespace Neo4jCinema.Infrastructure
{
    public class NinjectDependencyResolver : IDependencyResolver
    {
        private IKernel kernel;

        public NinjectDependencyResolver(IKernel kernelParam)
        {
            kernel = kernelParam;
            AddBindings();
        }

        public object GetService(Type serviceType)
        {
            return kernel.TryGet(serviceType);
        }

        public IEnumerable<object> GetServices(Type serviceType)
        {
            return kernel.GetAll(serviceType);
        }

        private void AddBindings()
        {
            kernel.Bind<ICityRepository>().To<NeoCityRepository>();
            kernel.Bind<ICinemaRepository>().To<NeoCinemaRepository>();
            kernel.Bind<IActorRepository>().To<NeoActorRepository>();
            kernel.Bind<IMovieRepository>().To<NeoMovieRepository>();
            kernel.Bind<IEventRepository>().To<NeoEventRepository>();
            kernel.Bind<IUserRepository>().To<NeoUserRepository>();
        }
    }
}