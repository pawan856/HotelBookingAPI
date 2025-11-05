# HotelBookingAPI

A comprehensive hotel booking and management system built with ASP.NET Core 8.0, featuring JWT authentication, role-based authorization, payment integration, and booking management.

## 🚀 Features

### Authentication & Authorization
- **JWT-based Authentication** - Secure token-based authentication system
- **Role-Based Access Control** - Admin and Guest roles with granular permissions
- **ASP.NET Core Identity** - User management and password hashing
- **Protected Endpoints** - Secure API endpoints with [Authorize] attributes

### Booking & Availability
- **Room Availability Search** - Search available rooms by date range, guest count, amenities, and price
- **Booking Management** - Create, view, and cancel bookings
- **Double Booking Prevention** - Smart query logic prevents overlapping reservations
- **Date Range Validation** - Handles inclusive/exclusive date edge cases

### Payment Integration
- **Stripe Integration** - Process payments securely with Stripe
- **Payment Intent API** - Create payment intents for bookings
- **Webhook Support** - Secure webhook endpoint for payment confirmations
- **Payment Status Tracking** - Track payment lifecycle (Pending → Succeeded/Failed → Refunded)

### Cancellation & Refunds
- **Cancellation Policies** - Flexible cancellation window and refund percentages
- **Automatic Refunds** - Process refunds based on cancellation policies
- **Email Notifications** - Send confirmation emails for bookings, payments, and cancellations
- **Status Management** - Update booking and payment statuses automatically

### Technical Features
- **Entity Framework Core** - ORM for database operations
- **SQL Server** - Relational database with migrations support
- **Memory Caching** - Cache availability searches for performance
- **Serilog** - Structured logging to files
- **Swagger/OpenAPI** - Interactive API documentation

## 📋 Prerequisites

- .NET 8.0 SDK
- SQL Server (Express or Developer Edition)
- Stripe Account (for payment integration)
- SMTP Server (for email notifications - optional)

## 🔧 Installation

1. **Clone the repository**
```bash
git clone https://github.com/pawan856/HotelBookingAPI.git
cd HotelBookingAPI
```

2. **Update connection string**

Edit `appsettings.json` and update the connection string:
```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=YOUR_SERVER;Database=HotelDB;Trusted_Connection=True;TrustServerCertificate=True;"
  }
}
```

3. **Configure Stripe** (Optional for payment features)

Add your Stripe keys to `appsettings.json`:
```json
{
  "Stripe": {
    "SecretKey": "sk_test_your_stripe_secret_key",
    "PublishableKey": "pk_test_your_publishable_key",
    "WebhookSecret": "whsec_your_webhook_secret"
  }
}
```

4. **Install dependencies**
```bash
dotnet restore
```

5. **Run migrations**
```bash
dotnet ef database update
```

6. **Run the application**
```bash
dotnet run
```

The API will be available at `https://localhost:7XXX` (check console output for exact port).

## 🔐 Authentication Flow

### 1. Register a New User

**Endpoint:** `POST /api/Auth/Register`

**Request:**
```json
{
  "email": "user@example.com",
  "password": "YourPassword123!",
  "phoneNumber": "+1234567890",
  "role": "Guest"
}
```

**Response:**
```json
{
  "message": "User registered successfully"
}
```

### 2. Login

**Endpoint:** `POST /api/Auth/Login`

**Request:**
```json
{
  "email": "user@example.com",
  "password": "YourPassword123!"
}
```

**Response:**
```json
{
  "userId": 1,
  "message": "Login Successful",
  "isLogin": true,
  "token": "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9..."
}
```

### 3. Using the JWT Token

Add the token to the `Authorization` header for all protected endpoints:

```
Authorization: Bearer eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9...
```

## 📚 API Endpoints

### Authentication
- `POST /api/Auth/Register` - Register new user
- `POST /api/Auth/Login` - Login and get JWT token

### Room Availability & Booking
- `GET /api/Booking/availability` - Search available rooms (Public)
- `POST /api/Booking/create` - Create a new booking (Requires Auth)
- `POST /api/Booking/{bookingId}/cancel` - Cancel booking (Requires Auth)

### Rooms (Admin Only)
- `GET /api/Room/All` - Get all rooms
- `GET /api/Room/{id}` - Get room by ID
- `POST /api/Room/Create` - Create room (Admin only)
- `PUT /api/Room/Update/{id}` - Update room (Admin only)
- `DELETE /api/Room/Delete/{id}` - Delete room (Admin only)

### Room Types (Admin Only for CUD)
- `GET /api/RoomType/AllRoomTypes` - Get all room types
- `POST /api/RoomType/AddRoomType` - Add room type (Admin only)
- `PUT /api/RoomType/Update/{id}` - Update room type (Admin only)
- `DELETE /api/RoomType/Delete/{id}` - Delete room type (Admin only)

### Amenities (Admin Only for CUD)
- `GET /api/Amenity/Fetch` - Get all amenities
- `POST /api/Amenity/Add` - Add amenity (Admin only)
- `PUT /api/Amenity/Update/{id}` - Update amenity (Admin only)
- `DELETE /api/Amenity/Delete/{id}` - Delete amenity (Admin only)

### Payment Webhook
- `POST /api/Booking/webhook/stripe` - Stripe webhook endpoint

## 🎯 Usage Examples

### Search Available Rooms

```bash
curl -X GET "https://localhost:7XXX/api/Booking/availability?start=2025-12-01&end=2025-12-05&guests=2&minPrice=50&maxPrice=200" \
  -H "Accept: application/json"
```

### Create a Booking

```bash
curl -X POST "https://localhost:7XXX/api/Booking/create" \
  -H "Content-Type: application/json" \
  -H "Authorization: Bearer YOUR_JWT_TOKEN" \
  -d '{
    "roomId": 1,
    "startDate": "2025-12-01",
    "endDate": "2025-12-05",
    "guestCount": 2,
    "specialRequests": "Late check-in please"
  }'
```

### Cancel a Booking

```bash
curl -X POST "https://localhost:7XXX/api/Booking/123/cancel" \
  -H "Authorization: Bearer YOUR_JWT_TOKEN"
```

## 💳 Payment Flow

1. **Customer creates booking** → Booking status: `Pending`
2. **Payment Intent created** → Returns `clientSecret` to frontend
3. **Customer completes payment** → Stripe processes payment
4. **Webhook notification** → API receives payment confirmation
5. **Booking confirmed** → Booking status: `Confirmed`, Payment status: `Succeeded`

## 📧 Email Notifications

The system sends email notifications for:
- Booking confirmation
- Payment confirmation
- Cancellation confirmation with refund details

Configure SMTP settings in `appsettings.json`:
```json
{
  "Email": {
    "SmtpHost": "smtp.gmail.com",
    "SmtpPort": 587,
    "SenderEmail": "noreply@hotelbooking.com",
    "SenderName": "Hotel Booking System",
    "Username": "your_email@gmail.com",
    "Password": "your_app_password"
  }
}
```

## 🔒 Security Considerations

- **JWT Tokens**: Tokens expire after 60 minutes (configurable)
- **Password Hashing**: ASP.NET Core Identity handles secure password storage
- **Webhook Verification**: Stripe webhook signatures are verified
- **Role-Based Access**: Admin-only endpoints protected with `[Authorize(Roles = "Admin")]`
- **HTTPS**: Always use HTTPS in production

## 🗄️ Database Schema

### Key Tables
- **AspNetUsers** - User accounts (Identity)
- **AspNetRoles** - User roles (Admin, Guest)
- **Bookings** - Booking records with status tracking
- **Payments** - Payment records linked to bookings
- **CancellationPolicies** - Flexible cancellation rules

## 🧪 Testing

### Default Admin Account
- **Email**: `admin@hotelbooking.com`
- **Password**: `Admin@123`

### Testing Workflow
1. Register as Guest user
2. Login to get JWT token
3. Search available rooms
4. Create a booking
5. (Optional) Cancel booking to test refund flow

## 📝 Default Admin User

On first run, the system automatically creates an admin user:
- Email: `admin@hotelbooking.com`
- Password: `Admin@123`

**Important**: Change this password in production!

## 🛠️ Technologies Used

- ASP.NET Core 8.0
- Entity Framework Core
- ASP.NET Core Identity
- JWT Bearer Authentication
- Stripe API (Payment Processing)
- Serilog (Logging)
- Swagger/OpenAPI
- SQL Server

## 📄 License

This project is licensed under the MIT License - see the LICENSE.txt file for details.

## 👤 Author

**Pawan**
- GitHub: [@pawan856](https://github.com/pawan856)

## 🤝 Contributing

Contributions, issues, and feature requests are welcome!

## ⭐ Show your support

Give a ⭐️ if this project helped you!
