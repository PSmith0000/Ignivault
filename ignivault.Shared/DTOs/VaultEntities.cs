using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace ignivault.Shared.DTOs.Vault
{
    /// <summary>
    /// ReEncryptedItemDto is used when re-encrypting existing vault items with new encryption keys.
    /// </summary>
    public class ReEncryptedItemDto
    {
        public int ItemId { get; set; }
        [Required]
        public byte[] EncryptedData { get; set; }
        [Required]
        public byte[] Iv { get; set; }
    }

    /// <summary>
    /// VaultItemSummaryDto provides a summary view of a vault item, excluding sensitive encrypted data.
    /// </summary>
    public class VaultItemSummaryDto
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public VaultItemType ItemType { get; set; }
        public DateTime UpdatedAt { get; set; }
    }

    /// <summary>
    /// VaultItemDetailDto provides detailed information about a vault item, including its encrypted data and metadata.
    /// </summary>
    public class VaultItemDetailDto
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public VaultItemType ItemType { get; set; }
        public byte[] EncryptedData { get; set; }
        public byte[] Iv { get; set; }
        public Guid? BlobId { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
    }

    /// <summary>
    /// CreateVaultItemDto is used to create a new vault item with a pre-encrypted payload.
    /// </summary>
    public class CreateVaultItemDto
    {
        [Required, StringLength(100)]
        public string Name { get; set; }
        [Required]
        public VaultItemType ItemType { get; set; }
        [Required]
        public byte[] EncryptedData { get; set; }
        [Required]
        public byte[] Iv { get; set; }
    }

    /// <summary>
    /// UpdateVaultItemDto is used to update an existing vault item's name and optionally its encrypted data.
    /// </summary>
    public class UpdateVaultItemDto
    {
        [Required, StringLength(100)]
        public string Name { get; set; }
        public byte[]? EncryptedData { get; set; }
        public byte[]? Iv { get; set; }
    }

    /// <summary>
    /// FileDownloadDto encapsulates the data needed for a user to download an encrypted file from the vault.
    /// </summary>
    public class FileDownloadDto
    {
        public string FileName { get; set; }
        public string ContentType { get; set; }
        public byte[] EncryptedData { get; set; }
    }

    /// <summary>
    /// VaultItemType defines the types of items that can be stored in the vault.
    /// </summary>
    public enum VaultItemType
    {
        [Display(Name = "File / Document")]
        File,

        [Display(Name = "Secure Note")]
        Note,

        [Display(Name = "Login Credential")]
        Credential
    }

    /// <summary>
    /// FileUploadRequestDto is used to encapsulate the file upload along with its associated IV for encryption purposes.
    /// </summary>
    public class FileUploadRequestDto
    {
        [Required]
        public IFormFile File { get; set; }

        [Required]
        public string IvBase64 { get; set; }
    }
}