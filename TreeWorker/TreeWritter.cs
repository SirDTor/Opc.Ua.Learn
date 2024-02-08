using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TreeWorker
{
    using System;
    using System.IO;

    public static class TreeWriter
    {
        public static void WriteTreeToFile(TreeNode root, string filename)
        {
            try
            {
                using (StreamWriter writer = new StreamWriter(filename))
                {
                    WriteNodeToFile(root, writer, "",false);
                }
                Console.WriteLine("Tree structure has been written to the file successfully.");
            }
            catch (IOException e)
            {
                Console.WriteLine("An error occurred while writing to the file: " + e.Message);
            }
        }

        private static void WriteNodeToFile(TreeNode node, StreamWriter writer, string prefix, bool isLastChild)
        {
            // Write the current node to the file with the prefix
            writer.WriteLine($"{prefix}{(isLastChild ? "└──" : "├──")}{node.Info.Name}");

            // Recursively write the children of the current node with updated prefix
            for (int i = 0; i < node.Children.Count; i++)
            {
                string newPrefix = $"{prefix}{(isLastChild ? "    " : "│   ")}";
                bool isLast = i == node.Children.Count - 1;
                WriteNodeToFile(node.Children[i], writer, newPrefix, isLast);
            }
        }
    }
}
