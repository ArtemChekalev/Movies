using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Movies.Data;
using HtmlAgilityPack;

namespace Movies.Interface
{
    public class SQLRepository: Imovies
    {
        private readonly ApplicationContext Context;
        public SQLRepository(ApplicationContext context)
        {
            Context = context;
        }

        public IEnumerable<Movie> GetAllMovies()
        {
            return Context.Movies;
        }

        public string GetMovie(string Name)
        {
            string res = "";
            System.Net.WebClient wc = new System.Net.WebClient();
            var movies = (from movie in Context.Movies where movie.FilmName == Name select movie).ToList();
            if (movies.Count == 1)
            {
                var mv = movies.First();
                byte[] raw = wc.DownloadData("https://www.omdbapi.com/?i=" + mv.MovieId + "&apikey=561b7006");
                string webData = System.Text.Encoding.UTF8.GetString(raw);
                var year = webData.IndexOf("Year");
                var rated = webData.IndexOf("Rated");
                var posterind = webData.IndexOf("Poster");
                var ratingsind = webData.IndexOf("Ratings");
                var runtimeind = webData.IndexOf("Runtime");
                var genreind = webData.IndexOf("Genre");
                var plotind = webData.IndexOf("Plot");
                var langind = webData.IndexOf("Language");
                var mvyear = webData.Substring(year + 7, rated - year - 10);
                var link = webData.Substring(posterind + 9, ratingsind - posterind - 12);
                var runtime = webData.Substring(runtimeind + 10, genreind - runtimeind - 13);
                var plot = webData.Substring(plotind + 7, langind - plotind - 10);
                var director = (from dir in Context.Directors where dir.Movies.Contains(mv) select dir).FirstOrDefault();
                var actors = (from act in Context.Actors where act.Movies.Contains(mv) select act).ToList();
                var tg = (from tag in Context.Tags where tag.Movies.Contains(mv) select tag).ToList();
                mv.Actors = actors;
                mv.Director = director;
                mv.Tags = tg;
                res += mv.ToString();
                res += link + '\n';
                res += runtime + '\n';
                res += plot + '\n';
                res += $"({mvyear})";
                return res;
            }
            else return res = "Такого фильма не найдено.";
        }

        public string GetActor(string Name)
        {
            string res = "";
            var act = (from actor in Context.Actors where actor.Name == Name select actor).FirstOrDefault();
            var mv = (from movies in Context.Movies where movies.Actors.Contains(act) select movies).ToList();
            act.Movies = mv;
            res += act.ToString();
            return res;
        }

        public string GetDirector(string Name)
        {
            string res = "";
            var dir = (from director in Context.Directors where director.Name == Name select director).FirstOrDefault();
            var mv = (from movies in Context.Movies where movies.Director.DirectorId == dir.DirectorId select movies).ToList();
            dir.Movies = mv;
            res += dir.ToString();
            return res;
        }

        public string GetTag(string Name)
        {
            string res = "";
            var tag = (from tg in Context.Tags where tg.Name == Name select tg).FirstOrDefault();
            var mv = (from movies in Context.Movies where movies.Tags.Contains(tag) select movies).ToList();
            tag.Movies = mv;
            res += tag.ToString();
            return res;
        }

        public string FilmByCode(string code)
        {
            var Name = (from mv in Context.Movies where mv.MovieId == code select mv.FilmName).FirstOrDefault();
            return Name;
        }

        public string GetValue(string value)
        {
            var movie = (from mv in Context.Movies where mv.FilmName == value select mv).ToList();
            if ((from mv in Context.Movies where mv.FilmName == value select mv).FirstOrDefault() is not null)
                return "0";
            else if ((from act in Context.Actors where act.Name == value select act).FirstOrDefault() is not null)
                return "1";
            else if ((from dir in Context.Directors where dir.Name == value select dir).FirstOrDefault() is not null)
                return "2";
            else if ((from tag in Context.Tags where tag.Name == value select tag).FirstOrDefault() is not null)
                return "3";
            else return "4";
        }
    }
}
