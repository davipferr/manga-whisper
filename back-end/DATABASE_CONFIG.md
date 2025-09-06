# Database Configuration Guide

This guide explains how to securely configure your PostgreSQL database connection for the MangaWhisper project.

## 🚀 Quick Setup (Recommended)

### Step 1: Create a `.env` file in `back-end/MangaWhisper.Api/`

Inside the directory `back-end/MangaWhisper.Api/` make a copy of the file `.env.example` and rename it to `.env`

### Step 2: Replace with Your PostgreSQL Details

```bash
ConnectionStrings__DefaultConnection=Host=localhost;Database=your_db_name;Username=your_username;Password=your_password;Port=5432
```

### Example values:
- **Host**: `localhost` or your PostgreSQL server address
- **Database**: `manga_whisper_dev` (development)
- **Username**: Your PostgreSQL username (usually `postgres`)
- **Password**: Your PostgreSQL password
- **Port**: `5432` (default PostgreSQL port)

## 🧪 Testing Your Configuration

Run the application to test the connection:

```bash
cd back-end/MangaWhisper.Api
dotnet run
```

If the connection is successful, you'll see no database errors in the startup logs.

## 🛠️ Troubleshooting

### Common Issues:

1. **"Database connection string not found"**
   - Ensure you've set the connection string

2. **"Cannot connect to PostgreSQL"**
   - Verify PostgreSQL is running
   - Check host, port, and credentials
   - Ensure the database exists

3. **"Authentication failed"**
   - Verify username and password
   - Check PostgreSQL user permissions

💡 **Remember**: The `.env` file is ignored by Git, so your credentials are safe!