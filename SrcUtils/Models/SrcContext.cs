using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using HCGStudio.SrcUtils.ViewModels;
using Microsoft.EntityFrameworkCore;

namespace HCGStudio.SrcUtils.Models
{
    public class SrcContext : DbContext
    {
        public DbSet<Source> Sources { get; set; }
        public DbSet<Tag> Tags { get; set; }
        public DbSet<Category> Categories { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder options)
        {
            if (Enum.TryParse(App.ConfigurationRoot["DataBaseProvider"],out DataBaseProvider provider))
            {
                switch (provider)
                {
                    case DataBaseProvider.SqlLite:
                        options.UseSqlite($"Data Source={App.ConfigurationRoot["SqlLiteDataSource"]}");
                        break;
                    case DataBaseProvider.SqlServer:
                        throw new ArgumentException("SQL Server is not supported in this version.");
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }
        }
    }

    public class Category
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int CategoryId { get; set; }
        public string Name { get; set; }
    }

    public class Tag
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int TagId { get; set; }
        public string Name { get; set; }
    }

    public class Source : ICloneable
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int SourceId { get; set; }
        public string Name { get; set; }
        public string Category { get; set; }
        public string Path { get; set; }
        public string Description { get; set; }
        [NotMapped] public List<string> Tags => TagsString?.Split(';')?.ToList() ?? new List<string>();
        public string TagsString { get; set; }
        public string Thumbnail { get; set; }

        public Source Clone()
        {
            var val = new Source
            {
                SourceId = SourceId,
                Name = Name,
                Category = Category,
                Path = Path,
                Description = Description,
                TagsString = TagsString
            };
            return val;
        }

        object ICloneable.Clone()
        {
            return Clone();
        }
    }
}
