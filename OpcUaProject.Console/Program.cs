using Opc.Ua.InputModels;
using Opc.Ua.OpcBrowser;
using Opc.Ua.OpcConnectionFactory;
using Opc.Ua.OpcConnectionFactory.Models;
using Opc.Ua.OpcHuaReader;
using Opc.Ua.OpcUaReader;
using Opc.Ua.OpcUaReader.InputModels;
using Opc.Ua.ViewModels;
using Workstation.ServiceModel.Ua;
using Node = Opc.Ua.InputModels.Node;

namespace OpcUaProject.console
{
    public static class Program
    {
        private const AttributeId attributeId = AttributeId.NodeId;

        [STAThread]
        public static async Task Main(string[] args)
        {
            var channelFactory = new OpcUaConnectionFactory();
            var opcReader = new OpcUaReader(channelFactory);
            var opcBrowser = new OpcBrowser(channelFactory);
            var opcHuaReader = new OpcHuaReader(channelFactory);
            var cim = new ConnectionInputModel("opc.tcp://10.13.33.1:62450",
                new AnonymousIdentity(),
                SecurityPolicy.None,
                ".\\certificate\\",
                "Name");
            var UidConnection = await channelFactory.CreateUaChannel(cim);

            await Task.Delay(1000);
            var result = await opcReader.Read(UidConnection,
            attributeId,
            new Node("", 1));
            var readTags = await opcBrowser.Browse(UidConnection, new Node("", 1));
            foreach (var node in result)
            {
                Console.WriteLine(node.Variant);
            }

            await channelFactory.RemoveUaChannel(UidConnection);
        }
    }
}
