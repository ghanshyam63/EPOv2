using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EPOv2.DependencyResolver
{
    using EPOv2.Business;
    using EPOv2.Business.Interfaces;
    using EPOv2.Repositories.Interfaces;

    using global::Repositories;

    using Ninject.Modules;
    using Ninject.Web.Common;

    public class BusinessModule : NinjectModule
    {
        public override void Load()
        {
            Bind<IAd>().To<AD>().InRequestScope();
            Bind<IData>().To<Data>().InRequestScope();
            Bind<IUserInterface>().To<UserInterface>().InRequestScope();
            Bind<IMain>().To<Main>().InRequestScope();
            Bind<IUserManagement>().To<UserManagement>();
            Bind<IRouting>().To<Routing>().InRequestScope();
            Bind<IOutput>().To<Output>().InRequestScope();
            Bind<IMock>().To<Mock>().InRequestScope();
        }
    }
}
