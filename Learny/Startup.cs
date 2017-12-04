using Microsoft.Owin;
using Owin;
using System.Data.Entity.Infrastructure;
using System.Xml;
using Learny.Models;

[assembly: OwinStartupAttribute(typeof(Learny.Startup))]
namespace Learny
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            // Derive the model from the context!
            //
            //public void CreateDesignModel()
            //{
                //using (var context = new ApplicationDbContext())
                //{
                //    XmlWriterSettings settings = new XmlWriterSettings();
                //    settings.Indent = true;

                //    using (XmlWriter writer = XmlWriter.Create(@"C:\Dropbox\Visual Studio 2017\Projects\LEXICON\Övningar\Final Project - Lerny\Learny\Model.edmx", settings))
                //    {
                //        EdmxWriter.WriteEdmx(context, writer);
                //    }
                //}
          //  }
            ConfigureAuth(app);


        }
    }
}
