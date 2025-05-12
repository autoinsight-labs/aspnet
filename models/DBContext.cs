using Microsoft.EntityFrameworkCore;

public class AutoInsightDB: DbContext
{
  public AutoInsightDB(DbContextOptions<AutoInsightDB> options) : base(options)
  {
  }

  public DbSet<Address> Addresses => Set<Address>();
  public DbSet<Booking> Bookings => Set<Booking>();
  public DbSet<EmployeeInvite> EmployeeInvites => Set<EmployeeInvite>();
  public DbSet<Model> Models => Set<Model>();
  public DbSet<QRCode> QRCodes => Set<QRCode>();
  public DbSet<Vehicle> Vehicles => Set<Vehicle>();
  public DbSet<Yard> Yards => Set<Yard>();
  public DbSet<YardEmployee> YardEmployees => Set<YardEmployee>();
  public DbSet<YardVehicle> YardVehicles => Set<YardVehicle>();
}
