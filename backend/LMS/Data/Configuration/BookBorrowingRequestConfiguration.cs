using LMS.Models.Enitties;
using LMS.Models.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace LMS.Data.Configuration
{
    public class BookBorrowingRequestConfiguration : IEntityTypeConfiguration<BookBorrowingRequest>
    {
        public void Configure(EntityTypeBuilder<BookBorrowingRequest> builder)
        {
            builder.HasKey(b => b.Id);
            builder.Property(b => b.Id).ValueGeneratedOnAdd();
            builder.Property(b => b.DateRequest).IsRequired().HasDefaultValueSql("GETUTCDATE()");
            builder.Property(b => b.Status)
                .HasDefaultValue(Status.Waiting)
                .HasConversion<int>()
                .IsRequired(false);
            builder.Property(b => b.CreatedAt)
                .IsRequired()
                .HasDefaultValueSql("GETUTCDATE()");
            builder.Property(b => b.UpdatedAt)
                .IsRequired(false)
                .HasDefaultValueSql("GETUTCDATE()")
                .ValueGeneratedOnUpdate();
            builder.HasOne(b => b.Requestor)
                .WithMany(x => x.BookBorrowingRequestors)
                .HasForeignKey(b => b.RequestorId)
                .OnDelete(DeleteBehavior.NoAction);
            builder.HasOne(b => b.Approver)
                .WithMany(x => x.BookBorrowingApprovers)
                .HasForeignKey(b => b.ApproverId)
                .OnDelete(DeleteBehavior.NoAction);
        }
    }
}
