using Hidder.Properties;

namespace Hidder.Models
{
    /// <summary>
    /// 多言語化リソースを提供します。
    /// </summary>
    public class ResourceService
    {
        private static readonly ResourceService _currentResourceService = new ResourceService();
        public static ResourceService CurrentResourceService { get { return _currentResourceService; } }

        private readonly Hidder.Properties.Resources _resources = new Resources();

        public Hidder.Properties.Resources Resources { get { return _resources; } }
    }
}
