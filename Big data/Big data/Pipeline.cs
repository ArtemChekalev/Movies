using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Collections.Concurrent;

namespace Big_data
{
    internal static class Pipeline
    {
        public static async Task LoadContentAsync(BlockingCollection<string> output, string filename)
        {
            using (var stream = File.OpenRead(filename))
            {
                var reader = new StreamReader(stream);
                var line = reader.ReadLine();
                line = null;
                while ((line = await reader.ReadLineAsync()) != null) output.Add(line);
            }
            output.CompleteAdding();
            Console.WriteLine("End download: " + filename);
        }

        public static void ProcessAddTagCodes(BlockingCollection<string> input, ConcurrentDictionary<string, Tag> output)
        {
            Console.WriteLine("Start ProcessAddTagCodes");
            foreach (var line in input.GetConsumingEnumerable())
            {
                var ind = line.IndexOf(',');
                var tagid = line.Substring(0, ind);
                var tagname = line.Substring(ind + 1);
                var tag = new Tag(tagid, tagname);  
                output.AddOrUpdate(tagid, tag, (s, i) => i);
            }
            Console.WriteLine("End ProcessAddTagCodes");
        }

        public static List<Tag> AddToListTag(List<Tag> ls, Tag str)
        {
            ls.Add(str);
            return ls;
        }

        public static void ProcessAddTagScores(BlockingCollection<string> input,
            ConcurrentDictionary<string, List<Tag>> output,
            ConcurrentDictionary<string, Tag> inputDict)
        {
            Console.WriteLine("Start ProcessAddTagScores");
            foreach (var line in input.GetConsumingEnumerable())
            {
                var firstind = line.IndexOf(',');
                var secind = line.IndexOf(',', firstind + 1);
                var movieid = line.Substring(0, firstind);
                var tagid = line.Substring(firstind + 1, secind - firstind - 1);
                var rel = line.Substring(secind + 1);
                if (double.Parse(rel.Replace('.',',')) > 0.5 &&
                    inputDict.ContainsKey(tagid))
                {
                    inputDict.TryGetValue(tagid, out var tag);
                    output.AddOrUpdate(movieid, new List<Tag>(new[] { tag }),
                        (s, i) => AddToListTag(i, tag));
                }
                    
            }
            Console.WriteLine("End ProcessAddTagScores");
        }

        public static void ProcessAddLinks(BlockingCollection<string> input,
            ConcurrentDictionary<string, List<Tag>> output, ConcurrentDictionary<string, List<Tag>> scoreDic)
        {
            Console.WriteLine("Start ProcessAddLinks");
            foreach (var line in input.GetConsumingEnumerable())
            {
                var firstind = line.IndexOf(',');
                var secind = line.IndexOf(',', firstind + 1);
                var movieid = line.Substring(0, firstind);
                var imdbid = line.Substring(firstind + 1, secind - firstind - 1);
                if (scoreDic.TryGetValue(movieid, out var val))
                    output.AddOrUpdate("tt" + imdbid, val, (s, i) => i);
            }

            Console.WriteLine("End ProcessAddLinks");
        }

        public static void ProcessAddMovieCodes(BlockingCollection<string> input, ConcurrentDictionary<string, Movie> movies,
            ConcurrentDictionary<string, Director> directorbyFilm, ConcurrentDictionary<string, string> rating,
            ConcurrentDictionary<string, List<Actor>> actorsByFilm, ConcurrentDictionary<string, List<Tag>> tagsbyFilm,
            ConcurrentDictionary<string, Movie> output)
        {
            Console.WriteLine("Start ProcessAddMovieCodes");
            foreach (var line in input.GetConsumingEnumerable())
            {
                var firstind = line.IndexOf('\t');
                var secind = line.IndexOf('\t', firstind + 1);
                var thirdid = line.IndexOf('\t', secind + 1);
                var find = line.IndexOf('\t', thirdid + 1);
                var movieid = line.Substring(0, firstind);
                var title = line.Substring(secind + 1, thirdid - secind - 1);
                var language = line.Substring(thirdid + 1, find - thirdid - 1);
                if ((language == "EN" || language == "RU") & !(title == "\\\\N"))
                {
                    directorbyFilm.TryGetValue(movieid, out var director);
                    rating.TryGetValue(movieid, out var rat);
                    actorsByFilm.TryGetValue(movieid, out var actors);
                    tagsbyFilm.TryGetValue(movieid, out var tags);
                    if (actors!=null & tags != null)
                    {
                        var mv = new Movie(title, movieid, director, rat, actors, tags);
                        movies.AddOrUpdate(title, mv,
                        (s, i) => mv);
                        output.AddOrUpdate(movieid, mv,
                            (s, i) => mv);
                    }
                }
            }

            Console.WriteLine("End ProcessAddMovieCodes");
        }

        public static List<string> AddToList(List<string> ls, string str)
        {
            ls.Add(str);
            return ls;
        }

        public static void ProcessAddRating(BlockingCollection<string> input,
            ConcurrentDictionary<string, string> output)
        {
            Console.WriteLine("Start ProcessAddRating");
            foreach (var line in input.GetConsumingEnumerable())
            {
                var firstind = line.IndexOf('\t');
                var secind = line.IndexOf('\t', firstind + 1);
                var movieid = line.Substring(0, firstind);
                var rating = line.Substring(firstind + 1, secind - firstind - 1);
                output.AddOrUpdate(movieid, rating, (s, i) => i);
            }

            Console.WriteLine("End ProcessAddRating");
        }

        public static List<Actor> AddToListActor(List<Actor> ls, Actor str)
        {
            ls.Add(str);
            return ls;
        }

        public static void ProcessActorsDirectorByCode(BlockingCollection<string> input, ConcurrentDictionary<string, Actor> ActorByCode,
            ConcurrentDictionary<string, Director> DirectorByCode, ConcurrentDictionary<string, string> actorsbynumbDict, 
            ConcurrentDictionary<string, List<string>> MovieActor, ConcurrentDictionary<string, string> MovieDirector)
        {
            Console.WriteLine("Start ProcessActorsDirectorByCode");
            foreach (var line in input.GetConsumingEnumerable())
            {
                var firstind = line.IndexOf('\t');
                var secind = line.IndexOf('\t', firstind + 1);
                var thirdid = line.IndexOf('\t', secind + 1);
                var find = line.IndexOf('\t', thirdid + 1);
                var movieid = line.Substring(0, firstind);
                var actorid = line.Substring(secind + 1, thirdid - secind - 1);
                var profession = line.Substring(thirdid + 1, find - thirdid - 1);
                if (actorsbynumbDict.ContainsKey(actorid))
                {
                    if (profession == "actor" || profession == "actress")
                    {
                        var actor = new Actor(actorid, actorsbynumbDict[actorid]);
                        ActorByCode.AddOrUpdate(actorid,actor,
                            (s, i) => i);
                        MovieActor.AddOrUpdate(movieid, new List<string>(new[] { actorid }), (s, i) =>AddToList(i,actorid));
                    }

                    else if (profession == "director")
                    {
                        var dir = new Director(actorid, actorsbynumbDict[actorid]);
                        DirectorByCode.AddOrUpdate(actorid, dir, (s, i) => i);
                        MovieDirector.AddOrUpdate(movieid, actorid, (s, i) => i);
                    }
                    
                }
            }
            Console.WriteLine("End ProcessActorsDirectorByCode");

        }

        public static void ProcessActorsDirectorCodes(ConcurrentDictionary<string, Actor> ActorByCode,
            ConcurrentDictionary<string, Director> DirectorByCode, ConcurrentDictionary<string, List<string>> MovieActor, 
            ConcurrentDictionary<string, string> MovieDirector,
            ConcurrentDictionary<string, List<Actor>> outputFilmsActors, ConcurrentDictionary<string, Director> outputFilmDirector)
        {
            foreach(var mv in MovieActor.Keys)
            {
                MovieActor.TryGetValue(mv, out var ls);
                foreach(var act in ls)
                {
                    ActorByCode.TryGetValue(act, out var actor);
                    outputFilmsActors.AddOrUpdate(mv, new List<Actor>(new[] { actor }),
                        (s, i) => AddToListActor(i, actor));
                }
            }
            foreach(var mv in MovieDirector.Keys)
            {
                MovieDirector.TryGetValue(mv, out var dir);
                DirectorByCode.TryGetValue(dir, out var director);
                outputFilmDirector.AddOrUpdate(mv, director, (s, i) => i);
            }
        }

        public static void ProcessActorDirectorNames(BlockingCollection<string> input,
            ConcurrentDictionary<string, string> outputActorsNames,
            ConcurrentDictionary<string, List<string>> outputActorsFilms)
        {
            Console.WriteLine("Start ProcessActorDirectorNames");
            foreach (var line in input.GetConsumingEnumerable())
            {
                var firstind = line.IndexOf('\t');
                var secind = line.IndexOf('\t', firstind + 1);
                var thirdid = line.IndexOf('\t', secind + 1);
                var find = line.IndexOf('\t', thirdid + 1);
                var fiveind = line.IndexOf('\t', find + 1);
                var actorid = line.Substring(0, firstind);
                var actorname = line.Substring(firstind + 1, secind - firstind - 1);
                var films = line.Substring(fiveind + 1);
                outputActorsNames.AddOrUpdate(actorid, actorname, (s, i) => i);
                var movies = films.Split(',');
                outputActorsFilms.AddOrUpdate(actorid, new List<string>(movies),
                    (s, i) => AddToListArr(i, movies));
            }

            Console.WriteLine("End ProcessActorDirectorNames");
        }

        public static List<string> AddToListArr(List<string> list, string[] arr)
        {
            foreach (var item in arr)
            {
                list.Add(item);
            }
            return list;
        }

        public static void ProcessCreateTagsList(ConcurrentDictionary<Tag, List<Movie>> output,
            ConcurrentDictionary<string, Movie> filmbyCode, ConcurrentDictionary<string, List<Tag>> linksDict)
        {
            foreach (var key in linksDict.Keys)
            {
                linksDict.TryGetValue(key, out var list);
                foreach(var tag in list)
                {
                    if (filmbyCode.ContainsKey(key))
                    {
                        filmbyCode.TryGetValue(key, out var mv);
                        output.AddOrUpdate(tag, new List<Movie>(new[] { mv }),
                            (s, i) => AddToMovieList(i, mv));
                    }
                }
            }
        }

        public static List<Movie> AddToMovieList(List<Movie> list, Movie mv)
        {
            list.Add(mv);
            return list;
        }


        public static void ProcessFilmsbyActor(ConcurrentDictionary<string, List<Actor>> actorsbyFilmDict,
            ConcurrentDictionary<string, Movie> filmByCodeDict, ConcurrentDictionary<Actor, List<Movie>> filmsbyActorName)
        {
            foreach (var key in actorsbyFilmDict.Keys)
            {
                actorsbyFilmDict.TryGetValue(key, out var list);
                foreach (var actor in list)
                {
                    if (filmByCodeDict.ContainsKey(key))
                    {
                        filmByCodeDict.TryGetValue(key, out var mv);
                        filmsbyActorName.AddOrUpdate(actor, new List<Movie>(new[] { mv }),
                            (s, i) => AddToMovieList(i, mv));
                    }
                }
            }
        }

        public static void ProcessFilmsbyDirector(ConcurrentDictionary<string, Director> directorbyFilmDict,
            ConcurrentDictionary<string, Movie> filmByCodeDict, ConcurrentDictionary<Director, List<Movie>> filmsbyDirectorName)
        {
            foreach (var key in directorbyFilmDict.Keys)
            {
                directorbyFilmDict.TryGetValue(key, out var director);
                if (filmByCodeDict.ContainsKey(key))
                {
                    filmByCodeDict.TryGetValue(key, out var mv);
                    filmsbyDirectorName.AddOrUpdate(director, new List<Movie>(new[] { mv }),
                        (s, i) => AddToMovieList(i, mv));
                }
            }
        }
    }
}
