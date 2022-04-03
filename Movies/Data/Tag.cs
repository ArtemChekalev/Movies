using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Movies.Data
{
    public class Tag
    {
        public string TagId { get; set; }
        public string Name { get; set; }
        public List<Movie> Movies { get; set; }

        public Tag(string id, string name)
        {
            TagId = id;
            Name = name;
        }

        public Tag()
        {

        }

        public override bool Equals(object obj)
        {
            Tag tag = obj as Tag;
            if (TagId == tag.TagId) return true;
            else return false;
        }

        public static bool operator ==(Tag tag1, Tag tag2)
        {
            if (tag1.TagId == tag2.TagId) return true;
            else return false;
        }

        public static bool operator !=(Tag tag1, Tag tag2)
        {
            if (tag1.TagId != tag2.TagId) return true;
            else return false;
        }

        public override int GetHashCode()
        {
            return int.Parse(TagId);
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
