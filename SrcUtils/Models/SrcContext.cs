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
            => options.UseSqlite("Data Source=default.db");
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
