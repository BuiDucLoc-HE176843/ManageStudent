﻿using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace ManageCourse.Models;

public partial class LearningManagementSystemContext : DbContext
{
    public LearningManagementSystemContext()
    {
    }

    public LearningManagementSystemContext(DbContextOptions<LearningManagementSystemContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Class> Classes { get; set; }

    public virtual DbSet<Course> Courses { get; set; }

    public virtual DbSet<Enrollment> Enrollments { get; set; }

    public virtual DbSet<Grade> Grades { get; set; }

    public virtual DbSet<Notification> Notifications { get; set; }

    public virtual DbSet<StudentAttendance> StudentAttendances { get; set; }

    public virtual DbSet<User> Users { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        var config = new ConfigurationBuilder().AddJsonFile("appsettings.json").Build();
        if (!optionsBuilder.IsConfigured)
        {
            optionsBuilder.UseSqlServer(config.GetConnectionString("DB"));
        }
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Class>(entity =>
        {
            entity.HasKey(e => e.ClassId).HasName("PK__Classes__CB1927A041C6C417");

            entity.Property(e => e.ClassId).HasColumnName("ClassID");
            entity.Property(e => e.ClassName).HasMaxLength(50);
            entity.Property(e => e.CourseId).HasColumnName("CourseID");
            entity.Property(e => e.TeacherId).HasColumnName("TeacherID");
            entity.Property(e => e.TotalSessions).HasDefaultValue(20);

            entity.HasOne(d => d.Course).WithMany(p => p.Classes)
                .HasForeignKey(d => d.CourseId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Classes__CourseI__52593CB8");

            entity.HasOne(d => d.Teacher).WithMany(p => p.Classes)
                .HasForeignKey(d => d.TeacherId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Classes__Teacher__534D60F1");
        });

        modelBuilder.Entity<Course>(entity =>
        {
            entity.HasKey(e => e.CourseId).HasName("PK__Courses__C92D7187BE9508A5");

            entity.Property(e => e.CourseId).HasColumnName("CourseID");
            entity.Property(e => e.CourseName).HasMaxLength(50);
        });

        modelBuilder.Entity<Enrollment>(entity =>
        {
            entity.HasKey(e => e.EnrollmentId).HasName("PK__Enrollme__7F6877FBDC8C3610");

            entity.Property(e => e.EnrollmentId).HasColumnName("EnrollmentID");
            entity.Property(e => e.ClassId).HasColumnName("ClassID");
            entity.Property(e => e.StudentId).HasColumnName("StudentID");

            entity.HasOne(d => d.Class).WithMany(p => p.Enrollments)
                .HasForeignKey(d => d.ClassId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Enrollmen__Class__571DF1D5");

            entity.HasOne(d => d.Student).WithMany(p => p.Enrollments)
                .HasForeignKey(d => d.StudentId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Enrollmen__Stude__5629CD9C");
        });

        modelBuilder.Entity<Grade>(entity =>
        {
            entity.HasKey(e => e.GradeId).HasName("PK__Grades__54F87A3712F4966E");

            entity.Property(e => e.GradeId).HasColumnName("GradeID");
            entity.Property(e => e.ClassId).HasColumnName("ClassID");
            entity.Property(e => e.FinalScore).HasComputedColumnSql("(case when [Score1] IS NULL AND [Score2] IS NULL AND [Score3] IS NULL AND [Score4] IS NULL AND [Score5] IS NULL then NULL else ((((coalesce([Score1]*(0.2),(0))+coalesce([Score2]*(0.2),(0)))+coalesce([Score3]*(0.2),(0)))+coalesce([Score4]*(0.2),(0)))+coalesce([Score5]*(0.2),(0)))/nullif((((case when [Score1] IS NOT NULL then (0.2) else (0) end+case when [Score2] IS NOT NULL then (0.2) else (0) end)+case when [Score3] IS NOT NULL then (0.2) else (0) end)+case when [Score4] IS NOT NULL then (0.2) else (0) end)+case when [Score5] IS NOT NULL then (0.2) else (0) end,(0)) end)", true);
            entity.Property(e => e.StudentId).HasColumnName("StudentID");

            entity.HasOne(d => d.Class).WithMany(p => p.Grades)
                .HasForeignKey(d => d.ClassId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Grades__ClassID__656C112C");

            entity.HasOne(d => d.Student).WithMany(p => p.Grades)
                .HasForeignKey(d => d.StudentId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Grades__StudentI__6477ECF3");
        });

        modelBuilder.Entity<Notification>(entity =>
        {
            entity.HasKey(e => e.NotificationId).HasName("PK__Notifica__20CF2E3292576DA4");

            entity.Property(e => e.NotificationId).HasColumnName("NotificationID");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.Message).HasMaxLength(500);
            entity.Property(e => e.ReceiverId).HasColumnName("ReceiverID");
            entity.Property(e => e.SenderId).HasColumnName("SenderID");

            entity.HasOne(d => d.Receiver).WithMany(p => p.NotificationReceivers)
                .HasForeignKey(d => d.ReceiverId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Notificat__Recei__6A30C649");

            entity.HasOne(d => d.Sender).WithMany(p => p.NotificationSenders)
                .HasForeignKey(d => d.SenderId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Notificat__Sende__693CA210");
        });

        modelBuilder.Entity<StudentAttendance>(entity =>
        {
            entity.HasKey(e => e.AttendanceId).HasName("PK__StudentA__8B69263C053E0162");

            entity.ToTable("StudentAttendance");

            entity.Property(e => e.AttendanceId).HasColumnName("AttendanceID");
            entity.Property(e => e.ClassId).HasColumnName("ClassID");
            entity.Property(e => e.StudentId).HasColumnName("StudentID");

            entity.HasOne(d => d.Class).WithMany(p => p.StudentAttendances)
                .HasForeignKey(d => d.ClassId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__StudentAt__Class__5CD6CB2B");

            entity.HasOne(d => d.Student).WithMany(p => p.StudentAttendances)
                .HasForeignKey(d => d.StudentId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__StudentAt__Stude__5BE2A6F2");
        });

        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(e => e.UserId).HasName("PK__Users__1788CCAC4CF1394A");

            entity.HasIndex(e => e.Email, "UQ__Users__A9D1053409E23A47").IsUnique();

            entity.Property(e => e.UserId).HasColumnName("UserID");
            entity.Property(e => e.Email).HasMaxLength(100);
            entity.Property(e => e.FullName).HasMaxLength(100);
            entity.Property(e => e.Hometown).HasMaxLength(255);
            entity.Property(e => e.PasswordHash).HasMaxLength(255);
            entity.Property(e => e.PhoneNumber)
                .HasMaxLength(15)
                .IsUnicode(false);
            entity.Property(e => e.Role).HasMaxLength(20);
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
