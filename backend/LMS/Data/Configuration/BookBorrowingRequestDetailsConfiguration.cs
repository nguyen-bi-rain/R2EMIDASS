using LMS.Models.Enitties;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace LMS.Data.Configuration;

public class BookBorrowingRequestDetailsConfiguration : IEntityTypeConfiguration<BookBorrowingRequestDetails>
{
    public void Configure(EntityTypeBuilder<BookBorrowingRequestDetails> builder)
    {
        builder.HasKey(b => b.Id);
        builder.Property(b => b.Id).ValueGeneratedOnAdd();
        builder.Property(b => b.BookId).IsRequired();
        builder.Property(b => b.CreatedAt).IsRequired().HasDefaultValueSql("GETUTCDATE()");
        builder.Property(b => b.UpdatedAt).IsRequired(false).ValueGeneratedOnUpdate().HasDefaultValueSql("GETUTCDATE()");
        builder.HasOne(b => b.Book)
            .WithMany( x => x.BookBorrowingRequestDetails)
            .HasForeignKey(b => b.BookId)
            .OnDelete(DeleteBehavior.NoAction);
        builder.HasOne(b => b.BookBorrowingRequest)
            .WithMany(b => b.BookBorrowingRequestDetails)
            .HasForeignKey(b => b.BookBorrowingRequestId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
