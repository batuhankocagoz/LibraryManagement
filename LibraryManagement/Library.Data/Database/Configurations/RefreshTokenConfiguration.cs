using Library.Core.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Library.Data.Database.Configurations;

public class RefreshTokenConfiguration : IEntityTypeConfiguration<RefreshToken>
{
    public void Configure(EntityTypeBuilder<RefreshToken> builder)
    {
        builder.HasKey(t => t.Id);

        builder.Property(t => t.ExpirationDate).IsRequired();
        builder.Property(t => t.Token).IsRequired();
        builder.Property(t => t.UserId).IsRequired();

        builder.HasOne(t => t.User)
            .WithOne(u => u.RefreshToken)
            .HasForeignKey<RefreshToken>(t => t.UserId);

    }
}
