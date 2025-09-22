using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace ignivault.Shared.DTOs.Vault
{
    public class ReEncryptedItemDto
    {
        public int ItemId { get; set; }
        [Required]
        public byte[] EncryptedData { get; set; }
        [Required]
        public byte[] Iv { get; set; }
    }

    public class VaultItemSummaryDto
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public VaultItemType ItemType { get; set; }
        public DateTime UpdatedAt { get; set; }
    }

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

    public class UpdateVaultItemDto
    {
        [Required, StringLength(100)]
        public string Name { get; set; }
        public byte[]? EncryptedData { get; set; }
        public byte[]? Iv { get; set; }
    }

    public class FileDownloadDto
    {
        public string FileName { get; set; }
        public string ContentType { get; set; }
        public byte[] EncryptedData { get; set; }
    }

    public enum VaultItemType
    {
        [Display(Name = "File / Document")]
        File,

        [Display(Name = "Secure Note")]
        Note,

        [Display(Name = "Login Credential")]
        Credential
    }

    public class FileUploadRequestDto
    {
        [Required]
        public IFormFile File { get; set; }

        [Required]
        public string IvBase64 { get; set; }
    }
}