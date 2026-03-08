using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;
using Tashyeed.Infrastructure.Entities;
using Tashyeed.Infrastructure.Identity;

namespace Tashyeed.Infrastructure.Persistence
{
    public class AppDBContext : IdentityDbContext<ApplicationUser>
    {
        public AppDBContext(DbContextOptions<AppDBContext> options) : base(options)
        {
        }
        override protected void OnModelCreating(ModelBuilder builder)
        {
            builder.Entity<Project>(entity =>
            {
                entity.HasKey(p => p.Id);
                entity.Property(p => p.Name).IsRequired().HasMaxLength(200);
                entity.Property(p => p.Location).IsRequired().HasMaxLength(300);
                entity.Property(p => p.Budget).HasColumnType("decimal(18,2)");
                entity.Property(p => p.SpentAmount).HasColumnType("decimal(18,2)");
            });

            builder.Entity<ProjectAssignment>(entity =>
            {
                entity.HasKey(pa => pa.Id);

                entity.HasOne(pa => pa.Project)
                    .WithMany(p => p.Assignments)
                    .HasForeignKey(pa => pa.ProjectId);

                entity.HasOne(pa => pa.User)
                    .WithMany()
                    .HasForeignKey(pa => pa.UserId);
            });
            builder.Entity<Custody>(entity =>
            {
                entity.HasKey(c => c.Id);
                entity.Property(c => c.Amount).HasColumnType("decimal(18,2)");
                entity.Property(c => c.RemainingAmount).HasColumnType("decimal(18,2)");

                entity.HasOne(c => c.Project)
                    .WithMany(p => p.Custodies)
                    .HasForeignKey(c => c.ProjectId);

                entity.HasOne(c => c.GivenBy)
                    .WithMany()
                    .HasForeignKey(c => c.GivenByUserId)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(c => c.GivenTo)
                    .WithMany()
                    .HasForeignKey(c => c.GivenToUserId)
                    .OnDelete(DeleteBehavior.Restrict);
            });
            builder.Entity<Expense>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Amount).HasColumnType("decimal(18,2)");

                entity.HasOne(e => e.Project)
                    .WithMany(p => p.Expenses)
                    .HasForeignKey(e => e.ProjectId);

                entity.HasOne(e => e.SubmittedBy)
                    .WithMany()
                    .HasForeignKey(e => e.SubmittedByUserId)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(e => e.ApprovedBy)
                    .WithMany()
                    .HasForeignKey(e => e.ApprovedByUserId)
                    .OnDelete(DeleteBehavior.Restrict);
            });
            builder.Entity<PurchaseRequest>(entity =>
            {
                entity.HasKey(pr => pr.Id);

                entity.HasOne(pr => pr.Project)
                    .WithMany(p => p.PurchaseRequests)
                    .HasForeignKey(pr => pr.ProjectId);

                entity.HasOne(pr => pr.RequestedBy)
                    .WithMany()
                    .HasForeignKey(pr => pr.RequestedByUserId)
                    .OnDelete(DeleteBehavior.Restrict);
            });

            builder.Entity<PurchaseOrder>(entity =>
            {
                entity.HasKey(po => po.Id);
                entity.Property(po => po.Amount).HasColumnType("decimal(18,2)");

                entity.HasOne(po => po.Project)
                    .WithMany()
                    .HasForeignKey(po => po.ProjectId)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(po => po.PurchaseRequest)
                    .WithOne(pr => pr.PurchaseOrder)
                    .HasForeignKey<PurchaseOrder>(po => po.PurchaseRequestId)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(po => po.PurchasedBy)
                    .WithMany()
                    .HasForeignKey(po => po.PurchasedByUserId)
                    .OnDelete(DeleteBehavior.Restrict);
            });
            builder.Entity<WorkerRequest>(entity =>
            {
                entity.HasKey(wr => wr.Id);
                entity.HasOne(wr => wr.Project)
                    .WithMany(p => p.WorkerRequests)
                    .HasForeignKey(wr => wr.ProjectId);
                entity.HasOne(wr => wr.RequestedBy)
                    .WithMany()
                    .HasForeignKey(wr => wr.RequestedByUserId)
                    .OnDelete(DeleteBehavior.Restrict);
                entity.HasOne(wr => wr.ApprovedBy)
                    .WithMany()
                    .HasForeignKey(wr => wr.ApprovedByUserId)
                    .OnDelete(DeleteBehavior.Restrict);
            });

            builder.Entity<Worker>(entity =>
            {
                entity.HasKey(w => w.Id);
                entity.Property(w => w.DailyRate).HasColumnType("decimal(18,2)");
                entity.Property(w => w.OvertimeHourRate).HasColumnType("decimal(18,2)");
                entity.HasOne(w => w.Project)
                    .WithMany()
                    .HasForeignKey(w => w.ProjectId)
                    .OnDelete(DeleteBehavior.Restrict);
            });

            builder.Entity<DailyAttendance>(entity =>
            {
                entity.HasKey(da => da.Id);
                entity.Property(da => da.OvertimeHours).HasColumnType("decimal(18,2)");
                entity.HasOne(da => da.Worker)
                    .WithMany(w => w.DailyAttendances)
                    .HasForeignKey(da => da.WorkerId)
                    .OnDelete(DeleteBehavior.Restrict);
                entity.HasOne(da => da.SubmittedBy)
                    .WithMany()
                    .HasForeignKey(da => da.SubmittedByUserId)
                    .OnDelete(DeleteBehavior.Restrict);
            });

            builder.Entity<MonthlyWorkerReport>(entity =>
            {
                entity.HasKey(mr => mr.Id);
                entity.Property(mr => mr.OvertimeHours).HasColumnType("decimal(18,2)");
                entity.HasOne(mr => mr.Worker)
                    .WithMany(w => w.MonthlyReports)
                    .HasForeignKey(mr => mr.WorkerId)
                    .OnDelete(DeleteBehavior.Restrict);
                entity.HasOne(mr => mr.SubmittedBy)
                    .WithMany()
                    .HasForeignKey(mr => mr.SubmittedByUserId)
                    .OnDelete(DeleteBehavior.Restrict);
            });
            base.OnModelCreating(builder);
        }
        public DbSet<Project> Projects { get; set; }
        public DbSet<ProjectAssignment> ProjectAssignments { get; set; }
        public DbSet<Custody> Custodies { get; set; }
        public DbSet<Expense> Expenses { get; set; }
        public DbSet<PurchaseRequest> PurchaseRequests { get; set; }
        public DbSet<PurchaseOrder> PurchaseOrders { get; set; }
        public DbSet<Worker> Workers { get; set; }
        public DbSet<WorkerRequest> WorkerRequests { get; set; }
        public DbSet<DailyAttendance> DailyAttendances { get; set; }
        public DbSet<MonthlyWorkerReport> MonthlyWorkerReports { get; set; }

    }
}
