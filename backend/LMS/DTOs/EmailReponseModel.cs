
namespace LMS.DTOs
{
    public class EmailBodyModel
    {

        public int Id { get; set; }
        public DateTime DateRequest { get; set; }
        public string UserName { get; set; }
        public string Status { get; set; }
        public string Message { get; set; }
        public string StatusColor { get; set; }
    }
}