---
applyTo:
  - "Repositeries/**/*.cs"
---

# Repository Layer Guidelines

## Overview
The Repository Layer is responsible for **all database interactions** via Entity Framework Core. It acts as the bridge between the Service Layer and the database, providing a clean abstraction for data access.

## Core Principles

### 1. Every Repository Must Have an Interface
```csharp
// ✅ CORRECT
public interface IUserRepository
{
    Task<User> GetById(int id);
    Task<User> AddUser(User user);
    Task<IEnumerable<User>> GetUsers();
}

public class UserRepository : IUserRepository
{
    // Implementation
}

// ❌ WRONG - Never implement without interface
public class UserRepository
{
    // ...
}
```

### 2. Dependency Injection via Constructor
```csharp
// ✅ CORRECT
private readonly Store_215962135Context _context;

public UserRepository(Store_215962135Context context)
{
    _context = context;
}

// ❌ WRONG - Never use static DbContext
public static Store_215962135Context _context = new();
```

### 3. All Methods MUST Be Async
```csharp
// ✅ CORRECT
public async Task<User> GetById(int id)
{
    return await _context.Users.FindAsync(id);
}

// ❌ WRONG - Synchronous methods block threads
public User GetById(int id)
{
    return _context.Users.Find(id);
}
```

## Method Patterns

### SELECT Operations

#### Get Single Entity by ID
```csharp
public async Task<User?> GetById(int id)
{
    return await _context.Users
        .Include(u => u.Orders)  // Load related data if needed
        .FirstOrDefaultAsync(u => u.UserId == id);
}
```

#### Get All Entities
```csharp
public async Task<IEnumerable<User>> GetAll()
{
    return await _context.Users.ToListAsync();
}
```

#### Search/Filter with Complex Queries
```csharp
public async Task<(IEnumerable<Product> products, int total)> GetProducts(
    int[]? categoryId, 
    string? q, 
    decimal? minPrice, 
    decimal? maxPrice,
    string? sort,
    int? skip,
    int? position)
{
    int pageSize = (skip.HasValue && skip.Value > 0) ? skip.Value : 12;
    int page = (position.HasValue && position.Value > 0) ? position.Value : 1;

    var query = _context.Products.AsQueryable();

    // Add filters conditionally
    if (categoryId != null && categoryId.Length > 0)
        query = query.Where(p => categoryId.Contains(p.CategoryId));

    if (minPrice.HasValue)
        query = query.Where(p => p.Price >= minPrice.Value);

    if (!string.IsNullOrWhiteSpace(q))
        query = query.Where(p => 
            p.ProductsName.Contains(q) || 
            p.ProductsDescreption.Contains(q));

    // Sorting
    if (string.Equals(sort, "desc", StringComparison.OrdinalIgnoreCase))
        query = query.OrderByDescending(p => p.Price);
    else if (string.Equals(sort, "asc", StringComparison.OrdinalIgnoreCase))
        query = query.OrderBy(p => p.Price);

    // Pagination
    int total = await query.CountAsync();
    var results = await query
        .Skip((page - 1) * pageSize)
        .Take(pageSize)
        .ToListAsync();

    return (results, total);
}
```

### INSERT Operations
```csharp
public async Task<User> AddUser(User user)
{
    await _context.Users.AddAsync(user);
    await _context.SaveChangesAsync();
    return user;
}
```

### UPDATE Operations
```csharp
public async Task<Order?> UpdateOrderStatus(int orderId, string newStatus)
{
    var order = await _context.Orders.FindAsync(orderId);
    if (order == null)
        return null;

    order.OrderStatus = newStatus;
    await _context.SaveChangesAsync();
    return order;
}
```

### DELETE Operations
```csharp
public async Task<bool> DeleteProduct(int productId)
{
    var product = await _context.Products.FindAsync(productId);
    if (product == null)
        return false;

    _context.Products.Remove(product);
    await _context.SaveChangesAsync();
    return true;
}
```

## Relationship Loading

### Eager Loading (RECOMMENDED)
```csharp
// ✅ CORRECT - Load related data upfront
var order = await _context.Orders
    .Include(o => o.OrdersItems)
    .ThenInclude(oi => oi.Products)
    .FirstOrDefaultAsync(o => o.OrderId == orderId);
```

### Lazy Loading (NOT RECOMMENDED)
```csharp
// ❌ AVOID - Causes N+1 queries
var order = await _context.Orders.FindAsync(orderId);
var items = order.OrdersItems; // Triggers separate query
```

## Query Optimization

### 1. Use AsQueryable() for Complex Filtering
```csharp
var query = _context.Products.AsQueryable();

// Build query conditionally without loading
if (minPrice.HasValue)
    query = query.Where(p => p.Price >= minPrice.Value);

if (maxPrice.HasValue)
    query = query.Where(p => p.Price <= maxPrice.Value);

// Execute once with all filters combined
var results = await query.ToListAsync();
```

### 2. Pagination (Always Use for Large Result Sets)
```csharp
int pageSize = 12;
int page = 1;

var total = await query.CountAsync();
var items = await query
    .Skip((page - 1) * pageSize)
    .Take(pageSize)
    .ToListAsync();
```

### 3. Select Only Needed Columns (for large queries)
```csharp
// Load only needed fields, not entire entity
var userNames = await _context.Users
    .Select(u => u.UserName)
    .ToListAsync();
```

## Null Handling

### Return null for "Not Found" Scenarios
```csharp
// ✅ CORRECT
public async Task<User?> GetById(int id)
{
    return await _context.Users.FindAsync(id);
    // Returns null if not found
}
```

### Never Return null for Lists
```csharp
// ✅ CORRECT - Return empty list
public async Task<IEnumerable<User>> GetAll()
{
    return await _context.Users.ToListAsync();
    // Returns empty list if no users
}
```

## Exception Handling

### Propagate Database Errors to Service Layer
```csharp
// ✅ CORRECT - Let exceptions bubble up
public async Task<User> AddUser(User user)
{
    await _context.Users.AddAsync(user);
    await _context.SaveChangesAsync();
    // If DB throws (e.g., duplicate key), let it propagate
    return user;
}
```

### Don't Swallow Exceptions
```csharp
// ❌ WRONG
try
{
    await _context.SaveChangesAsync();
}
catch
{
    return null; // Hides real errors
}
```

## DbContext Best Practices

### 1. Always Inject DbContext (Never Instantiate)
```csharp
// ✅ CORRECT
private readonly Store_215962135Context _context;

public UserRepository(Store_215962135Context context)
{
    _context = context;
}
```

### 2. Access DbSets via Properties
```csharp
// ✅ CORRECT
var users = await _context.Users.ToListAsync();
var orders = await _context.Orders.ToListAsync();
```

## Naming Conventions

### Method Naming
- `GetById(id)` → Single entity by ID
- `GetAll()` / `GetUsers()` → All entities
- `Get{EntityName}By{Property}(value)` → Filter by specific property
- `Add{EntityName}(entity)` → Insert
- `Update{EntityName}(...)` → Update
- `Delete{EntityName}(id)` → Delete

### Return Types
- Single entity: `Task<Entity?>` (nullable)
- Collection: `Task<IEnumerable<Entity>>` (never null, empty if none)
- Success indicator: `Task<bool>`
- Complex result: `Task<(IEnumerable<Entity> items, int total)>` (for pagination)

## Testing Repositories

Repositories are tested via **Integration Tests** with a real database:
```csharp
public class UserRepositoryTests : IClassFixture<DatabaseFixture>
{
    private readonly Store_215962135Context _context;
    
    public UserRepositoryTests(DatabaseFixture fixture)
    {
        _context = fixture.CreateContext();
    }

    [Fact]
    public async Task GetById_WithValidId_ReturnsUser()
    {
        // Arrange
        var userId = 1;
        
        // Act
        var user = await _context.Users.FindAsync(userId);
        
        // Assert
        Assert.NotNull(user);
    }
}
```

## Common Mistakes to Avoid

| ❌ WRONG | ✅ CORRECT |
|---------|-----------|
| `public User GetById(id)` | `public async Task<User?> GetById(id)` |
| `return _context.Users.ToList()` | `return await _context.Users.ToListAsync()` |
| `if (users != null && users.Any())` | `var users = await _context.Users.ToListAsync(); if (users.Any())` |
| No interface for repository | `public interface IUserRepository` + `public class UserRepository : IUserRepository` |
| `.Result` or `.Wait()` | Always use `await` |
| Hardcoded DbContext | Inject via constructor |
| No pagination for large queries | Always paginate |
| Lazy loading relationships | Use `.Include()` for eager loading |
