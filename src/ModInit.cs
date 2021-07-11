using Nfense.NProxy;

namespace Nfense.ModProxy
{
    internal class ModInit : Module
    {
        ModInit () {}

        public override void Init()
        {
            NProxy.NProxy.AddHandler("proxy", new ProxyHandler());
        }

        public override string GetName()
        {
            return "ModProxy";
        }
    }
}
