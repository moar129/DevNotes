using DevNotesApi.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace DevNotesApi.Data
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        // DbSets for all entities
        public DbSet<Folder> Folders { get; set; }
        public DbSet<Note> Notes { get; set; }
        public DbSet<NoteImage> NoteImages { get; set; }
        public DbSet<SharedNote> SharedNotes { get; set; }
        public DbSet<SharedFolder> SharedFolders { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            // ------------------------
            // Folder self-referencing (ParentFolder -> SubFolders)
            // ------------------------
            builder.Entity<Folder>()
                .HasOne(f => f.ParentFolder)
                .WithMany(f => f.SubFolders)
                .HasForeignKey(f => f.ParentFolderId)
                .OnDelete(DeleteBehavior.Restrict);

            // Folder -> User
            builder.Entity<Folder>()
                .HasOne(f => f.User)
                .WithMany(u => u.Folders)
                .HasForeignKey(f => f.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            // Note -> User
            builder.Entity<Note>()
                .HasOne(n => n.User)
                .WithMany(u => u.Notes)
                .HasForeignKey(n => n.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            // Note -> Folder
            builder.Entity<Note>()
                .HasOne(n => n.Folder)
                .WithMany(f => f.Notes)
                .HasForeignKey(n => n.FolderId)
                .OnDelete(DeleteBehavior.Restrict);

            // NoteImage -> Note
            builder.Entity<NoteImage>()
                .HasOne(i => i.Note)
                .WithMany(n => n.Images)
                .HasForeignKey(i => i.NoteId)
                .OnDelete(DeleteBehavior.Cascade);

            // ------------------------
            // SharedNote relationships
            // ------------------------
            builder.Entity<SharedNote>()
                .HasOne(s => s.Note)
                .WithMany()
                .HasForeignKey(s => s.NoteId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.Entity<SharedNote>()
                .HasOne(s => s.FromUser)
                .WithMany(u => u.SentNotes)
                .HasForeignKey(s => s.FromUserId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Entity<SharedNote>()
                .HasOne(s => s.ToUser)
                .WithMany(u => u.ReceivedNotes)
                .HasForeignKey(s => s.ToUserId)
                .OnDelete(DeleteBehavior.Restrict);

            // ------------------------
            // SharedFolder relationships
            // ------------------------
            builder.Entity<SharedFolder>()
                .HasOne(sf => sf.Folder)
                .WithMany()
                .HasForeignKey(sf => sf.FolderId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.Entity<SharedFolder>()
                .HasOne(sf => sf.Sender)
                .WithMany(u => u.SentFolders)
                .HasForeignKey(sf => sf.SenderId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Entity<SharedFolder>()
                .HasOne(sf => sf.Receiver)
                .WithMany(u => u.ReceivedFolders)
                .HasForeignKey(sf => sf.ReceiverId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
