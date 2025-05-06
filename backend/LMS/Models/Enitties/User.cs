using LMS.Models.Enitties.Base;
using LMS.Models.Enums;

namespace LMS.Models.Enitties;
public class User : BaseEntity<Guid>
{
    public string UserName { get; set; }
    public string Email { get; set; }
    public string PasswordHash { get; set; }
    public string PhoneNumber { get; set; }
    public Gender Gender { get; set; }
    public string RefreshToken {get; set; }
    public DateTime RefreshTokenExpiryTime { get; set; }
    public int RoleId { get; set; }
    public Role Role { get; set; }
    public ICollection<BookBorrowingRequest> BookBorrowingRequestors { get; set; }
    public ICollection<BookBorrowingRequest>  BookBorrowingApprovers{ get; set; }
}
