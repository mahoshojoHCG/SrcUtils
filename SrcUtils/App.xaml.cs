using System.IO;
using System.Reflection;
using System.Windows;
using HCGStudio.SrcUtils.Models;
using HCGStudio.SrcUtils.ViewModels;
using Microsoft.EntityFrameworkCore;
using ReactiveUI;
using Splat;

namespace HCGStudio.SrcUtils
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        public App()
        {
            Locator.CurrentMutable.RegisterViewsForViewModels(Assembly.GetCallingAssembly());
            using var context = new SrcContext();

            context.Database.EnsureCreated();

            if (!File.Exists("default.db"))
            {
                //No database, creating
                context.Database.Migrate();
                context.SaveChanges();
            }
        }
    }
}
