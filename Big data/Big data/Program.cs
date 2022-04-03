using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using System.Collections.Concurrent;
using System.IO;

namespace Big_data
{
    class Program
    {
        static void Main(string[] args)
        {
            BlockingCollection<string> codes = new BlockingCollection<string>();
            BlockingCollection<string> ratings = new BlockingCollection<string>();
            BlockingCollection<string> actorNames = new BlockingCollection<string>();
            BlockingCollection<string> actorCodes = new BlockingCollection<string>();
            BlockingCollection<string> scores = new BlockingCollection<string>();
            BlockingCollection<string> links = new BlockingCollection<string>();
            BlockingCollection<string> movieCodes = new BlockingCollection<string>();
            ConcurrentDictionary<string, Tag> tagCodesDict = new ConcurrentDictionary<string, Tag>();
            ConcurrentDictionary<string, List<Tag>> tagScoresDict = new ConcurrentDictionary<string, List<Tag>>();
            ConcurrentDictionary<string, List<Tag>> linksDict = new ConcurrentDictionary<string, List<Tag>>();
            ConcurrentDictionary<string, string> ratingDict = new ConcurrentDictionary<string, string>();
            ConcurrentDictionary<string, string> actorNameDict = new ConcurrentDictionary<string, string>();
            ConcurrentDictionary<string, List<string>> filmsByActor = new ConcurrentDictionary<string, List<string>>();
            ConcurrentDictionary<string, Actor> ActorByCode = new ConcurrentDictionary<string, Actor>();
            ConcurrentDictionary<string, Director> DirectorByCode = new ConcurrentDictionary<string, Director>();
            ConcurrentDictionary<string, List<string>> MovieActor = new ConcurrentDictionary<string, List<string>>();
            ConcurrentDictionary<string, string> MovieDirector = new ConcurrentDictionary<string, string>();
            ConcurrentDictionary<string, List<Actor>> actorsbyFilmDict = new ConcurrentDictionary<string, List<Actor>>();
            ConcurrentDictionary<string, Director> directorbyFilmDict = new ConcurrentDictionary<string, Director>();
            ConcurrentDictionary<string, Movie> filmByNameDict = new ConcurrentDictionary<string, Movie>();
            ConcurrentDictionary<string, Movie> filmByCodeDict = new ConcurrentDictionary<string, Movie>();
            ConcurrentDictionary<Tag, List<Movie>> filmsbyTag = new ConcurrentDictionary<Tag, List<Movie>>();
            ConcurrentDictionary<Actor, List<Movie>> filmsbyActorName = new ConcurrentDictionary<Actor, List<Movie>>();
            ConcurrentDictionary<Director, List<Movie>> filmsbyDirectorName = new ConcurrentDictionary<Director, List<Movie>>();

            var t1 = Pipeline.LoadContentAsync(codes, @"C:\Users\Admin\Desktop\ml-latest\TagCodes_MovieLens.csv");
            var t2 = Pipeline.LoadContentAsync(ratings, @"C:\Users\Admin\Desktop\ml-latest\Ratings_IMDB.tsv");
            var t3 = Pipeline.LoadContentAsync(actorNames, @"C:\Users\Admin\Desktop\ml-latest\ActorsDirectorsNames_IMDB.txt");
            var t4 = Pipeline.LoadContentAsync(actorCodes, @"C:\Users\Admin\Desktop\ml-latest\ActorsDirectorsCodes_IMDB.tsv");
            var t5 = Pipeline.LoadContentAsync(scores, @"C:\Users\Admin\Desktop\ml-latest\TagScores_MovieLens.csv");
            var t6 = Pipeline.LoadContentAsync(links, @"C:\Users\Admin\Desktop\ml-latest\links_IMDB_MovieLens.csv");
            var t7 = Pipeline.LoadContentAsync(movieCodes, @"C:\Users\Admin\Desktop\ml-latest\MovieCodes_IMDB.tsv");

            var tagCodesTask = new Task(() => Pipeline.ProcessAddTagCodes(codes, tagCodesDict), TaskCreationOptions.LongRunning);
            tagCodesTask.Start();
            var tagScoresTask = tagCodesTask.ContinueWith(x => Pipeline.ProcessAddTagScores(scores, tagScoresDict, tagCodesDict));
            var linksTask = tagScoresTask.ContinueWith(x => Pipeline.ProcessAddLinks(links, linksDict, tagScoresDict));
            var ratingTask = new Task(() => Pipeline.ProcessAddRating(ratings, ratingDict), TaskCreationOptions.LongRunning);
            ratingTask.Start();
            var actornamesTask = new Task(() => Pipeline.ProcessActorDirectorNames(actorNames, actorNameDict, filmsByActor), TaskCreationOptions.LongRunning);
            actornamesTask.Start();
            var actorByCodeTask = actornamesTask.ContinueWith(x => Pipeline.ProcessActorsDirectorByCode(actorCodes,ActorByCode,
                DirectorByCode, actorNameDict,MovieActor, MovieDirector));
            var actorCodesTask = actorByCodeTask.ContinueWith(x => Pipeline.ProcessActorsDirectorCodes(ActorByCode, DirectorByCode,
                MovieActor, MovieDirector, actorsbyFilmDict, directorbyFilmDict));
            var waiter = Task.WhenAll(linksTask, ratingTask, actorCodesTask);
            waiter.Wait();

            var moviecodesTask = new Task(() => Pipeline.ProcessAddMovieCodes(movieCodes, filmByNameDict,
                directorbyFilmDict, ratingDict, actorsbyFilmDict, linksDict, filmByCodeDict), TaskCreationOptions.LongRunning);
            moviecodesTask.Start();
            moviecodesTask.Wait();

            var tagslistTask = new Task(() => Pipeline.ProcessCreateTagsList(filmsbyTag, filmByCodeDict, linksDict),TaskCreationOptions.LongRunning);
            var filmsbyactorTask = new Task(() => Pipeline.ProcessFilmsbyActor(actorsbyFilmDict, filmByCodeDict, filmsbyActorName), TaskCreationOptions.LongRunning);
            var filmsbydirectorTask = new Task(() => Pipeline.ProcessFilmsbyDirector(directorbyFilmDict, filmByCodeDict, filmsbyDirectorName), TaskCreationOptions.LongRunning);
            tagslistTask.Start();
            filmsbyactorTask.Start();
            filmsbydirectorTask.Start();
            var wait = Task.WhenAll(tagslistTask, filmsbyactorTask, filmsbydirectorTask);
            wait.Wait();
            Console.WriteLine("Foreach Actors");
            foreach(var actor in filmsbyActorName.Keys)
            {
                filmsbyActorName.TryGetValue(actor, out var list);
                foreach(var mv in list)
                {
                    for (int i = 0; i < list.Count(); i++)
                    {
                        mv.Top10Method(list[i]);
                    }
                }
            }
            Console.WriteLine("Foreach Directors");
            foreach (var director in filmsbyDirectorName.Keys)
            {
                filmsbyDirectorName.TryGetValue(director, out var list);
                foreach (var mv in list)
                {
                    for (int i = 0; i < list.Count(); i++)
                    {
                        mv.Top10Method(list[i]);
                    }
                }
            }
            Console.WriteLine("Foreach Tags");
            foreach(var tag in filmsbyTag.Keys)
            {
                filmsbyTag.TryGetValue(tag, out var list);
                foreach(var mv in list)
                {
                    for (int i = 0;i<list.Count; i++)
                    {
                        if (mv.GetTopCount() < 10) mv.Top10Method(list[i]);
                    }
                }
            }
            Console.WriteLine("Foreach codes");
            foreach(var code in filmByCodeDict.Keys)
            {
                filmByCodeDict.TryGetValue(code, out var mv);
                mv.SetTopTen();
            }
            using (ApplicationContext db = new ApplicationContext())
            {
                Console.WriteLine("Start fulfilling DB");
                db.Database.EnsureDeleted();
                db.Database.EnsureCreated();
                foreach (var key in filmByCodeDict.Keys)
                {
                    filmByCodeDict.TryGetValue(key, out var mv);
                    db.Movies.Add(mv);
                }
                db.SaveChanges();
                Console.WriteLine("End fulfilling DB");
            }

            Console.WriteLine("The end");
        }
    }
}
