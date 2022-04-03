using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Collections.Concurrent;

namespace Movies.Data
{
    public class Movie
    {
        public string MovieId { get; set; }
        public string FilmName { get; set; }
        public List<Actor> Actors { get; set; }
        public Director Director { get; set; }
        public List<Tag> Tags { get; set; }
        public string Rating { get; set; }
        private ConcurrentDictionary<string, double> SimilarMovies = new ConcurrentDictionary<string, double>();
        public string TopTen { get; set; }

        public Movie(string name, string code, Director dir, string rate, List<Actor> actors, List<Tag> tags)
        {
            FilmName = name;
            MovieId = code;
            Director = dir;
            Rating = rate;
            Actors = actors;
            Tags = tags;
        }


        public Movie()
        {

        }
        public void Top10Method(Movie mv)
        {
            if (MovieId != mv.MovieId)
            {
                if (!SimilarMovies.ContainsKey(mv.MovieId))
                {
                    double res = 0;
                    double k = 0.5;
                    int actcount = 0;
                    int tagcount = 0;
                    if (Director == mv.Director) res += k / 3;
                    foreach (var actor in Actors)
                    {
                        if (mv.Actors.Contains(actor)) actcount += 1;
                    }
                    res += ((actcount * k) / (Actors.Count() * 3));
                    foreach (var tag in Tags)
                    {
                        if (mv.Tags.Contains(tag)) tagcount += 1;
                    }
                    res += ((tagcount * k) / (Tags.Count() * 3));
                    res += Convert.ToDouble((mv.Rating).Replace('.', ',')) * 0.05;
                    res += 0;
                    SimilarMovies.AddOrUpdate(mv.MovieId, res, (s, i) => i);
                }
            }
        }

        public void SetTopTen()
        {
            string top = "";
            foreach (var pair in SimilarMovies.OrderByDescending(pair => pair.Value))
            {
                top += $"{pair.Key} ";
            }
            if (top.Length > 100) TopTen = top.Substring(0, 100);
            else TopTen = top;
        }

        public int GetTopCount() => SimilarMovies.Count();

        public override string ToString()
        {
            string res = "";
            res += $"{FilmName}\n";
            res += $"{Rating}\n";
            if (Director is not null) res += Director.Name + '\n';
            else res += $"No information about Director." + '\n';
            foreach (var actor in Actors) res += actor.Name + "_";
            res += '\n';
            foreach (var tag in Tags) res += tag.Name + "_";
            res += '\n';
            res += TopTen + '\n';
            return res;
        }
    }
}
