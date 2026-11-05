using WinFormsApp11.bai6._1.BUS;
using WinFormsApp11.bai6._1.DTO;

namespace WinFormsApp11.bai6._1;

public partial class Form1 : Form
{
    private readonly ProductBus productBus = new();
    private readonly CustomerBus customerBus = new();
    private readonly EmployeeBus employeeBus = new();
    private readonly SalesBus salesBus = new();
    private readonly BindingSource productSource = new();
    private readonly BindingSource customerSource = new();
    private readonly BindingSource employeeSource = new();
    private readonly BindingSource cartSource = new();
    private readonly List<CartItemDto> cart = new();
    private readonly DataGridView productGrid = Grid();
    private readonly DataGridView customerGrid = Grid();
    private readonly DataGridView employeeGrid = Grid();
    private readonly DataGridView cartGrid = Grid();
    private readonly DataGridView reportGrid = Grid();
    private readonly TextBox productSearch = new() { Width = 220, PlaceholderText = "Tên hoặc mã sản phẩm" };
    private readonly TextBox productCode = new() { Width = 120 };
    private readonly TextBox productName = new() { Width = 220 };
    private readonly NumericUpDown productPrice = new() { Width = 120, DecimalPlaces = 2, Maximum = 1000000000 };
    private readonly NumericUpDown productStock = new() { Width = 100, Maximum = 1000000000 };
    private readonly TextBox customerName = new() { Width = 200 };
    private readonly TextBox customerPhone = new() { Width = 150 };
    private readonly TextBox employeeName = new() { Width = 200 };
    private readonly TextBox employeePhone = new() { Width = 150 };
    private readonly TextBox employeeRole = new() { Width = 150 };
    private readonly TextBox saleCode = new() { Width = 140, PlaceholderText = "Mã sản phẩm" };
    private readonly NumericUpDown saleQuantity = new() { Width = 80, Minimum = 1, Maximum = 100000, Value = 1 };
    private readonly ComboBox employeeCombo = new() { Width = 220, DropDownStyle = ComboBoxStyle.DropDownList };
    private readonly ComboBox customerCombo = new() { Width = 220, DropDownStyle = ComboBoxStyle.DropDownList };
    private readonly Label totalLabel = new() { AutoSize = true, Text = "Tổng tiền: 0 VNĐ", Font = new Font("Segoe UI", 11, FontStyle.Bold) };

    public Form1()
    {
        InitializeComponent();
        Text = "Hệ thống quản lý bán hàng & siêu thị mini";
        Width = 1200;
        Height = 750;
        BuildUi();
        Load += (_, _) => RefreshAll();
    }

    private void BuildUi()
    {
        var tabs = new TabControl { Dock = DockStyle.Fill };
        tabs.TabPages.Add(BuildProductsTab()); tabs.TabPages.Add(BuildCustomersTab()); tabs.TabPages.Add(BuildEmployeesTab()); tabs.TabPages.Add(BuildSalesTab()); tabs.TabPages.Add(BuildReportsTab());
        Controls.Add(tabs);
    }

    private TabPage BuildProductsTab()
    {
        var page = new TabPage("Sản phẩm"); var top = new FlowLayoutPanel { Dock = DockStyle.Top, Height = 80, Padding = new Padding(8) };
        var search = new Button { Text = "Tìm kiếm" }; search.Click += (_, _) => Safe(() => productSource.DataSource = productBus.Search(productSearch.Text));
        top.Controls.Add(productSearch); top.Controls.Add(search); top.Controls.Add(new Label { Text = "Mã", AutoSize = true, Padding = new Padding(10, 7, 0, 0) }); top.Controls.Add(productCode); top.Controls.Add(new Label { Text = "Tên", AutoSize = true, Padding = new Padding(10, 7, 0, 0) }); top.Controls.Add(productName); top.Controls.Add(new Label { Text = "Giá", AutoSize = true, Padding = new Padding(10, 7, 0, 0) }); top.Controls.Add(productPrice); top.Controls.Add(new Label { Text = "Tồn", AutoSize = true, Padding = new Padding(10, 7, 0, 0) }); top.Controls.Add(productStock);
        var add = new Button { Text = "Thêm" }; add.Click += (_, _) => Safe(() => { productBus.Add(ProductFromFields()); RefreshProducts(); }); var update = new Button { Text = "Sửa" }; update.Click += (_, _) => Safe(() => { productBus.Save(ProductFromFields()); RefreshProducts(); }); var delete = new Button { Text = "Xóa" }; delete.Click += (_, _) => Safe(() => { productBus.Delete(productCode.Text); RefreshProducts(); });
        top.Controls.Add(add); top.Controls.Add(update); top.Controls.Add(delete); productGrid.Dock = DockStyle.Fill; productGrid.DataSource = productSource; productGrid.SelectionChanged += (_, _) => SelectProduct(); page.Controls.Add(productGrid); page.Controls.Add(top); return page;
    }

    private TabPage BuildCustomersTab()
    {
        var page = new TabPage("Khách hàng"); var top = new FlowLayoutPanel { Dock = DockStyle.Top, Height = 42, Padding = new Padding(8) }; top.Controls.Add(new Label { Text = "Tên", AutoSize = true, Padding = new Padding(0, 7, 0, 0) }); top.Controls.Add(customerName); top.Controls.Add(new Label { Text = "Điện thoại", AutoSize = true, Padding = new Padding(10, 7, 0, 0) }); top.Controls.Add(customerPhone);
        var save = new Button { Text = "Lưu (thêm/sửa)" }; save.Click += (_, _) => Safe(() => { customerBus.Save(SelectedCustomer()); RefreshCustomers(); }); var delete = new Button { Text = "Xóa" }; delete.Click += (_, _) => Safe(() => { if (customerGrid.CurrentRow?.DataBoundItem is CustomerDto c) customerBus.Delete(c.MaKH); RefreshCustomers(); }); top.Controls.Add(save); top.Controls.Add(delete); customerGrid.Dock = DockStyle.Fill; customerGrid.DataSource = customerSource; customerGrid.SelectionChanged += (_, _) => { if (customerGrid.CurrentRow?.DataBoundItem is CustomerDto c) { customerName.Text = c.TenKH; customerPhone.Text = c.DienThoai; } }; page.Controls.Add(customerGrid); page.Controls.Add(top); return page;
    }

    private TabPage BuildEmployeesTab()
    {
        var page = new TabPage("Nhân viên"); var top = new FlowLayoutPanel { Dock = DockStyle.Top, Height = 42, Padding = new Padding(8) }; top.Controls.Add(new Label { Text = "Tên", AutoSize = true, Padding = new Padding(0, 7, 0, 0) }); top.Controls.Add(employeeName); top.Controls.Add(new Label { Text = "Điện thoại", AutoSize = true, Padding = new Padding(10, 7, 0, 0) }); top.Controls.Add(employeePhone); top.Controls.Add(new Label { Text = "Chức vụ", AutoSize = true, Padding = new Padding(10, 7, 0, 0) }); top.Controls.Add(employeeRole);
        var save = new Button { Text = "Lưu (thêm/sửa)" }; save.Click += (_, _) => Safe(() => { employeeBus.Save(SelectedEmployee()); RefreshEmployees(); }); var delete = new Button { Text = "Xóa" }; delete.Click += (_, _) => Safe(() => { if (employeeGrid.CurrentRow?.DataBoundItem is EmployeeDto e) employeeBus.Delete(e.MaNV); RefreshEmployees(); }); top.Controls.Add(save); top.Controls.Add(delete); employeeGrid.Dock = DockStyle.Fill; employeeGrid.DataSource = employeeSource; employeeGrid.SelectionChanged += (_, _) => { if (employeeGrid.CurrentRow?.DataBoundItem is EmployeeDto e) { employeeName.Text = e.TenNV; employeePhone.Text = e.SoDienThoai; employeeRole.Text = e.ChucVu; } }; page.Controls.Add(employeeGrid); page.Controls.Add(top); return page;
    }

    private TabPage BuildSalesTab()
    {
        var page = new TabPage("Lập hóa đơn"); var top = new FlowLayoutPanel { Dock = DockStyle.Top, Height = 80, Padding = new Padding(8) }; top.Controls.Add(new Label { Text = "Nhân viên", AutoSize = true, Padding = new Padding(0, 7, 0, 0) }); top.Controls.Add(employeeCombo); top.Controls.Add(new Label { Text = "Khách hàng", AutoSize = true, Padding = new Padding(10, 7, 0, 0) }); top.Controls.Add(customerCombo); top.Controls.Add(saleCode); top.Controls.Add(saleQuantity); var add = new Button { Text = "Thêm vào giỏ" }; add.Click += (_, _) => Safe(AddToCart); var pay = new Button { Text = "Thanh toán" }; pay.Click += (_, _) => Safe(Pay); top.Controls.Add(add); top.Controls.Add(pay); top.Controls.Add(totalLabel); cartGrid.Dock = DockStyle.Fill; cartGrid.DataSource = cartSource; page.Controls.Add(cartGrid); page.Controls.Add(top); return page;
    }

    private TabPage BuildReportsTab()
    {
        var page = new TabPage("Báo cáo"); var top = new FlowLayoutPanel { Dock = DockStyle.Top, Height = 45, Padding = new Padding(8) }; var date = new DateTimePicker { Value = DateTime.Today, Format = DateTimePickerFormat.Short }; var report = new Button { Text = "Doanh thu ngày + Top 5" }; report.Click += (_, _) => Safe(() => { var from = date.Value.Date; var revenue = salesBus.Revenue(from, from.AddDays(1)); totalLabel.Text = $"Doanh thu ngày: {revenue:N0} VNĐ"; reportGrid.DataSource = salesBus.TopProducts(from, from.AddDays(1)); }); top.Controls.Add(date); top.Controls.Add(report); top.Controls.Add(new Label { Text = "Doanh thu hiển thị trên màn hình bán hàng", AutoSize = true, Padding = new Padding(10, 7, 0, 0) }); reportGrid.Dock = DockStyle.Fill; page.Controls.Add(reportGrid); page.Controls.Add(top); return page;
    }

    private void AddToCart()
    {
        var product = productBus.Search(saleCode.Text).FirstOrDefault(x => x.MaSP.Equals(saleCode.Text.Trim(), StringComparison.OrdinalIgnoreCase)); if (product is null) throw new InvalidOperationException("Không tìm thấy mã sản phẩm."); var existing = cart.FirstOrDefault(x => x.MaSP == product.MaSP); if (existing is not null) existing.SoLuong += (int)saleQuantity.Value; else cart.Add(new CartItemDto { MaSP = product.MaSP, TenSP = product.TenSP, DonGia = product.GiaBan, SoLuong = (int)saleQuantity.Value }); RefreshCart();
    }

    private void Pay()
    {
        var employee = employeeCombo.SelectedItem as EmployeeDto ?? throw new InvalidOperationException("Vui lòng chọn nhân viên."); var customer = customerCombo.SelectedItem as CustomerDto; var id = salesBus.Checkout(employee.MaNV, customer?.MaKH, cart, out var total); MessageBox.Show($"Thanh toán thành công hóa đơn #{id}\nTổng tiền: {total:N0} VNĐ", "Thông báo"); cart.Clear(); RefreshCart(); RefreshProducts();
    }

    private void RefreshAll() => Safe(() => { RefreshProducts(); RefreshCustomers(); RefreshEmployees(); });
    private void RefreshProducts() => productSource.DataSource = productBus.Search(productSearch.Text);
    private void RefreshCustomers() { var list = customerBus.GetAll(); customerSource.DataSource = list; customerCombo.Items.Clear(); customerCombo.Items.Add("Khách lẻ"); foreach (var item in list) customerCombo.Items.Add(item); customerCombo.DisplayMember = nameof(CustomerDto.TenKH); customerCombo.SelectedIndex = 0; }
    private void RefreshEmployees() { var list = employeeBus.GetAll(); employeeSource.DataSource = list; employeeCombo.DataSource = list; employeeCombo.DisplayMember = nameof(EmployeeDto.TenNV); }
    private void RefreshCart() { cartSource.DataSource = null; cartSource.DataSource = cart.ToList(); totalLabel.Text = $"Tổng tiền: {cart.Sum(x => x.ThanhTien):N0} VNĐ"; }
    private void SelectProduct() { if (productGrid.CurrentRow?.DataBoundItem is ProductDto p) { productCode.Text = p.MaSP; productName.Text = p.TenSP; productPrice.Value = p.GiaBan; productStock.Value = p.SoLuongTon; } }
    private ProductDto ProductFromFields() => new() { MaSP = productCode.Text.Trim(), TenSP = productName.Text.Trim(), GiaBan = productPrice.Value, SoLuongTon = (int)productStock.Value };
    private CustomerDto SelectedCustomer() => customerGrid.CurrentRow?.DataBoundItem is CustomerDto c ? new CustomerDto { MaKH = c.MaKH, TenKH = customerName.Text.Trim(), DienThoai = customerPhone.Text.Trim() } : new CustomerDto { TenKH = customerName.Text.Trim(), DienThoai = customerPhone.Text.Trim() };
    private EmployeeDto SelectedEmployee() => employeeGrid.CurrentRow?.DataBoundItem is EmployeeDto e ? new EmployeeDto { MaNV = e.MaNV, TenNV = employeeName.Text.Trim(), SoDienThoai = employeePhone.Text.Trim(), ChucVu = employeeRole.Text.Trim() } : new EmployeeDto { TenNV = employeeName.Text.Trim(), SoDienThoai = employeePhone.Text.Trim(), ChucVu = employeeRole.Text.Trim() };
    private static DataGridView Grid() => new() { AutoGenerateColumns = true, ReadOnly = true, AllowUserToAddRows = false, SelectionMode = DataGridViewSelectionMode.FullRowSelect, AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill };
    private static void Safe(Action action) { try { action(); } catch (Exception ex) { MessageBox.Show(ex.Message, "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error); } }
}