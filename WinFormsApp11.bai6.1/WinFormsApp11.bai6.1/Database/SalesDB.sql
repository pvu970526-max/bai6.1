IF DB_ID(N'SalesDB') IS NULL
BEGIN
	CREATE DATABASE SalesDB;
END
GO

USE SalesDB;
GO

IF OBJECT_ID(N'dbo.ChiTietHoaDon', N'U') IS NOT NULL DROP TABLE dbo.ChiTietHoaDon;
IF OBJECT_ID(N'dbo.HoaDon', N'U') IS NOT NULL DROP TABLE dbo.HoaDon;
IF OBJECT_ID(N'dbo.SanPham', N'U') IS NOT NULL DROP TABLE dbo.SanPham;
IF OBJECT_ID(N'dbo.KhachHang', N'U') IS NOT NULL DROP TABLE dbo.KhachHang;
IF OBJECT_ID(N'dbo.NhanVien', N'U') IS NOT NULL DROP TABLE dbo.NhanVien;
GO

CREATE TABLE dbo.NhanVien
(
	MaNV int IDENTITY(1,1) NOT NULL CONSTRAINT PK_NhanVien PRIMARY KEY,
	TenNV nvarchar(100) NOT NULL,
	SoDienThoai varchar(15) NULL CONSTRAINT UQ_NhanVien_SoDienThoai UNIQUE,
	ChucVu nvarchar(50) NULL
);

CREATE TABLE dbo.KhachHang
(
	MaKH int IDENTITY(1,1) NOT NULL CONSTRAINT PK_KhachHang PRIMARY KEY,
	TenKH nvarchar(100) NOT NULL,
	DienThoai varchar(15) NULL CONSTRAINT UQ_KhachHang_DienThoai UNIQUE,
	DiemTichLuy int NOT NULL CONSTRAINT DF_KhachHang_Diem DEFAULT 0,
	CONSTRAINT CK_KhachHang_Diem CHECK (DiemTichLuy >= 0)
);

CREATE TABLE dbo.SanPham
(
	MaSP varchar(20) NOT NULL CONSTRAINT PK_SanPham PRIMARY KEY,
	TenSP nvarchar(150) NOT NULL,
	GiaBan decimal(18,2) NOT NULL,
	SoLuongTon int NOT NULL CONSTRAINT DF_SanPham_Ton DEFAULT 0,
	CONSTRAINT CK_SanPham_Gia CHECK (GiaBan > 0),
	CONSTRAINT CK_SanPham_Ton CHECK (SoLuongTon >= 0)
);

CREATE TABLE dbo.HoaDon
(
	MaHD int IDENTITY(1,1) NOT NULL CONSTRAINT PK_HoaDon PRIMARY KEY,
	NgayLap datetime NOT NULL CONSTRAINT DF_HoaDon_Ngay DEFAULT GETDATE(),
	MaNV int NOT NULL,
	MaKH int NULL,
	TongTien decimal(18,2) NOT NULL,
	CONSTRAINT FK_HoaDon_NhanVien FOREIGN KEY (MaNV) REFERENCES dbo.NhanVien(MaNV),
	CONSTRAINT FK_HoaDon_KhachHang FOREIGN KEY (MaKH) REFERENCES dbo.KhachHang(MaKH),
	CONSTRAINT CK_HoaDon_TongTien CHECK (TongTien >= 0)
);

CREATE TABLE dbo.ChiTietHoaDon
(
	MaHD int NOT NULL,
	MaSP varchar(20) NOT NULL,
	SoLuong int NOT NULL,
	DonGia decimal(18,2) NOT NULL,
	CONSTRAINT PK_ChiTietHoaDon PRIMARY KEY (MaHD, MaSP),
	CONSTRAINT FK_ChiTiet_HoaDon FOREIGN KEY (MaHD) REFERENCES dbo.HoaDon(MaHD),
	CONSTRAINT FK_ChiTiet_SanPham FOREIGN KEY (MaSP) REFERENCES dbo.SanPham(MaSP),
	CONSTRAINT CK_ChiTiet_SoLuong CHECK (SoLuong > 0),
	CONSTRAINT CK_ChiTiet_DonGia CHECK (DonGia > 0)
);
GO

CREATE INDEX IX_SanPham_TenSP ON dbo.SanPham(TenSP);
CREATE INDEX IX_HoaDon_NgayLap ON dbo.HoaDon(NgayLap);
GO

INSERT INTO dbo.NhanVien (TenNV, SoDienThoai, ChucVu) VALUES
(N'Nguyễn Văn An', '0900000001', N'Thu ngân'),
(N'Trần Thị Bình', '0900000002', N'Quản lý');

INSERT INTO dbo.KhachHang (TenKH, DienThoai) VALUES
(N'Khách hàng thân thiết', '0911111111'),
(N'Nguyễn Minh Châu', '0922222222');

INSERT INTO dbo.SanPham (MaSP, TenSP, GiaBan, SoLuongTon) VALUES
('SP001', N'Nước suối 500ml', 5000, 100),
('SP002', N'Mì ăn liền', 7000, 80),
('SP003', N'Sữa tươi hộp', 32000, 50);
GO
