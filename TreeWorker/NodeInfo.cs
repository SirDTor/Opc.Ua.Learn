using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TreeWorker
{
    public class NodeInfo
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public string? Path { get; set; }

        public NodeInfo(string name, string description, string? path)
        {
            Name = name;
            Description = description;
            Path = path;
        }
    }
}
