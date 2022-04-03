using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Movies.Data
{
    public class Actor
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
            return int.Parse(ActorId.Trim('n', 'm'));
        }

        public override bool Equals(object obj)
        {
            Actor actor = obj as Actor;
            if (ActorId == actor.ActorId) return true;
            else return false;
        }

        public override string ToString()
        {
            string res = "";
            res += Name + '\n';
            foreach (var mv in Movies) res += mv.FilmName + "_";
            return res;
        }
    }
}
