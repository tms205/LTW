/* ==========================================================
   TẠO DATABASE
   ========================================================== */
CREATE DATABASE QL_NHAHANG;
GO
USE QL_NHAHANG;
GO

/* ==========================================================
   1. KHÁCH HÀNG
   ========================================================== */
CREATE TABLE KHACHHANG
(
    MaKH VARCHAR(10) PRIMARY KEY,
    HoTen NVARCHAR(50) NOT NULL,
    SDT CHAR(10) NOT NULL,
    LoaiKH NVARCHAR(30) DEFAULT N'Vãng lai',
    DiemTichLuy INT DEFAULT 0 CHECK (DiemTichLuy >= 0)
);
GO

/* ==========================================================
   2. BÀN ĂN
   ========================================================== */
CREATE TABLE BANAN
(
    MaBan VARCHAR(10) PRIMARY KEY,
    TenBan NVARCHAR(50) NOT NULL,
    SucChua INT CHECK (SucChua > 0),
    TrangThai NVARCHAR(30) DEFAULT N'Trống'
);
GO

/* ==========================================================
   3. LOẠI MÓN
   ========================================================== */
CREATE TABLE LOAIMON
(
    MaLoai VARCHAR(10) PRIMARY KEY,
    TenLoai NVARCHAR(100) NOT NULL
);
GO

/* ==========================================================
   4. MÓN ĂN (ĐÃ GỘP MÔ TẢ + HÌNH ẢNH)
   ========================================================== */
CREATE TABLE MONAN
(
    MaMon VARCHAR(10) PRIMARY KEY,
    TenMon NVARCHAR(100) NOT NULL,
    Gia INT CHECK(Gia > 0),
    DonViTinh NVARCHAR(30),
    TrangThai NVARCHAR(30) DEFAULT N'Còn món',
    MaLoai VARCHAR(10) FOREIGN KEY REFERENCES LOAIMON(MaLoai),
    MoTa NVARCHAR(255) NULL,
    HinhAnh NVARCHAR(255) NULL   -- GỘP ẢNH Ở ĐÂY
);
GO

/* ==========================================================
   5. NHÂN VIÊN
   ========================================================== */
CREATE TABLE NHANVIEN
(
    MaNV VARCHAR(10) PRIMARY KEY,
    HoTen NVARCHAR(50) NOT NULL,
    VaiTro NVARCHAR(30) CHECK (VaiTro IN 
        (N'Thu ngân', N'Phục vụ', N'Bếp', N'Thủ kho', N'Quản lý')),
    Luong INT CHECK (Luong >= 0)
);
GO

/* ==========================================================
   6. ĐẶT BÀN
   ========================================================== */
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

/* ==========================================================
   7. HÓA ĐƠN
   ========================================================== */
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

/* ==========================================================
   8. CHI TIẾT HÓA ĐƠN
   ========================================================== */
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

/* ==========================================================
   9. TÀI KHOẢN
   ========================================================== */
CREATE TABLE TAIKHOAN
(
    TenDangNhap NVARCHAR(50) PRIMARY KEY,
    MatKhau NVARCHAR(200) NOT NULL,
    MaNV VARCHAR(10) NULL FOREIGN KEY REFERENCES NHANVIEN(MaNV),
    MaKH VARCHAR(10) NULL FOREIGN KEY REFERENCES KHACHHANG(MaKH),
    Quyen NVARCHAR(50) CHECK (Quyen IN ('Admin','User','Guest'))
);
GO

/* ==========================================================
   10. NGUYÊN LIỆU
   ========================================================== */
CREATE TABLE NGUYENLIEU
(
    MaNL VARCHAR(10) PRIMARY KEY,
    TenNL NVARCHAR(100) NOT NULL,
    DonViTinh NVARCHAR(20),
    SoLuongTon INT DEFAULT 0 CHECK (SoLuongTon >= 0),
    MucCanhBao INT DEFAULT 10
);
GO

/* ==========================================================
   11. MÓN – NGUYÊN LIỆU
   ========================================================== */
CREATE TABLE MONAN_NGUYENLIEU
(
    MaMon VARCHAR(10) FOREIGN KEY REFERENCES MONAN(MaMon),
    MaNL VARCHAR(10) FOREIGN KEY REFERENCES NGUYENLIEU(MaNL),
    SoLuong INT CHECK (SoLuong > 0),
    PRIMARY KEY(MaMon, MaNL)
);
GO

/* ==========================================================
   12. PHIẾU NHẬP KHO
   ========================================================== */
CREATE TABLE PHIEUNHAPKHO
(
    MaPN VARCHAR(10) PRIMARY KEY,
    NgayNhap DATE DEFAULT GETDATE(),
    MaNV VARCHAR(10) FOREIGN KEY REFERENCES NHANVIEN(MaNV)
);
GO

/* ==========================================================
   13. CHI TIẾT NHẬP KHO
   ========================================================== */
CREATE TABLE CHITIET_NHAPKHO
(
    MaPN VARCHAR(10) FOREIGN KEY REFERENCES PHIEUNHAPKHO(MaPN),
    MaNL VARCHAR(10) FOREIGN KEY REFERENCES NGUYENLIEU(MaNL),
    SoLuong INT CHECK (SoLuong > 0),
    DonGia INT CHECK (DonGia >= 0),
    PRIMARY KEY(MaPN, MaNL)
);
GO

/* ==========================================================
   14. NHẬT KÝ PHÂN QUYỀN
   ========================================================== */
CREATE TABLE NHATKY_PHANQUYEN
(
    ID INT IDENTITY PRIMARY KEY,
    AdminThucHien NVARCHAR(50) FOREIGN KEY REFERENCES TAIKHOAN(TenDangNhap),
    NguoiDuocGiao NVARCHAR(50) FOREIGN KEY REFERENCES TAIKHOAN(TenDangNhap),
    QuyenCu NVARCHAR(50),
    QuyenMoi NVARCHAR(50),
    ThoiGian DATETIME DEFAULT GETDATE()
);
GO

/* ==========================================================
   15. NHẬT KÝ ĐĂNG NHẬP
   ========================================================== */
CREATE TABLE NHATKY_DANGNHAP
(
    ID INT IDENTITY PRIMARY KEY,
    TenDangNhap NVARCHAR(50),
    ThoiGian DATETIME DEFAULT GETDATE(),
    DiaChiIP NVARCHAR(50)
);
GO

/* ==========================================================
   DỮ LIỆU MẪU
   ========================================================== */

/* --- KHÁCH HÀNG --- */
INSERT INTO KHACHHANG VALUES
('KH01', N'Nguyễn Văn A', '0909123456', N'Thân thiết', 120),
('KH02', N'Lê Thị B', '0912345678', N'Vãng lai', 0),
('KH03', N'Trần Hữu C', '0988777666', N'Thân thiết', 250),
('KH04', N'Phạm Ngọc D', '0977333444', N'Vãng lai', 0),
('KH05', N'Hoàng Thị E', '0933555777', N'Thân thiết', 90);
GO

/* --- BÀN ĂN --- */
INSERT INTO BANAN VALUES
('B01', N'Bàn 1', 4, N'Đang dùng'),
('B02', N'Bàn 2', 4, N'Trống'),
('B03', N'Bàn 3', 6, N'Trống'),
('B04', N'Bàn 4', 2, N'Chờ dọn'),
('B05', N'Bàn VIP 1', 8, N'Đang dùng');
GO

/* --- LOẠI MÓN --- */
INSERT INTO LOAIMON VALUES
('LM01', N'Món chính'),
('LM02', N'Món khai vị'),
('LM03', N'Tráng miệng'),
('LM04', N'Đồ uống'),
('LM05', N'Món chay');
GO

/* ============================
   MÓN ĂN (20 món – phân đúng 5 loại)
   ============================ */
INSERT INTO MONAN 
(MaMon, TenMon, Gia, DonViTinh, TrangThai, MaLoai, MoTa, HinhAnh) VALUES

/* ====================== LM01 – MÓN CHÍNH ====================== */
('MA01', N'Cơm chiên hải sản', 50000, N'Phần', N'Còn món', 'LM01',
N'Cơm chiên tôm và mực tươi, hạt cơm tơi, thơm mùi tỏi phi. 
Gia vị đậm đà, không quá dầu mỡ. 
Phù hợp cho mọi bữa ăn trong ngày.
Món được yêu thích tại nhà hàng.',
'/img/monan/comchien.jpg'),

('MA02', N'Phở bò tái', 60000, N'Bát', N'Còn món', 'LM01',
N'Nước dùng được hầm từ xương bò trong nhiều giờ. 
Thịt bò tái mềm, tươi, kết hợp hương vị quế – hồi. 
Sợi phở mềm, thơm mùi gạo đặc trưng.
Món Việt truyền thống.',
'/img/monan/phobo.jpg'),

('MA03', N'Mì cay hải sản', 60000, N'Tô', N'Còn món', 'LM01',
N'Hải sản tươi: tôm, mực, nghêu hòa trong nước dùng cay đậm đà. 
Cấp độ cay có thể tùy chọn theo sở thích. 
Phong cách Hàn Quốc được giới trẻ yêu thích.',
'/img/monan/micay.jpg'),

('MA04', N'Sườn nướng BBQ', 85000, N'Phần', N'Còn món', 'LM01',
N'Sườn heo được ướp sốt BBQ đặc biệt, nướng thơm lừng. 
Thịt mềm, thấm gia vị, không bị khô. 
Phù hợp dùng cùng khoai tây hoặc salad.',
'/img/monan/suonbbq.jpg'),

/* ====================== LM02 – KHAI VỊ ====================== */
('MA05', N'Gỏi cuốn tôm thịt', 30000, N'Phần', N'Còn món', 'LM02',
N'Bánh tráng mềm cuốn tôm, thịt, bún và rau sống. 
Chấm cùng nước mắm chua ngọt chuẩn vị Nam. 
Món nhẹ, ít dầu mỡ, dễ ăn.',
'/img/monan/goicuon.jpg'),

('MA06', N'Khoai tây chiên', 25000, N'Đĩa', N'Còn món', 'LM02',
N'Khoai chiên vàng giòn bên ngoài, mềm bên trong. 
Không thấm nhiều dầu, phù hợp mọi lứa tuổi. 
Món khai vị phổ biến nhất.',
'/img/monan/khoaitaychien.jpg'),

('MA07', N'Salad trộn dầu giấm', 30000, N'Đĩa', N'Còn món', 'LM02',
N'Rau củ tươi giòn, được rửa sạch bằng công nghệ ozone. 
Dầu giấm pha chế chua nhẹ, giúp kích thích vị giác. 
Tốt cho sức khỏe và hệ tiêu hóa.',
'/img/monan/salad.jpg'),

('MA08', N'Da cá chiên giòn', 35000, N'Phần', N'Còn món', 'LM02',
N'Da cá hồi chiên giòn rụm, tẩm muối tiêu nhẹ. 
Giòn tan và thơm đặc trưng. 
Món nhắm lý tưởng cho mọi bữa tiệc.',
'/img/monan/daca.jpg'),

/* ====================== LM03 – TRÁNG MIỆNG ====================== */
('MA09', N'Bánh flan caramel', 20000, N'Phần', N'Còn món', 'LM03',
N'Flan mềm mịn, tan ngay khi đưa vào miệng. 
Caramel thơm nhẹ, ngọt vừa. 
Món tráng miệng phù hợp mọi lứa tuổi.',
'/img/monan/flan.jpg'),

('MA10', N'Chè khúc bạch', 28000, N'Ly', N'Còn món', 'LM03',
N'Khúc bạch thơm sữa, mềm nhưng không bở. 
Nước chè thanh mát với nhãn và hạnh nhân rang. 
Thích hợp dùng lạnh trong ngày nóng.',
'/img/monan/khuclbach.jpg'),

('MA11', N'Tào phớ nóng', 20000, N'Bát', N'Còn món', 'LM03',
N'Tào phớ mịn, béo nhẹ vị đậu nành. 
Nước đường gừng ấm, thơm dịu. 
Món tráng miệng truyền thống Việt Nam.',
'/img/monan/taopho.jpg'),

('MA12', N'Kem vani socola', 25000, N'Ly', N'Còn món', 'LM03',
N'Kem vani mát lạnh kết hợp sốt socola đậm đà. 
Topping cốm hoặc đậu phộng tùy chọn. 
Phù hợp trẻ nhỏ và gia đình.',
'/img/monan/kemvani.jpg'),

/* ====================== LM04 – ĐỒ UỐNG ====================== */
('MA13', N'Sinh tố bơ', 35000, N'Ly', N'Còn món', 'LM04',
N'Sinh tố bơ sáp nguyên chất, béo mịn. 
Không pha loãng, giữ nguyên hương vị tự nhiên. 
Giúp bổ sung năng lượng và vitamin.',
'/img/monan/sinhobo.jpg'),

('MA14', N'Cà phê sữa đá', 25000, N'Ly', N'Còn món', 'LM04',
N'Cà phê phin đậm vị kết hợp sữa đặc ngọt béo. 
Thức uống quen thuộc của người Việt. 
Giúp tỉnh táo và tập trung.',
'/img/monan/cafesua.jpg'),

('MA15', N'Trà chanh mật ong', 30000, N'Ly', N'Còn món', 'LM04',
N'Trà xanh thanh mát hòa với mật ong rừng. 
Thêm lát chanh vàng tạo hương thơm tự nhiên. 
Giải nhiệt tuyệt vời.',
'/img/monan/trachanh.jpg'),

('MA16', N'Nước cam ép', 35000, N'Ly', N'Còn món', 'LM04',
N'Nước cam vắt từ cam tươi 100%. 
Bổ sung vitamin C, tốt cho sức khỏe. 
Không đường cho khách ăn kiêng.',
'/img/monan/nuoccam.jpg'),

/* ====================== LM05 – MÓN CHAY ====================== */
('MA17', N'Đậu hũ chiên sả ớt', 40000, N'Phần', N'Còn món', 'LM05',
N'Đậu hũ chiên vàng, giòn nhẹ. 
Sả ớt phi thơm tạo hương vị hấp dẫn. 
Món chay ngon, dễ ăn và giàu dinh dưỡng.',
'/img/monan/dauhu.jpg'),

('MA18', N'Đậu hũ sốt cà chua', 35000, N'Phần', N'Còn món', 'LM05',
N'Món đậu hũ mềm được chiên sơ rồi sốt cà chua. 
Vị chua ngọt dễ ăn, quen thuộc trong bữa cơm Việt. 
Thích hợp người ăn chay.',
'/img/monan/dauhusot.jpg'),

('MA19', N'Rau củ xào thập cẩm', 45000, N'Đĩa', N'Còn món', 'LM05',
N'Bông cải, cà rốt, nấm, đậu que xào giòn nhẹ. 
Ít dầu mỡ, nhiều chất xơ. 
Hương vị thanh đạm nhưng hấp dẫn.',
'/img/monan/raucuxao.jpg'),

('MA20', N'Lẩu nấm chay', 120000, N'Nồi', N'Còn món', 'LM05',
N'Nước dùng từ rau củ hầm trong nhiều giờ. 
Kết hợp nấm kim châm, nấm đùi gà, nấm đông cô. 
Món lẩu thanh mát, tốt cho sức khỏe.',
'/img/monan/launam.jpg');

GO


/* --- NHÂN VIÊN --- */
INSERT INTO NHANVIEN VALUES
('NV01', N'Trần Thị B', N'Thu ngân', 8000000),
('NV02', N'Nguyễn Minh C', N'Phục vụ', 7000000),
('NV03', N'Lê Văn D', N'Bếp', 9000000),
('NV04', N'Phạm Thị E', N'Thủ kho', 8500000),
('NV05', N'Võ Hoàng F', N'Quản lý', 12000000);
GO

/* --- ĐẶT BÀN --- */
INSERT INTO DATBAN VALUES
('DB01', 'KH01', 'B01', '2025-10-20', '18:30', N'Đang dùng'),
('DB02', 'KH02', 'B02', '2025-10-20', '19:00', N'Đang dùng'),
('DB03', 'KH03', 'B03', '2025-10-19', '12:00', N'Đã thanh toán'),
('DB04', 'KH04', 'B04', '2025-10-18', '20:00', N'Hủy'),
('DB05', 'KH05', 'B05', '2025-10-20', '18:00', N'Đang dùng');
GO

/* --- HÓA ĐƠN --- */
INSERT INTO HOADON VALUES
('HD01', 'DB01', 'NV01', '2025-10-20', 85000, N'Tiền mặt'),
('HD02', 'DB02', 'NV01', '2025-10-20', 120000, N'Tiền mặt'),
('HD03', 'DB03', 'NV02', '2025-10-19', 150000, N'POS'),
('HD04', 'DB05', 'NV05', '2025-10-20', 95000, N'Tiền mặt'),
('HD05', 'DB04', 'NV01', '2025-10-18', 0, N'Hủy');
GO

/* --- CHI TIẾT HÓA ĐƠN --- */
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

/* --- TÀI KHOẢN --- */
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

/* ==========================================================
   TRIGGER TRỪ NGUYÊN LIỆU KHI BÁN MÓN
   ========================================================== */
CREATE TRIGGER TRG_TruKhoSauKhiBan
ON CHITIETHOADON
AFTER INSERT
AS
BEGIN
    UPDATE NL
    SET NL.SoLuongTon = NL.SoLuongTon - (i.SoLuong * mnl.SoLuong)
    FROM NGUYENLIEU NL
    JOIN MONAN_NGUYENLIEU mnl ON NL.MaNL = mnl.MaNL
    JOIN inserted i ON i.MaMon = mnl.MaMon;
END
GO
