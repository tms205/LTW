-- =============================================
-- TẠO CSDL
-- =============================================
CREATE DATABASE QL_NHAHANG;
GO
USE QL_NHAHANG;
GO

-- =============================================
-- 1. KHÁCH HÀNG
-- =============================================
CREATE TABLE KHACHHANG
(
    MaKH VARCHAR(10) PRIMARY KEY,
    HoTen NVARCHAR(50) NOT NULL,
    SDT CHAR(10) NOT NULL,
    LoaiKH NVARCHAR(30) DEFAULT N'Vãng lai',
    DiemTichLuy INT DEFAULT 0 CHECK (DiemTichLuy >= 0)
);
GO

-- =============================================
-- 2. BÀN ĂN
-- =============================================
CREATE TABLE BANAN
(
    MaBan VARCHAR(10) PRIMARY KEY,
    TenBan NVARCHAR(50) NOT NULL,
    SucChua INT CHECK (SucChua > 0),
    TrangThai NVARCHAR(30) DEFAULT N'Trống'
);
GO

-- =============================================
-- 3. LOẠI MÓN
-- =============================================
CREATE TABLE LOAIMON
(
    MaLoai VARCHAR(10) PRIMARY KEY,
    TenLoai NVARCHAR(100) NOT NULL
);
GO

-- =============================================
-- 4. MÓN ĂN
-- =============================================
CREATE TABLE MONAN
(
    MaMon VARCHAR(10) PRIMARY KEY,
    TenMon NVARCHAR(100) NOT NULL,
    Gia INT CHECK(Gia > 0),
    DonViTinh NVARCHAR(30),
    TrangThai NVARCHAR(30) DEFAULT N'Còn món',
    MaLoai VARCHAR(10) FOREIGN KEY REFERENCES LOAIMON(MaLoai)
);
GO

-- =============================================
-- 5. NHÂN VIÊN
-- =============================================
CREATE TABLE NHANVIEN
(
    MaNV VARCHAR(10) PRIMARY KEY,
    HoTen NVARCHAR(50) NOT NULL,
    VaiTro NVARCHAR(30) CHECK (VaiTro IN (N'Thu ngân', N'Phục vụ', N'Bếp', N'Thủ kho', N'Quản lý')),
    Luong INT CHECK (Luong >= 0)
);
GO

-- =============================================
-- 6. ĐẶT BÀN
-- =============================================
CREATE TABLE DATBAN
(
    MaDatBan VARCHAR(10) PRIMARY KEY,
    MaKH VARCHAR(10) FOREIGN KEY REFERENCES KHACHHANG(MaKH),
    MaBan VARCHAR(10) FOREIGN KEY REFERENCES BANAN(MaBan),
    NgayDat DATE DEFAULT GETDATE(),
    GioDat TIME NOT NULL,
    TrangThai NVARCHAR(50) DEFAULT N'Đang dùng'
);
GO

-- =============================================
-- 7. HÓA ĐƠN
-- =============================================
CREATE TABLE HOADON
(
    MaHD VARCHAR(10) PRIMARY KEY,
    MaDatBan VARCHAR(10) FOREIGN KEY REFERENCES DATBAN(MaDatBan),
    MaNV VARCHAR(10) FOREIGN KEY REFERENCES NHANVIEN(MaNV),
    NgayLap DATETIME DEFAULT GETDATE(),
    TongTien INT CHECK (TongTien >= 0),
    PhuongThucTT NVARCHAR(30) DEFAULT N'Tiền mặt'
);
GO

-- =============================================
-- 8. CHI TIẾT HÓA ĐƠN
-- =============================================
CREATE TABLE CHITIETHOADON
(
    MaHD VARCHAR(10),
    MaMon VARCHAR(10),
    SoLuong INT CHECK (SoLuong > 0),
    DonGia INT NOT NULL,
    ThanhTien AS (SoLuong * DonGia) PERSISTED,
    PRIMARY KEY(MaHD, MaMon),
    FOREIGN KEY (MaHD) REFERENCES HOADON(MaHD),
    FOREIGN KEY (MaMon) REFERENCES MONAN(MaMon)
);
GO

-- =============================================
-- 9. TÀI KHOẢN
-- =============================================
CREATE TABLE TAIKHOAN
(
    TenDangNhap NVARCHAR(50) PRIMARY KEY,
    MatKhau NVARCHAR(200) NOT NULL,
    MaNV VARCHAR(10) NULL FOREIGN KEY REFERENCES NHANVIEN(MaNV),
    MaKH VARCHAR(10) NULL FOREIGN KEY REFERENCES KHACHHANG(MaKH),
    Quyen NVARCHAR(50) CHECK (Quyen IN (N'Admin', N'User', N'Guest'))
);
GO

-- =============================================
-- DỮ LIỆU MẪU
-- =============================================

-- KHÁCH HÀNG
INSERT INTO KHACHHANG VALUES
('KH01', N'Nguyễn Văn A', '0909123456', N'Thân thiết', 120),
('KH02', N'Lê Thị B', '0912345678', N'Vãng lai', 0),
('KH03', N'Trần Hữu C', '0988777666', N'Thân thiết', 250),
('KH04', N'Phạm Ngọc D', '0977333444', N'Vãng lai', 0),
('KH05', N'Hoàng Thị E', '0933555777', N'Thân thiết', 90);
GO

-- BÀN ĂN
INSERT INTO BANAN VALUES
('B01', N'Bàn 1', 4, N'Đang dùng'),
('B02', N'Bàn 2', 4, N'Trống'),
('B03', N'Bàn 3', 6, N'Trống'),
('B04', N'Bàn 4', 2, N'Chờ dọn'),
('B05', N'Bàn VIP 1', 8, N'Đang dùng');
GO

-- LOẠI MÓN
INSERT INTO LOAIMON VALUES
('LM01', N'Món chính'),
('LM02', N'Món khai vị'),
('LM03', N'Tráng miệng'),
('LM04', N'Đồ uống'),
('LM05', N'Món chay');
GO

-- MÓN ĂN (đã sửa lỗi tham số)
INSERT INTO MONAN VALUES
('MA01', N'Cơm chiên hải sản', 50000, N'Phần', N'Còn món', 'LM01'),
('MA02', N'Gỏi cuốn tôm thịt', 30000, N'Phần', N'Còn món', 'LM02'),
('MA03', N'Bánh flan', 20000, N'Phần', N'Còn món', 'LM03'),
('MA04', N'Sinh tố bơ', 35000, N'Ly', N'Còn món', 'LM04'),
('MA05', N'Đậu hủ chiên sả ớt', 40000, N'Phần', N'Còn món', 'LM05');
GO

-- NHÂN VIÊN
INSERT INTO NHANVIEN VALUES
('NV01', N'Trần Thị B', N'Thu ngân', 8000000),
('NV02', N'Nguyễn Minh C', N'Phục vụ', 7000000),
('NV03', N'Lê Văn D', N'Bếp', 9000000),
('NV04', N'Phạm Thị E', N'Thủ kho', 8500000),
('NV05', N'Võ Hoàng F', N'Quản lý', 12000000);
GO

-- ĐẶT BÀN
INSERT INTO DATBAN VALUES
('DB01', 'KH01', 'B01', '2025-10-20', '18:30', N'Đang dùng'),
('DB02', 'KH02', 'B02', '2025-10-20', '19:00', N'Đang dùng'),
('DB03', 'KH03', 'B03', '2025-10-19', '12:00', N'Đã thanh toán'),
('DB04', 'KH04', 'B04', '2025-10-18', '20:00', N'Hủy'),
('DB05', 'KH05', 'B05', '2025-10-20', '18:00', N'Đang dùng');
GO

-- HÓA ĐƠN
INSERT INTO HOADON VALUES
('HD01', 'DB01', 'NV01', '2025-10-20', 85000, N'Tiền mặt'),
('HD02', 'DB02', 'NV01', '2025-10-20', 120000, N'Tiền mặt'),
('HD03', 'DB03', 'NV02', '2025-10-19', 150000, N'POS'),
('HD04', 'DB05', 'NV05', '2025-10-20', 95000, N'Tiền mặt'),
('HD05', 'DB04', 'NV01', '2025-10-18', 0, N'Hủy');
GO

-- CHI TIẾT HÓA ĐƠN
INSERT INTO CHITIETHOADON (MaHD, MaMon, SoLuong, DonGia) VALUES
('HD01', 'MA01', 1, 50000),
('HD01', 'MA02', 1, 30000),
('HD01', 'MA04', 1, 35000),
('HD02', 'MA05', 2, 40000),
('HD02', 'MA03', 1, 20000),
('HD03', 'MA01', 2, 50000),
('HD03', 'MA04', 2, 35000),
('HD04', 'MA05', 1, 40000),
('HD04', 'MA03', 1, 20000),
('HD04', 'MA02', 1, 30000);
GO

-- TÀI KHOẢN
INSERT INTO TAIKHOAN VALUES
(N'admin',    	N'123456', 'NV05', NULL, N'Admin'),
(N'thungan01',	N'pass01', 'NV01', NULL, N'User'),
(N'phucvu01', 	N'pass02', 'NV02', NULL, N'User'),
(N'bep01',    	N'pass03', 'NV03', NULL, N'User'),
(N'thukho01', 	N'pass04', 'NV04', NULL, N'User'),
(N'khach01',  	N'1111', NULL, 'KH01', N'Guest'),
(N'khach02',  	N'2222', NULL, 'KH02', N'Guest'),
(N'khach03',  	N'3333', NULL, 'KH03', N'Guest'),
(N'khach04',  	N'4444', NULL, 'KH04', N'Guest'),
(N'khach05',  	N'5555', NULL, 'KH05', N'Guest');
GO
