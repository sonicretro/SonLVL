using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SonicRetro.SonLVL.API
{
    public class NamedList<T> : List<T>
    {
        public string Name { get; set; }

        public NamedList(string name)
            : base()
        {
            Name = name;
        }

        public NamedList(string name, int capacity)
            : base(capacity)
        {
            Name = name;
        }

        public NamedList(string name, IEnumerable<T> collection)
            : base(collection)
        {
            Name = name;
        }
    }
}
