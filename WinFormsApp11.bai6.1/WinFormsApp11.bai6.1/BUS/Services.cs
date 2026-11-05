using WinFormsApp11.bai6._1.DAL;
using WinFormsApp11.bai6._1.DTO;

namespace WinFormsApp11.bai6._1.BUS;

public sealed class ProductBus
{
    private readonly ProductDal dal = new();
    public List<ProductDto> Search(string keyword) => dal.Search(keyword.Trim());
    public void Save(ProductDto item)
    {
        if (string.IsNullOrWhiteSpace(item.MaSP) || string.IsNullOrWhiteSpace(item.TenSP)) throw new ArgumentException("Mã và tên sản phẩm không được trống.");
        if (item.GiaBan <= 0 || item.SoLuongTon < 0) throw new ArgumentException("Giá bán phải > 0 và tồn kho phải >= 0.");
        try { dal.Update(item); } catch (Microsoft.Data.SqlClient.SqlException ex) when (ex.Number == 0) { throw; }
    }
    public void Add(ProductDto item)
    {
        if (string.IsNullOrWhiteSpace(item.MaSP) || string.IsNullOrWhiteSpace(item.TenSP) || item.GiaBan <= 0 || item.SoLuongTon < 0) throw new ArgumentException("Thông tin sản phẩm không hợp lệ.");
        dal.Insert(item);
    }
    public void Delete(string id) => dal.Delete(id);
}

public sealed class CustomerBus
{
    private readonly CustomerDal dal = new();
    public List<CustomerDto> GetAll() => dal.GetAll();
    public void Save(CustomerDto item)
    {
        if (string.IsNullOrWhiteSpace(item.TenKH)) throw new ArgumentException("Tên khách hàng không được trống.");
        if (item.MaKH == 0) dal.Insert(item); else dal.Update(item);
    }
    public void Delete(int id) => dal.Delete(id);
}

public sealed class EmployeeBus
{
    private readonly EmployeeDal dal = new();
    public List<EmployeeDto> GetAll() => dal.GetAll();
    public void Save(EmployeeDto item)
    {
        if (string.IsNullOrWhiteSpace(item.TenNV)) throw new ArgumentException("Tên nhân viên không được trống.");
        if (item.MaNV == 0) dal.Insert(item); else dal.Update(item);
    }
    public void Delete(int id) => dal.Delete(id);
}

public sealed class SalesBus
{
    private readonly SalesDal dal = new();
    public int Checkout(int employeeId, int? customerId, IReadOnlyCollection<CartItemDto> cart, out decimal total)
    {
        if (employeeId <= 0) throw new ArgumentException("Vui lòng chọn nhân viên.");
        if (cart.Count == 0) throw new ArgumentException("Giỏ hàng đang trống.");
        if (cart.Any(x => x.SoLuong <= 0)) throw new ArgumentException("Số lượng bán phải lớn hơn 0.");
        return dal.Checkout(employeeId, customerId, cart, out total);
    }
    public decimal Revenue(DateTime from, DateTime to) => dal.Revenue(from, to);
    public List<TopProductDto> TopProducts(DateTime from, DateTime to) => dal.TopProducts(from, to);
}
