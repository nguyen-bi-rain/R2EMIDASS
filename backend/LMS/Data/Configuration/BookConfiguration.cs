using LMS.Models.Enitties;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace LMS.Data.Configuration
{
    public class BookConfiguration : IEntityTypeConfiguration<Book>
    {
        public void Configure(EntityTypeBuilder<Book> builder)
        {
            builder.HasKey(b => b.Id);
            builder.Property(b => b.Id).ValueGeneratedOnAdd().HasDefaultValueSql("NEWID()");
            builder.Property(b => b.Title)
                .IsRequired()
                .HasMaxLength(100);
            builder.Property(b => b.Author)
                .IsRequired()
                .HasMaxLength(100);
            builder.Property(b => b.Description)
                .IsRequired()
                .HasMaxLength(500);
            builder.Property(b => b.Quantity)
                .IsRequired()
                .HasDefaultValue(0);
            builder.Property(b => b.Available)
                .IsRequired()
                .HasDefaultValue(0);
            builder.Property(b => b.CreatedAt)
                .IsRequired()
                .HasDefaultValueSql("GETDATE()");
            builder.Property(b => b.UpdatedAt)
                .IsRequired(false)
                .HasDefaultValueSql("GETUTCDATE()")
                .ValueGeneratedOnUpdate();
        }
    }
}
