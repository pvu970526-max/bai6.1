namespace WinFormsApp11.bai6._1.DTO;

public sealed class ProductDto
{
    public string MaSP { get; set; } = "";
    public string TenSP { get; set; } = "";
    public decimal GiaBan { get; set; }
    public int SoLuongTon { get; set; }
}

public sealed class CustomerDto
{
    public int MaKH { get; set; }
    public string TenKH { get; set; } = "";
    public string? DienThoai { get; set; }
    public int DiemTichLuy { get; set; }
}

public sealed class EmployeeDto
{
    public int MaNV { get; set; }
    public string TenNV { get; set; } = "";
    public string? SoDienThoai { get; set; }
    public string? ChucVu { get; set; }
}

public sealed class CartItemDto
{
    public string MaSP { get; set; } = "";
    public string TenSP { get; set; } = "";
    public decimal DonGia { get; set; }
    public int SoLuong { get; set; }
    public decimal ThanhTien => DonGia * SoLuong;
}

public sealed class TopProductDto
{
    public string MaSP { get; set; } = "";
    public string TenSP { get; set; } = "";
    public int SoLuongBan { get; set; }
}
