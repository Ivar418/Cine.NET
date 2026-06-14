### Order Types and Order/Payment Status Locations

This document lists all order types and status values found in the project and their locations, intended for replacement with Enums.

---

#### 1. Order Types (`OrderType`)

**Identified Values:**
- **Reservation** (Default in `Order.cs`)
- **Online** (Used in tests)
- **Touch** (Mentioned in ERD)
- **Website** (Mentioned in ERD)
- **Cashier** (Mentioned in ERD)

**Locations:**

| Value | File Path | Line(s) | Context |
|-------|-----------|---------|---------|
| Reservation | `SharedLibrary\Domain\Entities\Order.cs` | 11 | Default value |
| Reservation | `API\src\Infrastructure\Database\DBSeeder.cs` | 184 | Seed data |
| Online | `UnitTest\APITests\Controllers\OrdersControllerTests.cs` | 27, 47 | Test data |
| Touch, Website, Cashier | `API\docs\ERD\Datamodel Cine.NET.md` | 23 | ERD Documentation |

**Current Enum Definition:**
Located at: `SharedLibrary\Domain\Entities\Enums\OrderTypes.cs`
```csharp
public enum OrderTypes {
    
}
```
*(Currently empty, needs values above)*

---

#### 2. Order Payment Status (`PaymentStatus`)

**Identified Values:**
- **Pending** (Default)
- **Paid**
- **Failed** (Mentioned in `Ticket.cs` comment)
- **Cancelled** (Mentioned in `Ticket.cs` comment)

**Locations (Order & Ticket):**

| Value | File Path | Line(s) | Context |
|-------|-----------|---------|---------|
| Pending | `SharedLibrary\Domain\Entities\Order.cs` | 12 | Default value (Order) |
| Pending | `SharedLibrary\Domain\Entities\Ticket.cs` | 28 | Default value (Ticket) |
| Pending | `API\src\Services\Implementations\OrderService.cs` | 58, 269, 273 | Logic |
| Pending | `API\src\Infrastructure\Database\DBSeeder.cs` | 174, 185 | Seed data |
| Paid | `API\src\Services\Implementations\OrderService.cs` | 132, 133, 137 | Logic (Mark as paid) |
| Paid | `API\src\Services\Implementations\OrderPdfService.cs` | 129 | PDF generation check |
| Paid | `UnitTest\APITests\Controllers\OrdersControllerTests.cs` | 154 | Test assertion |
| Paid | `UnitTest\APITests\Repositories\TicketRepositoryTests.cs` | 207, 213 | Test assertion |
| Paid | `UnitTest\APITests\Repositories\OrderRepositoryTests.cs` | 140, 143 | Test assertion |
| Failed | `SharedLibrary\Domain\Entities\Ticket.cs` | 28 | Comment |
| Cancelled | `SharedLibrary\Domain\Entities\Ticket.cs` | 28 | Comment |

---

#### 3. Reservation & Ticket Status (`Status`)

Though not explicitly "Order Status", these are closely related.

**Reservation Status Values:**
- **Pending**
- **Confirmed**
- **Cancelled**

**Ticket Status Values:**
- **Active**
- **Used**
- **Cancelled**
- **Expired**

**Locations:**

| Entity | Value | File Path | Line(s) | Context |
|--------|-------|-----------|---------|---------|
| Reservation | Pending | `SharedLibrary\Domain\Entities\Reservation.cs` | 14 | Default value |
| Reservation | Confirmed | `API\src\Controllers\ReservationController.cs` | 150 | Logic |
| Ticket | Active | `SharedLibrary\Domain\Entities\Ticket.cs` | 25, 82, 91, 100, 107 | Logic & Default |
| Ticket | Used | `SharedLibrary\Domain\Entities\Ticket.cs` | 25, 85, 108 | Logic |
| Ticket | Cancelled | `SharedLibrary\Domain\Entities\Ticket.cs` | 25, 94, 109 | Logic |
| Ticket | Expired | `SharedLibrary\Domain\Entities\Ticket.cs` | 25, 103, 110 | Logic |
