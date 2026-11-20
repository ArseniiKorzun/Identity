# Movie API: контракти та валідація

ASP.NET Core Web API (.NET 10): Controller -> Service -> Repository -> DbContext -> SQL Server LocalDB. Шари винесено в окремі проєкти (див. розділ 9).

## Інструкція запуску

1. Потрібні .NET 10 SDK і SQL Server LocalDB (входить у Visual Studio).
2. Рядок підключення в `WebApplication1/appsettings.json` (ключ `ConnectionStrings:DefaultConnection`).
3. Застосувати міграції з кореня репозиторію (папка з `WebApplication1.slnx`):
   ```
   dotnet ef database update --project Cinema.DataAccess --startup-project WebApplication1
   ```
   (або `update-database` у Package Manager Console: Startup Project = WebApplication1, Default project = Cinema.DataAccess)
   Міграція `AddIdentity` видаляє старих користувачів, після неї їх треба зареєструвати заново (розділ 10).
4. Запустити з папки `WebApplication1`: `dotnet run --launch-profile http` і відкрити `http://localhost:5199/swagger`.

## Ендпоінти фільмів (`api/Movie`)

| Метод | Маршрут | Тіло | Відповідь |
|---|---|---|---|
| GET | `/movies` | - | 200 `MovieResponse[]` |
| GET | `/movies/{id}` | - | 200 `MovieResponse` / 404 |
| POST | `/movies` | `CreateMovieRequest` | 201 `MovieResponse` / 400 |
| PUT | `/movies/{id}` | `UpdateMovieRequest` | 200 `MovieResponse` / 400 / 404 |
| DELETE | `/movies/{id}` | - | 204 / 404 |

Id не входить у `CreateMovieRequest` (його генерує БД) і в `UpdateMovieRequest` (береться з маршруту). Поля фільму: `Title`, `Description`, `PhotoPath`, `Director`, `Runtime`, `ReleaseYear`, `PosterUrl`, `MovieUrl`.

## Ендпоінти коментарів (`api/Movie`)

| Метод | Маршрут | Тіло | Відповідь |
|---|---|---|---|
| GET | `/movies/{movieId}/comments` | - | 200 `MovieCommentResponse[]` / 404 |
| POST | `/movies/{movieId}/comments` | `CreateMovieCommentRequest` | 201 `MovieCommentResponse` / 400 / 404 |
| PUT | `/movies/{movieId}/comments/{id}` | `UpdateMovieCommentRequest` | 200 `MovieCommentResponse` / 400 / 404 |
| DELETE | `/movies/{movieId}/comments/{id}` | - | 204 / 404 |

`Id` і `CreatedAt` коментаря задає сервер. Коментар до неіснуючого фільму створити не можна (404). Видалення фільму видаляє і його коментарі (каскад).

Swagger UI з усіма ендпоінтами та схемами контрактів:

![Swagger](docs/screenshots/swagger.png)

## 1. Data Annotations

| Атрибут | Призначення | Приклад |
|---|---|---|
| `[Required]` | значення обов'язкове (для рядка: не null і не порожній) | `[Required(ErrorMessage = "Title is required.")]` |
| `[StringLength(max, MinimumLength = min)]` | діапазон довжини рядка | `[StringLength(100, MinimumLength = 1)]` |
| `[MinLength(n)]` / `[MaxLength(n)]` | мінімальна / максимальна довжина рядка або колекції | `[MaxLength(1000)]` |
| `[Range(min, max)]` | діапазон числа | `[Range(1, 600)]` |
| `[EmailAddress]` | формат email | `[EmailAddress]` |
| `[Phone]` | формат телефону | `[Phone]` |
| `[Url]` | формат URL | `[Url]` |
| `[RegularExpression(pattern)]` | відповідність шаблону | `[RegularExpression(@"^\d{4}$")]` |
| `[Compare("Other")]` | збіг з іншою властивістю | `[Compare("Password")]` |
| `[CreditCard]` | формат номера картки | `[CreditCard]` |
| `[Key]` | первинний ключ (для EF, не валідація) | `[Key] public int Id` |

Пояснення атрибутів і типів контрактів:

- **Модель БД (Entity `Movie`)** описує таблицю, містить Id і всі поля, які зберігаються.
- **Контракт запиту** описує лише те, що клієнту дозволено надіслати, і містить правила перевірки.
- **Контракт відповіді (`MovieResponse`)** описує те, що сервер віддає клієнту (Id плюс потрібні поля, без службових даних).
- **Де перевіряти:** на межі системи, тобто в контролері до сервісу. Клієнту не можна довіряти: запит можуть надіслати зі зламаного клієнта чи зловмисно. З `[ApiController]` ASP.NET перевіряє атрибути сам і повертає 400.
- **Чому не давати клієнту задавати всі поля сутності (overposting):** клієнт зміг би підставити Id, службові поля або права (наприклад `IsAdmin`), перезаписати чужі записи. Окремий контракт відкриває тільки безпечні поля.

## 4. Результати перевірки (попередня версія без коментарів і нових полів)

Запити виконані на запущеному застосунку тими самими HTTP-викликами, що йдуть зі Swagger UI. Початковий стан БД: 7 фільмів. Після всіх помилкових запитів кількість і дані не змінились.

| # | Запит | Очікувано | Фактично |
|---|---|---|---|
| OK1 | POST `/movies` `{"title":"Inception","description":"Dream heist","photoPath":"http://x/y.jpg","director":"Nolan","runtime":148}` | 201, фільм з Id | 201, `{"id":22,...}` |
| OK2 | GET `/movies` | 200, список | 200, список з новим фільмом |
| OK3 | GET `/movies/22` | 200, фільм | 200, `{"id":22,"title":"Inception",...}` |
| OK4 | PUT `/movies/22` `{"title":"Inception 2",...,"runtime":150}` | 200, оновлений | 200, `title":"Inception 2","runtime":150` |
| OK5 | DELETE `/movies/22` | 204 | 204, у списку фільму більше немає |
| ERR1 | POST, `"title":""` | 400, помилка Title | 400, `Title: ["Title is required.","Title must be between 1 and 100 characters."]` |
| ERR2 | POST, title із 101 символу | 400, помилка Title | 400, `Title: ["Title must be between 1 and 100 characters."]` |
| ERR3 | POST, `"runtime":-5` | 400, помилка Runtime | 400, `Runtime: ["Runtime must be between 1 and 600 minutes."]` |
| ERR4 | POST, `"runtime":601` | 400, помилка Runtime | 400, `Runtime: ["Runtime must be between 1 and 600 minutes."]` |
| ERR5 | POST без `director` | 400, помилка Director | 400, `Director: ["Director is required."]` |
| ERR6 | GET `/movies/99999` | 404 | 404, `Movie with id 99999 not found.` |
| ERR7 | PUT `/movies/99999` з коректним тілом | 404 | 404, `Movie with id 99999 not found.` |
| ERR8 | PUT `/movies/22`, `"runtime":0` | 400, дані не змінились | 400, `Runtime: [...]`, запис не змінився |
| ERR9 | DELETE `/movies/99999` | 404 | 404, `Movie with id 99999 not found.` |

Після ERR1-ERR9 GET `/movies` показав ті самі дані, що й до них (некоректні дані не збереглись).

## 5. FluentValidation і Data Annotations

| | Data Annotations | FluentValidation |
|---|---|---|
| Де правила | атрибути на моделі | окремий клас-валідатор |
| Пакет | вбудований | NuGet |
| Виклик | автоматично з `[ApiController]` | вручну (`Validate`) |
| Складні умови (`When`, залежності між полями, сервіси) | обмежено | зручно |
| Модель | засмічена атрибутами | чиста |

Те саме правило «Title обов'язковий, до 100 символів»:

```csharp
// Data Annotations
[Required(ErrorMessage = "Title is required.")]
[MaxLength(100, ErrorMessage = "Title cannot exceed 100 characters.")]
public string Title { get; set; }

// FluentValidation
RuleFor(x => x.Title)
    .NotEmpty().WithMessage("Title is required.")
    .MaximumLength(100).WithMessage("Title cannot exceed 100 characters.");
```

Контракти фільмів і коментарів валідуються через FluentValidation (`Validations/`): `CreateMovieRequestValidator`, `UpdateMovieRequestValidator`, `CreateMovieCommentRequestValidator`, `UpdateMovieCommentRequestValidator`. Спільні правила винесені в базові класи `MovieRequestValidator<T>` і `MovieCommentRequestValidator<T>`. Data Annotations залишились лише на Entity-моделях (схема БД). Реєстрація та логін (User) також використовують FluentValidation.

## 6. Розширення: ReleaseYear, PosterUrl, MovieUrl, коментарі

Шари: Controller -> Service -> Repository (інтерфейси, реєстрація в DI через `AddBusiness` / `AddDataAccess`) -> DbContext. Контролери не звертаються до `AppDbContext` напряму.

Правила валідації:
- `Title`, `Director` обов'язкові, до 100 символів; `Description` до 1000; `PhotoPath`, `PosterUrl`, `MovieUrl` до 500.
- `Runtime` від 1 до 600.
- `ReleaseYear` не раніше 1888 і не більше ніж поточний рік + 1.
- `PosterUrl` і `MovieUrl` необов'язкові, але якщо вказані, мають бути абсолютними `http` або `https` посиланнями.
- Коментар: `AuthorName` обов'язковий до 100 символів, `Text` обов'язковий до 1000 символів.

Міграція: `AddMovieExtensionsAndComments` (нові колонки `ReleaseYear`, `PosterUrl`, `MovieUrl`, таблиця `MovieComments` з FK і каскадним видаленням). Старі фільми отримали `ReleaseYear = 0`, його треба оновити через PUT.

Результати перевірки (Swagger-еквівалентні HTTP-запити на запущеному застосунку):

| # | Запит | Очікувано | Фактично |
|---|---|---|---|
| OK1 | POST `/movies` з усіма полями | 201 | 201, `{"id":23,...,"releaseYear":2014}` |
| OK2 | GET `/movies/23` | 200 | 200 |
| OK3 | PUT `/movies/23` з `runtime: 170` | 200 | 200, нові значення |
| OK4 | POST `/movies/23/comments` (два коментарі) | 201 | 201, `id` і `createdAt` від сервера |
| OK5 | GET `/movies/23/comments` | 200, список | 200, два коментарі |
| OK6 | PUT `/movies/23/comments/1` | 200 | 200, оновлений текст |
| OK7 | DELETE `/movies/23/comments/2` | 204 | 204 |
| ERR1 | POST, `"title":""` | 400 | 400, `Title: ["Title is required."]` |
| ERR2 | POST, `"releaseYear":2100` | 400 | 400, `ReleaseYear: ["Release year cannot be more than one year in the future."]` |
| ERR3 | POST, `"posterUrl":"ftp://x/y.jpg"` | 400 | 400, `PosterUrl: ["Poster URL must be a valid http or https link."]` |
| ERR4 | PUT, `"runtime":0` | 400, дані не змінились | 400, `Runtime: ["Runtime must be between 1 and 600 minutes."]` |
| ERR5 | POST коментаря до фільму 99999 | 404 | 404, `Movie with id 99999 not found.` |
| ERR6 | POST коментаря з порожнім `text` | 400 | 400, `Text: ["Comment text is required."]` |
| ERR7 | POST коментаря, `authorName` 101 символ | 400 | 400, `AuthorName: ["Author name cannot exceed 100 characters."]` |
| ERR8 | PUT коментаря через неіснуючий фільм | 404 | 404, `Comment with id 1 not found for movie 99999.` |
| ERR9 | GET коментарів неіснуючого фільму | 404 | 404 |
| ERR10 | DELETE фільму 99999 | 404 | 404 |
| OK8 | DELETE фільму 23, далі GET його коментарів | 204, 404 | 204, 404 (коментарі видалені каскадно) |

## 7. Сеанси фільмів (`api/MovieSession`)

Модель `MovieSession`: `Id`, `MovieId` (зв'язок «один фільм — багато сеансів», FK з каскадним видаленням), `StartsAt`, `HallName`, `Price` (`decimal(10,2)`), `AvailableSeats`. Зв'язок налаштовано в `AppDbContext` через Fluent API. Міграція: `AddMovieSessions`.

Часовий пояс: **усі часи сеансів у UTC**. API приймає `StartsAt` лише з суфіксом `Z` (наприклад `2030-01-15T18:00:00Z`); зсув на кшталт `+02:00` відхиляється. Відповідь завжди містить `Z`. В інтерфейсі час підписано як UTC.

| Метод | Маршрут | Тіло | Відповідь |
|---|---|---|---|
| GET | `/MovieSession/sessions?movieId=&date=&maxPrice=` | - | 200 `MovieSessionResponse[]` / 400 |
| GET | `/MovieSession/sessions/{id}` | - | 200 `MovieSessionResponse` / 404 |
| POST | `/MovieSession/sessions` | `CreateMovieSessionRequest` | 201 `MovieSessionResponse` / 400 |
| PUT | `/MovieSession/sessions/{id}` | `UpdateMovieSessionRequest` | 200 `MovieSessionResponse` / 400 / 404 |
| DELETE | `/MovieSession/sessions/{id}` | - | 204 / 404 |

- GET повертає лише **майбутні** сеанси, відсортовані від найближчого. `date` (`yyyy-MM-dd`, UTC-день) і `maxPrice` є необов'язковими фільтрами.
- `MovieSessionResponse` містить `MovieTitle` (назву фільму), щоб клієнту не потрібен другий запит.
- Правила: фільм має існувати (`MovieId`), `HallName` обов'язкова до 50 символів, `Price` і `AvailableSeats` не від'ємні, `Price` не більше 99999999.99. Створити сеанс у минулому не можна. При редагуванні перевіряється час лише тоді, коли він змінився.
- Видалення фільму з майбутніми сеансами повертає 409 і не видаляє дані. Видалення фільму без майбутніх сеансів каскадно видаляє його минулі сеанси та коментарі.

DTO розділені: `CreateMovieSessionRequest`, `UpdateMovieSessionRequest` і `MovieSessionResponse`. Валідація: `MovieSessionRequestValidator<T>` (спільні правила), `CreateMovieSessionRequestValidator` (додає правило «не в минулому»).

Асинхронність: контролер, сервіс і репозиторій для фільмів, коментарів і сеансів повертають `Task`/`Task<T>` і використовують `ToListAsync`, `FirstOrDefaultAsync`, `AnyAsync`. `SaveChangesAsync` винесено в окремий метод репозиторію, який сервіс викликає після змін. Авторизація (`AuthController`, `IdentityAuthService`, `UserRepository`) теж асинхронна і працює через Identity, див. розділ 10.

## 8. Запуск клієнта

```
cd client
npm ci
npm run dev
```

Відкрити `http://localhost:5173`. Сторінки (хеш-маршрути):
- `#/afisha`: афіша майбутніх сеансів з фільтрами за датою та максимальною ціною, сортування від найближчого.
- `#/movies`: список фільмів, додавання і видалення. Назва фільму відкриває `#/movies/{id}`.
- `#/movies/{id}`: інформація про фільм і його майбутні сеанси.
- `#/admin`: панель керування сеансами (створення, редагування, видалення). Фільм обирається зі списку, який віддає API.

Приховування кнопок не є захистом API. Рольовий доступ і JWT не реалізовані.

## 9. Структура рішення

`WebApplication1.slnx` складається з чотирьох проєктів. Залежності йдуть в один бік: API -> Business -> DataAccess -> Domain.

| Проєкт | Що містить | Посилається на |
|---|---|---|
| `Cinema.Domain` | сутності БД (`Entities/`: `Movie`, `MovieComment`, `MovieSession`, `User` як `IdentityUser<int>`) | ASP.NET Core (Identity) |
| `Cinema.DataAccess` | `Context/AppDbContext` (IdentityDbContext, Fluent API), `Repositories/`, `Migrations/`, `AddDataAccess` | Domain, EF Core SqlServer, Identity EF |
| `Cinema.Business` | `Services/` (бізнес-правила), `AddBusiness` | DataAccess, Domain |
| `WebApplication1` | контролери, контракти (DTO), FluentValidation, AutoMapper, `Program.cs` | Business, Domain |

- Окремих DTO у бізнес-шарі немає: сервіси працюють із сутностями, а контракти й мапінг належать API.
- У `WebApplication1.csproj` увімкнено `DisableTransitiveProjectReferences`, тому API не бачить `Cinema.DataAccess`. Спроба звернутися з контролера до `AppDbContext` чи репозиторію дає помилку компіляції CS0234.
- `Program.cs` реєструє шари одним викликом `builder.Services.AddBusiness(connectionString)`.
- Нова міграція (з кореня репозиторію): `dotnet ef migrations add <Назва> --project Cinema.DataAccess --startup-project WebApplication1`.

## 10. Identity: користувачі та паролі

`User` успадковує `IdentityUser<int>` (поля `Id`, `UserName`, `Email`, `PasswordHash`, `SecurityStamp` тощо) і додає `Name` та `CreatedAt`. Таблиця називається `AspNetUsers`.

- **Паролі хешуються.** Identity зберігає в `PasswordHash` хеш (PBKDF2), а не сам пароль.
- **Email унікальний.** Перевірка в контролері (409) і унікальний індекс `EmailIndex` на `NormalizedEmail` у БД. `UserName` дорівнює email.
- **Політика пароля** така сама, як у валідаторі: мінімум 6 символів, без вимог до цифр, регістру й символів. Налаштовується в `AddDataAccess`.
- **Вхід** за логіном (`UserName`, у якому зберігається email) через `IdentityAuthService.ValidateUserCredentialsAsync`: `FindByNameAsync`, потім `UserManager.CheckPasswordAsync`. Відповідь 200 з `true` або `false`, неіснуючий логін і невірний пароль дають однаковий `false`. Схема за конспектом описана в `docs/identity.md`.
- **Сесій, JWT і ролей поки немає.** Логін повертає лише `true` або `false`, як у конспекті. Захист ендпоінтів і ролі будуть на наступному етапі, тому `GET /api/auth/users` поки доступний без авторизації.
- **Збереження.** `UserManager` сам зберігає зміни, тому для користувачів `SaveChangesAsync` у сервісі не викликається. Це виняток із загального правила.

| Метод | Маршрут | Тіло | Відповідь |
|---|---|---|---|
| POST | `/auth/register` | `IdentityRegisterRequest` | 201 `UserResponse` / 400 / 409 |
| POST | `/auth/login` | `IdentityLoginRequest` (`Login`, `Password`) | 200 `true` / `false` / 400 |
| GET | `/auth/users` | - | 200 `UserResponse[]` |

Міграція `AddIdentity` видаляє старі рядки таблиці `Users`: їхні паролі були збережені відкритим текстом, і Identity не змогла б їх перевірити. Далі таблиця перейменовується в `AspNetUsers`, і створюються таблиці ролей, claims, логінів і токенів. Фільми, сеанси й коментарі не змінюються.

Результати перевірки (HTTP-запити на запущеному застосунку):

| # | Запит | Очікувано | Фактично |
|---|---|---|---|
| OK1 | POST `/auth/register` з коректними даними | 201 | 201, `{"id":11,...}` |
| OK2 | POST `/auth/login` з правильним паролем | 200 | 200, профіль користувача |
| OK3 | GET `/auth/users` | 200, без паролів | 200, список без полів пароля |
| ERR1 | Повторна реєстрація з тим самим email (у т.ч. у верхньому регістрі) | 409 | 409, `User with this email already exists.` |
| ERR2 | Пароль коротший за 6 символів | 400 | 400, `Password: ["Password must be at least 6 characters long."]` |
| ERR3 | Паролі не збігаються | 400 | 400, `ConfirmPassword: ["Passwords do not match."]` |
| ERR4 | Некоректний email | 400 | 400, `Email: [...]` |
| ERR5 | Невірний пароль | 200 `false` | 200, `false` |
| ERR6 | Неіснуючий логін | 200 `false` | 200, `false` |
| ERR7 | Порожній пароль при вході | 400 | 400, масив `[{"propertyName":"Password","errorMessage":"Password is required.",...}]` |
| DB1 | Значення `PasswordHash` у таблиці `AspNetUsers` | хеш, не текст | починається з `AQAAAA` (формат Identity), довжина 84 |
| REG1 | Регресія: фільми, сеанси, коментарі, вхід через Vite-проксі | без змін | 200 у всіх випадках |

Тестовий акаунт `identity.test@example.com` залишено в БД.

## 11. Пошук фільмів (`api/Movie/movies/search`)

`GET /api/Movie/movies/search` приймає необов'язкові query-параметри. Усі передані параметри діють разом (AND), відсутні або порожні не обмежують пошук. Без параметрів повертаються всі фільми.

| Параметр | Умова |
|---|---|
| `id` | точний збіг ID |
| `name` | назва містить текст (без урахування регістру) |
| `genre` | жанр збігається |
| `releaseYear` | рік випуску збігається |
| `country` | країна збігається |
| `keyword` | текст міститься в назві або в описі |

Відповідь: 200 `MovieResponse[]` (порожній масив, якщо нічого не знайдено) або 400, якщо `id` чи `releaseYear` не ціле число.

Умови формуються в `MovieService.SearchAsync` (`MovieSearchDto` -> `Expression<Func<Movie, bool>>`) і передаються в `MovieRepository.GetAsync`.

Для пошуку додано поля `Genre` і `Country` (до 50 символів, необов'язкові) у `Movie`, контракти та валідацію. Міграція `AddMovieGenreAndCountry` лише додає ці колонки, існуючі фільми отримують `NULL`.

Перевірка: без параметрів 20 фільмів (як у `GET /movies`); один параметр; кілька параметрів; комбінація без збігів (порожній масив); нечислове `id` (400); жанр довший за 50 символів при створенні (400).

## 12. Учасники кіноклубу

Користувач (`User : IdentityUser<int>`) має поля: `DisplayName` (відображуване ім'я, 3–30 символів; колишнє `Name`, перейменовано міграцією `UserClubProfile` з збереженням даних), `City` (до 60) і `FavoriteGenre` (текст, до 50). `UserName` дорівнює логіну, а email зберігається окремо.

| Метод | Маршрут | Тіло | Відповідь |
|---|---|---|---|
| POST | `/auth/register` | `IdentityRegisterRequest` (`Login`, `Email`, `Password`, `ConfirmPassword`, `DisplayName`, `City`, `FavoriteGenre`) | 201 `UserProfileResponse` / 400 / 409 |
| POST | `/auth/login` | `IdentityLoginRequest` | 200 `true` / `false` |
| GET | `/auth/users` | - | 200 `UserProfileResponse[]` |
| GET | `/auth/users/{id}` | - | 200 `UserProfileResponse` / 404 |

- `UserProfileResponse` містить лише `Id`, `DisplayName`, `City`, `FavoriteGenre`. Email, пароль, `PasswordHash`, `SecurityStamp` та інші службові поля не повертаються.
- Повторний логін (`Login` уже зайнятий) дає 409 з повідомленням «Login is already taken.». Помилки `IdentityResult` (наприклад, недопустимі символи логіна) передаються клієнту з 400 і кодом помилки.
- Пароль хешує Identity (`UserManager`). Правила пароля: мінімум 6 символів, без вимог до цифр і регістру. Це налаштування `AddDataAccess`.
- Некоректні дані (порожнє або з пробілів ім'я, ім'я з 2 або 31 символом, невірний email, різні паролі) повертають 400 з описом, без HTTP 500.

### Data Annotations

Контракти фільмів, коментарів і сеансів мають `[Required]`, `[MaxLength]` та `[Range]`. FluentValidation залишено для складніших правил. Через `[ApiController]` атрибути перевіряються першими, тому для порожнього `Title` відповідь 400 формується стандартним способом ASP.NET з описом поля.

### Зміни в коді

- `Cinema.Domain/Entities/User.cs`: `DisplayName`, `City`, `FavoriteGenre`.
- `Cinema.DataAccess/Migrations/20261120130500_UserClubProfile.cs`: перейменування `Name` в `DisplayName` (без втрати даних), нові колонки.
- `WebApplication1/Contracts/IdentityRegisterRequest.cs`, `UserProfileResponse.cs` (новий), `UserResponse.cs` (видалено).
- `WebApplication1/Validations/RegisterRequestValidator.cs`: правила для логіна, email, пароля, імені, міста, жанру.
- `Cinema.DataAccess/Repositories/UserRepository.cs`, `IUserRepository.cs`: `FindByIdAsync`.
- `Cinema.Business/Services/IdentityAuthService.cs`: профіль, перевірка логіна.
- `WebApplication1/Controllers/AuthController.cs`: 409 для зайнятого логіна і email, `IdentityResult`, `GET users/{id}`.
- `WebApplication1/Mapping/MappingProfile.cs`: логін в `UserName`, очищення пробілів.
- `client/src/components/AuthPage.jsx`: форма реєстрації з новими полями.
- Data Annotations у `CreateMovieRequest`, `UpdateMovieRequest`, `CreateMovieCommentRequest`, `UpdateMovieCommentRequest`, `CreateMovieSessionRequest`, `UpdateMovieSessionRequest`.

Знімки: `docs/screenshots/club/` (структура БД Identity, реєстрація, помилки, вхід, профіль).
