// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IoC.cs" company="Web Advanced">
// Copyright 2012 Web Advanced (www.webadvanced.com)
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
//
// http://www.apache.org/licenses/LICENSE-2.0

// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using System.Configuration;
using System.Security.Principal;
using System.Web;

using MediaCommMvc.Web.Infrastructure;
using MediaCommMvc.Web.Infrastructure.Nh;

using StructureMap;
namespace MediaCommMvc.Web.DependencyResolution {
    public static class IoC {
        public static IContainer Initialize()
        {
            ObjectFactory.Initialize(InitContainer);
            return ObjectFactory.Container;
        }

        private static void InitContainer(IInitializationExpression container)
        {
            container.Scan(
                scan =>
                {
                    scan.TheCallingAssembly();
                    scan.WithDefaultConventions();
                });

            container.For<ISessionContainer>().Use<HttpContextSessionContainer>();
            container.For<IConfigAccessor>().Use<FileConfigAccessor>();
            container.For<ILogger>().Use<Log4NetLogger>();
            container.For<IImageGenerator>().Use<ImageGenerator>();
            container.For<IIdentity>().Use(i => HttpContext.Current.User.Identity);
            container.For<INotificationSender>().Use<AsyncNotificationSender>();

            var mailConfiguration = new MailConfiguration { MailFrom = ConfigurationManager.AppSettings["mail-from"], SmtpHost = ConfigurationManager.AppSettings["mail-smtpHost"] };
            container.For<MailConfiguration>().Use(m => mailConfiguration);
        }
    }
}