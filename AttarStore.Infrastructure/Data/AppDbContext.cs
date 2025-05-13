using AttarStore.Domain.Entities;
using AttarStore.Domain.Entities.Auth;
using AttarStore.Domain.Entities.Catalog;
using AttarStore.Domain.Entities.Shopping;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;

namespace AttarStore.Services.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options)
            : base(options)
        {
        }

        // ─── Core ────────────────────────────────────────────────────────────────
        public DbSet<Admin> Admins { get; set; }
        public DbSet<User> Users { get; set; }
        public DbSet<Client> Clients { get; set; }
        public DbSet<Vendor> Vendors { get; set; }
        public DbSet<RefreshToken> RefreshTokens { get; set; }
        public DbSet<Permission> Permissions { get; set; }
        public DbSet<RolePermission> RolePermissions { get; set; }
        public DbSet<UserPermission> UserPermissions { get; set; }

        // ─── Catalog ─────────────────────────────────────────────────────────────
        public DbSet<Category> Categories { get; set; }
        public DbSet<Subcategory> Subcategories { get; set; }
        public DbSet<CategoryRequest> CategoryRequests { get; set; }
        public DbSet<Product> Products { get; set; }
        public DbSet<ProductImage> ProductImages { get; set; }
        public DbSet<ProductVariant> ProductVariants { get; set; }
        public DbSet<VariantOption> VariantOptions { get; set; }
        public DbSet<VariantOptionValue> VariantOptionValues { get; set; }
        public DbSet<ProductVariantAttribute> ProductVariantAttributes { get; set; }

        // ─── Shopping ────────────────────────────────────────────────────────────
        public DbSet<Cart> Carts { get; set; }
        public DbSet<CartItem> CartItems { get; set; }
        public DbSet<Order> Orders { get; set; }
        public DbSet<OrderItem> OrderItems { get; set; }

        // ─── Inventory ───────────────────────────────────────────────────────────
        public DbSet<InventoryTransaction> InventoryTransactions { get; set; }

        // ─── Audit logs ──────────────────────────────────────────────────────────
        public DbSet<AuditLog> AuditLogs { get; set; }

        // ─── Notifications ──────────────────────────────────────────────────────────

        public DbSet<Notification> Notifications { get; set; } = null!;
        public DbSet<UserNotification> UserNotifications { get; set; } = null!;
        public DbSet<AdminNotification> AdminNotifications { get; set; } = null!;
        public DbSet<ClientNotification> ClientNotifications { get; set; } = null!;
        public DbSet<VendorNotification> VendorNotifications { get; set; } = null!;

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // ─── Indexes ─────────────────────────────────────────────────────────
            modelBuilder.Entity<Admin>().HasIndex(a => a.Name).IsUnique();
            modelBuilder.Entity<Admin>().HasIndex(a => a.Email).IsUnique();
            modelBuilder.Entity<User>().HasIndex(u => u.Name).IsUnique();
            modelBuilder.Entity<User>().HasIndex(u => u.Email).IsUnique();
            modelBuilder.Entity<Client>().HasIndex(c => c.Name).IsUnique();
            modelBuilder.Entity<Client>().HasIndex(c => c.Email).IsUnique();
            modelBuilder.Entity<Permission>().HasIndex(p => p.Name).IsUnique();

            // ─── Core relationships ───────────────────────────────────────────────
            modelBuilder.Entity<Vendor>()
                .HasMany(v => v.Users)
                .WithOne(u => u.Vendor)
                .HasForeignKey(u => u.VendorId)
                .OnDelete(DeleteBehavior.SetNull);

            modelBuilder.Entity<RefreshToken>()
                .HasOne(rt => rt.User)
                .WithMany(u => u.RefreshTokens)
                .HasForeignKey(rt => rt.UserId)
                .OnDelete(DeleteBehavior.Cascade);
            modelBuilder.Entity<RefreshToken>()
                .HasOne(rt => rt.Admin)
                .WithMany(a => a.RefreshTokens)
                .HasForeignKey(rt => rt.AdminId)
                .OnDelete(DeleteBehavior.Cascade);
            modelBuilder.Entity<RefreshToken>()
                .HasOne(rt => rt.Client)
                .WithMany(c => c.RefreshTokens)
                .HasForeignKey(rt => rt.ClientId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<RolePermission>()
                .HasOne(rp => rp.Permission)
                .WithMany(p => p.RolePermissions)
                .HasForeignKey(rp => rp.PermissionId);
            modelBuilder.Entity<RolePermission>()
                .Property(rp => rp.RoleName)
                .IsRequired()
                .HasMaxLength(50);

            modelBuilder.Entity<UserPermission>()
                .HasKey(up => up.Id);
            modelBuilder.Entity<UserPermission>()
                .HasOne(up => up.User)
                .WithMany(u => u.UserPermissions)
                .HasForeignKey(up => up.UserId)
                .OnDelete(DeleteBehavior.Cascade);
            modelBuilder.Entity<UserPermission>()
                .HasIndex(up => new { up.UserId, up.PermissionName })
                .IsUnique();

            // ─── Catalog ─────────────────────────────────────────────────────────
            modelBuilder.Entity<Category>()
                .HasMany(c => c.Subcategories)
                .WithOne(sc => sc.Category)
                .HasForeignKey(sc => sc.CategoryId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Subcategory>()
                .HasMany(sc => sc.Products)
                .WithOne(p => p.Subcategory)
                .HasForeignKey(p => p.SubcategoryId)
                .OnDelete(DeleteBehavior.SetNull);
            modelBuilder.Entity<Subcategory>()
                .HasIndex(sc => new { sc.CategoryId, sc.Name })
                .IsUnique();

            modelBuilder.Entity<Product>()
                .HasMany(p => p.Images)
                .WithOne(pi => pi.Product)
                .HasForeignKey(pi => pi.ProductId)
                .OnDelete(DeleteBehavior.Cascade);
            modelBuilder.Entity<ProductImage>()
                .Property(pi => pi.Url).IsRequired();

            modelBuilder.Entity<Product>()
                .HasMany(p => p.Variants)
                .WithOne(v => v.Product)
                .HasForeignKey(v => v.ProductId)
                .OnDelete(DeleteBehavior.Cascade);
            modelBuilder.Entity<ProductVariant>()
                .HasIndex(v => new { v.ProductId, v.Sku })
                .IsUnique();

            // ── Hide products of suspended vendors ────────────────────────────────
            modelBuilder.Entity<Product>()
                .HasQueryFilter(p => p.Vendor == null || p.Vendor.IsActive);

            modelBuilder.Entity<VariantOption>()
                .HasMany(o => o.Values)
                .WithOne(v => v.VariantOption!)
                .HasForeignKey(v => v.VariantOptionId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<ProductVariantAttribute>()
                .HasKey(pa => new { pa.ProductVariantId, pa.VariantOptionId, pa.VariantOptionValueId });
            modelBuilder.Entity<ProductVariantAttribute>()
                .HasOne(pa => pa.ProductVariant)
                .WithMany(pv => pv.Attributes)
                .HasForeignKey(pa => pa.ProductVariantId)
                .OnDelete(DeleteBehavior.Cascade);
            modelBuilder.Entity<ProductVariantAttribute>()
                .HasOne(pa => pa.VariantOption)
                .WithMany()
                .HasForeignKey(pa => pa.VariantOptionId)
                .OnDelete(DeleteBehavior.Restrict);
            modelBuilder.Entity<ProductVariantAttribute>()
                .HasOne(pa => pa.VariantOptionValue)
                .WithMany()
                .HasForeignKey(pa => pa.VariantOptionValueId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Vendor>()
                .HasMany(v => v.Products)
                .WithOne(p => p.Vendor)
                .HasForeignKey(p => p.VendorId)
                .OnDelete(DeleteBehavior.Cascade);

            // ─── Shopping Cart & Orders ──────────────────────────────────────────
            modelBuilder.Entity<Client>()
                .HasOne(c => c.Cart)
                .WithOne(ca => ca.Client)
                .HasForeignKey<Cart>(ca => ca.ClientId)
                .OnDelete(DeleteBehavior.Cascade);
            modelBuilder.Entity<Cart>()
                .HasIndex(c => c.ClientId).IsUnique();

            modelBuilder.Entity<CartItem>()
                .HasOne(ci => ci.Cart)
                .WithMany(c => c.Items)
                .HasForeignKey(ci => ci.CartId)
                .OnDelete(DeleteBehavior.Cascade);
            modelBuilder.Entity<CartItem>()
                .HasOne(ci => ci.ProductVariant)
                .WithMany()
                .HasForeignKey(ci => ci.ProductVariantId)
                .OnDelete(DeleteBehavior.Restrict);
            modelBuilder.Entity<CartItem>()
                .HasIndex(ci => new { ci.CartId, ci.ProductVariantId })
                .IsUnique();

            modelBuilder.Entity<Order>()
                .HasOne(o => o.Client)
                .WithMany(c => c.Orders)
                .HasForeignKey(o => o.ClientId)
                .OnDelete(DeleteBehavior.Cascade);
            modelBuilder.Entity<OrderItem>()
                .HasOne(oi => oi.Order)
                .WithMany(o => o.Items)
                .HasForeignKey(oi => oi.OrderId)
                .OnDelete(DeleteBehavior.Cascade);
            modelBuilder.Entity<OrderItem>()
                .HasOne(oi => oi.ProductVariant)
                .WithMany()
                .HasForeignKey(oi => oi.ProductVariantId)
                .OnDelete(DeleteBehavior.Restrict);
            modelBuilder.Entity<OrderItem>()
                .HasIndex(oi => new { oi.OrderId, oi.ProductVariantId })
                .IsUnique();

            // ─── Category Requests ────────────────────────────────────────────────
            modelBuilder.Entity<CategoryRequest>()
                .HasOne(cr => cr.Vendor)
                .WithMany(v => v.CategoryRequests)
                .HasForeignKey(cr => cr.VendorId)
                .OnDelete(DeleteBehavior.Cascade);
            modelBuilder.Entity<CategoryRequest>()
                .Property(cr => cr.Status)
                .HasConversion<string>()
                .IsRequired();

            // ─── Inventory Transactions ───────────────────────────────────────────
            modelBuilder.Entity<InventoryTransaction>(eb =>
            {
                eb.HasKey(it => it.Id);

                eb.HasOne(it => it.ProductVariant)
                  .WithMany(v => v.InventoryTransactions)
                  .HasForeignKey(it => it.ProductVariantId)
                  .OnDelete(DeleteBehavior.Cascade);

                eb.HasOne(it => it.Product)
                  .WithMany(p => p.InventoryTransactions)
                  .HasForeignKey(it => it.ProductId)
                  .OnDelete(DeleteBehavior.Restrict);

                eb.HasOne(it => it.User)
                  .WithMany(u => u.InventoryTransactions)
                  .HasForeignKey(it => it.UserId)
                  .OnDelete(DeleteBehavior.Restrict);

                eb.Property(it => it.Reason).IsRequired();
                eb.Property(it => it.Timestamp)
                  .HasDefaultValueSql("GETUTCDATE()");
            });


            // ─── AuditLog ─────────────────────────────────────────────────────────
            modelBuilder.Entity<AuditLog>(eb =>
            {
                eb.HasKey(a => a.Id);
                // no foreign keys here!
                eb.Property(a => a.ActorType).IsRequired().HasMaxLength(50);
                eb.Property(a => a.ActorName).IsRequired();
                eb.Property(a => a.ActorRole).IsRequired();
                eb.Property(a => a.Action).IsRequired();
                eb.Property(a => a.EntityType).IsRequired();
                eb.Property(a => a.Timestamp)
                  .HasDefaultValueSql("GETUTCDATE()");
            });

            // ─── Notifications ─────────────────────────────────────────────────────────


            // UserNotification
            modelBuilder.Entity<UserNotification>()
             .HasKey(un => new { un.UserId, un.NotificationId });
            modelBuilder.Entity<UserNotification>()
             .HasOne(un => un.User).WithMany(u => u.UserNotifications).HasForeignKey(un => un.UserId);
            modelBuilder.Entity<UserNotification>()
             .HasOne(un => un.Notification).WithMany(n => n.UserNotifications).HasForeignKey(un => un.NotificationId);

            // AdminNotification
            modelBuilder.Entity<AdminNotification>()
             .HasKey(an => new { an.AdminId, an.NotificationId });
            modelBuilder.Entity<AdminNotification>()
             .HasOne(an => an.Admin).WithMany(a => a.AdminNotifications).HasForeignKey(an => an.AdminId);
            modelBuilder.Entity<AdminNotification>()
             .HasOne(an => an.Notification).WithMany(n => n.AdminNotifications).HasForeignKey(an => an.NotificationId);

            // ClientNotification
            modelBuilder.Entity<ClientNotification>()
             .HasKey(cn => new { cn.ClientId, cn.NotificationId });
            modelBuilder.Entity<ClientNotification>()
             .HasOne(cn => cn.Client).WithMany(c => c.ClientNotifications).HasForeignKey(cn => cn.ClientId);
            modelBuilder.Entity<ClientNotification>()
             .HasOne(cn => cn.Notification).WithMany(n => n.ClientNotifications).HasForeignKey(cn => cn.NotificationId);

            // VendorNotification
            modelBuilder.Entity<VendorNotification>()
             .HasKey(vn => new { vn.VendorId, vn.NotificationId });
            modelBuilder.Entity<VendorNotification>()
             .HasOne(vn => vn.Vendor).WithMany(v => v.VendorNotifications).HasForeignKey(vn => vn.VendorId);
            modelBuilder.Entity<VendorNotification>()
             .HasOne(vn => vn.Notification).WithMany(n => n.VendorNotifications).HasForeignKey(vn => vn.NotificationId);



            // ─── Seed: Default Admin ───────────────────────────────────────────────
            var adminPassword = BCrypt.Net.BCrypt.HashPassword("password");
            modelBuilder.Entity<Admin>().HasData(new Admin
            {
                Id = 1,
                Name = "admin",
                Email = "ahmad.al.shelhawi@gmail.com",
                Phone = "096654467",
                Password = adminPassword,
                ResetToken = null,
                ResetTokenExpiry = null,
                IsDeleted = false
            });

            // ─── Seed: Permissions ─────────────────────────────────────────────────
            modelBuilder.Entity<Permission>().HasData(
                new Permission { Id = 1, Name = "Category.Create", Description = "Create new categories" },
                new Permission { Id = 2, Name = "Category.Read", Description = "View categories" },
                new Permission { Id = 3, Name = "Category.Update", Description = "Edit categories" },
                new Permission { Id = 4, Name = "Category.Delete", Description = "Delete categories" },
                new Permission { Id = 5, Name = "Product.Create", Description = "Create new products" },
                new Permission { Id = 6, Name = "Product.Read", Description = "View products" },
                new Permission { Id = 7, Name = "Product.Update", Description = "Edit products" },
                new Permission { Id = 8, Name = "Product.Delete", Description = "Delete products" },
                new Permission { Id = 9, Name = "Order.Create", Description = "Place orders" },
                new Permission { Id = 10, Name = "Order.ReadAll", Description = "View all orders (admins)" },
                new Permission { Id = 11, Name = "Order.ReadOwn", Description = "View own orders" },
                new Permission { Id = 12, Name = "Order.Update", Description = "Update orders" },
                new Permission { Id = 13, Name = "Order.Delete", Description = "Cancel orders" },
                new Permission { Id = 14, Name = "Permission.Create", Description = "Manage permissions" },
                new Permission { Id = 15, Name = "Permission.Read", Description = "View permissions" },
                new Permission { Id = 16, Name = "Permission.Update", Description = "Edit permissions" },
                new Permission { Id = 17, Name = "Permission.Delete", Description = "Remove permissions" },
                new Permission { Id = 18, Name = "CategoryRequest.Create", Description = "Vendor requests a new category" },
                new Permission { Id = 19, Name = "CategoryRequest.ReadOwn", Description = "Vendor reads own category requests" },
                new Permission { Id = 20, Name = "CategoryRequest.ReadAll", Description = "Admin reads all category requests" },
                new Permission { Id = 21, Name = "CategoryRequest.Update", Description = "Admin approves/rejects requests" },
                new Permission { Id = 22, Name = "VendorUser.Create", Description = "Admin add new user assigned to a vendor" },
                new Permission { Id = 23, Name = "VendorUser.Read", Description = "Admin reads users of a specific vendor" },
                new Permission { Id = 24, Name = "VendorUser.Update", Description = "Admin updates vendor’s user" },
                new Permission { Id = 40, Name = "Vendor.Create", Description = "Create vendors" },
                new Permission { Id = 41, Name = "Vendor.Read", Description = "View vendors" },
                new Permission { Id = 42, Name = "Vendor.Update", Description = "Edit vendors" },
                new Permission { Id = 43, Name = "Vendor.Delete", Description = "Delete vendors" }
            );

            // ─── Seed: RolePermissions ─────────────────────────────────────────────
            modelBuilder.Entity<RolePermission>().HasData(
                // Admin full
                new RolePermission { Id = 1, RoleName = Roles.Admin, PermissionId = 1 },
                new RolePermission { Id = 2, RoleName = Roles.Admin, PermissionId = 2 },
                new RolePermission { Id = 3, RoleName = Roles.Admin, PermissionId = 3 },
                new RolePermission { Id = 4, RoleName = Roles.Admin, PermissionId = 4 },
                new RolePermission { Id = 5, RoleName = Roles.Admin, PermissionId = 5 },
                new RolePermission { Id = 6, RoleName = Roles.Admin, PermissionId = 6 },
                new RolePermission { Id = 7, RoleName = Roles.Admin, PermissionId = 7 },
                new RolePermission { Id = 8, RoleName = Roles.Admin, PermissionId = 8 },
                new RolePermission { Id = 9, RoleName = Roles.Admin, PermissionId = 9 },
                new RolePermission { Id = 10, RoleName = Roles.Admin, PermissionId = 10 },
                new RolePermission { Id = 11, RoleName = Roles.Admin, PermissionId = 11 },
                new RolePermission { Id = 12, RoleName = Roles.Admin, PermissionId = 12 },
                new RolePermission { Id = 13, RoleName = Roles.Admin, PermissionId = 13 },

                // VendorAdmin limited
                new RolePermission { Id = 14, RoleName = Roles.VendorAdmin, PermissionId = 1 },
                new RolePermission { Id = 15, RoleName = Roles.VendorAdmin, PermissionId = 2 },
                new RolePermission { Id = 16, RoleName = Roles.VendorAdmin, PermissionId = 3 },
                new RolePermission { Id = 17, RoleName = Roles.VendorAdmin, PermissionId = 4 },
                new RolePermission { Id = 18, RoleName = Roles.VendorAdmin, PermissionId = 5 },
                new RolePermission { Id = 19, RoleName = Roles.VendorAdmin, PermissionId = 6 },
                new RolePermission { Id = 20, RoleName = Roles.VendorAdmin, PermissionId = 7 },
                new RolePermission { Id = 21, RoleName = Roles.VendorAdmin, PermissionId = 8 },
                new RolePermission { Id = 22, RoleName = Roles.VendorAdmin, PermissionId = 11 },

                // VendorUser
                new RolePermission { Id = 23, RoleName = Roles.VendorUser, PermissionId = 6 },
                new RolePermission { Id = 24, RoleName = Roles.VendorUser, PermissionId = 7 },
                new RolePermission { Id = 25, RoleName = Roles.VendorUser, PermissionId = 5 },

                // Client
                new RolePermission { Id = 26, RoleName = Roles.Client, PermissionId = 6 },
                new RolePermission { Id = 27, RoleName = Roles.Client, PermissionId = 9 },
                new RolePermission { Id = 28, RoleName = Roles.Client, PermissionId = 11 },

                // Permission management (admin only)
                new RolePermission { Id = 29, RoleName = Roles.Admin, PermissionId = 14 },
                new RolePermission { Id = 30, RoleName = Roles.Admin, PermissionId = 15 },
                new RolePermission { Id = 31, RoleName = Roles.Admin, PermissionId = 16 },
                new RolePermission { Id = 32, RoleName = Roles.Admin, PermissionId = 17 },

                // CategoryRequest
                new RolePermission { Id = 33, RoleName = Roles.VendorAdmin, PermissionId = 18 },
                new RolePermission { Id = 34, RoleName = Roles.VendorAdmin, PermissionId = 19 },
                new RolePermission { Id = 35, RoleName = Roles.Admin, PermissionId = 20 },
                new RolePermission { Id = 36, RoleName = Roles.Admin, PermissionId = 21 },

                // Vendor CRUD perms for admin
                new RolePermission { Id = 37, RoleName = Roles.Admin, PermissionId = 40 },
                new RolePermission { Id = 38, RoleName = Roles.Admin, PermissionId = 41 },
                new RolePermission { Id = 39, RoleName = Roles.Admin, PermissionId = 42 },
                new RolePermission { Id = 40, RoleName = Roles.Admin, PermissionId = 43 },

                // VendorAdmin may view/update own vendor
                new RolePermission { Id = 41, RoleName = Roles.VendorAdmin, PermissionId = 41 },
                new RolePermission { Id = 42, RoleName = Roles.VendorAdmin, PermissionId = 42 }
            );
        }
    }
}
