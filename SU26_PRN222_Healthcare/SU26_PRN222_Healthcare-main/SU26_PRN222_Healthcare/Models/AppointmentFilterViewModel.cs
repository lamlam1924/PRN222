using System.ComponentModel.DataAnnotations;

namespace SU26_PRN222_Healthcare.Models
{
    public class AppointmentFilterViewModel
    {

        // TỪ KHÓA:
        // tìm theo tên bệnh nhân, tên bác sĩ, chuyên khoa
        public string? Keyword { get; set; }

        // LỌC NGÀY:
        // hiện đang lọc theo AppointmentDate
        [DataType(DataType.Date)]
        public DateTime? FromDate { get; set; }

        [DataType(DataType.Date)]
        public DateTime? ToDate { get; set; }

        // TOP:
        // số lượng record muốn lấy ra
        [Range(1, 1000)]
        public int? Top { get; set; }

        // có lấy lịch đã hủy hay không
        public bool IncludeCancelled { get; set; } = true;

        // BONUS FILTER:
        // có thể dùng nếu muốn lọc theo bác sĩ active
        public bool? DoctorActive { get; set; }

        // BONUS FILTER:
        // có thể dùng nếu muốn lọc theo specialty
        public string? Specialty { get; set; }

    }
}
