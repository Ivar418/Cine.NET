### Payment Types and Locations

This document lists all payment types found in the project and their locations, intended for replacement with Enums.

#### Identified Payment Types
- **PIN** / **Pin**
- **iDEAL**
- **Credit Card** / **Creditard** / **CREDITCARD**
- **Reservation** / **Reserveren**

#### Locations

| Payment Type | File Path | Line(s) | Context |
|--------------|-----------|---------|---------|
| PIN | `API\src\Infrastructure\Database\DBSeeder.cs` | 66 | Database seeding |
| Pin | `API\src\Controllers\OrderController.cs` | 150, 156 | Logic & Comments |
| iDEAL | `API\src\Infrastructure\Database\DBSeeder.cs` | 67 | Database seeding |
| iDEAL | `API\src\Controllers\OrderController.cs` | 150, 157 | Logic & Comments |
| IDEAL | `API\src\Infrastructure\Database\DBSeeder.cs` | 186 | Seed data |
| Credit Card | `API\src\Infrastructure\Database\DBSeeder.cs` | 68 | Display name in Seeder |
| Credit Card | `API\src\Controllers\OrderController.cs` | 150 | Comment |
| CREDITCARD | `API\src\Infrastructure\Database\DBSeeder.cs` | 68 | Code in Seeder |
| Creditard | `API\src\Controllers\OrderController.cs` | 158 | Logic (Switch expression) |
| Reservation | `API\src\Controllers\OrderController.cs` | 154 | Logic |
| Reserveren | `API\src\Controllers\OrderController.cs` | 155 | Logic |

#### Current Enum Definition
Located at: `SharedLibrary\Domain\Entities\Enums\PaymentTypes.cs`
```csharp
public enum PaymentTypes {
    CreditCard = 1,
    Pin = 2,
    iDEAL = 3,
    Reservation = 4,
}
```

#### Other Occurrences of "PaymentMethod" (to be checked for hardcoded strings)
- `SharedLibrary\Domain\Entities\Order.cs`: Default value "Unknown" (Line 13).
- `API\src\Services\Implementations\OrderService.cs`: Used in mapping and validation.
- `API\src\Mappers\OrderMapper.cs`: Used in mapping.
- `SharedLibrary\DTOs\Requests\CreateOrderRequest.cs`: Property definition.
- `SharedLibrary\DTOs\Responses\CreateOrderResponse.cs`: Property definition.
- `SharedLibrary\DTOs\Responses\OrderPdfResponse.cs`: Property definition.
- `API\src\Infrastructure\Database\ApiDbContext.cs`: EF Core configuration.
