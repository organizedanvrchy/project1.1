using Microsoft.EntityFrameworkCore;
using bankingApp.data.entities.accounts;
using bankingApp.data.entities.chequebook;
using bankingApp.data.entities.transactions;
using bankingApp.data.entities.users;

namespace bankingApp.data;

public class BankingDbContext : DbContext
{
    public BankingDbContext(DbContextOptions<BankingDbContext> options) : base(options) { }
   
    public DbSet<User> Users { get; set; } = null!;
    public DbSet<BankAccount> BankAccounts { get; set; } = null!;
    public DbSet<Transaction> Transactions { get; set; } = null!;
    public DbSet<ChequeBookRequest> ChequeBookRequests { get; set; } = null!;

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // ---- User hierarchy (TPH) ----
        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(e => e.UserId);
            entity.ToTable("users");

            entity.Property(e => e.UserId)
                  .HasColumnName("user_id");

            entity.Property(e => e.Username)
                  .HasMaxLength(25)
                  .IsUnicode(false)
                  .HasColumnName("username");

            entity.Property(e => e.PasswordHash)
                  .HasMaxLength(60)
                  .IsUnicode(false)
                  .HasColumnName("password_hash");

            entity.Property<string>("user_type")
                  .HasMaxLength(20)
                  .IsUnicode(false);

            entity.HasDiscriminator<string>("user_type")
                  .HasValue<Customer>("Customer")
                  .HasValue<Admin>("Admin");
        });

        // ---- BankAccount hierarchy (TPH) ----
        modelBuilder.Entity<BankAccount>(entity =>
        {
            entity.HasKey(e => e.AccountId);
            entity.ToTable("bank_accounts");

            entity.Property(e => e.AccountId)
                  .HasColumnName("account_id");

            entity.Property(e => e.CustomerId)
                  .HasColumnName("customer_id");

            entity.Property(e => e.Balance)
                  .HasColumnType("decimal(19,4)")
                  .HasColumnName("balance");

            entity.Property<string>("account_type")
                  .HasMaxLength(20)
                  .IsUnicode(false);

            entity.HasDiscriminator<string>("account_type")
                  .HasValue<CheckingAccount>("Checking")
                  .HasValue<SavingsAccount>("Savings")
                  .HasValue<LoanAccount>("Loan");

            entity.HasOne(d => d.Customer)
                  .WithMany(p => p.BankAccounts)
                  .HasForeignKey(d => d.CustomerId)
                  .OnDelete(DeleteBehavior.Cascade);
        });

        modelBuilder.Entity<SavingsAccount>(entity =>
        {
            entity.Property(e => e.WithdrawalLimitPerMonth)
                  .HasColumnName("withdrawal_limit_per_month");

            entity.Property(e => e.WithdrawalsThisPeriod)
                  .HasColumnName("withdrawals_this_period");

            entity.Property(e => e.PeriodStart)
                  .HasColumnName("period_start");
        });

        modelBuilder.Entity<LoanAccount>(entity =>
        {
            entity.Property(e => e.OriginalLoanAmount)
                  .HasColumnType("decimal(19,4)")
                  .HasColumnName("original_loan_amount");
        });

        // ---- Transaction ----
        modelBuilder.Entity<Transaction>(entity =>
        {
            entity.HasKey(e => e.TransactionId);
            entity.ToTable("transactions");

            entity.Property(e => e.TransactionId)
                  .HasColumnName("transaction_id");

            entity.Property(e => e.TransactionAmount)
                  .HasColumnType("decimal(19,4)")
                  .HasColumnName("transaction_amount");

            entity.Property(e => e.TransactionType)
                  .HasConversion<string>()
                  .HasMaxLength(20)
                  .IsUnicode(false)
                  .HasColumnName("transaction_type");

            entity.Property(e => e.TransactionDate)
                  .HasDefaultValueSql("(getdate())")
                  .HasColumnName("transaction_date");

            entity.Property(e => e.AccountId)
                  .HasColumnName("account_id");

            entity.Property(e => e.RecipientAccountId)
                  .HasColumnName("recipient_account_id");

            entity.HasOne(d => d.Account)
                  .WithMany(p => p.Transactions)
                  .HasForeignKey(d => d.AccountId)
                  .OnDelete(DeleteBehavior.Cascade)
                  .HasConstraintName("FK_transactions_accounts");

            entity.HasOne(d => d.RecipientAccount)
                  .WithMany()
                  .HasForeignKey(d => d.RecipientAccountId)
                  .OnDelete(DeleteBehavior.NoAction)
                  .HasConstraintName("FK_transactions_recipient");
                
            entity.Property(e => e.Direction)
                  .HasConversion<string>()
                  .HasMaxLength(10)
                  .IsUnicode(false)
                  .HasColumnName("transaction_direction");
        });

        // ---- ChequeBookRequest ----
        modelBuilder.Entity<ChequeBookRequest>(entity =>
        {
            entity.HasKey(e => e.RequestId);
            entity.ToTable("cheque_book_requests");

            entity.Property(e => e.RequestId)
                  .HasColumnName("request_id");

            entity.Property(e => e.AccountId)
                  .HasColumnName("account_id");

            entity.Property(e => e.RequestDate)
                  .HasDefaultValueSql("(getdate())")
                  .HasColumnName("request_date");

            entity.Property(e => e.RequestStatus)
                  .HasConversion<string>()
                  .HasMaxLength(20)
                  .IsUnicode(false)
                  .HasDefaultValue(RequestStatus.Pending)
                  .HasColumnName("request_status");

            entity.Property(e => e.RequestApprovedDate)
                  .HasColumnName("request_approved_date");

            entity.HasOne(d => d.Account)
                  .WithMany()
                  .HasForeignKey(d => d.AccountId)
                  .OnDelete(DeleteBehavior.Cascade)
                  .HasConstraintName("FK_cheque_requests_accounts");
        });
    }
}