using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Big_data
{
    class Tag
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

        public void SetMovies(List<Movie> mv)
        {
            Movies = mv;
        }

        public List<Movie> GetMovies() => Movies;
    }
}
