namespace LMS.Models.Enitties.Base;
public interface IBaseEntity<T>
{
    public T Id { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
    // public User CreatedBy { get; set; }
    // public User UpdatedBy { get; set; }
}
