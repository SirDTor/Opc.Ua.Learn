using Opc.Ua.InputModels;
using Opc.Ua.OpcBrowser;
using Opc.Ua.OpcBrowser.ViewModels;
using Opc.Ua.OpcConnectionFactory;
using Opc.Ua.OpcConnectionFactory.Models;
using Opc.Ua.OpcHuaReader;
using Opc.Ua.OpcUaReader;
using Opc.Ua.OpcUaReader.InputModels;
using Opc.Ua.OpcUaWriter;
using Opc.Ua.ViewModels;
using System.Collections.ObjectModel;
using System.Data;
using TreeWorker;
using Workstation.ServiceModel.Ua;
using Node = Opc.Ua.InputModels.Node;

namespace OpcUaProject.console
{
    public static class Program
    {
        private const AttributeId attributeId = AttributeId.Value;

        private static OpcUaConnectionFactory channelFactory = new OpcUaConnectionFactory();

        private static OpcUaReader opcReader = new OpcUaReader(channelFactory);

        private static OpcUaWriter opcWritter = new OpcUaWriter(channelFactory);

        private static OpcBrowser opcBrowser = new OpcBrowser(channelFactory);

        [STAThread]
        public static async Task Main(string[] args)
        {
            ObservableCollection<NodeInfo> treeView = new ObservableCollection<NodeInfo>();
            var UidConnection = await ConnectToServer();
            Console.WriteLine(UidConnection.ToString());
            var result = await opcReader.Read(UidConnection, attributeId, new Node("Modbus.TC.TS1", 1));
            foreach (var node in result)
            {
                Console.WriteLine(node.Variant);
            }

            await BrowseOpcServer(UidConnection, treeView);
            await channelFactory.RemoveUaChannel(UidConnection);
            var tree = TreeBuilder.CreateTree(treeView, '.');
            TreeWorker.Program.PrintTree(tree);
            TreeWriter.WriteTreeToFile(tree, "TreeView.txt");
        }

        private static async Task<Guid> ConnectToServer()
        {
            var cim = new ConnectionInputModel("opc.tcp://10.13.33.3:62443",
                new AnonymousIdentity(),
                SecurityPolicy.None,
                ".\\certificate\\",
                "OpcUaReader");
            return await channelFactory.CreateUaChannel(cim);
        }

        //private static async Task BrowseOpcServer(Guid UidConnection, ObservableCollection<NodeInfo> treeView)
        //{
        //    var readTags = await opcBrowser.Browse(UidConnection, new NumericNode(85));
        //    foreach (var node in readTags)
        //    {
        //        if (node.NodeId.NodeId.IdType != IdType.Numeric && node.NodeId.NodeId.Identifier.ToString() != "SERVICE")
        //        {
        //            var browseNext = await opcBrowser.Browse(UidConnection, new Node(node.NodeId.NodeId.Identifier.ToString(), 1));
        //            foreach (var node2 in browseNext)
        //            {
        //                if (node2.DisplayName.Text != "FolderType")
        //                {
        //                    var browseNext1 = await opcBrowser.Browse(UidConnection, new Node(node2.NodeId.NodeId.Identifier.ToString(), 1));
        //                    foreach (var node3 in browseNext1)
        //                    {
        //                        if (node3.DisplayName.Text != "FolderType")
        //                        {
        //                            treeView.Add(new NodeInfo(node3.NodeId.NodeId.Identifier.ToString(), "", ""));
        //                        }
        //                    }
        //                }
        //            }
        //        }
        //    }
        //}

        private static async Task BrowseOpcServer(Guid UidConnection, ObservableCollection<NodeInfo> treeView)
        {
            await BrowseNodes(UidConnection, treeView, new NumericNode(85));
        }

        private static async Task BrowseNodes(Guid UidConnection, ObservableCollection<NodeInfo> treeView, NumericNode node)
        {
            var readTags = await opcBrowser.Browse(UidConnection, node);
            foreach (var currentNode in readTags)
            {
                if (currentNode.NodeId.NodeId.IdType != IdType.Numeric && currentNode.NodeId.NodeId.Identifier.ToString() != "SERVICE")
                {
                    await RecursiveBrowse(UidConnection, treeView, currentNode);
                }
            }
        }

        private static async Task RecursiveBrowse(Guid UidConnection, ObservableCollection<NodeInfo> treeView, Browsed nodeId)
        {
            try
            {
                var browseNext = await opcBrowser.Browse(UidConnection, new Node(nodeId.NodeId.NodeId.Identifier.ToString(), 1));
                Console.WriteLine(browseNext[1].NodeId.NodeId.Identifier);
                foreach (var nextNode in browseNext)
                {
                    if (nextNode.DisplayName.Text != "FolderType")
                    {
                        if(nextNode.NodeId.NodeId.Identifier.ToString() != "63" &&
                           nextNode.NodeId.NodeId.Identifier.ToString() != "68" &&
                           nextNode.NodeId.NodeId.Identifier.ToString() != "62" &&
                           nextNode.TypeDefinition.NodeId.Identifier.ToString() != "68")
                        {                            
                            treeView.Add(new NodeInfo(nextNode.NodeId.NodeId.Identifier.ToString(), "", ""));
                            await RecursiveBrowse(UidConnection, treeView, nextNode);
                        }
                    }
                }
            }
            catch (Exception ex) 
            {
                Console.WriteLine(ex.Message);
                return;
            }
        }
    }
}
