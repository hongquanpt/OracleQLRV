using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace OracleQLRV.Models;

public partial class AppDbContext : DbContext
{
    public AppDbContext()
    {
    }

    public AppDbContext(DbContextOptions<AppDbContext> options)
        : base(options)
    {
    }

    public virtual DbSet<CanboDuyet> CanboDuyets { get; set; }

    public virtual DbSet<Capbac> Capbacs { get; set; }

    public virtual DbSet<Chitietdanhsach> Chitietdanhsaches { get; set; }

    public virtual DbSet<ChitietdanhsachGiayto> ChitietdanhsachGiaytos { get; set; }

    public virtual DbSet<Chucvu> Chucvus { get; set; }

    public virtual DbSet<Donvi> Donvis { get; set; }

    public virtual DbSet<Giayto> Giaytos { get; set; }

    public virtual DbSet<LichSuJson> LichSuJsons { get; set; }

    public virtual DbSet<Nhom> Nhoms { get; set; }

    public virtual DbSet<NhomQuyen> NhomQuyens { get; set; }

    public virtual DbSet<Quannhan> Quannhans { get; set; }

    public virtual DbSet<Quyen> Quyens { get; set; }

    public virtual DbSet<Rangoai> Rangoais { get; set; }

    public virtual DbSet<Taikhoan> Taikhoans { get; set; }

    public virtual DbSet<Vipham> Viphams { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see https://go.microsoft.com/fwlink/?LinkId=723263.
        => optionsBuilder.UseOracle("User Id=C##Quan;Password=Anhquan2002*;Data Source=//localhost:1521/orcl");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder
            .HasDefaultSchema("C##QUAN")
            .UseCollation("USING_NLS_COMP");

        modelBuilder.Entity<CanboDuyet>(entity =>
        {
            entity.HasKey(e => e.MacbD);

            entity.ToTable("CANBO_DUYET");

            entity.Property(e => e.MacbD)
                .HasPrecision(10)
                .HasColumnName("MACB_D");
            entity.Property(e => e.Ghichu)
                .HasMaxLength(200)
                .HasColumnName("GHICHU");
            entity.Property(e => e.Macb)
                .HasPrecision(10)
                .HasColumnName("MACB");
            entity.Property(e => e.Mactds)
                .HasPrecision(10)
                .HasColumnName("MACTDS");
            entity.Property(e => e.Thoigianduyet)
                .HasPrecision(6)
                .HasColumnName("THOIGIANDUYET");
        });

        modelBuilder.Entity<Capbac>(entity =>
        {
            entity.HasKey(e => e.Macapbac);

            entity.ToTable("CAPBAC");

            entity.Property(e => e.Macapbac)
                .HasPrecision(10)
                .HasColumnName("MACAPBAC");
            entity.Property(e => e.Capbac1)
                .HasMaxLength(100)
                .HasColumnName("CAPBAC");
            entity.Property(e => e.Kyhieu)
                .HasMaxLength(10)
                .IsUnicode(false)
                .HasColumnName("KYHIEU");
        });

        modelBuilder.Entity<Chitietdanhsach>(entity =>
        {
            entity.HasKey(e => e.Mactds);

            entity.ToTable("CHITIETDANHSACH");

            entity.Property(e => e.Mactds)
                .HasPrecision(10)
                .HasColumnName("MACTDS");
            entity.Property(e => e.Diadiem)
                .HasMaxLength(100)
                .HasColumnName("DIADIEM");
            entity.Property(e => e.Ghichu)
                .HasMaxLength(200)
                .HasColumnName("GHICHU");
            entity.Property(e => e.Hinhthucrn)
                .HasPrecision(10)
                .HasColumnName("HINHTHUCRN");
            entity.Property(e => e.Lydo)
                .HasMaxLength(100)
                .HasColumnName("LYDO");
            entity.Property(e => e.Mahocvien)
                .HasPrecision(10)
                .HasColumnName("MAHOCVIEN");
            entity.Property(e => e.Thoigianra)
                .HasPrecision(6)
                .HasColumnName("THOIGIANRA");
            entity.Property(e => e.Thoigianvao)
                .HasPrecision(6)
                .HasColumnName("THOIGIANVAO");
            entity.Property(e => e.Tinhtrang)
                .HasPrecision(10)
                .HasColumnName("TINHTRANG");
        });

        modelBuilder.Entity<ChitietdanhsachGiayto>(entity =>
        {
            entity.HasKey(e => new { e.Magiayto, e.Mactds });

            entity.ToTable("CHITIETDANHSACH_GIAYTO");

            entity.Property(e => e.Magiayto)
                .HasPrecision(10)
                .HasColumnName("MAGIAYTO");
            entity.Property(e => e.Mactds)
                .HasPrecision(10)
                .ValueGeneratedOnAdd()
                .HasColumnName("MACTDS");
            entity.Property(e => e.Ghichu)
                .HasMaxLength(200)
                .HasColumnName("GHICHU");
            entity.Property(e => e.Thoigianlay)
                .HasPrecision(6)
                .HasColumnName("THOIGIANLAY");
            entity.Property(e => e.Thoigiantra)
                .HasPrecision(6)
                .HasColumnName("THOIGIANTRA");
        });

        modelBuilder.Entity<Chucvu>(entity =>
        {
            entity.HasKey(e => e.Macv);

            entity.ToTable("CHUCVU");

            entity.Property(e => e.Macv)
                .HasPrecision(10)
                .HasColumnName("MACV");
            entity.Property(e => e.Kyhieu)
                .HasMaxLength(10)
                .IsUnicode(false)
                .HasColumnName("KYHIEU");
            entity.Property(e => e.Tencv)
                .HasMaxLength(100)
                .HasColumnName("TENCV");
        });

        modelBuilder.Entity<Donvi>(entity =>
        {
            entity.HasKey(e => e.Madv);

            entity.ToTable("DONVI");

            entity.Property(e => e.Madv)
                .HasPrecision(10)
                .HasColumnName("MADV");
            entity.Property(e => e.Cap)
                .HasPrecision(5)
                .HasColumnName("CAP");
            entity.Property(e => e.Tendv)
                .HasMaxLength(100)
                .HasColumnName("TENDV");
        });

        modelBuilder.Entity<Giayto>(entity =>
        {
            entity.HasKey(e => e.Magiayto);

            entity.ToTable("GIAYTO");

            entity.Property(e => e.Magiayto)
                .HasPrecision(10)
                .HasColumnName("MAGIAYTO");
            entity.Property(e => e.Ghichu)
                .HasMaxLength(200)
                .HasColumnName("GHICHU");
            entity.Property(e => e.Madv)
                .HasPrecision(10)
                .HasColumnName("MADV");
            entity.Property(e => e.Sogiay)
                .HasPrecision(10)
                .HasColumnName("SOGIAY");
            entity.Property(e => e.Tinhtrang)
                .HasColumnType("NUMBER(1)")
                .HasColumnName("TINHTRANG")
                .HasConversion(
                    v => v ? 1 : 0, // Convert bool to number
                    v => v == 1     // Convert number to bool
                );
        });

        modelBuilder.Entity<LichSuJson>(entity =>
        {
            entity.HasKey(e => e.Mactds).HasName("SYS_C0020563");

            entity.ToTable("LICH_SU_JSON");

            entity.Property(e => e.Mactds)
                .HasColumnType("NUMBER")
                .HasColumnName("MACTDS");
            entity.Property(e => e.JsonData)
                .HasColumnType("CLOB")
                .HasColumnName("JSON_DATA");
        });

        modelBuilder.Entity<Nhom>(entity =>
        {
            entity.HasKey(e => e.Manhom);

            entity.ToTable("NHOM");

            entity.Property(e => e.Manhom)
                .HasPrecision(10)
                .ValueGeneratedNever()
                .HasColumnName("MANHOM");
            entity.Property(e => e.Tennhom)
                .HasMaxLength(200)
                .HasColumnName("TENNHOM");
        });

        modelBuilder.Entity<NhomQuyen>(entity =>
        {
            entity.HasKey(e => new { e.Maquyen, e.Manhom });

            entity.ToTable("NHOM_QUYEN");

            entity.Property(e => e.Maquyen)
                .HasPrecision(10)
                .HasColumnName("MAQUYEN");
            entity.Property(e => e.Manhom)
                .HasPrecision(10)
                .HasColumnName("MANHOM");
            entity.Property(e => e.Ghichu)
                .HasMaxLength(10)
                .IsFixedLength()
                .HasColumnName("GHICHU");
        });

        modelBuilder.Entity<Quannhan>(entity =>
        {
            entity.HasKey(e => e.Maqn);

            entity.ToTable("QUANNHAN");

            entity.Property(e => e.Maqn)
                .HasPrecision(10)
                .HasColumnName("MAQN");
            entity.Property(e => e.Diachi)
                .HasMaxLength(100)
                .HasColumnName("DIACHI");
            entity.Property(e => e.Hoten)
                .HasMaxLength(100)
                .HasColumnName("HOTEN");
            entity.Property(e => e.Macapbac)
                .HasPrecision(10)
                .HasColumnName("MACAPBAC");
            entity.Property(e => e.Macv)
                .HasPrecision(10)
                .HasColumnName("MACV");
            entity.Property(e => e.Madv)
                .HasPrecision(10)
                .HasColumnName("MADV");
        });

        modelBuilder.Entity<Quyen>(entity =>
        {
            entity.HasKey(e => e.Maquyen);

            entity.ToTable("QUYEN");

            entity.Property(e => e.Maquyen)
                .HasPrecision(10)
                .HasColumnName("MAQUYEN");
            entity.Property(e => e.Actionname)
                .HasMaxLength(200)
                .HasColumnName("ACTIONNAME");
            entity.Property(e => e.Controllername)
                .HasMaxLength(200)
                .HasColumnName("CONTROLLERNAME");
            entity.Property(e => e.Ten)
                .HasMaxLength(200)
                .HasColumnName("TEN");
        });

        modelBuilder.Entity<Rangoai>(entity =>
        {
            entity.HasKey(e => e.Marn);

            entity.ToTable("RANGOAI");

            entity.Property(e => e.Marn)
                .HasPrecision(10)
                .HasColumnName("MARN");
            entity.Property(e => e.Ghichu)
                .HasMaxLength(200)
                .HasColumnName("GHICHU");
            entity.Property(e => e.Mactds)
                .HasPrecision(10)
                .HasColumnName("MACTDS");
            entity.Property(e => e.Magiayto)
                .HasPrecision(10)
                .HasColumnName("MAGIAYTO");
            entity.Property(e => e.Thoigianra)
                .HasPrecision(6)
                .HasColumnName("THOIGIANRA");
            entity.Property(e => e.Thoigianvao)
                .HasPrecision(6)
                .HasColumnName("THOIGIANVAO");
        });

        modelBuilder.Entity<Taikhoan>(entity =>
        {
            entity.HasKey(e => e.Mataikhoan);

            entity.ToTable("TAIKHOAN");

            entity.Property(e => e.Mataikhoan)
                .HasPrecision(10)
                .HasColumnName("MATAIKHOAN");
            entity.Property(e => e.Manhom)
                .HasPrecision(10)
                .HasColumnName("MANHOM");
            entity.Property(e => e.Maqn)
                .HasPrecision(10)
                .HasColumnName("MAQN");
            entity.Property(e => e.Matkhau)
                .HasMaxLength(200)
                .IsUnicode(false)
                .HasColumnName("MATKHAU");
            entity.Property(e => e.Tdn)
                .HasMaxLength(20)
                .IsUnicode(false)
                .HasColumnName("TDN");
        });

        modelBuilder.Entity<Vipham>(entity =>
        {
            entity.HasKey(e => e.Mavp);

            entity.ToTable("VIPHAM");

            entity.Property(e => e.Mavp)
                .HasPrecision(10)
                .HasColumnName("MAVP");
            entity.Property(e => e.Ghichu)
                .HasMaxLength(200)
                .HasColumnName("GHICHU");
            entity.Property(e => e.Mahv)
                .HasPrecision(10)
                .HasColumnName("MAHV");
            entity.Property(e => e.Mota)
                .HasMaxLength(200)
                .HasColumnName("MOTA");
            entity.Property(e => e.Thoigian)
                .HasColumnType("DATE")
                .HasColumnName("THOIGIAN");
        });
        modelBuilder.HasSequence("ISEQ$_85315");

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
