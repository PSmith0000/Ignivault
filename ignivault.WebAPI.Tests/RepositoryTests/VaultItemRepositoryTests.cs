using ignivault.WebAPI.Data;
using ignivault.WebAPI.Data.Entities;
using ignivault.WebAPI.Data.Repositories;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ignivault.WebAPI.Tests.RepositoryTests
{
    public class VaultItemRepositoryTests
    {
        [Fact]
        public async Task GetItemByIdAndUserIdAsync_ShouldReturnCorrectItem()
        {
            var options = new DbContextOptionsBuilder<AppDbContext>().UseInMemoryDatabase(databaseName: "VaultTestDb").Options;

            var testUserId = "user123";
            var expectedItem = new VaultEntity
            {
                ItemId = 1,
                UserId = testUserId,
                Name = "Test Item",
                EncryptedData = Array.Empty<byte>(),
                Iv = Array.Empty<byte>()
            };

            using (var context = new AppDbContext(options))
            {
                context.VaultItems.Add(expectedItem);
                context.SaveChanges();
            }

            using (var context = new AppDbContext(options))
            {
                var repository = new VaultItemRepository(context);
                var result = await repository.GetItemByIdAndUserIdAsync(1, testUserId);

                Assert.NotNull(result);
                Assert.Equal(expectedItem.Name, result.Name);
                Assert.Equal(testUserId, result.UserId);
            }
        }
    }
}
