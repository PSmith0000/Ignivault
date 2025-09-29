namespace ignivault.WebAPI.Data.Entities
{
    /// <summary>
    /// StoredBlob represents a large binary object (BLOB) associated with a vault item, such as an encrypted file.
    /// </summary>
    public class StoredBlob
    {
        [Key]
        public Guid BlobId { get; set; }

        public int VaultItemId { get; set; }
        public VaultEntity VaultItem { get; set; }

        public byte[] Data { get; set; }
    }
}
