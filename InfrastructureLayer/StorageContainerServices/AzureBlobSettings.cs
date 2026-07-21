namespace InfrastructureLayer.StorageContainerServices
{
    public class AzureBlobSettings
    {
        public const string SectionName = "StorageAccount";
        public string StorageName { get; set; } = string.Empty;
        public string ContainerName { get; set; } = string.Empty;
        public string OrdersContainerName { get; set; } = string.Empty;
    }
}
