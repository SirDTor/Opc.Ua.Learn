using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TreeWorker;

namespace TreeWorker
{
    public class TreeNode
    {
        public NodeInfo Info { get; set; }
        public List<TreeNode> Children { get; set; }

        public TreeNode(NodeInfo info)
        {
            Info = info;
            Children = new List<TreeNode>();
        }
    }
}
