using Microsoft.EntityFrameworkCore;
using StudentApplication.Data.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StudentApplication.Data
{
    public class DatabaseContext : DbContext
    {
        public DbSet<User> Users { get; set; }
        public DbSet<Student> Students { get; set; }
        public DbSet<Professor> Professors { get; set; }
        public DbSet<Admin> Admins { get; set; }
        public DbSet<Subject> Subjects { get; set; }
        public DbSet<Grade> Grades { get; set; }
        public DbSet<Department> Departments { get; set; }
        public DbSet<Enrollment> Enrollments { get; set; }

        public DatabaseContext(DbContextOptions<DatabaseContext> options) : base(options) { }

        public DatabaseContext() { }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<User>(b =>
            {
                b.HasIndex(u => u.Email).IsUnique();
                b.HasIndex(u => u.Username).IsUnique();
            });

            modelBuilder.Entity<Department>(b =>
            {
                b.Property(d => d.Name).IsRequired().HasMaxLength(200);
            });

            modelBuilder.Entity<Admin>(b =>
            {
                b.Property(a => a.Username).IsRequired().HasMaxLength(100);
            });

            modelBuilder.Entity<Student>(b =>
            {
                b.Property(s => s.FirstName).IsRequired().HasMaxLength(100);
                b.Property(s => s.LastName).IsRequired().HasMaxLength(100);

                b.HasOne(s => s.Department)
                 .WithMany(d => d.Students)
                 .HasForeignKey(s => s.DepartmentId)
                 .OnDelete(DeleteBehavior.SetNull);

                b.HasOne(s => s.User)
                 .WithOne(u => u.StudentProfile)
                 .HasForeignKey<Student>(s => s.UserId)
                 .OnDelete(DeleteBehavior.SetNull);
            });

            modelBuilder.Entity<Professor>(b =>
            {
                b.Property(p => p.FirstName).IsRequired().HasMaxLength(100);
                b.Property(p => p.LastName).IsRequired().HasMaxLength(100);
                b.Property(p => p.Title).HasMaxLength(100);

                b.HasOne(p => p.Department)
                 .WithMany(d => d.Professors)
                 .HasForeignKey(p => p.DepartmentId)
                 .OnDelete(DeleteBehavior.SetNull);

                b.HasOne(p => p.User)
                 .WithOne(u => u.ProfessorProfile)
                 .HasForeignKey<Professor>(p => p.UserId)
                 .OnDelete(DeleteBehavior.SetNull);

                b.HasOne(p => p.ApprovedByAdmin)
                 .WithMany(a => a.ApprovedProfessors)
                 .HasForeignKey(p => p.ApprovedByAdminId)
                 .OnDelete(DeleteBehavior.SetNull);
            });

            modelBuilder.Entity<Subject>(b =>
            {
                b.Property(s => s.Title).IsRequired().HasMaxLength(200);
                b.Property(s => s.Description).HasMaxLength(2000);

                b.HasOne(s => s.Professor)
                 .WithMany(p => p.Subjects)
                 .HasForeignKey(s => s.ProfessorId)
                 .OnDelete(DeleteBehavior.Restrict);
            });

            modelBuilder.Entity<Enrollment>(b =>
            {
                b.HasOne(e => e.Student)
                 .WithMany(s => s.Enrollments)
                 .HasForeignKey(e => e.StudentId)
                 .OnDelete(DeleteBehavior.Cascade);

                b.HasOne(e => e.Subject)
                 .WithMany(s => s.Enrollments)
                 .HasForeignKey(e => e.SubjectId)
                 .OnDelete(DeleteBehavior.Cascade);

                b.HasOne(e => e.Grade)
                 .WithOne(g => g.Enrollment)
                 .HasForeignKey<Grade>(g => g.EnrollmentId)
                 .OnDelete(DeleteBehavior.Cascade);
            });

            modelBuilder.Entity<Grade>(b =>
            {
                b.HasOne(g => g.Professor)
                 .WithMany(p => p.GivenGrades)
                 .HasForeignKey(g => g.ProfessorId)
                 .OnDelete(DeleteBehavior.NoAction);

                b.HasOne(g => g.Subject)
                 .WithMany()
                 .HasForeignKey(g => g.SubjectId)
                 .OnDelete(DeleteBehavior.NoAction);

                b.HasOne(g => g.Student)
                 .WithMany(s => s.Grades)
                 .HasForeignKey(g => g.StudentId)
                 .OnDelete(DeleteBehavior.NoAction);
            });
        }
    }
}
