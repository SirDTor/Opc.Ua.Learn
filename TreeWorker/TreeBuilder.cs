using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TreeWorker;

namespace OpcUaClient
{
    public static class TreeBuilder
    {
        public static TreeNode CreateTree(ObservableCollection<NodeInfo> inputs, char fieldSeparator)
        {
            var rootInfo = new NodeInfo("root", "", "");
            var root = new TreeNode(rootInfo);

            foreach (var input in inputs)
            {
                var fields = input.Name.Split(fieldSeparator);
                var currentNode = root;
                var description = fields.Length == input.Name.Split(fieldSeparator).Length ? input.Description : "";

                foreach (var field in fields)
                {
                    var name = field;
                    var existingNode = currentNode.Children.Find(x => x.Info.Name == name);

                    if (existingNode != null)
                    {
                        currentNode = existingNode;

                        if (name == fields.Last())
                        {
                            currentNode.Info.Description = description;
                        }
                    }
                    else
                    {
                        var newNodeInfo = new NodeInfo(name, "", "");
                        var newNode = new TreeNode(newNodeInfo);

                        if (name == fields.Last())
                        {
                            newNode.Info.Description = description;
                        }

                        currentNode.Children.Add(newNode);
                        newNode.Info.Path = $"{currentNode.Info.Path}.{newNode.Info.Name}".TrimStart('.');
                        currentNode = newNode;
                    }
                }
            }

            return root;
        }

        private static void SetPaths(TreeNode node, string parentPath)
        {
            node.Info.Path = $"{parentPath}.{node.Info.Name}".TrimStart('.');

            foreach (var child in node.Children)
            {
                SetPaths(child, node.Info.Path);
            }
        }
    }
}
