using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;



namespace Big_data
{
    class Actor
    {
        public string ActorId { get; set; }
        public string Name { get; set; }
        public List<Movie> Movies { get; set; }

        public Actor(string id, string name)
        {
            ActorId = id;
            Name = name;
        }

        public Actor()
        {

        }

        public static bool operator ==(Actor act1, Actor act2)
        {
            if (act1.ActorId == act2.ActorId) return true;
            else return false;
        }

        public static bool operator !=(Actor act1, Actor act2)
        {
            if (act1.ActorId != act2.ActorId) return true;
            else return false;
        }

        public override int GetHashCode()
        {
            return int.Parse(ActorId.Trim('n','m'));
        }

        public override bool Equals(object obj)
        {
            Actor actor = obj as Actor;
            if (ActorId == actor.ActorId) return true;
            else return false;
        }

        public void SetMovies(List<Movie> mv)
        {
            Movies = mv;
        }

        public List<Movie> GetMovies() => Movies;
        
    }
}
