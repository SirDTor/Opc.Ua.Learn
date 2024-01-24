using TreeWorker;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TreeWorker
{
    public static class Program
    {
        [STAThread]
        public static async Task Main()
        {
            var inputs = new ObservableCollection<NodeInfo>
            {
                new NodeInfo("A.1", "1", ""),
                new NodeInfo("A.2", "2", ""),
                new NodeInfo("A.3.B.1", "3", ""),
                new NodeInfo("A.3.B.2.C.1", "4", ""),
                new NodeInfo("A.4.D.1.E.1", "5", ""),
                new NodeInfo("A.4.D.2.F.1", "6", "")
            };
            ObservableCollection<NodeInfo>? inp = await ReaderCsv.ReadTagsFromFileAsync();
            
            var tree = TreeBuilder.CreateTree(inputs, '.');
            PrintTree(tree);
            var fileName1 = @"tree1";
            TreeVisualizer.TreeVisualizer.CreateTreeImage(tree, fileName1);
            Console.WriteLine($"Дерево номер 1 сохранено в файл {fileName1}.png в папке с программой \n\n");

            var intTree = TreeBuilder.CreateTree(inp, '.');
            PrintTree(intTree);
            var fileName2 = @"tree2";
            TreeVisualizer.TreeVisualizer.CreateTreeImage(intTree, fileName2);

            Console.WriteLine($"Дерево номер 2 сохранено в файл {fileName2}.png в папке с программой");
        }

        public static void PrintTree(TreeNode node, string indent = "", bool last = true)
        {
            // Определяем символы для текущего уровня вложенности
            var branch = last ? "└─ " : "├─ ";
            var line = last ? "   " : "│  ";

            // Выводим информацию о текущем узле
            Console.Write(indent);
            Console.Write(branch);
            Console.WriteLine($"{node.Info.Name} - {node.Info.Description}");

            // Рекурсивно вызываем этот метод для каждого дочернего узла
            for (int i = 0; i < node.Children.Count; i++)
            {
                var child = node.Children[i];
                var isLast = i == node.Children.Count - 1;
                var newIndent = indent + line;
                PrintTree(child, newIndent, isLast);
            }
        }
    }
}
