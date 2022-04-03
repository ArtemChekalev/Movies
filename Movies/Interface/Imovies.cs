using Movies.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Movies.Interface
{
    public interface Imovies
    {
        IEnumerable<Movie> GetAllMovies();
        string GetMovie(string Name);
        string GetActor(string Name);
        string GetDirector(string Name);
        string GetTag(string Name);
        string FilmByCode(string code);
        string GetValue(string value);
    }
}
