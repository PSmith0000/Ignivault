namespace ignivault.WebAPI.Data.Entities
{
    public class StoredBlob
    {
        [Key]
        public Guid BlobId { get; set; }

        public int VaultItemId { get; set; }
        public VaultEntity VaultItem { get; set; }

        public byte[] Data { get; set; }
    }
}
