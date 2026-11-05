using Microsoft.Data.SqlClient;
using WinFormsApp11.bai6._1.DTO;

namespace WinFormsApp11.bai6._1.DAL;

public sealed class ProductDal
{
    public List<ProductDto> Search(string keyword)
    {
        const string sql = "SELECT MaSP, TenSP, GiaBan, SoLuongTon FROM SanPham WHERE MaSP LIKE @Keyword OR TenSP LIKE @Keyword ORDER BY TenSP";
        using var connection = Database.OpenConnection();
        using var command = new SqlCommand(sql, connection);
        command.Parameters.AddWithValue("@Keyword", $"%{keyword}%");
        using var reader = command.ExecuteReader();
        var result = new List<ProductDto>();
        while (reader.Read()) result.Add(Map(reader));
        return result;
    }

    public void Insert(ProductDto item) => Execute("INSERT INTO SanPham(MaSP, TenSP, GiaBan, SoLuongTon) VALUES(@MaSP,@TenSP,@GiaBan,@SoLuongTon)", item);
    public void Update(ProductDto item) => Execute("UPDATE SanPham SET TenSP=@TenSP, GiaBan=@GiaBan, SoLuongTon=@SoLuongTon WHERE MaSP=@MaSP", item);
    public void Delete(string id)
    {
        using var connection = Database.OpenConnection();
        using var command = new SqlCommand("DELETE FROM SanPham WHERE MaSP=@MaSP", connection);
        command.Parameters.AddWithValue("@MaSP", id);
        command.ExecuteNonQuery();
    }

    private static void Execute(string sql, ProductDto item)
    {
        using var connection = Database.OpenConnection();
        using var command = new SqlCommand(sql, connection);
        command.Parameters.AddWithValue("@MaSP", item.MaSP);
        command.Parameters.AddWithValue("@TenSP", item.TenSP);
        command.Parameters.AddWithValue("@GiaBan", item.GiaBan);
        command.Parameters.AddWithValue("@SoLuongTon", item.SoLuongTon);
        command.ExecuteNonQuery();
    }

    private static ProductDto Map(SqlDataReader reader) => new() { MaSP = reader.GetString(0), TenSP = reader.GetString(1), GiaBan = reader.GetDecimal(2), SoLuongTon = reader.GetInt32(3) };
}

public sealed class CustomerDal
{
    public List<CustomerDto> GetAll()
    {
        using var connection = Database.OpenConnection();
        using var command = new SqlCommand("SELECT MaKH, TenKH, DienThoai, DiemTichLuy FROM KhachHang ORDER BY TenKH", connection);
        using var reader = command.ExecuteReader();
        var result = new List<CustomerDto>();
        while (reader.Read()) result.Add(new CustomerDto { MaKH = reader.GetInt32(0), TenKH = reader.GetString(1), DienThoai = reader.IsDBNull(2) ? null : reader.GetString(2), DiemTichLuy = reader.GetInt32(3) });
        return result;
    }
    public void Insert(CustomerDto item) => Execute("INSERT INTO KhachHang(TenKH,DienThoai) VALUES(@TenKH,@DienThoai)", item);
    public void Update(CustomerDto item) => Execute("UPDATE KhachHang SET TenKH=@TenKH,DienThoai=@DienThoai WHERE MaKH=@MaKH", item);
    public void Delete(int id)
    {
        using var connection = Database.OpenConnection();
        using var command = new SqlCommand("DELETE FROM KhachHang WHERE MaKH=@MaKH", connection);
        command.Parameters.AddWithValue("@MaKH", id);
        command.ExecuteNonQuery();
    }
    private static void Execute(string sql, CustomerDto item)
    {
        using var connection = Database.OpenConnection();
        using var command = new SqlCommand(sql, connection);
        command.Parameters.AddWithValue("@TenKH", item.TenKH);
        command.Parameters.AddWithValue("@DienThoai", (object?)item.DienThoai ?? DBNull.Value);
        command.Parameters.AddWithValue("@MaKH", item.MaKH);
        command.ExecuteNonQuery();
    }
}

public sealed class EmployeeDal
{
    public List<EmployeeDto> GetAll()
    {
        using var connection = Database.OpenConnection();
        using var command = new SqlCommand("SELECT MaNV,TenNV,SoDienThoai,ChucVu FROM NhanVien ORDER BY TenNV", connection);
        using var reader = command.ExecuteReader();
        var result = new List<EmployeeDto>();
        while (reader.Read()) result.Add(new EmployeeDto { MaNV = reader.GetInt32(0), TenNV = reader.GetString(1), SoDienThoai = reader.IsDBNull(2) ? null : reader.GetString(2), ChucVu = reader.IsDBNull(3) ? null : reader.GetString(3) });
        return result;
    }
    public void Insert(EmployeeDto item) => Execute("INSERT INTO NhanVien(TenNV,SoDienThoai,ChucVu) VALUES(@TenNV,@SoDienThoai,@ChucVu)", item);
    public void Update(EmployeeDto item) => Execute("UPDATE NhanVien SET TenNV=@TenNV,SoDienThoai=@SoDienThoai,ChucVu=@ChucVu WHERE MaNV=@MaNV", item);
    public void Delete(int id)
    {
        using var connection = Database.OpenConnection();
        using var command = new SqlCommand("DELETE FROM NhanVien WHERE MaNV=@MaNV", connection);
        command.Parameters.AddWithValue("@MaNV", id);
        command.ExecuteNonQuery();
    }
    private static void Execute(string sql, EmployeeDto item)
    {
        using var connection = Database.OpenConnection();
        using var command = new SqlCommand(sql, connection);
        command.Parameters.AddWithValue("@TenNV", item.TenNV);
        command.Parameters.AddWithValue("@SoDienThoai", (object?)item.SoDienThoai ?? DBNull.Value);
        command.Parameters.AddWithValue("@ChucVu", (object?)item.ChucVu ?? DBNull.Value);
        command.Parameters.AddWithValue("@MaNV", item.MaNV);
        command.ExecuteNonQuery();
    }
}

public sealed class SalesDal
{
    public int Checkout(int employeeId, int? customerId, IReadOnlyCollection<CartItemDto> cart, out decimal total)
    {
        total = cart.Sum(x => x.ThanhTien);
        using var connection = Database.OpenConnection();
        using var transaction = connection.BeginTransaction();
        try
        {
            using var invoice = new SqlCommand("INSERT INTO HoaDon(MaNV,MaKH,TongTien) OUTPUT INSERTED.MaHD VALUES(@MaNV,@MaKH,@TongTien)", connection, transaction);
            invoice.Parameters.AddWithValue("@MaNV", employeeId);
            invoice.Parameters.AddWithValue("@MaKH", (object?)customerId ?? DBNull.Value);
            invoice.Parameters.AddWithValue("@TongTien", total);
            var invoiceId = (int)invoice.ExecuteScalar()!;

            foreach (var item in cart)
            {
                using var detail = new SqlCommand("INSERT INTO ChiTietHoaDon(MaHD,MaSP,SoLuong,DonGia) VALUES(@MaHD,@MaSP,@SoLuong,@DonGia)", connection, transaction);
                detail.Parameters.AddWithValue("@MaHD", invoiceId);
                detail.Parameters.AddWithValue("@MaSP", item.MaSP);
                detail.Parameters.AddWithValue("@SoLuong", item.SoLuong);
                detail.Parameters.AddWithValue("@DonGia", item.DonGia);
                detail.ExecuteNonQuery();

                using var stock = new SqlCommand("UPDATE SanPham SET SoLuongTon=SoLuongTon-@SoLuong WHERE MaSP=@MaSP AND SoLuongTon>=@SoLuong", connection, transaction);
                stock.Parameters.AddWithValue("@SoLuong", item.SoLuong);
                stock.Parameters.AddWithValue("@MaSP", item.MaSP);
                if (stock.ExecuteNonQuery() != 1) throw new InvalidOperationException($"Sản phẩm {item.MaSP} không đủ tồn kho.");
            }

            if (customerId.HasValue)
            {
                using var points = new SqlCommand("UPDATE KhachHang SET DiemTichLuy=DiemTichLuy+@Diem WHERE MaKH=@MaKH", connection, transaction);
                points.Parameters.AddWithValue("@Diem", decimal.ToInt32(total / 100000m));
                points.Parameters.AddWithValue("@MaKH", customerId.Value);
                points.ExecuteNonQuery();
            }
            transaction.Commit();
            return invoiceId;
        }
        catch
        {
            transaction.Rollback();
            throw;
        }
    }

    public decimal Revenue(DateTime from, DateTime to)
    {
        using var connection = Database.OpenConnection();
        using var command = new SqlCommand("SELECT COALESCE(SUM(TongTien),0) FROM HoaDon WHERE NgayLap>=@From AND NgayLap<@To", connection);
        command.Parameters.AddWithValue("@From", from);
        command.Parameters.AddWithValue("@To", to);
        return Convert.ToDecimal(command.ExecuteScalar());
    }

    public List<TopProductDto> TopProducts(DateTime from, DateTime to)
    {
        const string sql = "SELECT TOP 5 c.MaSP, s.TenSP, SUM(c.SoLuong) AS SoLuongBan FROM ChiTietHoaDon c INNER JOIN HoaDon h ON h.MaHD=c.MaHD INNER JOIN SanPham s ON s.MaSP=c.MaSP WHERE h.NgayLap>=@From AND h.NgayLap<@To GROUP BY c.MaSP,s.TenSP ORDER BY SoLuongBan DESC";
        using var connection = Database.OpenConnection();
        using var command = new SqlCommand(sql, connection);
        command.Parameters.AddWithValue("@From", from);
        command.Parameters.AddWithValue("@To", to);
        using var reader = command.ExecuteReader();
        var result = new List<TopProductDto>();
        while (reader.Read()) result.Add(new TopProductDto { MaSP = reader.GetString(0), TenSP = reader.GetString(1), SoLuongBan = Convert.ToInt32(reader.GetInt64(2)) });
        return result;
    }
}
