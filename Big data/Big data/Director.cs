using System.Collections.Generic;

namespace Big_data
{
    class Director
    {
        public string DirectorId { get; set; }
        public string Name { get; set; }
        public List<Movie> Movies { get; set; }

        public Director(string id, string name)
        {
            DirectorId = id;
            Name = name;
        }
        public Director()
        {

        }

        public static bool operator ==(Director dir1, Director dir2)
        {
            try
            {
                if ((dir1 is null)||(dir2 is null)) return false;
                else if (dir1.DirectorId == dir2.DirectorId) return true;
                else return false;
            }
            catch (System.NullReferenceException)
            {
                return false;
            }
        }

        public static bool operator !=(Director dir1, Director dir2)
        {
            if (dir2 == null) return true;
            if (dir1.DirectorId != dir2.DirectorId) return true;
            else return false;
        }

        public override int GetHashCode()
        {
            return int.Parse(DirectorId.Trim('n', 'm'));
        }

        public override bool Equals(object obj)
        {
            Director director = obj as Director;
            if (DirectorId == director.DirectorId) return true;
            else return false;
        }

    }
}
