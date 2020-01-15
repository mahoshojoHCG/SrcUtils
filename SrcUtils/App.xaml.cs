using System.IO;
using System.Reflection;
using System.Windows;
using HCGStudio.SrcUtils.Models;
using HCGStudio.SrcUtils.ViewModels;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using ReactiveUI;
using Splat;

namespace HCGStudio.SrcUtils
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        internal static IConfigurationRoot ConfigurationRoot;
        public App()
        {
            Locator.CurrentMutable.RegisterViewsForViewModels(Assembly.GetCallingAssembly());

            ConfigurationRoot = BuildConfiguration();

            using var context = new SrcContext();

            context.Database.EnsureCreated();


            //Check database status
            context.Database.EnsureCreated();
            context.Database.Migrate();
            context.SaveChanges();
        }

        private IConfigurationRoot BuildConfiguration()
        {
            if (!File.Exists("settings.json"))
            {
                File.WriteAllText("settings.json", JsonConvert.SerializeObject(new Settings()));
            }

            return new ConfigurationBuilder()
                .AddJsonFile("settings.json")
                .Build();
        }
    }
}
