using System.Diagnostics;
using System.IO;
using System.Text;
using TreeWorker;
using QuickGraph;

namespace TreeVisualizer
{
    public static class TreeVisualizer
    {
        public static void CreateTreeImage(TreeNode tree, string fileName)
        {
            // Создаем новый граф
            var graph = new BidirectionalGraph<string, IEdge<string>>();

            // Создаем вершину-корень
            var root = tree.Info.Name.ToString();
            graph.AddVertex(root);

            // Рекурсивно добавляем дочерние вершины
            AddChildNodes(tree, root, graph);

            // Создаем строку с описанием графа в формате DOT
            var dot = new StringBuilder();
            dot.AppendLine("digraph G {");
            foreach (var edge in graph.Edges)
            {
                dot.AppendLine($"\"{edge.Source}\" -> \"{edge.Target}\";");
            }
            dot.AppendLine("}");

            // Сохраняем строку в файл
            File.WriteAllText(fileName + ".dot", dot.ToString());

            // Генерируем изображение с помощью Graphviz
            var processStartInfo = new ProcessStartInfo
            {
                FileName = "C:\\Program Files\\Graphviz\\bin\\dot.exe",
                Arguments = $"-Tpng \"{fileName}.dot\" -o \"{fileName}.png\"",
                UseShellExecute = false,
                RedirectStandardOutput = true,
                CreateNoWindow = true
            };
            using (var process = Process.Start(processStartInfo))
            {
                process.WaitForExit();
            }

            // Удаляем временный файл с описанием графа в формате DOT
            File.Delete(fileName + ".dot");
        }

        private static void AddChildNodes(TreeNode tree, string parent, BidirectionalGraph<string, IEdge<string>> graph)
        {
            foreach (var child in tree.Children)
            {
                // Создаем новую вершину для каждого ребенка
                var node = child.Info.Name.ToString();
                graph.AddVertex(node);
                graph.AddEdge(new Edge<string>(parent, node));

                // Рекурсивно добавляем дочерние вершины
                AddChildNodes(child, node, graph);
            }
        }

    }
}

