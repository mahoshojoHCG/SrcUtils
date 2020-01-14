using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Reactive.Linq;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using HCGStudio.SrcUtils.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Query.SqlExpressions;
using ReactiveUI;

namespace HCGStudio.SrcUtils.ViewModels
{
    public class AppViewModel : ReactiveObject
    {
        private string _searchTerm;
        public string SearchTerm
        {
            get => _searchTerm;
            set => this.RaiseAndSetIfChanged(ref _searchTerm, value);
        }

        private bool FirstRun { get; set; }

        private readonly ObservableAsPropertyHelper<IEnumerable<SourceViewModel>> _searchResults;
        public IEnumerable<SourceViewModel> SearchResults => _searchResults.Value;

        private readonly ObservableAsPropertyHelper<bool> _isAvailable;
        public bool IsAvailable => _isAvailable.Value;

        public AppViewModel()
        {
            _searchResults = this
                .WhenAnyValue(x => x.SearchTerm)
                .Throttle(TimeSpan.FromMilliseconds(800))
                .Select(term => term?.Trim())
                .DistinctUntilChanged()
                .SelectMany(SearchSrcAsync)
                .ObserveOn(RxApp.MainThreadScheduler)
                .ToProperty(this, x => x.SearchResults);

            _isAvailable = this
                .WhenAnyValue(x => x.SearchResults)
                .Select(searchResults => searchResults != null)
                .ToProperty(this, x => x.IsAvailable);

        }


        private async Task<IEnumerable<SourceViewModel>> SearchSrcAsync(string term, CancellationToken token)
        {

            try
            {
                await using var context = new SrcContext();

                var result = new List<SourceViewModel>();

                //Empty, return all
                if (string.IsNullOrWhiteSpace(term))
                {
                    foreach (var src in context.Sources)
                    {
                        result.Add(new SourceViewModel(src));
                    }

                    return result;
                }

                var strings = term.Split(" ");

                var categoryRegex = new Regex("category:.*",RegexOptions.IgnoreCase);
                var tagRegex = new Regex("tag:.*", RegexOptions.IgnoreCase);

                var category = string.Empty;
                var tags = new List<string>();
                var normal = new List<string>();

                foreach (var s in strings)
                {
                    if (string.IsNullOrEmpty(s))
                        continue;
                    var cat = categoryRegex.Match(s);
                    var tag = tagRegex.Match(s);
                    if (cat.Success && category == string.Empty)
                        category = s[9..];
                    else if (tag.Success)
                        tags.Add(s[4..]);
                    else
                        normal.Add(s);
                }

                //Do not convert to SQL
                foreach (var src in context.Sources)
                {
                    if ((tags.Count == 0 || tags.All(src.Tags.Contains)) &&
                        (category == string.Empty || src.Category == category) &&
                        (normal.All(src.Name.Contains) || normal.All(src.Description.Contains)))
                    {
                        result.Add(new SourceViewModel(src));
                    }
                }
                return result;

            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }

        }
    }
}
