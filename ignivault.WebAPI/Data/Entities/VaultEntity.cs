namespace ignivault.WebAPI.Data.Entities
{
    /// <summary>
    /// VaultEntity represents a single item stored in a user's secure vault, such as passwords, notes, or files.
    /// </summary>
    public class VaultEntity
    {
        [Key]
        public int ItemId { get; set; }
        public string UserId { get; set; }

        public string Name { get; set; }

        public bool IsBlobStored { get; set; } = false;

        public VaultItemType ItemType { get; set; }

        public byte[] EncryptedData { get; set; }

        public Guid? BlobId { get; set; }
        public StoredBlob StoredBlob { get; set; }

        public LoginUser User { get; set; }

        public byte[] Iv { get; set; } //per-item IV for encryption

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
    }
}
