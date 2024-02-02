using System.Diagnostics;
using System.IO;
using System.Text;
using TreeWorker;
using QuickGraph;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.Drawing;

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

        public static void DrawTree(TreeNode root, string filename)
        {
            int nodeSize = 40;
            int marginX = 700;
            int marginY = 30;
            int levelHeight = 100;

            Bitmap bitmap = new Bitmap(1920, 1080);
            Graphics graphics = Graphics.FromImage(bitmap);
            graphics.SmoothingMode = SmoothingMode.HighQuality;

            Font font = new Font("Arial", 10);

            int x = marginX;
            int y = marginY;

            DrawNode(root, x, y);

            void DrawNode(TreeNode node, int x, int y)
            {
                graphics.FillEllipse(Brushes.White, x - nodeSize / 2, y - nodeSize / 2, nodeSize, nodeSize);
                graphics.DrawEllipse(Pens.Black, x - nodeSize / 2, y - nodeSize / 2, nodeSize, nodeSize);
                graphics.DrawString(node.Info.Name, font, Brushes.Black, x - 10, y - 10);

                if (node.Children != null)
                {
                    int childY = y + levelHeight;
                    int childCount = node.Children.Count;
                    int childX = x - ((childCount - 1) * 50) / 2;

                    foreach (var child in node.Children)
                    {
                        graphics.DrawLine(Pens.Black, x, y + nodeSize / 2, childX + nodeSize / 2 - 20, childY - nodeSize / 2);
                        DrawNode(child, childX, childY);
                        childX += 50;
                    }
                }
            }

            bitmap.Save(filename, ImageFormat.Png);
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

