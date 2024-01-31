using Opc.Ua.InputModels;
using Opc.Ua.OpcBrowser;
using Opc.Ua.OpcConnectionFactory;
using Opc.Ua.OpcConnectionFactory.Models;
using Opc.Ua.OpcHuaReader;
using Opc.Ua.OpcUaReader;
using Opc.Ua.OpcUaReader.InputModels;
using Opc.Ua.ViewModels;
using System.Collections.ObjectModel;
using TreeWorker;
using Workstation.ServiceModel.Ua;
using Node = Opc.Ua.InputModels.Node;

namespace OpcUaProject.console
{
    public static class Program
    {
        private const AttributeId attributeId = AttributeId.Description;

        [STAThread]
        public static async Task Main(string[] args)
        {
            ObservableCollection<NodeInfo> treeView = new ObservableCollection<NodeInfo>();

            var channelFactory = new OpcUaConnectionFactory();
            var opcReader = new OpcUaReader(channelFactory);
            var opcBrowser = new OpcBrowser(channelFactory);
            //var opcHuaReader = new OpcHuaReader(channelFactory);
            var cim = new ConnectionInputModel("opc.tcp://10.13.33.1:62450",
                new AnonymousIdentity(),
                SecurityPolicy.None,
                ".\\certificate\\",
                "Name");
            var UidConnection = await channelFactory.CreateUaChannel(cim);
            var result = await opcReader.Read(UidConnection, attributeId, new Node("Modbus.TC.TS1", 1));
            var readTags = await opcBrowser.Browse(UidConnection, new NumericNode(85));
            foreach (var node in result)
            {
                Console.WriteLine(node.Variant);
            }
            foreach (var node in readTags)
            {
                if (node.NodeId.NodeId.IdType != IdType.Numeric && node.NodeId.NodeId.Identifier.ToString() != "SERVICE")
                {
                    //Console.WriteLine("->" + node.NodeId.NodeId.Identifier);
                    var browseNext = await opcBrowser.Browse(UidConnection, new Node(node.NodeId.NodeId.Identifier.ToString(), 1));
                    foreach (var node2 in browseNext)
                    {
                        if (node2.DisplayName.Text != "FolderType")
                        {
                            //Console.WriteLine("    ->" + node2.DisplayName);
                            var browseNext1 = await opcBrowser.Browse(UidConnection, new Node(node2.NodeId.NodeId.Identifier.ToString(), 1));
                            foreach (var node3 in browseNext1)
                            {
                                if (node3.DisplayName.Text != "FolderType")
                                {
                                    //Console.WriteLine("        ->" + node3.NodeId.NodeId.Identifier);
                                    treeView.Add(new NodeInfo(node3.NodeId.NodeId.Identifier.ToString(), "", ""));
                                }
                            }
                        }
                    }
                }
            }
            await channelFactory.RemoveUaChannel(UidConnection);
            var tree = TreeBuilder.CreateTree(treeView, '.');
            TreeWorker.Program.PrintTree(tree);
        }
    }
}
