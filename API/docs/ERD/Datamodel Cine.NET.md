NOG ONDER VOORBEHOUD, ENKEL EEN LEIDRAAD! GIEL 11/03/26

Datamodel
| Tabel                  | Kolommen                                                                                                           | Relaties                                                           | Constraints                                                                                                 |
| ---------------------- | ------------------------------------------------------------------------------------------------------------------ | ------------------------------------------------------------------ | ----------------------------------------------------------------------------------------------------------- |
| **User**               | Id, FirstName, LastName, EmailAddress, PasswordHash, UserType, NewsletterSubscription                              | 1-* Order, 1-* Review, 1-* SpecialCard, 1-* Newsletter             | PK(Id), Email UNIQUE, UserType enum                                                                         |
| **Movie**              | Id, Title, Description, Duration, Genre, Director, Language, ReleaseDate                                           | 1-* Showing, 1-* Photo, 1-* Review                                 | PK(Id)                                                                                                      |
| **Photo**              | Id, MovieId, FilePath                                                                                              | *-1 Movie                                                          | PK(Id), FK(MovieId)                                                                                         |
| **Auditorium**         | Id, Name, NumberOfRows, SeatsPerRow, WheelchairAccessible, 3DCompatible                                            | 1-* Showing                                                        | PK(Id)                                                                                                      |
| **Showing**            | Id, AuditoriumId, MovieId, StartTime, EndTime, IsActive                                                            | *-1 Movie, *-1 Auditorium, 1-* Ticket                              | PK(Id), FK(MovieId), FK(AuditoriumId)                                                                       |
| **PriceConfiguration** | Id, TicketType, Price, IncludesPopcorn, IncludesDrink, Description                                                 | 1-* Ticket                                                         | PK(Id), TicketType enum                                                                                     |
| **Order**              | Id, UserId, OrderDate, OrderCode, TotalAmount, OrderType, PaymentMethod, CashierEmployeeId, IsPrinted              | *-1 User, 1-* Ticket                                               | PK(Id), FK(UserId), OrderType enum, PaymentMethod enum                                                      |
| **Ticket**             | Id, OrderId, ShowingId, PriceConfigurationId, SpecialCardId, RowNumber, SeatNumber, FinalPrice, TicketCode, IsUsed | *-1 Order, *-1 Showing, *-1 PriceConfiguration, *-0..1 SpecialCard | PK(Id), FK(OrderId), FK(ShowingId), FK(PriceConfigurationId), FK(SpecialCardId NULLABLE), TicketCode UNIQUE |
| **SpecialCard**        | Id, CardType, UserId, RemainingRides, Value, ValidForDays, ValidUntil                                              | *-1 User, 1-* Ticket                                               | PK(Id), FK(UserId), CardType enum                                                                           |
| **Review**             | Id, MovieId, UserId, Title, Content, Rating, CreatedAt                                                             | *-1 Movie, *-1 User                                                | PK(Id), FK(MovieId), FK(UserId), Rating CHECK (1–10)                                                        |
| **Newsletter**         | Id, SentByUserId, Subject, Content, SentAt, RecipientCount                                                         | *-1 User                                                           | PK(Id), FK(SentByUserId)                                                                                    |

Beschikbare Enums
| Enum              | Waarden                                                        |
| ----------------- | -------------------------------------------------------------- |
| **TicketType**    | Regular, Child, Senior, Student, VIP, SecretMovie, PopcornDeal |
| **CardType**      | TenRideCard, Subscription, MaDiWoDoBonus, CinemaCard           |
| **OrderType**     | Touch, Website, Cashier                                        |
| **PaymentMethod** | PIN, Ideal, CreditCard                                         |
| **UserType**      | Admin, Manager, BackOfficeEmployee, Cashier, Customer          |
